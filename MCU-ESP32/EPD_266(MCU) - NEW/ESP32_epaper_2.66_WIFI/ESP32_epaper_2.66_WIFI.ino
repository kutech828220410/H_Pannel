#include "EPD.h"
#include <AsyncUDP.h>
#include <WiFiUdp.h> 
#include <WiFi.h>
#include <Wire.h>
#include "WiFiConfig.h"
#include "Arduino.h"
#include "LED.h"
#include "FastLED.h"
#include "Output.h"
#include "Input.h" 
#include <esp_task_wdt.h>
#include "esp32-hal-cpu.h"
#include "esp_system.h"
#include "ESP32Ping.h"
#include <ArduinoJson.h>
#include <HTTPClient.h>
#include <HTTPUpdate.h>

String Version = "Ver 1.5.4";

#define SYSTEM_LED_PIN 2
#define NUM_WS2812B_CRGB 100             // LED灯珠数量
#define LED_DT 4                // Arduino输出控制信号引脚
#define LED_TYPE WS2812B         // LED灯带型号
#define COLOR_ORDER GRB         // RGB灯珠中红色、绿色、蓝色LED的排列顺序
MyLED MyLED_WS2812;
uint8_t max_bright = 70;       // LED亮度控制变量，可使用数值为 0 ～ 255， 数值越大则光带亮度越高
CRGB WS2812B_CRGB[NUM_WS2812B_CRGB];    
byte WS2812B_CRGB_BUF[NUM_WS2812B_CRGB * 3];

MyLED MyLED_IS_Connented;

byte* framebuffer_RW;
EPD epd;
WiFiConfig wiFiConfig;
int UDP_SemdTime = 0;
int Localport = 0;
IPAddress ServerIp;
int Serverport;
String GetwayStr;

bool Init = false;
MyTimer MyTimer_BoardInit;
bool flag_boradInit = false;
bool flag_httpInit = false;
bool flag_JsonSend = false;
bool flag_udp_232back = true;
#define WDT_TIMEOUT 5
const int wdtTimeout = 5;  //time in ms to trigger the watchdog
bool Core_1_feedwdt = true;
bool Core_2_feedwdt = true;

TaskHandle_t Core0Task1Handle;
TaskHandle_t Core0Task2Handle;
TaskHandle_t Core0Task3Handle;

TaskHandle_t Core1Task1Handle;
TaskHandle_t Core1Task2Handle;
TaskHandle_t Core1Task3Handle;
TaskHandle_t Core1Task4Handle;


