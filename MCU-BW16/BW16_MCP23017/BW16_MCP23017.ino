#include "WS2812B.h"
#include <WiFi.h>
#include <WiFiUdp.h> 
#include <FreeRTOS.h>
#include <task.h>
#include <FlashMemory.h>
#include "Timer.h"
#include "LED.h"
#include "Output.h"
#include "Input.h" 
#include "WiFiConfig.h"
#include "MyJPEGDecoder.h"
#include "MyWS2812.h"
#include <ArduinoJson.h>
#include <SPI.h>
#include <SD.h>
#include <SoftwareSerial.h>
#include "DFRobot_MCP23017.h"

#define NUM_WS2812B_CRGB  450
#define NUM_OF_LEDS NUM_WS2812B_CRGB
#define SYSTEM_LED_PIN PA30
#define PIN_485_Tx_Eanble PB3

bool flag_udp_232back = false;
bool flag_JsonSend = false;
bool flag_writeMode = false;
String CardID[5];
String CardID_buf[5];

DFRobot_MCP23017 mcp(Wire, /*addr =*/0x20);//constructor, change the Level of A2, A1, A0 via DIP switch to revise the I2C address within 0x20~0x27.

WiFiConfig wiFiConfig;
int UDP_SemdTime = 0;
int Localport = 0;
IPAddress ServerIp;
int Serverport;
String GetwayStr;

bool flag_WS2812B_Refresh = false;
MyWS2812 myWS2812;

byte* framebuffer;

MyTimer MyTimer_BoardInit;
bool flag_boradInit = false;
MyLED MyLED_IS_Connented;

TaskHandle_t Core0Task1Handle;
TaskHandle_t Core0Task2Handle;
TaskHandle_t Core0Task3Handle;
TaskHandle_t Core0Task4Handle;

SoftwareSerial mySerial(PA8, PA7); // RX, TX
SoftwareSerial mySerial_485(PB2, PB1); // RX, TX

String Version = "Ver 1.0.2";

void setup() 
{
    mySerial_485.begin(9600);
    pinMode(PIN_485_Tx_Eanble, OUTPUT);
    
    mySerial.begin(115200);   
    mySerial.println(Version);
    while(mcp.begin() != 0)
    {
      Serial.println("Initialization of the chip failed, please confirm that the chip connection is correct!");
      delay(1000);
    };
    
    wiFiConfig.mySerial = &mySerial;
    wiFiConfig.Init(Version);
    Localport = wiFiConfig.Get_Localport();
    Serverport = wiFiConfig.Get_Serverport();
    ServerIp = wiFiConfig.Get_Server_IPAdressClass();
    UDP_SemdTime = wiFiConfig.Get_UDP_SemdTime();
    GetwayStr = wiFiConfig.Get_Gateway_Str();

    SPI.begin(); //SCLK, MISO, MOSI, SS
    myWS2812.Init(NUM_WS2812B_CRGB);

    mySerial.print("Dynamic memory size: ");
    mySerial.println(os_get_free_heap_size_arduino());
    mySerial.println();
    
    IO_Init();
    MyLED_IS_Connented.Init(SYSTEM_LED_PIN);
    
    MyTimer_BoardInit.StartTickTime(3000);
        
    xTaskCreate(Core0Task1,"Core0Task1", 1024,NULL,1,&Core0Task1Handle);
    xTaskCreate(Core0Task2,"Core0Task2", 1024,NULL,1,&Core0Task2Handle);

}
bool flag_pb2 = true;
void loop() 
{
   
   if(MyTimer_BoardInit.IsTimeOut() && !flag_boradInit)
   {     
       flag_boradInit = true;
   }
   if(flag_boradInit)
   {
      
      if(WiFi.status() != WL_CONNECTED)
      {
         wiFiConfig.WIFI_Connenct();
         Connect_UDP(Localport);
         
      }   
 
   }
   
   if(WiFi.status() == WL_CONNECTED)
   {
      sub_UDP_Send();
      onPacketCallBack();
   }
      
    
}

void Core0Task1( void * pvParameters )
{
    for(;;)
    {      
       
       if(flag_boradInit)
       {
          serialEvent();
          sub_IO_Program();
          MyLED_IS_Connented.Blink();
          if( WiFi.status() == WL_CONNECTED  )
          {
              MyLED_IS_Connented.BlinkTime = 100;      
          }
          else
          {
              MyLED_IS_Connented.BlinkTime = 500;
          }
          
       }
          
       delay(10);
    }
    
}
void Core0Task2( void * pvParameters )
{
    for(;;)
    {      
       
       if(flag_boradInit)
       {
          sub_RFID_program();
       }
       delay(1);
    }
    
}