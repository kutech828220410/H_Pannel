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

#define PIN_485_Tx_Eanble 12
#define WAKEUP_MS 100

String Version = "Ver 1.2.3";
  
MyLED MyLED_IS_Connented;
byte* framebuffer_RW;
WiFiConfig wiFiConfig;
int UDP_SemdTime = 0;
int Localport;
IPAddress ServerIp;
int Serverport;
String GetwayStr;
String CardID[5];
String CardID_buf[5];

bool Init = false;
MyTimer MyTimer_BoardInit;
bool flag_boradInit = false;
bool flag_httpInit = false;
bool flag_JsonSend = false;
bool flag_udp_232back = true;

#define WDT_TIMEOUT 5
const int wdtTimeout = 5;  //time in ms to trigger the watchdog
IPAddress Server_ip;
int Server_port;

bool Core_1_feedwdt = true;
bool Core_2_feedwdt = true;

TaskHandle_t Core0Task1Handle;
TaskHandle_t Core0Task2Handle;
TaskHandle_t Core0Task3Handle;
TaskHandle_t Core0Task4Handle;
TaskHandle_t Core0Task5Handle;
TaskHandle_t Core0Task6Handle;

TaskHandle_t Core1Task1Handle;
TaskHandle_t Core1Task2Handle;
TaskHandle_t Core1Task3Handle;
TaskHandle_t Core1Task4Handle;
TaskHandle_t Core1Task5Handle;
bool flag_CardID_IsChanged = false;
bool flag_OTAUpdate = false;
String OTAUpdate_IP = "";
void setup()
{

    Serial.begin(115200);   
    setCpuFrequencyMhz(240); 
    Serial.setRxBufferSize(1024);  
    Serial.printf("CPU Frenqency is: %d  Mhz \n",getCpuFrequencyMhz());
    pinMode(PIN_485_Tx_Eanble, OUTPUT);
    Set_RS485_Rx_Enable();
    

    wiFiConfig.Init(Version);
    Localport = wiFiConfig.Get_Localport();
    Serverport = wiFiConfig.Get_Serverport();
    ServerIp = wiFiConfig.Get_Server_IPAdressClass();
    UDP_SemdTime = wiFiConfig.Get_UDP_SemdTime();    
    Server_ip = wiFiConfig.Get_Server_IPAdressClass();
    Server_port = wiFiConfig.Get_Serverport();
    GetwayStr = wiFiConfig.Get_Gateway_Str();
    
  
    MyTimer_BoardInit.StartTickTime(3000);
    
    xTaskCreatePinnedToCore(Core0Task1,"Core0Task1", 10000,NULL,1,&Core0Task1Handle,0);
    xTaskCreatePinnedToCore(Core0Task2,"Core0Task2", 10000,NULL,1,&Core0Task2Handle,0); 
    xTaskCreatePinnedToCore(Core0Task3,"Core0Task3", 10000,NULL,1,&Core0Task3Handle,0); 
    xTaskCreatePinnedToCore(Core0Task4,"Core0Task4", 10000,NULL,1,&Core0Task4Handle,0); 
    xTaskCreatePinnedToCore(Core0Task5,"Core0Task5", 10000,NULL,1,&Core0Task5Handle,0); 
//    
//    xTaskCreatePinnedToCore(Core1Task1,"Core1Task1", 10000,NULL,1,&Core1Task1Handle,1); 
//    xTaskCreatePinnedToCore(Core1Task2,"Core1Task2", 10000,NULL,1,&Core1Task2Handle,1); 
//    xTaskCreatePinnedToCore(Core1Task3,"Core1Task3", 10000,NULL,1,&Core1Task3Handle,1); 
    
    //xTaskCreatePinnedToCore(Core1Task4,"Core1Task4", 10000,NULL,1,&Core1Task4Handle,1); 
    //xTaskCreatePinnedToCore(Core1Task5,"Core1Task5", 10000,NULL,1,&Core1Task5Handle,1); 
}

