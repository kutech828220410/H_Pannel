#include <Arduino.h>
#line 1 "C:\\Users\\Evan\\source\\repos\\H_Pannel\\MCU-BW16\\BW16_EPD266\\BW16_EPD266.ino"
#include "Config.h"
#include "EPD.h"
#include "DHT.h"
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
bool flag_WS2812B_breathing_Ex_ON_OFF  = false;
bool flag_WS2812B_breathing_Ex_lightOff  = false;
bool flag_WS2812B_breathing_Ex_dTrigger = false;
int WS2812B_breathing_ON_OFF_cnt = 0;
int WS2812B_breathing_Ex_ON_OFF_cnt = 0;
int WS2812B_breathing_onAddVal  = 20;
int WS2812B_breathing_offSubVal  = 20;
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
SemaphoreHandle_t xSpiMutex;

SoftwareSerial mySerial(PA8, PA7); // RX, TX
SoftwareSerial mySerial2(PB2, PB1); // RX, TX

#ifdef DHTSensor
DHT dht(DHTPIN, DHTTYPE);
float dht_h = 0;
float dht_t = 0;
float dht_f = 0;
float dht_hif = 0;
float dht_hic = 0;
#endif


#line 93 "C:\\Users\\Evan\\source\\repos\\H_Pannel\\MCU-BW16\\BW16_EPD266\\BW16_EPD266.ino"
void setup();
#line 103 "C:\\Users\\Evan\\source\\repos\\H_Pannel\\MCU-BW16\\BW16_EPD266\\BW16_EPD266.ino"
void loop();
#line 234 "C:\\Users\\Evan\\source\\repos\\H_Pannel\\MCU-BW16\\BW16_EPD266\\BW16_EPD266.ino"
void Core0Task1( void * pvParameters );
#line 279 "C:\\Users\\Evan\\source\\repos\\H_Pannel\\MCU-BW16\\BW16_EPD266\\BW16_EPD266.ino"
void Core0Task2( void * pvParameters );
#line 313 "C:\\Users\\Evan\\source\\repos\\H_Pannel\\MCU-BW16\\BW16_EPD266\\BW16_EPD266.ino"
void WS2812B_breathing_ON_OFF();
#line 358 "C:\\Users\\Evan\\source\\repos\\H_Pannel\\MCU-BW16\\BW16_EPD266\\BW16_EPD266.ino"
void WS2812B_breathing_Ex_ON_OFF();
#line 79 "C:\\Users\\Evan\\source\\repos\\H_Pannel\\MCU-BW16\\BW16_EPD266\\IO.ino"
void IO_Init();
#line 138 "C:\\Users\\Evan\\source\\repos\\H_Pannel\\MCU-BW16\\BW16_EPD266\\IO.ino"
void sub_IO_Program();
#line 192 "C:\\Users\\Evan\\source\\repos\\H_Pannel\\MCU-BW16\\BW16_EPD266\\IO.ino"
void Output_Blink();
#line 205 "C:\\Users\\Evan\\source\\repos\\H_Pannel\\MCU-BW16\\BW16_EPD266\\IO.ino"
void Set_Input_dir(byte pin_num , bool value);
#line 218 "C:\\Users\\Evan\\source\\repos\\H_Pannel\\MCU-BW16\\BW16_EPD266\\IO.ino"
void Set_Input_dir(int value);
#line 241 "C:\\Users\\Evan\\source\\repos\\H_Pannel\\MCU-BW16\\BW16_EPD266\\IO.ino"
int Get_Input_dir();
#line 256 "C:\\Users\\Evan\\source\\repos\\H_Pannel\\MCU-BW16\\BW16_EPD266\\IO.ino"
void Set_Output_dir(byte pin_num , bool value);
#line 269 "C:\\Users\\Evan\\source\\repos\\H_Pannel\\MCU-BW16\\BW16_EPD266\\IO.ino"
void Set_Output_dir(int value);
#line 292 "C:\\Users\\Evan\\source\\repos\\H_Pannel\\MCU-BW16\\BW16_EPD266\\IO.ino"
int Get_Output_dir();
#line 309 "C:\\Users\\Evan\\source\\repos\\H_Pannel\\MCU-BW16\\BW16_EPD266\\IO.ino"
int GetInput();
#line 337 "C:\\Users\\Evan\\source\\repos\\H_Pannel\\MCU-BW16\\BW16_EPD266\\IO.ino"
int GetOutput();
#line 353 "C:\\Users\\Evan\\source\\repos\\H_Pannel\\MCU-BW16\\BW16_EPD266\\IO.ino"
void SetOutputTrigger(int value);
#line 366 "C:\\Users\\Evan\\source\\repos\\H_Pannel\\MCU-BW16\\BW16_EPD266\\IO.ino"
void SetOutputPINTrigger(byte pin_num , bool value);
#line 380 "C:\\Users\\Evan\\source\\repos\\H_Pannel\\MCU-BW16\\BW16_EPD266\\IO.ino"
void SetOutput(int value);
#line 428 "C:\\Users\\Evan\\source\\repos\\H_Pannel\\MCU-BW16\\BW16_EPD266\\IO.ino"
void SetOutputPIN(byte pin_num , bool value);
#line 9 "C:\\Users\\Evan\\source\\repos\\H_Pannel\\MCU-BW16\\BW16_EPD266\\UART.ino"
void serialEvent();
#line 345 "C:\\Users\\Evan\\source\\repos\\H_Pannel\\MCU-BW16\\BW16_EPD266\\UART.ino"
void Get_Checksum();
#line 14 "C:\\Users\\Evan\\source\\repos\\H_Pannel\\MCU-BW16\\BW16_EPD266\\UART2.ino"
void serial2Event();
#line 10 "C:\\Users\\Evan\\source\\repos\\H_Pannel\\MCU-BW16\\BW16_EPD266\\UDP_JsonSend.ino"
void sub_UDP_Send();
#line 24 "C:\\Users\\Evan\\source\\repos\\H_Pannel\\MCU-BW16\\BW16_EPD266\\UDP_Receive.ino"
void onPacketCallBack();
#line 636 "C:\\Users\\Evan\\source\\repos\\H_Pannel\\MCU-BW16\\BW16_EPD266\\UDP_Receive.ino"
void Clear_char_buf();
#line 640 "C:\\Users\\Evan\\source\\repos\\H_Pannel\\MCU-BW16\\BW16_EPD266\\UDP_Receive.ino"
void Get_Checksum_UDP();
#line 656 "C:\\Users\\Evan\\source\\repos\\H_Pannel\\MCU-BW16\\BW16_EPD266\\UDP_Receive.ino"
void Connect_UDP(int localport);
#line 662 "C:\\Users\\Evan\\source\\repos\\H_Pannel\\MCU-BW16\\BW16_EPD266\\UDP_Receive.ino"
void Send_Bytes(uint8_t *value ,int Size ,IPAddress RemoteIP ,int RemotePort);
#line 668 "C:\\Users\\Evan\\source\\repos\\H_Pannel\\MCU-BW16\\BW16_EPD266\\UDP_Receive.ino"
void Send_String(String str ,int remoteUdpPort);
#line 674 "C:\\Users\\Evan\\source\\repos\\H_Pannel\\MCU-BW16\\BW16_EPD266\\UDP_Receive.ino"
void Send_StringTo(String str ,IPAddress RemoteIP ,int RemotePort);
#line 682 "C:\\Users\\Evan\\source\\repos\\H_Pannel\\MCU-BW16\\BW16_EPD266\\UDP_Receive.ino"
String IpAddress2String(const IPAddress& ipAddress);
#line 93 "C:\\Users\\Evan\\source\\repos\\H_Pannel\\MCU-BW16\\BW16_EPD266\\BW16_EPD266.ino"
void setup() 
{
    MyTimer_BoardInit.StartTickTime(3000);          
    MyTimer_OLCD_144_Init.StartTickTime(5000);          
    MyTimer_CheckWIFI.StartTickTime(180000);   
    // 初始化互斥鎖
    xSpiMutex = xSemaphoreCreateMutex();

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
      if(EPD_TYPE == "EPD420_D")
      {
         wiFiConfig.Set_Localport(29005);
         wiFiConfig.Set_Serverport(30005);
      }
      if(Device == "RowLED") wiFiConfig.Set_Serverport(30001);
      if(Device == "OLCD114") 
      {
        wiFiConfig.Set_Localport(29008);
        wiFiConfig.Set_Serverport(30008);
      }
      #ifdef DHTSensor
      dht.begin();
      #endif
      Localport = wiFiConfig.Get_Localport();
      Serverport = wiFiConfig.Get_Serverport();
      ServerIp = wiFiConfig.Get_Server_IPAdressClass();
      UDP_SemdTime = wiFiConfig.Get_UDP_SemdTime();
      GetwayStr = wiFiConfig.Get_Gateway_Str();
      MyLED_IS_Connented.Init(SYSTEM_LED_PIN);
      SPI.begin(); //SCLK, MISO, MOSI, SS
      delay(200);
      myWS2812.Init(NUM_WS2812B_CRGB , xSpiMutex);
      
      if(Device == "EPD")
      {
        mySerial.println("EPD device init ...");
        epd.Init(xSpiMutex); 
        delay(200);
      }
     
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
          #ifdef MQTT
          wiFiConfig.MQTT_reconnect();        
          #else         
          onPacketCallBack();
          #endif
          sub_UDP_Send();
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
          else if(flag_WS2812B_breathing_Ex_ON_OFF)
          {
             WS2812B_breathing_Ex_ON_OFF();
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
          #ifdef DHTSensor
          dht_h = dht.readHumidity();
          // Read temperature as Celsius (the default)
          dht_t = dht.readTemperature();
          // Read temperature as Fahrenheit (isFahrenheit = true)
          dht_f = dht.readTemperature(true);
          // Check if any reads failed and exit early (to try again).
          if (isnan(dht_h) || isnan(dht_t) || isnan(dht_f)) 
          {
              mySerial.println(F("Failed to read from DHT sensor!"));
          }
          // Compute heat index in Fahrenheit (the default)
          dht_hif = dht.computeHeatIndex(dht_f, dht_h);
          // Compute heat index in Celsius (isFahreheit = false)
          dht_hic = dht.computeHeatIndex(dht_t, dht_h, false);
          #endif
          
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
void WS2812B_breathing_Ex_ON_OFF()
{
   if(flag_WS2812B_breathing_Ex_lightOff)
   {
       int numofLED = NUM_WS2812B_CRGB;
       byte bytes[numofLED * 3];
       for(int i = 0 ; i < numofLED ; i++)
       {             
         bytes[i * 3 + 0 + 0] = 0;
         bytes[i * 3 + 0 + 1] = 0;
         bytes[i * 3 + 0 + 2] = 0;
       }
       WS2812B_breathing_Ex_ON_OFF_cnt = 0;
       flag_WS2812B_breathing_Ex_ON_OFF = false;
       flag_WS2812B_breathing_Ex_lightOff= false;
       myWS2812.Show(bytes ,numofLED);
       return;
   }
   if(WS2812B_breathing_Ex_ON_OFF_cnt == 0)
   {
       int numofLED = NUM_WS2812B_CRGB;
       bool flag_black = true;
       WS2812B_breathing_val += ((float)WS2812B_breathing_onAddVal * 0.01);
       if(WS2812B_breathing_val >= 0.9) 
       {
           WS2812B_breathing_Ex_ON_OFF_cnt++;
       }
       byte bytes[numofLED * 3];
       for(int i = 0 ; i < numofLED ; i++)
       {             
         bytes[i * 3 + 0 + 0] = (int)(myWS2812.rgbBuffer[i * 3 + 0 + 0] * WS2812B_breathing_val);     // 将光带上第1个LED灯珠的RGB数值中R数值设置为255
         bytes[i * 3 + 0 + 1] = (int)(myWS2812.rgbBuffer[i * 3 + 0 + 1] * WS2812B_breathing_val);   // 将光带上第1个LED灯珠的RGB数值中G数值设置为255
         bytes[i * 3 + 0 + 2] = (int)(myWS2812.rgbBuffer[i * 3 + 0 + 2] * WS2812B_breathing_val);      // 将光带上第1个LED灯珠的RGB数值中B数值设置为0      

         if(myWS2812.rgbBuffer[i * 3 + 0 + 0] != 0 || myWS2812.rgbBuffer[i * 3 + 0 + 1]!= 0 || myWS2812.rgbBuffer[i * 3 + 0 + 2] != 0)flag_black = false;
       }                                                  
       myWS2812.Show(bytes ,numofLED);

   }
   if(WS2812B_breathing_Ex_ON_OFF_cnt == 1)
   {
       int numofLED = NUM_WS2812B_CRGB;
       bool flag_black = true;
       WS2812B_breathing_val -= ((float)WS2812B_breathing_offSubVal * 0.01);
       if(WS2812B_breathing_val < 0) WS2812B_breathing_val = 0;
       if(WS2812B_breathing_val <= 0.1) 
       {
           WS2812B_breathing_Ex_ON_OFF_cnt++;
       }
       byte bytes[numofLED * 3];
       for(int i = 0 ; i < numofLED ; i++)
       {             
         bytes[i * 3 + 0 + 0] = (int)(myWS2812.rgbBuffer[i * 3 + 0 + 0] * WS2812B_breathing_val);     // 将光带上第1个LED灯珠的RGB数值中R数值设置为255
         bytes[i * 3 + 0 + 1] = (int)(myWS2812.rgbBuffer[i * 3 + 0 + 1] * WS2812B_breathing_val);   // 将光带上第1个LED灯珠的RGB数值中G数值设置为255
         bytes[i * 3 + 0 + 2] = (int)(myWS2812.rgbBuffer[i * 3 + 0 + 2] * WS2812B_breathing_val);      // 将光带上第1个LED灯珠的RGB数值中B数值设置为0    

         if(myWS2812.rgbBuffer[i * 3 + 0 + 0] != 0 || myWS2812.rgbBuffer[i * 3 + 0 + 1]!= 0 || myWS2812.rgbBuffer[i * 3 + 0 + 2] != 0)flag_black = false;
       }                                                  
       myWS2812.Show(bytes ,numofLED);

   }
   if(WS2812B_breathing_Ex_ON_OFF_cnt == 2)
   {
      WS2812B_breathing_Ex_ON_OFF_cnt = 0;
   }
}

#line 1 "C:\\Users\\Evan\\source\\repos\\H_Pannel\\MCU-BW16\\BW16_EPD266\\IO.ino"
#ifdef MCP23017
#define INPUT_PIN01 0
#define INPUT_PIN02 1
#define INPUT_PIN03 2
#define INPUT_PIN04 3
#define INPUT_PIN05 4
#define INPUT_PIN06 5
#define INPUT_PIN07 6
#define INPUT_PIN08 7
#define INPUT_PIN09 -1
#define INPUT_PIN10 -1

#define OUTPUT_PIN01 8
#define OUTPUT_PIN02 9
#define OUTPUT_PIN03 10
#define OUTPUT_PIN04 11
#define OUTPUT_PIN05 12
#define OUTPUT_PIN06 13
#define OUTPUT_PIN07 14
#define OUTPUT_PIN08 15
#define OUTPUT_PIN09 -1
#define OUTPUT_PIN10 -1
#else
#define INPUT_PIN01 PB2
#define INPUT_PIN02 -1
#define INPUT_PIN03 -1
#define INPUT_PIN04 -1
#define INPUT_PIN05 -1
#define INPUT_PIN06 -1
#define INPUT_PIN07 -1
#define INPUT_PIN08 -1
#define INPUT_PIN09 -1
#define INPUT_PIN10 -1

#define OUTPUT_PIN01 PB1
#define OUTPUT_PIN02 -1
#define OUTPUT_PIN03 -1
#define OUTPUT_PIN04 -1
#define OUTPUT_PIN05 -1
#define OUTPUT_PIN06 -1
#define OUTPUT_PIN07 -1
#define OUTPUT_PIN08 -1
#define OUTPUT_PIN09 -1
#define OUTPUT_PIN10 -1
#endif

MyOutput MyOutput_PIN01;
MyOutput MyOutput_PIN02;
MyOutput MyOutput_PIN03;
MyOutput MyOutput_PIN04;
MyOutput MyOutput_PIN05;
MyOutput MyOutput_PIN06;
MyOutput MyOutput_PIN07;
MyOutput MyOutput_PIN08;
MyOutput MyOutput_PIN09;
MyOutput MyOutput_PIN10;

MyInput MyInput_PIN01;
MyInput MyInput_PIN02;
MyInput MyInput_PIN03;
MyInput MyInput_PIN04;
MyInput MyInput_PIN05;
MyInput MyInput_PIN06;
MyInput MyInput_PIN07;
MyInput MyInput_PIN08;
MyInput MyInput_PIN09;
MyInput MyInput_PIN10;

int Input_buf = -1;
int Input = 0;
int Output_buf = -1;
int Output = 0;
int Input_dir_buf = -1;
int Input_dir = 0;
int Output_dir_buf = -1;
int Output_dir = 0;
bool flag_Init = false;

void IO_Init()
{       

    Input_dir = wiFiConfig.Get_Input_dir();
    Set_Input_dir(Input_dir);
    Output_dir = wiFiConfig.Get_Output_dir();
    Set_Output_dir(Output_dir);

    #ifdef MCP23017
    MyOutput_PIN01.Init(OUTPUT_PIN01 ,mcp);
    MyOutput_PIN02.Init(OUTPUT_PIN02 ,mcp);
    MyOutput_PIN03.Init(OUTPUT_PIN03 ,mcp);
    MyOutput_PIN04.Init(OUTPUT_PIN04 ,mcp);
    MyOutput_PIN05.Init(OUTPUT_PIN05 ,mcp);
    MyOutput_PIN06.Init(OUTPUT_PIN06 ,mcp);
    MyOutput_PIN07.Init(OUTPUT_PIN07 ,mcp);
    MyOutput_PIN08.Init(OUTPUT_PIN08 ,mcp);
    MyOutput_PIN09.Init(OUTPUT_PIN09);
    MyOutput_PIN10.Init(OUTPUT_PIN10);
    
    MyInput_PIN01.Init(INPUT_PIN01 ,mcp);
    MyInput_PIN02.Init(INPUT_PIN02 ,mcp);
    MyInput_PIN03.Init(INPUT_PIN03 ,mcp);
    MyInput_PIN04.Init(INPUT_PIN04 ,mcp);
    MyInput_PIN05.Init(INPUT_PIN05 ,mcp);
    MyInput_PIN06.Init(INPUT_PIN06 ,mcp);
    MyInput_PIN07.Init(INPUT_PIN07 ,mcp);
    MyInput_PIN08.Init(INPUT_PIN08 ,mcp);
    MyInput_PIN09.Init(INPUT_PIN09);
    MyInput_PIN10.Init(INPUT_PIN10);
    #else
    MyOutput_PIN01.Init(INPUT_PIN01,OUTPUT_PIN01);
    MyOutput_PIN02.Init(OUTPUT_PIN02);
    MyOutput_PIN03.Init(OUTPUT_PIN03);
    MyOutput_PIN04.Init(OUTPUT_PIN04);
    MyOutput_PIN05.Init(OUTPUT_PIN05);
    MyOutput_PIN06.Init(OUTPUT_PIN06);
    MyOutput_PIN07.Init(OUTPUT_PIN07);
    MyOutput_PIN08.Init(OUTPUT_PIN08);
    MyOutput_PIN09.Init(OUTPUT_PIN09);
    MyOutput_PIN10.Init(OUTPUT_PIN10);
    
    MyInput_PIN01.Init(INPUT_PIN01);
    MyInput_PIN02.Init(INPUT_PIN02);
    MyInput_PIN03.Init(INPUT_PIN03);
    MyInput_PIN04.Init(INPUT_PIN04);
    MyInput_PIN05.Init(INPUT_PIN05);
    MyInput_PIN06.Init(INPUT_PIN06);
    MyInput_PIN07.Init(INPUT_PIN07);
    MyInput_PIN08.Init(INPUT_PIN08);
    MyInput_PIN09.Init(INPUT_PIN09);
    MyInput_PIN10.Init(INPUT_PIN10);
    #endif
 

  
}
bool flag_WL_CONNECTED = false;
bool flag_WL_DISCONNECTED = false;
void sub_IO_Program()
{

//    if(!flag_Init)
//    {
//       IO_Init();
//       flag_Init = true;
//    }
    Input = GetInput();
    Output = GetOutput();
    Input_dir = Get_Input_dir();
    Output_dir = Get_Output_dir();
//    if(WiFi.status() == WL_CONNECTED)
//    {
//       if(flag_WL_CONNECTED)
//       {
//          Set_Output_dir(wiFiConfig.Get_Output_dir());
//          flag_WL_CONNECTED = false;
//       }
//       
//       flag_WL_DISCONNECTED = true;
//    }
//    else
//    {
//       if(flag_WL_DISCONNECTED)
//       {
//          Set_Output_dir(0);
//          flag_WL_DISCONNECTED = false;
//       }
//       flag_WL_CONNECTED = true;
//    }
    Output_Blink();
    if(Input_buf != Input)
    {       
       Input_buf = Input;
       flag_JsonSend = true;
    }
    if(Output_buf != Output)
    {       
       Output_buf = Output;
       flag_JsonSend = true;
    }
    if(Input_dir_buf != Input_dir)
    {       
       Input_dir_buf = Input_dir;
       flag_JsonSend = true;
    }
    if(Output_dir_buf != Output_dir)
    {       
       Output_dir_buf = Output_dir;
       flag_JsonSend = true;
    }
}

void Output_Blink()
{
    MyOutput_PIN01.Blink(2000);
    MyOutput_PIN02.Blink(2000);
    MyOutput_PIN03.Blink(2000);
    MyOutput_PIN04.Blink(2000);
    MyOutput_PIN05.Blink(2000);
    MyOutput_PIN06.Blink(2000);
    MyOutput_PIN07.Blink(2000);
    MyOutput_PIN08.Blink(2000);
    MyOutput_PIN09.Blink(2000);
    MyOutput_PIN10.Blink(2000);
}
void Set_Input_dir(byte pin_num , bool value)
{
    if(pin_num == 1) MyInput_PIN01.Set_toggle(value);
    else if(pin_num == 2) MyInput_PIN02.Set_toggle(value);
    else if(pin_num == 3) MyInput_PIN03.Set_toggle(value);
    else if(pin_num == 4) MyInput_PIN04.Set_toggle(value);
    else if(pin_num == 5) MyInput_PIN05.Set_toggle(value);
    else if(pin_num == 6) MyInput_PIN06.Set_toggle(value);
    else if(pin_num == 7) MyInput_PIN07.Set_toggle(value);
    else if(pin_num == 8) MyInput_PIN08.Set_toggle(value);
    else if(pin_num == 9) MyInput_PIN09.Set_toggle(value);
    else if(pin_num == 10) MyInput_PIN10.Set_toggle(value);
}
void Set_Input_dir(int value)
{
    if(((value >> 0) % 2 ) ==  1) MyInput_PIN01.Set_toggle(true);
    else MyInput_PIN01.Set_toggle(false);
    if(((value >> 1) % 2 ) ==  1) MyInput_PIN02.Set_toggle(true);
    else MyInput_PIN02.Set_toggle(false);
    if(((value >> 2) % 2 ) ==  1) MyInput_PIN03.Set_toggle(true);
    else MyInput_PIN03.Set_toggle(false);
    if(((value >> 3) % 2 ) ==  1) MyInput_PIN04.Set_toggle(true);
    else MyInput_PIN04.Set_toggle(false);
    if(((value >> 4) % 2 ) ==  1) MyInput_PIN05.Set_toggle(true);
    else MyInput_PIN05.Set_toggle(false);
    if(((value >> 5) % 2 ) ==  1) MyInput_PIN06.Set_toggle(true);
    else MyInput_PIN06.Set_toggle(false);
    if(((value >> 6) % 2 ) ==  1) MyInput_PIN07.Set_toggle(true);
    else MyInput_PIN07.Set_toggle(false);
    if(((value >> 7) % 2 ) ==  1) MyInput_PIN08.Set_toggle(true);
    else MyInput_PIN08.Set_toggle(false);
    if(((value >> 8) % 2 ) ==  1) MyInput_PIN09.Set_toggle(true);
    else MyInput_PIN09.Set_toggle(false);
    if(((value >> 9) % 2 ) ==  1) MyInput_PIN10.Set_toggle(true);
    else MyInput_PIN10.Set_toggle(false);
}
int Get_Input_dir()
{
    int temp = 0;
    if(MyInput_PIN01.flag_toogle) temp += (1 << 0);
    if(MyInput_PIN02.flag_toogle) temp += (1 << 1);
    if(MyInput_PIN03.flag_toogle) temp += (1 << 2);
    if(MyInput_PIN04.flag_toogle) temp += (1 << 3);
    if(MyInput_PIN05.flag_toogle) temp += (1 << 4);
    if(MyInput_PIN06.flag_toogle) temp += (1 << 5);
    if(MyInput_PIN07.flag_toogle) temp += (1 << 6);
    if(MyInput_PIN08.flag_toogle) temp += (1 << 7);
    if(MyInput_PIN09.flag_toogle) temp += (1 << 8);
    if(MyInput_PIN10.flag_toogle) temp += (1 << 9);
    return temp;
}
void Set_Output_dir(byte pin_num , bool value)
{
    if(pin_num == 1) MyOutput_PIN01.Set_toggle(value);
    else if(pin_num == 2) MyOutput_PIN02.Set_toggle(value);
    else if(pin_num == 3) MyOutput_PIN03.Set_toggle(value);
    else if(pin_num == 4) MyOutput_PIN04.Set_toggle(value);
    else if(pin_num == 5) MyOutput_PIN05.Set_toggle(value);
    else if(pin_num == 6) MyOutput_PIN06.Set_toggle(value);
    else if(pin_num == 7) MyOutput_PIN07.Set_toggle(value);
    else if(pin_num == 8) MyOutput_PIN08.Set_toggle(value);
    else if(pin_num == 9) MyOutput_PIN09.Set_toggle(value);
    else if(pin_num == 10) MyOutput_PIN10.Set_toggle(value);
}
void Set_Output_dir(int value)
{
    if(((value >> 0) % 2 ) ==  1) MyOutput_PIN01.Set_toggle(true);
    else MyOutput_PIN01.Set_toggle(false);
    if(((value >> 1) % 2 ) ==  1) MyOutput_PIN02.Set_toggle(true);
    else MyOutput_PIN02.Set_toggle(false);
    if(((value >> 2) % 2 ) ==  1) MyOutput_PIN03.Set_toggle(true);
    else MyOutput_PIN03.Set_toggle(false);
    if(((value >> 3) % 2 ) ==  1) MyOutput_PIN04.Set_toggle(true);
    else MyOutput_PIN04.Set_toggle(false);
    if(((value >> 4) % 2 ) ==  1) MyOutput_PIN05.Set_toggle(true);
    else MyOutput_PIN05.Set_toggle(false);
    if(((value >> 5) % 2 ) ==  1) MyOutput_PIN06.Set_toggle(true);
    else MyOutput_PIN06.Set_toggle(false);
    if(((value >> 6) % 2 ) ==  1) MyOutput_PIN07.Set_toggle(true);
    else MyOutput_PIN07.Set_toggle(false);
    if(((value >> 7) % 2 ) ==  1) MyOutput_PIN08.Set_toggle(true);
    else MyOutput_PIN08.Set_toggle(false);
    if(((value >> 8) % 2 ) ==  1) MyOutput_PIN09.Set_toggle(true);
    else MyOutput_PIN09.Set_toggle(false);
    if(((value >> 9) % 2 ) ==  1) MyOutput_PIN10.Set_toggle(true);
    else MyOutput_PIN10.Set_toggle(false);
}
int Get_Output_dir()
{
    int temp = 0;
    if(MyOutput_PIN01.flag_toogle) temp += (1 << 0);
    if(MyOutput_PIN02.flag_toogle) temp += (1 << 1);
    if(MyOutput_PIN03.flag_toogle) temp += (1 << 2);
    if(MyOutput_PIN04.flag_toogle) temp += (1 << 3);
    if(MyOutput_PIN05.flag_toogle) temp += (1 << 4);
    if(MyOutput_PIN06.flag_toogle) temp += (1 << 5);
    if(MyOutput_PIN07.flag_toogle) temp += (1 << 6);
    if(MyOutput_PIN08.flag_toogle) temp += (1 << 7);
    if(MyOutput_PIN09.flag_toogle) temp += (1 << 8);
    if(MyOutput_PIN10.flag_toogle) temp += (1 << 9);
    return temp;

}

int GetInput()
{
    int temp = 0;
    MyInput_PIN01.GetState(10);
    MyInput_PIN02.GetState(10);
    MyInput_PIN03.GetState(10);
    MyInput_PIN04.GetState(10);
    MyInput_PIN05.GetState(10);
    MyInput_PIN06.GetState(10);
    MyInput_PIN07.GetState(10);
    MyInput_PIN08.GetState(10);
    MyInput_PIN09.GetState(10);
    MyInput_PIN10.GetState(10);
    
    if(MyInput_PIN01.State) temp += (1 << 0);
    if(MyInput_PIN02.State) temp += (1 << 1);
    if(MyInput_PIN03.State) temp += (1 << 2);
    if(MyInput_PIN04.State) temp += (1 << 3);
    if(MyInput_PIN05.State) temp += (1 << 4);
    if(MyInput_PIN06.State) temp += (1 << 5);
    if(MyInput_PIN07.State) temp += (1 << 6);
    if(MyInput_PIN08.State) temp += (1 << 7);
    if(MyInput_PIN09.State) temp += (1 << 8);
    if(MyInput_PIN10.State) temp += (1 << 9);
  
    return temp;
}

int GetOutput()
{
    int temp = 0;
    
    if(MyOutput_PIN01.State) temp += (1 << 0);
    if(MyOutput_PIN02.State) temp += (1 << 1);
    if(MyOutput_PIN03.State) temp += (1 << 2);
    if(MyOutput_PIN04.State) temp += (1 << 3);
    if(MyOutput_PIN05.State) temp += (1 << 4);
    if(MyOutput_PIN06.State) temp += (1 << 5);
    if(MyOutput_PIN07.State) temp += (1 << 6);
    if(MyOutput_PIN08.State) temp += (1 << 7);
    if(MyOutput_PIN09.State) temp += (1 << 8);
    if(MyOutput_PIN10.State) temp += (1 << 9);
    return temp;
}
void SetOutputTrigger(int value)
{
    if(((value >> 0) % 2 ) ==  1) MyOutput_PIN01.Trigger = true;
    if(((value >> 1) % 2 ) ==  1) MyOutput_PIN02.Trigger = true;
    if(((value >> 2) % 2 ) ==  1) MyOutput_PIN03.Trigger = true;
    if(((value >> 3) % 2 ) ==  1) MyOutput_PIN04.Trigger = true;
    if(((value >> 4) % 2 ) ==  1) MyOutput_PIN05.Trigger = true;
    if(((value >> 5) % 2 ) ==  1) MyOutput_PIN06.Trigger = true;
    if(((value >> 6) % 2 ) ==  1) MyOutput_PIN07.Trigger = true;
    if(((value >> 7) % 2 ) ==  1) MyOutput_PIN08.Trigger = true;
    if(((value >> 8) % 2 ) ==  1) MyOutput_PIN09.Trigger = true;
    if(((value >> 9) % 2 ) ==  1) MyOutput_PIN10.Trigger = true;
}
void SetOutputPINTrigger(byte pin_num , bool value)
{
    if(pin_num == 1) MyOutput_PIN01.Trigger = true;
    else if(pin_num == 2) MyOutput_PIN02.Trigger = true;
    else if(pin_num == 3) MyOutput_PIN03.Trigger = true;
    else if(pin_num == 4) MyOutput_PIN04.Trigger = true;
    else if(pin_num == 5) MyOutput_PIN05.Trigger = true;
    else if(pin_num == 6) MyOutput_PIN06.Trigger = true;
    else if(pin_num == 7) MyOutput_PIN07.Trigger = true;
    else if(pin_num == 8) MyOutput_PIN08.Trigger = true;
    else if(pin_num == 9) MyOutput_PIN09.Trigger = true;
    else if(pin_num == 10) MyOutput_PIN10.Trigger = true;
 
}
void SetOutput(int value)
{
    if(((value >> 0) % 2 ) ==  0) MyOutput_PIN01.Set_State(true);
    else MyOutput_PIN01.Set_State(false);
    if(((value >> 1) % 2 ) ==  0) MyOutput_PIN02.Set_State(true);
    else MyOutput_PIN02.Set_State(false);
    if(((value >> 2) % 2 ) ==  0) MyOutput_PIN03.Set_State(true);
    else MyOutput_PIN03.Set_State(false);
    if(((value >> 3) % 2 ) ==  0) MyOutput_PIN04.Set_State(true);
    else MyOutput_PIN04.Set_State(false);
    if(((value >> 4) % 2 ) ==  0) MyOutput_PIN05.Set_State(true);
    else MyOutput_PIN05.Set_State(false);
    if(((value >> 5) % 2 ) ==  0) MyOutput_PIN06.Set_State(true);
    else MyOutput_PIN06.Set_State(false);
    if(((value >> 6) % 2 ) ==  0) MyOutput_PIN07.Set_State(true);
    else MyOutput_PIN07.Set_State(false);
    if(((value >> 7) % 2 ) ==  0) MyOutput_PIN08.Set_State(true);
    else MyOutput_PIN08.Set_State(false);
    if(((value >> 8) % 2 ) ==  0) MyOutput_PIN09.Set_State(true);
    else MyOutput_PIN09.Set_State(false);
    if(((value >> 9) % 2 ) ==  0) MyOutput_PIN10.Set_State(true);
    else MyOutput_PIN10.Set_State(false);   
}
#ifdef MCP23017
void SetOutputEx(int value)
{
    if(((value >> 0) % 2 ) ==  0) MyOutput_PIN01.Set_StateEx(true);
    else MyOutput_PIN01.Set_StateEx(false);
    if(((value >> 1) % 2 ) ==  0) MyOutput_PIN02.Set_StateEx(true);
    else MyOutput_PIN02.Set_StateEx(false);
    if(((value >> 2) % 2 ) ==  0) MyOutput_PIN03.Set_StateEx(true);
    else MyOutput_PIN03.Set_StateEx(false);
    if(((value >> 3) % 2 ) ==  0) MyOutput_PIN04.Set_StateEx(true);
    else MyOutput_PIN04.Set_StateEx(false);
    if(((value >> 4) % 2 ) ==  0) MyOutput_PIN05.Set_StateEx(true);
    else MyOutput_PIN05.Set_StateEx(false);
    if(((value >> 5) % 2 ) ==  0) MyOutput_PIN06.Set_StateEx(true);
    else MyOutput_PIN06.Set_StateEx(false);
    if(((value >> 6) % 2 ) ==  0) MyOutput_PIN07.Set_StateEx(true);
    else MyOutput_PIN07.Set_StateEx(false);
    if(((value >> 7) % 2 ) ==  0) MyOutput_PIN08.Set_StateEx(true);
    else MyOutput_PIN08.Set_StateEx(false);
    if(((value >> 8) % 2 ) ==  0) MyOutput_PIN09.Set_StateEx(true);
    else MyOutput_PIN09.Set_StateEx(false);
    if(((value >> 9) % 2 ) ==  0) MyOutput_PIN10.Set_StateEx(true);
    else MyOutput_PIN10.Set_StateEx(false);   
}
#endif
void SetOutputPIN(byte pin_num , bool value)
{
    if(pin_num == 1) MyOutput_PIN01.Set_State(value);
    else if(pin_num == 2) MyOutput_PIN02.Set_State(value);
    else if(pin_num == 3) MyOutput_PIN03.Set_State(value);
    else if(pin_num == 4) MyOutput_PIN04.Set_State(value);
    else if(pin_num == 5) MyOutput_PIN05.Set_State(value);
    else if(pin_num == 6) MyOutput_PIN06.Set_State(value);
    else if(pin_num == 7) MyOutput_PIN07.Set_State(value);
    else if(pin_num == 8) MyOutput_PIN08.Set_State(value);
    else if(pin_num == 9) MyOutput_PIN09.Set_State(value);
    else if(pin_num == 10) MyOutput_PIN10.Set_State(value);
}

#line 1 "C:\\Users\\Evan\\source\\repos\\H_Pannel\\MCU-BW16\\BW16_EPD266\\UART.ino"
#include "Timer.h"
#define UART0_RX_SIZE 512
static byte UART0_RX[UART0_RX_SIZE];
int UART0_len = 0;
MyTimer MyTimer_UART0;

#define RESOURCE "OTA_All.bin"    

void serialEvent()
{

  if (mySerial.available())
  {
    UART0_RX[UART0_len] = mySerial.read();
    UART0_len++;
    MyTimer_UART0.TickStop();
    MyTimer_UART0.StartTickTime(10);
  }
  if (MyTimer_UART0.IsTimeOut())
  {
    MyTimer_UART0.TickStop();
    MyTimer_UART0.StartTickTime(1000);
    if (UART0_RX[0] == 'o')
    {
      String serverIP = wiFiConfig.server_IPAdress_str;
      int str_len = serverIP.length() + 1; 
      char serverIP_char_array[str_len];
      serverIP.toCharArray(serverIP_char_array , str_len );
      mySerial.print("Host :");
      mySerial.print(serverIP_char_array);
      mySerial.print(", port :");
      mySerial.println(8080);
      
      int ret = http_update_ota(serverIP_char_array, 8080, RESOURCE);
      printf("[%s] Update task exit\n\r", __FUNCTION__);
      if(!ret)
      {
        printf("[%s] Ready to reboot\n\r", __FUNCTION__); 
        ota_platform_reset();
      }
    }
    if (UART0_RX[0] == 'd' &&UART0_RX[1] == 'e' &&UART0_RX[2] == 'b' &&UART0_RX[3] == 'u' &&UART0_RX[4] == 'g' && UART0_len >= 5)
    {
       flag_udp_232back = true;
       mySerial.print("debug mode enable! \n");
    }
    if (UART0_RX[0] == 'r' && UART0_len == 3)
    {
      String str = "";
      str += (char)UART0_RX[0];
      str += (char)UART0_RX[1];
      str += (char)UART0_RX[2];
      mySerial.print("TOF10120 read command : ");
      mySerial.println(str);
      mySerial2.print(str);
    }
    if (UART0_RX[0] == 's'&& UART0_len == 3)
    {
      String str = "";
      for (int i = 0 ; i < UART0_len ; i++)
      {
         str += (char)UART0_RX[i];
      }
      mySerial.print("TOF10120 write command : ");
      mySerial.println(str);
      mySerial2.print(str);
    }
    if (UART0_RX[0] == 'c'&& UART0_len == 3)
    {
      mySerial.print("EPD Clear..\n ");
      epd.Clear();
    }
    if (UART0_RX[0] == 'w'&& UART0_len == 3)
    {
      mySerial.println("flag_WS2812B_breathing_ON_OFF");
      flag_WS2812B_breathing_ON_OFF = !flag_WS2812B_breathing_ON_OFF;
    }
    if (UART0_RX[0] == 'D'&& UART0_len == 3)
    {
      #ifdef DHTSensor
      mySerial.print(F("Humidity: "));
      mySerial.print(dht_h);
      mySerial.print(F("%  Temperature: "));
      mySerial.print(dht_t);
      mySerial.print(F("°C "));
      mySerial.print(dht_f);
      mySerial.print(F("°F  Heat index: "));
      mySerial.print(dht_hic);
      mySerial.print(F("°C "));
      mySerial.print(dht_hif);
      mySerial.println(F("°F"));
      #endif
    }
    if (UART0_RX[0] == 'b'&& UART0_len == 3)
    {
      String str_int = "";
      str_int += (char)UART0_RX[1];
      str_int += (char)UART0_RX[2];
      int val = str_int.toInt();
      myWS2812.brightness = val;
      mySerial.print("Set WS2812 Brightness : int>>");
      mySerial.print(val);
      mySerial.print(" string>>");
      mySerial.println(str_int);
      flag_WS2812B_Refresh = true;
      flag_WS2812B_breathing_ON_OFF = false;
    }
    if (UART0_RX[0] == 'P'&& UART0_len == 3)
    {
      mySerial.print("SetOutputPINTrigger(1 , (1 == 1));\n");
      SetOutputPINTrigger(1 , true);
    }
    if(UART0_RX[0] == 'M'&& UART0_len == 3)
    {
      uint8_t mac[6];
      WiFi.macAddress(mac);
      String HEX_0 =  String(mac[0], HEX); 
      String HEX_1 =  String(mac[1], HEX); 
      String HEX_2 =  String(mac[2], HEX); 
      String HEX_3 =  String(mac[3], HEX); 
      String HEX_4 =  String(mac[4], HEX); 
      String HEX_5 =  String(mac[5], HEX);   
      
      
      String MacAdress = "MacAdress :" + HEX_0 + ":"+ HEX_1 + ":"+ HEX_2 + ":"+ HEX_3 + ":"+ HEX_4 + ":"+ HEX_5;   
      mySerial.println(MacAdress);
    }
    if (UART0_RX[0] == 2 && UART0_RX[UART0_len - 1] == 3)
    {
      if (UART0_RX[1] == '0' && UART0_len == 3)
      {
        flag_writeMode = false;
        String str = "";
        str += (char)2;
        str += wiFiConfig.Get_IPAdress_Str();
        str += ",";
        str += wiFiConfig.Get_Subnet_Str();
        str += ",";
        str += wiFiConfig.Get_Gateway_Str();
        str += ",";
        str += wiFiConfig.Get_DNS_Str();
        str += ",";
        str += wiFiConfig.Get_Server_IPAdress_Str();
        str += ",";
        str += wiFiConfig.Get_Localport_Str();
        str += ",";
        str += wiFiConfig.Get_Serverport_Str();
        str += ",";
        str += wiFiConfig.Get_SSID_Str();
        str += ",";
        str += wiFiConfig.Get_Password_Str();
        str += ",";
        str += wiFiConfig.Get_Station_Str();
        str += ",";
        str += wiFiConfig.Get_UDP_SemdTime_Str();
        str += (char)3;
        mySerial.print(str);
        mySerial.flush();
      }
      else if (UART0_RX[1] == 'v')
      {
        String str = VERSION;
        mySerial.print(str);
        mySerial.flush();
      }
      else if (UART0_RX[1] == 'L')
      {
        String str = "Set Loker sucess :";
        if(UART0_RX[2] >= 1)
        {
           str += "1"; 
           wiFiConfig.Set_IsLocker(true);
        }
        else
        {
           str += "0"; 
           wiFiConfig.Set_IsLocker(false);
        }
        mySerial.print(str);
        mySerial.flush();
      }
      else if (UART0_RX[1] == 'C')
      {
        int len = UART0_len - 5;
        int startpo =  UART0_RX[2] | (UART0_RX[3] << 8);
        int numofLED = len / 3 ;
        int startLED = startpo / 3;         
          
        for(int i = 0 ; i < numofLED ; i++)
        {             
           myWS2812.rgbBuffer[i * 3 + startLED + 0] = *(UART0_RX + 4 + i * 3 + 0);     // 将光带上第1个LED灯珠的RGB数值中R数值设置为255
           myWS2812.rgbBuffer[i * 3 + startLED + 1] = *(UART0_RX + 4 + i * 3 + 1);   // 将光带上第1个LED灯珠的RGB数值中G数值设置为255
           myWS2812.rgbBuffer[i * 3 + startLED + 2] = *(UART0_RX + 4 + i * 3 + 2);      // 将光带上第1个LED灯珠的RGB数值中B数值设置为0      
        }     
        flag_WS2812B_Refresh = true;
        flag_WS2812B_breathing_ON_OFF = false;
        Get_Checksum();
      }
      else if (UART0_RX[1] == 'D')
      {
        epd.Wakeup();
        epd.Clear();
        Get_Checksum();
      }
      else if (UART0_RX[1] == 'b')
      {
        epd.Wakeup();
        Get_Checksum();
      }
      else if (UART0_RX[1] == 'c')
      {
        epd.DrawFrame_RW();
        delay(10);
        Get_Checksum();
      }
      else if (UART0_RX[1] == 'd')
      {
        epd.DrawFrame_BW();
        delay(10);
        Get_Checksum();
      }
      else if (UART0_RX[1] == 'f')
      {
        epd.RefreshCanvas();
        Get_Checksum();
      }
      else if (UART0_RX[1] == 'e')
      {
        int len = UART0_len - 7;
        int startpo_L = (*(UART0_RX + 2)) | (*(UART0_RX + 3) << 8);
        int startpo_H = (*(UART0_RX + 4)) | (*(UART0_RX + 5) << 8);
        long startpo = startpo_L | (startpo_H << 16);
        for(int i = 0 ; i < len ; i ++)
        {
           *(epd.framebuffer +startpo + i) = *(UART0_RX + 6 + i);
        }
        Get_Checksum();
      }
      else if (UART0_RX[1] == 'g')
      {
        OLCD114.LCD_Clear(RED);
        delay(100);
        OLCD114.LCD_Clear(GREEN);
        delay(100);
        OLCD114.LCD_Clear(BLUE);
        delay(100);
        OLCD114.LCD_Clear(YELLOW);
        Get_Checksum();
      }
      else if (UART0_RX[1] == 'h')
      {
        OLCD114.LCD_ShowPicture();         
      }
      else if (UART0_RX[1] == '1' && UART0_len == 5)
      {
        flag_writeMode = true;
        int station = UART0_RX[2] | (UART0_RX[3] << 8);
        wiFiConfig.Set_Station(station);
        Get_Checksum();
      }
      else if (UART0_RX[1] == '2' && UART0_len == 7)
      {
        flag_writeMode = true;
        wiFiConfig.Set_IPAdress(UART0_RX[2], UART0_RX[3], UART0_RX[4], UART0_RX[5]);
        Get_Checksum();
      }
      else if (UART0_RX[1] == '3' && UART0_len == 7)
      {
        flag_writeMode = true;
        wiFiConfig.Set_Subnet(UART0_RX[2], UART0_RX[3], UART0_RX[4], UART0_RX[5]);
        Get_Checksum();
      }
      else if (UART0_RX[1] == '4' && UART0_len == 7)
      {
        flag_writeMode = true;
        wiFiConfig.Set_Gateway(UART0_RX[2], UART0_RX[3], UART0_RX[4], UART0_RX[5]);
        Get_Checksum();
      }
      else if (UART0_RX[1] == '5' && UART0_len == 7)
      {
        flag_writeMode = true;
        wiFiConfig.Set_DNS(UART0_RX[2], UART0_RX[3], UART0_RX[4], UART0_RX[5]);
        Get_Checksum();
      }
      else if (UART0_RX[1] == '6' && UART0_len == 7)
      {
        flag_writeMode = true;
        wiFiConfig.Set_Server_IPAdress(UART0_RX[2], UART0_RX[3], UART0_RX[4], UART0_RX[5]);
        Get_Checksum();
      }
      else if (UART0_RX[1] == '7' && UART0_len == 5)
      {
        flag_writeMode = true;
        int port = UART0_RX[2] | (UART0_RX[3] << 8);
        wiFiConfig.Set_Localport(port);
        Get_Checksum();
      }
      else if (UART0_RX[1] == '8' && UART0_len == 5)
      {
        flag_writeMode = true;
        int port = UART0_RX[2] | (UART0_RX[3] << 8);
        wiFiConfig.Set_Serverport(port);
        Get_Checksum();
      }
      else if (UART0_RX[1] == '9')
      {
        flag_writeMode = true;
        String str = "";
        for (int i = 2 ; i < UART0_len - 1 ; i++)
        {
          str += (char)UART0_RX[i];
        }
        wiFiConfig.Set_SSID(str);
        Get_Checksum();
      }
      else if (UART0_RX[1] == 'A')
      {
        flag_writeMode = true;
        String str = "";
        for (int i = 2 ; i < UART0_len - 1 ; i++)
        {
          str += (char)UART0_RX[i];
        }
        wiFiConfig.Set_Password(str);
        Get_Checksum();
      }
      else if (UART0_RX[1] == 'B')
      {
        flag_writeMode = true;
        int ms = UART0_RX[2] | (UART0_RX[3] << 8);
        wiFiConfig.Set_UDP_SemdTime(ms);
        Get_Checksum();
      }
    }
    
    UART0_len = 0;
    for (int i = 0 ; i < UART0_RX_SIZE ; i++)
    {
      UART0_RX[i] = 0;
    }
  }

}

void Get_Checksum()
{
  byte checksum = 0;
  for (int i = 0 ; i < UART0_len; i ++)
  {
    checksum += UART0_RX[i];
  }
  int checksum_2 = checksum / 100;
  int checksum_1 = (checksum - checksum_2 * 100) / 10 ;
  int checksum_0 = (checksum - checksum_2 * 100 - checksum_1 * 10) ;
  byte str_checksum[5] = {2 ,(checksum_2 + 48), (checksum_1 + 48), (checksum_0 + 48) , 3};
  mySerial.write(str_checksum , 5);
  mySerial.flush();
}

#line 1 "C:\\Users\\Evan\\source\\repos\\H_Pannel\\MCU-BW16\\BW16_EPD266\\UART2.ino"
#include "Timer.h"
#define UART1_RX_SIZE 256
byte UART1_RX[UART1_RX_SIZE];
int UART1_len;
MyTimer MyTimer_UART1;
String str_distance = "999";
String str_TOF10120 = "";
int LaserDistance = -999;
int LASER_ON_cnt = 0;
bool LASER_ON = false;
bool LASER_ON_buf = false;
bool flag_UART1_Init = false;
int LASER_ON_num = 0;
void serial2Event()
{
  if(!flag_UART1_Init)
  {
     mySerial.println("mySerial2 init done!!");
     mySerial2.begin(9600);    
     
     flag_UART1_Init = true;
  }
  
  if (mySerial2.available())
  {
    if(UART1_len >= UART1_RX_SIZE)
    {
       mySerial.println("mySerial2 buffer overrange!!");
       UART1_len = 0;
       str_TOF10120 = "";
    }
    else
    {
       
       UART1_RX[UART1_len] = mySerial2.read();
       UART1_len++;
       MyTimer_UART1.TickStop();
       MyTimer_UART1.StartTickTime(2);
    }
    
  }
  if (MyTimer_UART1.IsTimeOut())
  {
    MyTimer_UART1.TickStop();
    MyTimer_UART1.StartTickTime(1000);
//    for (int i = 0 ; i < UART1_len ; i++)
//    {
//       str_TOF10120 += (char)UART1_RX[i];
//    }
//    mySerial.println(str_TOF10120);
    if (UART1_RX[UART1_len - 1] == 0X0A && UART1_RX[UART1_len - 2] == 0X0D && UART1_RX[UART1_len - 3] == 0X0D && UART1_RX[UART1_len - 4] == 0X6D && UART1_RX[UART1_len - 5] == 0X6D)
    {
        
      str_distance = "";
      for (int i = 0 ; i < UART1_len - 5 ; i++)
      {
         str_distance += (char)UART1_RX[i];
      }

      LaserDistance = str_distance.toInt();
      if((LaserDistance <= LASER_D_MAX) && (LaserDistance >= LASER_D_MIN))
      {
         LASER_ON_cnt++;
      }
      else
      {
         LASER_ON_cnt = 0;
         LASER_ON = false;
      }
      if(LASER_ON_cnt >= 1)
      {
         LASER_ON = true;
      }
      else
      {
         LASER_ON = false;
      }
      if(LASER_ON == true)
      {
         flag_JsonSend = true;
      }
      if(LASER_ON_buf != LASER_ON)
      {
         LASER_ON_buf = LASER_ON;
         if(LASER_ON)
         {
           LASER_ON_num++;
         }
         flag_JsonSend = true;
      }
//      mySerial.print("LaserDistance(");
//      mySerial.print(LASER_ON);
//      mySerial.print("):");
//      mySerial.println(str_distance);
    }
    
    UART1_len = 0;
    str_TOF10120 = "";
    for (int i = 0 ; i < UART1_RX_SIZE ; i++)
    {
      UART1_RX[i] = 0;
    }
  }
}

#line 1 "C:\\Users\\Evan\\source\\repos\\H_Pannel\\MCU-BW16\\BW16_EPD266\\UDP_JsonSend.ino"
#include "Arduino.h"
#include "Config.h"

int cnt_UDP_Send = 65534;
MyTimer UDP_Send_Timer;
DynamicJsonDocument doc(1024);
String JsonOutput = "";

int RSSI;
void sub_UDP_Send()
{
  if(cnt_UDP_Send == 65534)
  {

      cnt_UDP_Send = 65535;
  }
  if(cnt_UDP_Send == 65535)
  {
      if(flag_boradInit)cnt_UDP_Send = 1;
  }
  if(cnt_UDP_Send == 1)
  {  
      if(wiFiConfig.uDP_SemdTime == 0)
      {
          cnt_UDP_Send = 1;
      }
      else
      {
          UDP_Send_Timer.TickStop();
          UDP_Send_Timer.StartTickTime(wiFiConfig.uDP_SemdTime);
          cnt_UDP_Send++;
      }
      
  }
  if(cnt_UDP_Send == 2)
  {      
      if(UDP_Send_Timer.IsTimeOut() || flag_JsonSend)
      {
         doc["IP"] = wiFiConfig.Get_IPAdress_Str();
         doc["Port"] = wiFiConfig.Get_Localport();
         doc["RSSI"] = wiFiConfig.GetRSSI();        
         doc["Version"] = VERSION;
         doc["Input"] = Input;
         doc["Output"] = Output;
         doc["Input_dir"] = Input_dir;
         doc["Output_dir"] = Output_dir;
         doc["LaserDistance"] = LaserDistance;  
         doc["LASER_ON"] = LASER_ON;  
         doc["WS2812_State"] = myWS2812.IsON(200);
         doc["LASER_ON_num"] = LASER_ON_num;
         #ifdef DHTSensor
         doc["dht_h"] = dht_h;
         doc["dht_t"] = dht_t;
         #endif
         JsonOutput = "";
         serializeJson(doc, JsonOutput);
         #ifdef MQTT
         #ifdef DHTSensor
         wiFiConfig.MQTT_publishMessage("DHTSensor" , JsonOutput.c_str() , false);  
         #endif
         #else
         Send_StringTo(JsonOutput, wiFiConfig.server_IPAdress, wiFiConfig.serverport);      
         #endif
         
              
         flag_JsonSend = false;
         cnt_UDP_Send++;
      }
  }
  if(cnt_UDP_Send == 3)
  {
     cnt_UDP_Send = 65535;
  }
}

#line 1 "C:\\Users\\Evan\\source\\repos\\H_Pannel\\MCU-BW16\\BW16_EPD266\\UDP_Receive.ino"
#include "Arduino.h"
#define UDP_BUFFER_SIZE 45000
#define UDP_RX_BUFFER_SIZE 1500
char* UdpRead;
char* UdpRead_buf;
int remotePort = 0;
IPAddress remoteIP;
bool UDP_ISConnented = false;
int UdpRead_len = 0;
int UdpRead_len_buf = 0;
long UDPcheck_len = 0;
WiFiUDP Udp; 
WiFiUDP Udp1; 
bool flag_UDP_RX_BUFFER_Init = false;
bool flag_UDP_RX_OK = false;
bool flag_UDP_header = true;
bool flag_UDP0_packet = true;
bool flag_UDP1_packet = true;
MyTimer MyTimer_UDP;
MyTimer MyTimer_UDP_RX_TimeOut;
int ForeColor = 0;
int BackColor = 0;

void onPacketCallBack()
{
  
  if(!flag_UDP_RX_BUFFER_Init)
  {
     UdpRead = (char*) malloc(UDP_BUFFER_SIZE);
     UdpRead_buf = (char*) malloc(UDP_RX_BUFFER_SIZE);
     flag_UDP_RX_BUFFER_Init = true;
  }
  flag_UDP_RX_OK = false;
  flag_UDP_header = true;
  flag_UDP0_packet = false;
  flag_UDP1_packet = false;
  UdpRead_len = 0;
  UDPcheck_len = 0;
  MyTimer_UDP.TickStop();
  MyTimer_UDP.StartTickTime(0);
  while(true)
  {
     int packet_UDP0 = 0;
     int packet_UDP1 = 0;
     if(flag_UDP1_packet ==false)
     {
        packet_UDP0 = Udp.parsePacket();
        if(packet_UDP0 > 0) flag_UDP0_packet = true;
     }

     if(packet_UDP0 > 0 || packet_UDP1 > 0)
     {
        int len = 0;
        if(packet_UDP0 > 0)
        {               
            len = Udp.read(UdpRead_buf, UDP_RX_BUFFER_SIZE - 1);
        }
        if(flag_UDP_header)
        {
            if(len != 4)
            {
               if(flag_udp_232back)mySerial.println("Received header length error!!");
               break;
            }
            int len_LL = *(UdpRead_buf + 0);
            int len_LH = *(UdpRead_buf + 1);
            int len_L = len_LL | (len_LH << 8);
            int len_HL = *(UdpRead_buf + 2);
            int len_HH = *(UdpRead_buf + 3);
            int len_H = len_HL | (len_HH << 8);
            UDPcheck_len = len_L | (len_H << 16); 
            flag_UDP_header = false;
            if(flag_udp_232back)mySerial.print("Received header length : ");
            if(flag_udp_232back)mySerial.println(UDPcheck_len);
        }
        else
        {
          for(int i = 0 ; i < len ; i++)
          {
             *(UdpRead + UdpRead_len + i) = *(UdpRead_buf + i);
          }
          UdpRead_len += len;
          if(flag_udp_232back)mySerial.print("Received len sumary :");
          if(flag_udp_232back)mySerial.println(UdpRead_len);
          if(UDPcheck_len == UdpRead_len)
          {
             if(*(UdpRead + UdpRead_len - 1) == 3)
             {
                remoteIP = Udp.remoteIP();
                remotePort = Udp.remotePort();  
                flag_UDP_RX_OK = true;
                if(flag_udp_232back)mySerial.println("Received End code!!");
                break;
             }              
          }    
        }        
        MyTimer_UDP.TickStop();
        MyTimer_UDP.StartTickTime(200);
     }
     else
     {
        if(MyTimer_UDP.IsTimeOut())
        {
          if(UdpRead_len > 0)
          {
             if(flag_udp_232back)mySerial.println("-----RETRY!!-----");
             Send_StringTo("RETRY" ,remoteIP, remotePort);
          }
          break;
        }        
     }
  }

  if (flag_UDP_RX_OK)                     //如果有資料可用
  {
     if(flag_udp_232back) 
     {
        mySerial.print("Received packet of size ");
        mySerial.println(UdpRead_len);
        mySerial.print("From ");
        IPAddress remoteIp = remoteIP;
        mySerial.print(remoteIp);
        mySerial.print(", port ");
        mySerial.println(remotePort);
        
     }      
     if(*(UdpRead) == 2)
     {
        if(flag_udp_232back)mySerial.println("接收到起始碼[2]");
        if(*(UdpRead + UdpRead_len - 1) == 3)
        {
          if(*(UdpRead + 1) == 'a' && UdpRead_len == 3)
          {  
            if(flag_udp_232back)printf("EPD Sleep\n");        
            epd.Sleep();
            Get_Checksum_UDP();          
          }        
          else if(*(UdpRead + 1) == 'b' && UdpRead_len == 3)
          {
            if(flag_udp_232back)printf("EPD Wakeup\n");     
            flag_WS2812B_breathing_Ex_lightOff = true;       
            epd.Wakeup();            
            Get_Checksum_UDP();                           
          }  
          else if(*(UdpRead + 1) == 'c' && UdpRead_len == 3)
          {
            if(flag_udp_232back)printf("EPD DrawFrame_RW\n");
            flag_WS2812B_breathing_Ex_lightOff = true;
            epd.DrawFrame_RW();
            delay(10);
            Get_Checksum_UDP();         
          } 
          else if(*(UdpRead + 1) == 'd' && UdpRead_len == 3)
          {
            if(flag_udp_232back)printf("EPD DrawFrame_BW\n");
            flag_WS2812B_breathing_Ex_lightOff = true;
            epd.DrawFrame_BW();
            delay(10);
            Get_Checksum_UDP();           
          }  
          
          else if(*(UdpRead + 1) == 'f' && UdpRead_len == 3)
          {
            if(flag_udp_232back)printf("EPD RefreshCanvas\n");
            Get_Checksum_UDP();
            epd.RefreshCanvas();
            delay(100);
            myWS2812.ClearBytes();           
            flag_WS2812B_breathing_Ex_lightOff = true;
                     
          }  
          else if(*(UdpRead + 1) == 'g' && UdpRead_len == 3)
          {                                  
             Send_String(str_distance , wiFiConfig.localport);
             if(flag_udp_232back)printf("LaserDistance : %d\n" ,str_distance);
          } 
          else if (*(UdpRead + 1) == 'h')
          {
            epd.BW_Command();
            Get_Checksum();        
          }
          else if (*(UdpRead + 1) == 'i')
          {
            epd.RW_Command();
            Get_Checksum();        
          }
          else if(*(UdpRead + 1) == 'j')
          {
            int len = UdpRead_len - 7;
            int startpo_L = (*(UdpRead + 2)) | (*(UdpRead + 3) << 8);
            int startpo_H = (*(UdpRead + 4)) | (*(UdpRead + 5) << 8);
            long startpo = startpo_L | (startpo_H << 16);
            epd.SendSPI(UdpRead ,len , 6);
            if(flag_udp_232back)printf("EPD EPD_SendSPI\n");
            if(flag_udp_232back)printf("len : %d\n" ,len);
            if(flag_udp_232back)printf("startpo : %d\n" ,startpo);
    
            Get_Checksum_UDP();      
          }
          else if (*(UdpRead + 1) == 'e')
          {
            int len = UdpRead_len - 7;
            int startpo_L = (*(UdpRead + 2)) | (*(UdpRead + 3) << 8);
            int startpo_H = (*(UdpRead + 4)) | (*(UdpRead + 5) << 8);
            long startpo = startpo_L | (startpo_H << 16);
            int max_temp = startpo + len;
            if(max_temp > epd.buffer_max)
            {
              if(flag_udp_232back)printf("EPD framebuffer overload buffer_max\n");
              if(flag_udp_232back)printf("buffer_max : %d\n" ,epd.buffer_max);
              if(flag_udp_232back)printf("len : %d\n" ,len);
              if(flag_udp_232back)printf("startpo : %d\n" ,startpo);             
            }
            else
            {
              for(int i = 0 ; i < len ; i ++)
              {
                 *(epd.framebuffer +startpo + i) = *(UdpRead + 6 + i);
              }
              if(flag_udp_232back)printf("EPD framebuffer\n");
              if(flag_udp_232back)printf("len : %d\n" ,len);
              if(flag_udp_232back)printf("startpo : %d\n" ,startpo);
              Get_Checksum_UDP();
            }
            
          }
          else if (*(UdpRead + 1) == '@')
          {
            WS2812B_breathing_onAddVal = *(UdpRead + 2);
            WS2812B_breathing_offSubVal = *(UdpRead + 3);
            WS2812B_breathing_R = *(UdpRead + 4);
            WS2812B_breathing_G = *(UdpRead + 5);
            WS2812B_breathing_B = *(UdpRead + 6);
            
            if(WS2812B_breathing_onAddVal == 0 || WS2812B_breathing_offSubVal == 0 || (WS2812B_breathing_R == 0 && WS2812B_breathing_G == 0 && WS2812B_breathing_B == 0))
            {
                flag_WS2812B_breathing_ON_OFF = false;
                int numofLED = 450;
                for(int i = 0 ; i < numofLED ; i++)
                {             
                   myWS2812.rgbBuffer[i * 3 + 0 + 0] = (int)(0);     // 将光带上第1个LED灯珠的RGB数值中R数值设置为255
                   myWS2812.rgbBuffer[i * 3 + 0 + 1] = (int)(0);   // 将光带上第1个LED灯珠的RGB数值中G数值设置为255
                   myWS2812.rgbBuffer[i * 3 + 0 + 2] = (int)(0);      // 将光带上第1个LED灯珠的RGB数值中B数值设置为0      
                }
                flag_WS2812B_Refresh = true;    
                flag_WS2812B_breathing_ON_OFF = false;
            }
            else
            {
                flag_WS2812B_breathing_ON_OFF = true;
            }
            
            
            if(flag_udp_232back)printf("WS2812B_breathing_onAddVal : %d\n",WS2812B_breathing_onAddVal);
            if(flag_udp_232back)printf("WS2812B_breathing_offSubVal : %d\n",WS2812B_breathing_offSubVal);
            if(flag_udp_232back)printf("WS2812B_breathing_R : %d\n",WS2812B_breathing_R);
            if(flag_udp_232back)printf("WS2812B_breathing_G : %d\n",WS2812B_breathing_G);
            if(flag_udp_232back)printf("WS2812B_breathing_B : %d\n",WS2812B_breathing_B);
    
            Get_Checksum_UDP();
          }
          
          else if (*(UdpRead + 1) == 'L')
          {
            int len = UdpRead_len - 5;
            int startpo = (*(UdpRead + 2)) | (*(UdpRead + 3) << 8);
            int numofLED = len / 3 ;
            int startLED = startpo / 3;         
            if(flag_udp_232back)printf("Set WS2812 Buffer\n");
            if(flag_udp_232back)printf("len : %d\n", len);
            if(flag_udp_232back)printf("startpo : %d\n", startpo);
            if(flag_udp_232back)printf("numofLED : %d\n", numofLED);
            if(flag_udp_232back)printf("startLED : %d\n", startLED);
               
            byte bytes[numofLED * 3];
            if(numofLED - startpo > 450 )
            {
              Get_Checksum_UDP();
              
            }
            else
            {
              for(int i = 0 ; i < numofLED ; i++)
              {             
                 bytes[i * 3 + 0 + 0] = *(UdpRead + 4 + i * 3 + 0);     // 将光带上第1个LED灯珠的RGB数值中R数值设置为255
                 bytes[i * 3 + 0 + 1] = *(UdpRead + 4 + i * 3 + 1);   // 将光带上第1个LED灯珠的RGB数值中G数值设置为255
                 bytes[i * 3 + 0 + 2] = *(UdpRead + 4 + i * 3 + 2);      // 将光带上第1个LED灯珠的RGB数值中B数值设置为0      
              }  
              myWS2812.Show(bytes ,numofLED );
              flag_WS2812B_breathing_ON_OFF = false;
              flag_WS2812B_breathing_Ex_ON_OFF = false;
            }
            
            Get_Checksum_UDP();
            flag_JsonSend = true;
          }
          else if (*(UdpRead + 1) == '#')
          {
            int len = UdpRead_len - 5;
            int startpo = (*(UdpRead + 2)) | (*(UdpRead + 3) << 8);
            int numofLED = len / 3 ;
            int startLED = startpo / 3;         
            if(flag_udp_232back)printf("Set WS2812 Buffer B\n");
            if(flag_udp_232back)printf("len : %d\n", len);
            if(flag_udp_232back)printf("startpo : %d\n", startpo);
            if(flag_udp_232back)printf("numofLED : %d\n", numofLED);
            if(flag_udp_232back)printf("startLED : %d\n", startLED);
            bool flag_black = true;
            if(numofLED - startpo > 450 )
            {
              Get_Checksum_UDP();
              
            }
            else
            {
              for(int i = 0 ; i < numofLED ; i++)
              {             
                 myWS2812.rgbBuffer[i * 3 + startLED + 0] = *(UdpRead + 4 + i * 3 + 0);     // 将光带上第1个LED灯珠的RGB数值中R数值设置为255
                 myWS2812.rgbBuffer[i * 3 + startLED + 1] = *(UdpRead + 4 + i * 3 + 1);   // 将光带上第1个LED灯珠的RGB数值中G数值设置为255
                 myWS2812.rgbBuffer[i * 3 + startLED + 2] = *(UdpRead + 4 + i * 3 + 2);      // 将光带上第1个LED灯珠的RGB数值中B数值设置为0      
                 if(myWS2812.rgbBuffer[i * 3 + startLED + 0] > 0)flag_black =false;
                 if(myWS2812.rgbBuffer[i * 3 + startLED + 1] > 0)flag_black =false;
                 if(myWS2812.rgbBuffer[i * 3 + startLED + 2] > 0)flag_black =false;
              }                 
             
              flag_WS2812B_breathing_ON_OFF = false;
              flag_WS2812B_breathing_Ex_ON_OFF = true;
  
              if(flag_black)
              {
                  flag_WS2812B_breathing_Ex_lightOff = true;
              }
              else
              {
                  flag_WS2812B_breathing_Ex_lightOff = false;
              }
              
              Get_Checksum_UDP();
            }
            
            flag_JsonSend = true;
          }
          else if (*(UdpRead + 1) == 'O')
          {           
            int num_L = *(UdpRead + 2);
            int num_H = *(UdpRead + 3);
            int num= num_L | (num_H << 8);
            if(num > NUM_WS2812B_CRGB * 3) num = NUM_WS2812B_CRGB * 3;
            if(flag_udp_232back)printf("Get WS2812 Buffer\n");
            if(flag_udp_232back)printf("num : %d\n", num);
//            for(int i = 0 ; i < num ; i++)
//            {
//                Serial.println(*(myWS2812.rgbBuffer + i));
//            }
            Send_Bytes(myWS2812.rgbBuffer, num ,remoteIP, remotePort);                    
          }
          else if(*(UdpRead + 1) == 'B')
          {                  
              String serverIP = wiFiConfig.server_IPAdress_str;
              int str_len = serverIP.length() + 1; 
              char serverIP_char_array[str_len];
              serverIP.toCharArray(serverIP_char_array , str_len );
              mySerial.print("Host :");
              mySerial.print(serverIP_char_array);
              mySerial.print(", port :");
              mySerial.println(8080);
              Get_Checksum_UDP();
              int ret = http_update_ota(serverIP_char_array, 8080, RESOURCE);
              printf("[%s] Update task exit\n\r", __FUNCTION__);
              if(!ret)
              {
                printf("[%s] Ready to reboot\n\r", __FUNCTION__); 
                ota_platform_reset();
              }
          }
          else if(*(UdpRead + 1) == 'D')
          {                  
             int value = *(UdpRead + 2);
             if(flag_udp_232back)printf("Set_TOF : [value]%d\n" , value);
             wiFiConfig.Set_IsLocker((value == 1));
             Get_Checksum_UDP();
          }
          else if(*(UdpRead + 1) == 'F')
          {                  
              int ms_L = *(UdpRead + 2);
              int ms_H = *(UdpRead + 3);  
              
              int ms= ms_L | (ms_H << 8);

              if(flag_udp_232back)mySerial.println("Set_UDP_SendTime");
              if(flag_udp_232back)mySerial.print("ms[");
              if(flag_udp_232back)mySerial.print(ms);
              if(flag_udp_232back)mySerial.println("]");

              wiFiConfig.Set_UDP_SemdTime(ms);                                           
              Get_Checksum_UDP();
          }
          else if(*(UdpRead + 1) == 'J')
          {    
              int value_L = *(UdpRead + 2);
              int value_H = *(UdpRead + 3);             
              int value= value_L | (value_H << 8);              
              if(flag_udp_232back)printf("Set_Output : %d\n" , value);
              SetOutput(value);
              Get_Checksum_UDP();
          }
            else if(*(UdpRead + 1) == 'H')
          {                  
              int IPA = *(UdpRead + 2);
              int IPB = *(UdpRead + 3);
              int IPC = *(UdpRead + 4);
              int IPD = *(UdpRead + 5);
              int port_L = *(UdpRead + 6);
              int port_H = *(UdpRead + 7);
              int port= port_L | (port_H << 8);
              
              if(flag_udp_232back)printf("Server IP : %d.%d.%d.%d\n", (byte)IPA,(byte)IPB,(byte)IPC,(byte)IPD);
              if(flag_udp_232back)printf("Server Port : %d",port);

              wiFiConfig.Set_Server_IPAdress((byte)IPA,(byte)IPB,(byte)IPC,(byte)IPD);
              wiFiConfig.Set_Serverport(port);
              Get_Checksum_UDP();
          }
          else if(*(UdpRead + 1) == 'K')
          {                  
             int pin_num = *(UdpRead + 2);
             int value = *(UdpRead + 3);
             if(flag_udp_232back)printf("Set_Output : [PIN Num]%d [value]%d\n" , pin_num , value);
             SetOutputPIN(pin_num , (value == 1));
             Get_Checksum_UDP();
          }
          else if(*(UdpRead + 1) == 'P')
          {    
              int value_L = *(UdpRead + 2);
              int value_H = *(UdpRead + 3);             
              int value = value_L | (value_H << 8);              
              if(flag_udp_232back)printf("Set_OutputTrigger : %d\n" , value);
              SetOutputTrigger(value);
              Get_Checksum_UDP();
          }
          else if(*(UdpRead + 1) == 'Q')
          {                  
             #ifdef MCP23017

             #else
             int pin_num = *(UdpRead + 2);
             int value = *(UdpRead + 3);
             if(flag_udp_232back)printf("Set_OutputPINTrigger : [PIN Num]%d [value]%d\n" , pin_num , value);
             MyOutput_PIN01.ADC_Mode = false;
             SetOutputPINTrigger(pin_num , (value == 1));
             Get_Checksum_UDP();
             #endif
    
          }
          else if(*(UdpRead + 1) == '!')
          {
             #ifdef MCP23017
             
             #else
             int pin_num = *(UdpRead + 2);
             int time_ms_L = *(UdpRead + 3);
             int time_ms_H = *(UdpRead + 4);
             int time_ms = time_ms_L | (time_ms_H << 8);           
             MyOutput_PIN01.ADC_Trigger(time_ms);

             if(flag_udp_232back)printf("Set_ADCMotorTrigger : [PIN Num]%d [time_ms]%d\n" , pin_num , time_ms);
             Get_Checksum_UDP();
             #endif                  
             
          }
          else if(*(UdpRead + 1) == 'R')
          {                  
             int pin_num = *(UdpRead + 2);
             int value = *(UdpRead + 3);
             if(flag_udp_232back)printf("Set_Input_dir : [PIN Num]%d [value]%d\n" , pin_num , value);
             Set_Input_dir(pin_num , (value == 1));
             int temp = Get_Input_dir();
             if(flag_udp_232back)printf("Get_Input_dir : [value]%d\n" , temp);
             wiFiConfig.Set_Input_dir(temp);
             Get_Checksum_UDP();
          }
           else if(*(UdpRead + 1) == 'N')
          {                  
              int LocalPort_L = *(UdpRead + 2);
              int LocalPort_H = *(UdpRead + 3);  
              
              int _LocalPort = LocalPort_L | (LocalPort_H << 8);

              if(flag_udp_232back)mySerial.println("Set_Localport");
              if(flag_udp_232back)mySerial.print("LocalPort[");
              if(flag_udp_232back)mySerial.print(_LocalPort);
              if(flag_udp_232back)mySerial.println("]");

              wiFiConfig.Set_Localport(_LocalPort);                                           
              Get_Checksum_UDP();
          }
          else if(*(UdpRead + 1) == 'S')
          {                  
             int pin_num = *(UdpRead + 2);
             int value = *(UdpRead + 3);
             if(flag_udp_232back)printf("Set_Output_dir : [PIN Num]%d [value]%d\n" , pin_num , value);
             Set_Output_dir(pin_num , (value == 1));
             int temp = Get_Output_dir();
             if(flag_udp_232back)printf("Get_Output_dir : [value]%d\n" , temp);
             wiFiConfig.Set_Output_dir(temp);
             Get_Checksum_UDP();
          }
          else if(*(UdpRead + 1) == '2')
          {
            if(flag_udp_232back)mySerial.println("set graphicData..");
            int _data;
            int len = (UdpRead_len - 7) / 2;
            
            int startpo_LL = *(UdpRead + 2);
            int startpo_LH = *(UdpRead + 3);
            int startpo_L = startpo_LL | (startpo_LH << 8);
            int startpo_HL = *(UdpRead + 4);
            int startpo_HH = *(UdpRead + 5);
            int startpo_H = startpo_HL | (startpo_HH << 8);
            long startpo = startpo_L | (startpo_H << 16); 
            if(flag_udp_232back)mySerial.print("Start Position[");
            if(flag_udp_232back)mySerial.print(startpo);
            if(flag_udp_232back)mySerial.println("]");
            if(flag_udp_232back)mySerial.print("Num Of Data[");
            if(flag_udp_232back)mySerial.print(len);
            if(flag_udp_232back)mySerial.println("]");        
            
            for(int i = 0 ; i < len ; i ++)
            {
               _data = (*(UdpRead + 6 + i * 2)) |  (*(UdpRead + 7 + i * 2 ) << 8) ;   
              *(OLCD114.framebuffer + startpo + i ) = _data;
               
            }
            if(flag_udp_232back)mySerial.println("set graphicData done..");    
            Get_Checksum_UDP();
          } 
          else if(*(UdpRead + 1) == '1')
          {
            if(flag_udp_232back)mySerial.println("ClearCanvas");
            OLCD114.LCD_Clear(GRAY);
            Get_Checksum_UDP();
          }
          else if(*(UdpRead + 1) == '3')
          {
            if(flag_udp_232back)mySerial.println("DrawCanvas");
            OLCD114.LCD_ShowPicture();
            Get_Checksum_UDP();
          }
          else if(*(UdpRead + 1) == '7')
          {                  
              int BackColor_L = *(UdpRead + 2);
              int BackColor_H = *(UdpRead + 3);
              BackColor = BackColor_L | (BackColor_H << 8);
              int Forecolor_L = *(UdpRead + 4);
              int Forecolor_H = *(UdpRead + 5);
              ForeColor = Forecolor_L | (Forecolor_H << 8);

              if(flag_udp_232back)mySerial.println("Set_BackColor");
              if(flag_udp_232back)mySerial.print("BackColor[");
              if(flag_udp_232back)mySerial.print(BackColor);
              if(flag_udp_232back)mySerial.println("]");
              if(flag_udp_232back)mySerial.print("ForeColor[");
              if(flag_udp_232back)mySerial.print(ForeColor);
              if(flag_udp_232back)mySerial.println("]");
              
              Get_Checksum_UDP();
          }
          else if(*(UdpRead + 1) == '6')
          {                  
            int _data;
            
            if(flag_udp_232back)mySerial.println("設定GraphicDataEx");
            int UDP_len = (UdpRead_len - 9);
            int len_L = *(UdpRead + 2);
            int len_H = *(UdpRead + 3);
            int startpo_LL = *(UdpRead + 4);
            int startpo_LH = *(UdpRead + 5);
            int startpo_L = startpo_LL | (startpo_LH << 8);
            int startpo_HL = *(UdpRead + 6);
            int startpo_HH = *(UdpRead + 7);
            int startpo_H = startpo_HL | (startpo_HH << 8);
            long startpo = startpo_L | (startpo_H << 16); 
            int len = len_L | (len_H << 8);
            bool flag[8];
            if(flag_udp_232back)mySerial.print("Start Position[");
            if(flag_udp_232back)mySerial.print(startpo);
            if(flag_udp_232back)mySerial.println("]");
            if(flag_udp_232back)mySerial.print("Num Of Data[");
            if(flag_udp_232back)mySerial.print(len);
            if(flag_udp_232back)mySerial.println("]");        
            int index = 0;
            for(int i = 0 ; i < UDP_len ; i ++)
            {
               _data = *(UdpRead + 8 + i);
               flag[0] = (((_data >> 0) % 2) == 1);
               flag[1] = (((_data >> 1) % 2) == 1);
               flag[2] = (((_data >> 2) % 2) == 1);
               flag[3] = (((_data >> 3) % 2) == 1);
               flag[4] = (((_data >> 4) % 2) == 1);
               flag[5] = (((_data >> 5) % 2) == 1);
               flag[6] = (((_data >> 6) % 2) == 1);
               flag[7] = (((_data >> 7) % 2) == 1);
               
               for(int k = 0 ; k < 8 ; k++)
               {
                 if(index == len) break;
                 if(flag[k])
                 {
                    *(OLCD114.framebuffer + startpo + (i * 8) + k) = BackColor;
                 }
                 else
                 {
                    *(OLCD114.framebuffer + startpo + (i * 8) + k) = ForeColor;
                 }
                 index++;
               }
               
            }

            Get_Checksum_UDP();
          }          
          if(flag_udp_232back)
          {
            mySerial.println("接收到結束碼[3]");
            mySerial.println("-------------------------------------------");
          }
           
        }
        
     }
    
  }

   
}
void Clear_char_buf()
{
    memset(UdpRead,0,4096);
}
void Get_Checksum_UDP()
{
   byte checksum = 0;
   for(int i = 0 ; i < UdpRead_len; i ++)
   {
      checksum+= *(UdpRead + i);
   }  
   String str0 = String(checksum);
   if(str0.length() < 3) str0 = "0" + str0;
   if(str0.length() < 3) str0 = "0" + str0;
   if(str0.length() < 3) str0 = "0" + str0;
   if(flag_udp_232back)printf("Checksum String : %d\n",str0);
   if(flag_udp_232back)printf("Checksum Byte : %d \n" , checksum);
   Send_StringTo(str0 ,remoteIP, remotePort);
 
}
void Connect_UDP(int localport)
{
    printf("Connect UDP : %d \n" , localport);
    Udp.begin(localport);
    Udp1.begin(29500);
}
void Send_Bytes(uint8_t *value ,int Size ,IPAddress RemoteIP ,int RemotePort)
{ 
   Udp.beginPacket(RemoteIP, RemotePort); //準備傳送資料
   Udp.write(value, Size); //複製資料到傳送快取
   Udp.endPacket();            //傳送資料
}
void Send_String(String str ,int remoteUdpPort)
{  
   Udp.beginPacket(Udp.remoteIP(), remoteUdpPort); //準備傳送資料
   Udp.print(str); //複製資料到傳送快取
   Udp.endPacket();            //傳送資料
}
void Send_StringTo(String str ,IPAddress RemoteIP ,int RemotePort)
{  
   Udp.beginPacket(RemoteIP, RemotePort); //準備傳送資料
   Udp.print(str); //複製資料到傳送快取
   Udp.endPacket();            //傳送資料
   
}

String IpAddress2String(const IPAddress& ipAddress)
{
  return String(ipAddress[0]) + String(".") +\
  String(ipAddress[1]) + String(".") +\
  String(ipAddress[2]) + String(".") +\
  String(ipAddress[3])  ; 
}

