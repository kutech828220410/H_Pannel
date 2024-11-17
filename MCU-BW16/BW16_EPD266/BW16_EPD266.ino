#include "Config.h"
#include "EPD.h"
#include "OLCD114.h"
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
#ifdef MCP23017
#include "DFRobot_MCP23017.h"
DFRobot_MCP23017 mcp(Wire, /*addr =*/0x20);//constructor, change the Level of A2, A1, A0 via DIP switch to revise the I2C address within 0x20~0x27.
#endif

#define SPI_MOSI_PIN PA12
#define NUM_WS2812B_CRGB  450
#define NUM_OF_LEDS NUM_WS2812B_CRGB
#define SYSTEM_LED_PIN PA30

bool flag_udp_232back = false;
bool flag_JsonSend = false;
bool flag_writeMode = false;

EPD epd;
OLCD114 OLCD114;

WiFiConfig wiFiConfig;
int UDP_SemdTime = 0;
int Localport = 0;
IPAddress ServerIp;
int Serverport;
String GetwayStr;

bool flag_WS2812B_Refresh = true;
bool flag_WS2812B_breathing_ON_OFF  = false;
int WS2812B_breathing_ON_OFF_cnt = 0;
int WS2812B_breathing_onAddVal  = 3;
int WS2812B_breathing_offSubVal  = 3;
float WS2812B_breathing_val = 0.1F;
byte WS2812B_breathing_R = 255;
byte WS2812B_breathing_G = 0;
byte WS2812B_breathing_B = 0;

MyWS2812 myWS2812;

byte* framebuffer;

MyTimer MyTimer_BoardInit;
MyTimer MyTimer_OLCD_144_Init;
MyTimer MyTimer_CheckWS2812;
MyTimer MyTimer_CheckWIFI;

bool flag_boradInit = false;
bool flag_OLCD_144_boradInit = false;

MyLED MyLED_IS_Connented;

TaskHandle_t Core0Task1Handle;
TaskHandle_t Core0Task2Handle;
TaskHandle_t Core0Task3Handle;
TaskHandle_t Core0Task4Handle;

SoftwareSerial mySerial(PA8, PA7); // RX, TX
SoftwareSerial mySerial2(PB2, PB1); // RX, TX
void setup() 
{
    MyTimer_BoardInit.StartTickTime(3000);          
    MyTimer_OLCD_144_Init.StartTickTime(5000);          
    MyTimer_CheckWIFI.StartTickTime(180000);   
}
bool flag_pb2 = true;
void loop() 
{
   if(MyTimer_OLCD_144_Init.IsTimeOut() && !flag_OLCD_144_boradInit)
   {
      if(Device == "OLCD_114")
      {
        mySerial.println("OLCD114 device init ...");
        OLCD114.mySerial = &mySerial;
        OLCD114.Lcd_Init();
        delay(200);
        OLCD114.LCD_Clear(GRAY);
      }     
      flag_OLCD_144_boradInit = true;
   }
   if(MyTimer_BoardInit.IsTimeOut() && !flag_boradInit)
   {     

      
      mySerial.begin(115200);        
      mySerial.println(VERSION);  
      
      #ifdef MCP23017
      while(mcp.begin() != 0)
      {
        mySerial.println("Initialization of the chip failed, please confirm that the chip connection is correct!");
        delay(1000);
      }
      delay(500);    
      mcp.pinMode(mcp.eGPA0, /*mode = */INPUT_PULLUP);
      mcp.pinMode(mcp.eGPA1, /*mode = */INPUT_PULLUP);
      mcp.pinMode(mcp.eGPA2, /*mode = */INPUT_PULLUP);
      mcp.pinMode(mcp.eGPA3, /*mode = */INPUT_PULLUP);
      mcp.pinMode(mcp.eGPA4, /*mode = */INPUT_PULLUP);
      mcp.pinMode(mcp.eGPA5, /*mode = */INPUT_PULLUP);
      mcp.pinMode(mcp.eGPA6, /*mode = */INPUT_PULLUP);
      mcp.pinMode(mcp.eGPA7, /*mode = */INPUT_PULLUP);
    
      mcp.pinMode(mcp.eGPB0, /*mode = */OUTPUT);
      mcp.pinMode(mcp.eGPB1, /*mode = */OUTPUT);
      mcp.pinMode(mcp.eGPB2, /*mode = */OUTPUT);
      mcp.pinMode(mcp.eGPB3, /*mode = */OUTPUT);
      mcp.pinMode(mcp.eGPB4, /*mode = */OUTPUT);
      mcp.pinMode(mcp.eGPB5, /*mode = */OUTPUT);
      mcp.pinMode(mcp.eGPB6, /*mode = */OUTPUT);
      mcp.pinMode(mcp.eGPB7, /*mode = */OUTPUT);
      mcp.digitalWrite(mcp.eGPB0 , false);   
      mcp.digitalWrite(mcp.eGPB1 , false);   
      mcp.digitalWrite(mcp.eGPB2 , false);   
      mcp.digitalWrite(mcp.eGPB3 , false);   
      mcp.digitalWrite(mcp.eGPB4 , false);   
      mcp.digitalWrite(mcp.eGPB5 , false);   
      mcp.digitalWrite(mcp.eGPB6 , false);   
      mcp.digitalWrite(mcp.eGPB7 , true);   
      
      OLCD114._mcp = &mcp;
      mySerial.println("mcp.pinMode....ok");   
      delay(200);
      #endif
      
      wiFiConfig.mySerial = &mySerial;
      epd.mySerial = &mySerial;
      wiFiConfig.Init(VERSION);
      delay(200);
      IO_Init();
      delay(200);
      if(EPD_TYPE == "EPD266" || EPD_TYPE == "EPD290" || EPD_TYPE == "EPD420")
      {
         wiFiConfig.Set_Serverport(30000);
      }
      if(Device == "RowLED") wiFiConfig.Set_Serverport(30001);
      if(Device == "OLCD114") 
      {
        wiFiConfig.Set_Localport(29008);
        wiFiConfig.Set_Serverport(30008);
      }
      
      Localport = wiFiConfig.Get_Localport();
      Serverport = wiFiConfig.Get_Serverport();
      ServerIp = wiFiConfig.Get_Server_IPAdressClass();
      UDP_SemdTime = wiFiConfig.Get_UDP_SemdTime();
      GetwayStr = wiFiConfig.Get_Gateway_Str();
      MyLED_IS_Connented.Init(SYSTEM_LED_PIN);
      SPI.begin(); //SCLK, MISO, MOSI, SS
      delay(200);
      myWS2812.Init(NUM_WS2812B_CRGB);
      
      if(Device == "EPD")
      {
        mySerial.println("EPD device init ...");
        epd.Init(); 
        delay(200);
      }
     
      xTaskCreate(Core0Task1,"Core0Task1", 1024,NULL,1,&Core0Task1Handle); 
//      delay(200);    
      xTaskCreate(Core0Task2,"Core0Task2", 1024,NULL,1,&Core0Task2Handle);
//      delay(200);
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
      MyTimer_CheckWS2812.StartTickTime(30000);

      
   }    
}