void setup()
{
    
    Serial.begin(115200);   
    setCpuFrequencyMhz(240); 
    Serial.setRxBufferSize(1024);  
    Serial.printf("CPU Frenqency is: %d  Mhz \n",getCpuFrequencyMhz());
    
    MyLED_IS_Connented.BlinkTime = 500;
    MyLED_IS_Connented.Init(SYSTEM_LED_PIN);
  
    wiFiConfig.Init(Version);
    wiFiConfig.Set_Localport(29000);
    wiFiConfig.Set_Serverport(30000);
    Localport = 29000;
    Serverport = 30000;
    ServerIp = wiFiConfig.Get_Server_IPAdressClass();
    UDP_SemdTime = wiFiConfig.Get_UDP_SemdTime();
    GetwayStr = wiFiConfig.Get_Gateway_Str();
    
    epd.Init(); 
    LEDS.addLeds<LED_TYPE, LED_DT, COLOR_ORDER>(WS2812B_CRGB, NUM_WS2812B_CRGB);  // 初始化光带 
    FastLED.setBrightness(max_bright);                            // 设置光带亮度
    FastLED.clear();
    FastLED.show();
    
    MyLED_WS2812.BlinkTime = 1;
    MyLED_WS2812.LED_ON = WS2812_LED_ON;
    MyLED_WS2812.LED_OFF = WS2812_LED_OFF;
    MyTimer_BoardInit.StartTickTime(3000);
    
//    esp_task_wdt_init(WDT_TIMEOUT , true);
//    esp_task_wdt_add(NULL);

    xTaskCreatePinnedToCore(Core0Task1,"Core0Task1", 10000,NULL,1,&Core0Task1Handle,0);
    xTaskCreatePinnedToCore(Core0Task2,"Core0Task2", 10000,NULL,1,&Core0Task2Handle,0); 
    xTaskCreatePinnedToCore(Core0Task3,"Core0Task3", 10000,NULL,1,&Core0Task3Handle,0); 
    
    xTaskCreatePinnedToCore(Core1Task1,"Core1Task1", 10000,NULL,1,&Core1Task1Handle,1); 
    xTaskCreatePinnedToCore(Core1Task2,"Core1Task2", 10000,NULL,1,&Core1Task2Handle,1); 
    xTaskCreatePinnedToCore(Core1Task3,"Core1Task3", 10000,NULL,1,&Core1Task3Handle,1); 
    xTaskCreatePinnedToCore(Core1Task4,"Core1Task4", 10000,NULL,1,&Core1Task4Handle,1); 

}
void loop()                                                                                                                                                                                                                                                           
{    
    if(MyTimer_BoardInit.IsTimeOut() && !flag_boradInit)
    {     
       int temp = wiFiConfig.Get_PC_Restart();
       wiFiConfig.Set_PC_Restart(false);
       Serial2.begin(9600, SERIAL_8N1, 16, 17);
       Serial2.setRxBufferSize(1024); 
       flag_boradInit = true;
    }
    if(WiFi.status() == WL_CONNECTED)
    {
       if(!Init)
       {
          wiFiConfig.httpInit();
          Init = true;
       }
       wiFiConfig.HandleClient();   
    }
    if(flag_boradInit)
    {
       sub_IO_Program(); 
    }
      
}
void Core0Task1( void * pvParameters )
{
    for(;;)
    {      
      
      if(flag_boradInit)
      {
          if(WiFi.status() != WL_CONNECTED)
          {
             wiFiConfig.WIFI_Connenct();
             Connect_UDP(Localport);
          }             
      }
      MyLED_IS_Connented.Blink();
      if( WiFi.status() == WL_CONNECTED  )
      {
        MyLED_IS_Connented.BlinkTime = 100;      
      }
      else
      {
        MyLED_IS_Connented.BlinkTime = 500;
      }
      esp_task_wdt_reset();
      delay(1);
    }
    
}
void Core0Task2( void * pvParameters )
{
    for(;;)
    {      
      if(flag_boradInit)
      {          
        if( WiFi.status() == WL_CONNECTED)
        {
            sub_UDP_Send();
        }     
        else
        {
          
        }
        esp_task_wdt_reset();
      }
      delay(1);
    }
    
}
void Core0Task3( void * pvParameters )
{    
    for(;;)
    {      
      if(flag_boradInit)
      {      
        
        FastLED.show();
        if( WiFi.status() != WL_CONNECTED)
        {
            FastLED.clear();
        }     
        else
        {
            MyLED_WS2812.Blink();
        }
      }
      delay(1);
    }
}
uint8_t str_Empty[1] = {(uint8_t)(0 + 48)};
void Core1Task1( void * pvParameters )
{
  for(;;)
  {
     if(flag_boradInit)
     {
        serialEvent();
        serial2Event();                              
        if( WiFi.status() == WL_CONNECTED  )
        {          
          onPacketCallBack();          
        }
     } 
     esp_task_wdt_reset();     
  }  

}
void Core1Task2( void * pvParameters )
{
  for(;;)
  {

     if(flag_boradInit)
     {
        epd.Sleep_Check();
     }  
     esp_task_wdt_reset();
     delay(1); 
  }  

}
bool flag_OTAUpdate = false;
String OTAUpdate_IP = "";
void Core1Task3( void * pvParameters )
{
  for(;;)
  {
     if(flag_boradInit)
     {
        if(WiFi.status() == WL_CONNECTED)
        {
           if(flag_OTAUpdate)
           {
              OTAUpdate(OTAUpdate_IP , 8080);  
              flag_OTAUpdate = false;
           }
        }             
     }
     esp_task_wdt_reset(); 
     delay(1);  
  }

}
void Core1Task4( void * pvParameters )
{
    for(;;)
    {       
        if(flag_boradInit)
        {         
//          const char* GetwayChar = GetwayStr.c_str(); 
//           if( WiFi.status() == WL_CONNECTED)
//           {
//               if(!Ping.ping(GetwayChar)) 
//               {
//                   Serial.printf("ping [%d] error!" , GetwayStr);
//                   wiFiConfig.WIFI_Disconnenct();
//               }
//           }          
        } 
        esp_task_wdt_reset();
        delay(1);
    }  
}
void WS2812_LED_ON()
{
    for(int i = 0 ; i < NUM_WS2812B_CRGB ; i++)
    {
       WS2812B_CRGB[i].r = WS2812B_CRGB_BUF[i * 3 + 0];  
       WS2812B_CRGB[i].g = WS2812B_CRGB_BUF[i * 3 + 1];  
       WS2812B_CRGB[i].b = WS2812B_CRGB_BUF[i * 3 + 2];   
    } 
}
void WS2812_LED_OFF()
{
    for(int i = 0 ; i < NUM_WS2812B_CRGB ; i++)
    {
       WS2812B_CRGB[i].r = 0;
       WS2812B_CRGB[i].g = 0; 
       WS2812B_CRGB[i].b = 0;       
    } 
}
