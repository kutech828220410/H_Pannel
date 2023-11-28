#include "EPD.h"
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



#define SPI_MOSI_PIN PA12
#define NUM_WS2812B_CRGB  450
#define NUM_OF_LEDS NUM_WS2812B_CRGB
#define SYSTEM_LED_PIN PA30

bool flag_udp_232back = true;
bool flag_JsonSend = false;
bool flag_writeMode = false;

EPD epd;
WiFiConfig wiFiConfig;
int UDP_SemdTime = 0;
int Localport = 0;
IPAddress ServerIp;
int Serverport;
String GetwayStr;

bool flag_WS2812B_Refresh = true;
bool flag_WS2812B_breathing  = false;
int WS2812B_breathing_cnt = 0;
int WS2812B_breathing_onAddVal  = 3;
int WS2812B_breathing_offSubVal  = 3;
float WS2812B_breathing_val = 0.1F;
byte WS2812B_breathing_R = 255;
byte WS2812B_breathing_G = 0;
byte WS2812B_breathing_B = 0;

MyWS2812 myWS2812;

byte* framebuffer;

MyTimer MyTimer_BoardInit;
MyTimer MyTimer_CheckWS2812;
bool flag_boradInit = false;
MyLED MyLED_IS_Connented;

TaskHandle_t Core0Task1Handle;
TaskHandle_t Core0Task2Handle;
TaskHandle_t Core0Task3Handle;
TaskHandle_t Core0Task4Handle;

SoftwareSerial mySerial(PA8, PA7); // RX, TX
SoftwareSerial mySerial2(PB2, PB1); // RX, TX

String Version = "Ver 1.4.3";

#define EPD
//#define RowLED


#ifdef EPD
int MCU_TYPE = 1;
#elif defined(RowLED)
int MCU_TYPE = 2;
#endif

void setup() 
{
    MyTimer_BoardInit.StartTickTime(3000);          
}
bool flag_pb2 = true;
void loop() 
{
   
   if(MyTimer_BoardInit.IsTimeOut() && !flag_boradInit)
   {     
      mySerial.begin(115200);        
      mySerial.println(Version);  
      wiFiConfig.mySerial = &mySerial;
      epd.mySerial = &mySerial;
      wiFiConfig.Init(Version);
      
      if(MCU_TYPE == 1)
      {
        wiFiConfig.Set_Localport(29000);
        wiFiConfig.Set_Serverport(30000);
      }
      if(MCU_TYPE == 2)
      {
        wiFiConfig.Set_Localport(29001);
        wiFiConfig.Set_Serverport(30001);
      }

      
      Localport = wiFiConfig.Get_Localport();
      Serverport = wiFiConfig.Get_Serverport();
      ServerIp = wiFiConfig.Get_Server_IPAdressClass();
      UDP_SemdTime = wiFiConfig.Get_UDP_SemdTime();
      GetwayStr = wiFiConfig.Get_Gateway_Str();
      MyLED_IS_Connented.Init(SYSTEM_LED_PIN);
      SPI.begin(); //SCLK, MISO, MOSI, SS
      myWS2812.Init(NUM_WS2812B_CRGB);
      epd.Init(); 
      xTaskCreate(Core0Task1,"Core0Task1", 1024,NULL,1,&Core0Task1Handle);
      
      if(MCU_TYPE == 2)xTaskCreate(Core0Task2,"Core0Task2", 1024,NULL,1,&Core0Task2Handle);
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
              serial2Event();
              MyLED_IS_Connented.BlinkTime = 100;      
          }
          else
          {
              MyLED_IS_Connented.BlinkTime = 500;
          }
          
          epd.Sleep_Check();
          if(flag_WS2812B_breathing)
          {
               if(WS2812B_breathing_cnt == 0)
               {
                   int numofLED = 450;
                   int R = (int)(WS2812B_breathing_R * WS2812B_breathing_val);
                   int G = (int)(WS2812B_breathing_G * WS2812B_breathing_val);
                   int B = (int)(WS2812B_breathing_B * WS2812B_breathing_val);
                   WS2812B_breathing_val += ((float)WS2812B_breathing_onAddVal * 0.01);
                   if(WS2812B_breathing_val >= 0.9) 
                   {
                       WS2812B_breathing_cnt++;
                   }
                   for(int i = 0 ; i < numofLED ; i++)
                   {             
                       myWS2812.rgbBuffer[i * 3 + 0 + 0] = (int)(R);     // 将光带上第1个LED灯珠的RGB数值中R数值设置为255
                       myWS2812.rgbBuffer[i * 3 + 0 + 1] = (int)(G);   // 将光带上第1个LED灯珠的RGB数值中G数值设置为255
                       myWS2812.rgbBuffer[i * 3 + 0 + 2] = (int)(B);      // 将光带上第1个LED灯珠的RGB数值中B数值设置为0      
                   }                                                     
                   myWS2812.Show();
               }
               if(WS2812B_breathing_cnt == 1)
               {
                   int numofLED = 450;
                   int R = (int)(WS2812B_breathing_R * WS2812B_breathing_val);
                   int G = (int)(WS2812B_breathing_G * WS2812B_breathing_val);
                   int B = (int)(WS2812B_breathing_B * WS2812B_breathing_val);
                   WS2812B_breathing_val -= ((float)WS2812B_breathing_offSubVal * 0.01);
                   if(WS2812B_breathing_val <= 0.1) 
                   {
                       WS2812B_breathing_cnt++;
                   }
                   for(int i = 0 ; i < numofLED ; i++)
                   {             
                       myWS2812.rgbBuffer[i * 3 + 0 + 0] = (int)(R);     // 将光带上第1个LED灯珠的RGB数值中R数值设置为255
                       myWS2812.rgbBuffer[i * 3 + 0 + 1] = (int)(G);   // 将光带上第1个LED灯珠的RGB数值中G数值设置为255
                       myWS2812.rgbBuffer[i * 3 + 0 + 2] = (int)(B);      // 将光带上第1个LED灯珠的RGB数值中B数值设置为0      
                   }                                                
                   myWS2812.Show();
               }
               if(WS2812B_breathing_cnt == 2)
               {
                  WS2812B_breathing_cnt = 0;
               }
               
          }
          else if(flag_WS2812B_Refresh)
          {
               myWS2812.Show();
               flag_JsonSend = true;
               flag_WS2812B_Refresh = false;
          }  
       }
          
       delay(1);
    }
    
}
void Core0Task2( void * pvParameters )
{
    for(;;)
    {      
       
       if(flag_boradInit)
       {
          if(!myWS2812.IsON(200))
          {
            flag_WS2812B_Refresh = true;
          }
       }
          
       delay(20000);
    }
    
}