void Core0Task1( void * pvParameters )
{
    for(;;)
    {      
       
       if(flag_boradInit)
       {
          serialEvent();
                    
          MyLED_IS_Connented.Blink();
          if( WiFi.status() == WL_CONNECTED  )
          {
              MyLED_IS_Connented.BlinkTime = 100;      
          }
          else
          {
              MyLED_IS_Connented.BlinkTime = 500;
              if(MyTimer_CheckWIFI.IsTimeOut())
              {
//                 NVIC_SystemReset();
              }
          }
          
          epd.Sleep_Check();
          if(flag_WS2812B_breathing_ON_OFF)
          {               
               WS2812B_breathing_ON_OFF();
          }
          else if(flag_WS2812B_Refresh)
          {
               myWS2812.Show();
               flag_JsonSend = true;
               flag_WS2812B_Refresh = false;
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
          #ifdef HandSensor
          serial2Event();
          #endif
       }
      delay(0);
    }
    
}


void WS2812B_breathing_ON_OFF()
{
   if(WS2812B_breathing_ON_OFF_cnt == 0)
   {
       int numofLED = NUM_WS2812B_CRGB;
       int R = (int)(WS2812B_breathing_R * WS2812B_breathing_val);
       int G = (int)(WS2812B_breathing_G * WS2812B_breathing_val);
       int B = (int)(WS2812B_breathing_B * WS2812B_breathing_val);
       WS2812B_breathing_val += ((float)WS2812B_breathing_onAddVal * 0.01);
       if(WS2812B_breathing_val >= 0.9) 
       {
           WS2812B_breathing_ON_OFF_cnt++;
       }
       for(int i = 0 ; i < numofLED ; i++)
       {             
           myWS2812.rgbBuffer[i * 3 + 0 + 0] = (int)(R);     // 将光带上第1个LED灯珠的RGB数值中R数值设置为255
           myWS2812.rgbBuffer[i * 3 + 0 + 1] = (int)(G);   // 将光带上第1个LED灯珠的RGB数值中G数值设置为255
           myWS2812.rgbBuffer[i * 3 + 0 + 2] = (int)(B);      // 将光带上第1个LED灯珠的RGB数值中B数值设置为0      
       }                                                     
       myWS2812.Show();
   }
   if(WS2812B_breathing_ON_OFF_cnt == 1)
   {
       int numofLED = 450;
       int R = (int)(WS2812B_breathing_R * WS2812B_breathing_val);
       int G = (int)(WS2812B_breathing_G * WS2812B_breathing_val);
       int B = (int)(WS2812B_breathing_B * WS2812B_breathing_val);
       WS2812B_breathing_val -= ((float)WS2812B_breathing_offSubVal * 0.01);
       if(WS2812B_breathing_val <= 0.1) 
       {
           WS2812B_breathing_ON_OFF_cnt++;
       }
       for(int i = 0 ; i < numofLED ; i++)
       {             
           myWS2812.rgbBuffer[i * 3 + 0 + 0] = (int)(R);     // 将光带上第1个LED灯珠的RGB数值中R数值设置为255
           myWS2812.rgbBuffer[i * 3 + 0 + 1] = (int)(G);   // 将光带上第1个LED灯珠的RGB数值中G数值设置为255
           myWS2812.rgbBuffer[i * 3 + 0 + 2] = (int)(B);      // 将光带上第1个LED灯珠的RGB数值中B数值设置为0      
       }                                                
       myWS2812.Show();
   }
   if(WS2812B_breathing_ON_OFF_cnt == 2)
   {
      WS2812B_breathing_ON_OFF_cnt = 0;
   }
}