void loop()                                                                                                                                                                                                                                                           
{    
      if(MyTimer_BoardInit.IsTimeOut() && !flag_boradInit)
      {     
         Serial2.begin(9600, SERIAL_8N1, 16, 17);
         Serial2.setRxBufferSize(1024);  
         flag_boradInit = true;
      }
      if(flag_boradInit)
      {
        
        if(WiFi.status() == WL_CONNECTED)
        {
           if(!Init)
           {
              wiFiConfig.httpInit();
              Init = true;
           }
           wiFiConfig.HandleClient();   

           if(flag_OTAUpdate)
           {
              OTAUpdate(OTAUpdate_IP , 8080);  
              flag_OTAUpdate = false;
           }
        }
      }
     
}

void Core0Task1( void * pvParameters )
{
    for(;;)
    {      
      
       if(flag_boradInit)
        {
            sub_IO_Program();   
            if(WiFi.status() != WL_CONNECTED)
            {
               wiFiConfig.WIFI_Connenct();
               Connect_UDP(Localport);
            }                                 
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
        serialEvent();
        if( WiFi.status() == WL_CONNECTED )
        {
            onPacketCallBack();
        }
      }
      esp_task_wdt_reset();
      delay(1);
    }
}
void Core0Task4( void * pvParameters )
{
    for(;;)
    {      
      if(flag_boradInit)
      {
         flag_CardID_IsChanged = false;
         for(int i = 0 ; i < 5 ; i++)
         {
             if(CardID_buf[i] != CardID[i])
             {
                 CardID_buf[i] = CardID[i];
                 flag_CardID_IsChanged = true;
             }
         }
         if(flag_CardID_IsChanged) 
         {
            flag_JsonSend = true;
         }
      }
      esp_task_wdt_reset();
      delay(1);
    }
}

void Core0Task5( void * pvParameters )
{
    for(;;)
    {
       if(flag_boradInit)
       {
          sub_RFID_program();                     
       }
       esp_task_wdt_reset(); 
       delay(1);   
    }  
}

void Core1Task2( void * pvParameters )
{
  for(;;)
  {
//     flag_CardID_IsChanged = false;
//     for(int i = 0 ; i < 5 ; i++)
//     {
//         if(CardID_buf[i] != CardID[i])
//         {
//             CardID_buf[i] = CardID[i];
//             flag_CardID_IsChanged = true;
//         }
//     }
//     if(flag_CardID_IsChanged) 
//     {
//        flag_JsonSend = true;
//     }
//     esp_task_wdt_reset(); 
//     delay(1);  
  }  
}

void Core1Task3( void * pvParameters )
{
  for(;;)
  {
//     if(flag_boradInit)
//     {
//        if(WiFi.status() == WL_CONNECTED)
//        {
//           if(flag_OTAUpdate)
//           {
//              OTAUpdate(OTAUpdate_IP , 8080);  
//              flag_OTAUpdate = false;
//           }
//        }
//             
//     }
//     esp_task_wdt_reset(); 
//     delay(1);  
  }  
}


void Core1Task4( void * pvParameters )
{
    for(;;)
    {       
        if(flag_boradInit)
        {         
          const char* GetwayChar = GetwayStr.c_str(); 
           if( WiFi.status() == WL_CONNECTED)
           {
               if(!Ping.ping(GetwayChar)) 
               {
                   Serial.printf("ping [%d] error!" , GetwayStr);
                   wiFiConfig.WIFI_Disconnenct();
               }
           }          
        } 
        esp_task_wdt_reset();
        delay(1);
    }  
}
void Core1Task5( void * pvParameters )
{
    for(;;)
    {       
       if(flag_boradInit)
       {
          sub_RFID_program();                     
       }
       esp_task_wdt_reset(); 
       delay(1);   
    }  
}
uint16_t Get_CRC16(byte* pDataBytes , int len)
{
    uint16_t crc = 0xffff;
    uint16_t polynom = 0xA001;
    for (int i = 0; i < len; i++)
    {
        crc ^= *(pDataBytes + i);
        for (int j = 0; j < 8; j++)
        {
            if ((crc & 0x01) == 0x01)
            {
                crc >>= 1;
                crc ^= polynom;
            }
            else
            {
                crc >>= 1;
            }
        }
    }
    return crc;
}
