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
#include <ArduinoJson.h>
#include <SPI.h>
#include <SD.h>
#include <SoftwareSerial.h>

#define SYSTEM_LED_PIN PA30

bool flag_udp_232back = false;
bool flag_JsonSend = false;
bool flag_writeMode = false;

WiFiConfig wiFiConfig;
int UDP_SemdTime = 0;
int Localport = 0;
IPAddress ServerIp;
int Serverport;
String GetwayStr;


byte* framebuffer;

MyTimer MyTimer_BoardInit;
bool flag_boradInit = false;
MyLED MyLED_IS_Connented;

TaskHandle_t Core0Task1Handle;
TaskHandle_t Core0Task2Handle;
TaskHandle_t Core0Task3Handle;
TaskHandle_t Core0Task4Handle;

SoftwareSerial mySerial(PA8, PA7); // RX, TX
SoftwareSerial mySerial_R200(PB2, PB1); // RX, TX

String Version = "Ver 1.0.0";
DFRobot_MCP23017 mcp(Wire, /*addr =*/0x20);//constructor, change the Level of A2, A1, A0 via DIP switch to revise the I2C address within 0x20~0x27.

void setup() 
{

    
    

    
    MyTimer_BoardInit.StartTickTime(1000);
        
    

}
bool flag_pb2 = true;
void loop() 
{
   
   if(MyTimer_BoardInit.IsTimeOut() && !flag_boradInit)
   {     
    
      DFRobot_MCP23017 mcp(Wire, /*addr =*/0x20);//constructor, change the Level of A2, A1, A0 via DIP switch to revise the I2C address within 0x20~0x27.
      while(mcp.begin() != 0)
      {
        Serial.println("Initialization of the chip failed, please confirm that the chip connection is correct!");
        delay(1000);
      };
      mySerial.begin(115200);   
      mySerial.println(Version);
      mySerial_R200.begin(115200);
      
      wiFiConfig.mySerial = &mySerial;
      wiFiConfig.Init(Version);
      Localport = wiFiConfig.Get_Localport();
      Serverport = wiFiConfig.Get_Serverport();
      ServerIp = wiFiConfig.Get_Server_IPAdressClass();
      UDP_SemdTime = wiFiConfig.Get_UDP_SemdTime();
      GetwayStr = wiFiConfig.Get_Gateway_Str();
      MyLED_IS_Connented.Init(SYSTEM_LED_PIN);
      xTaskCreate(Core0Task1,"Core0Task1", 1024,NULL,1,&Core0Task1Handle);
      xTaskCreate(Core0Task2,"Core0Task2", 1024,NULL,1,&Core0Task2Handle);
      
      flag_boradInit = true;
   }
   if(flag_boradInit)
   {
      sub_IO_Program();
      if(WiFi.status() != WL_CONNECTED)
      {
         wiFiConfig.WIFI_Connenct();
         if(WiFi.status() == WL_CONNECTED)
         {
           Connect_UDP(Localport);  
         }
               
      }   
       if(WiFi.status() == WL_CONNECTED)
       {
          sub_UDP_Send();
          onPacketCallBack();
       }
   }
   
  
      
    
}

void Core0Task1( void * pvParameters )
{
    for(;;)
    {      
       
       if(flag_boradInit)
       {
          serialEvent();
          //serialEvent1();
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

          Get_CardID();
       }
          
    }
    
}
