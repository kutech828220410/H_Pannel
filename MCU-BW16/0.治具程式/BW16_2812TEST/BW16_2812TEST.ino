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
SoftwareSerial mySerial2(PB2, PB1); // RX, TX

String Version = "Ver 1.2.3";

void setup() 
{
    MyTimer_BoardInit.StartTickTime(1000);          
}
bool flag_pb2 = true;
void loop() 
{
   
   if(MyTimer_BoardInit.IsTimeOut() && !flag_boradInit)
   {     
      mySerial.begin(115200);        
      mySerial.println(Version);  
     
      MyLED_IS_Connented.Init(SYSTEM_LED_PIN);
      SPI.begin(); //SCLK, MISO, MOSI, SS
      myWS2812.Init(NUM_WS2812B_CRGB);
      xTaskCreate(Core0Task1,"Core0Task1", 1024,NULL,1,&Core0Task1Handle);
      flag_boradInit = true;
   }
   if(flag_boradInit)
   {
      for(int i = 0 ; i < NUM_WS2812B_CRGB ; i++)
      {             
         myWS2812.rgbBuffer[i * 3 + 0 + 0] = 0;     // 将光带上第1个LED灯珠的RGB数值中R数值设置为255
         myWS2812.rgbBuffer[i * 3 + 0 + 1] = 0;   // 将光带上第1个LED灯珠的RGB数值中G数值设置为255
         myWS2812.rgbBuffer[i * 3 + 0 + 2] = 0;      // 将光带上第1个LED灯珠的RGB数值中B数值设置为0      
      }
      myWS2812.Show();
      for(int i = 0 ; i < NUM_WS2812B_CRGB ; i++)
      {             
         myWS2812.rgbBuffer[i * 3 + 0 + 0] = 255;     // 将光带上第1个LED灯珠的RGB数值中R数值设置为255
         myWS2812.rgbBuffer[i * 3 + 0 + 1] = 0;   // 将光带上第1个LED灯珠的RGB数值中G数值设置为255
         myWS2812.rgbBuffer[i * 3 + 0 + 2] = 0;      // 将光带上第1个LED灯珠的RGB数值中B数值设置为0      
      }
      myWS2812.Show();
      mySerial.println("WS2812 Red Show!");
      delay(500);
      for(int i = 0 ; i < NUM_WS2812B_CRGB ; i++)
      {             
         myWS2812.rgbBuffer[i * 3 + 0 + 0] = 0;     // 将光带上第1个LED灯珠的RGB数值中R数值设置为255
         myWS2812.rgbBuffer[i * 3 + 0 + 1] = 0;   // 将光带上第1个LED灯珠的RGB数值中G数值设置为255
         myWS2812.rgbBuffer[i * 3 + 0 + 2] = 0;      // 将光带上第1个LED灯珠的RGB数值中B数值设置为0      
      }
      myWS2812.Show();
      for(int i = 0 ; i < NUM_WS2812B_CRGB ; i++)
      {             
         myWS2812.rgbBuffer[i * 3 + 0 + 0] = 0;     // 将光带上第1个LED灯珠的RGB数值中R数值设置为255
         myWS2812.rgbBuffer[i * 3 + 0 + 1] = 255;   // 将光带上第1个LED灯珠的RGB数值中G数值设置为255
         myWS2812.rgbBuffer[i * 3 + 0 + 2] = 0;      // 将光带上第1个LED灯珠的RGB数值中B数值设置为0      
      }
      myWS2812.Show();
      mySerial.println("WS2812 Green Show!");
      delay(500);
      for(int i = 0 ; i < NUM_WS2812B_CRGB ; i++)
      {             
         myWS2812.rgbBuffer[i * 3 + 0 + 0] = 0;     // 将光带上第1个LED灯珠的RGB数值中R数值设置为255
         myWS2812.rgbBuffer[i * 3 + 0 + 1] = 0;   // 将光带上第1个LED灯珠的RGB数值中G数值设置为255
         myWS2812.rgbBuffer[i * 3 + 0 + 2] = 0;      // 将光带上第1个LED灯珠的RGB数值中B数值设置为0      
      }
      myWS2812.Show();
      for(int i = 0 ; i < NUM_WS2812B_CRGB ; i++)
      {             
         myWS2812.rgbBuffer[i * 3 + 0 + 0] = 0;     // 将光带上第1个LED灯珠的RGB数值中R数值设置为255
         myWS2812.rgbBuffer[i * 3 + 0 + 1] = 0;   // 将光带上第1个LED灯珠的RGB数值中G数值设置为255
         myWS2812.rgbBuffer[i * 3 + 0 + 2] = 255;      // 将光带上第1个LED灯珠的RGB数值中B数值设置为0      
      }
      myWS2812.Show();
      mySerial.println("WS2812 Blue Show!");
      delay(500);
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
          
       }
          
       delay(1);
    }
    
}
