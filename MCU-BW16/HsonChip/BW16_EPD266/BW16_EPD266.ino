#include "Config.h"
#include "Global.h"
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

#if defined(MCP23017)
#include "DFRobot_MCP23017.h"
#else if defined(MCP23008)
#include "Adafruit_MCP23008.h"
#endif

TaskHandle_t Core0Task1Handle;
TaskHandle_t Core0Task2Handle;
TaskHandle_t Core0Task3Handle;
TaskHandle_t Core0Task4Handle;

int wtd_count = 0;


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
        oLCD114.mySerial = &mySerial;
        oLCD114.Lcd_Init();
        delay(200);
        oLCD114.LCD_Clear(GRAY);
      }     
      flag_OLCD_144_boradInit = true;
   }
   if(MyTimer_BoardInit.IsTimeOut() && !flag_boradInit)
   {          
      mySerial.begin(115200);        
      mySerial.println(VERSION);  
      
      #if defined(MCP23017) || defined(MCP23008)

      #if defined(MCP23017)
      while(mcp.begin() != 0)
      {
        mySerial.println("Initialization of the chip failed, please confirm that the chip connection is correct!");
        delay(1000);
      }
      delay(500); 
      #else if defined(MCP23008)
      mcp.begin(0x00);
      delay(500); 
      #endif
      #if defined(MCP23017)
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
      oLCD114._mcp = &mcp;
      #endif
         
      
      epd._mcp = &mcp;
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
      #if defined(BETTERY)  
      mcp.digitalWrite(BETTERY_OUTPUT , true);
      if(mcp.digitalRead(BETTERY_INPUT) == false)
      {
         mySerial.println(F("mcp.digitalRead(BETTERY_INPUT)"));
      }
      mySerial.println(F("mcp.digitalWrite(BETTERY_OUTPUT , true);"));
      delay(500);
      mcp.digitalWrite(BETTERY_OUTPUT , false);
      mySerial.println(F("mcp.digitalWrite(BETTERY_OUTPUT , false);"));
      delay(500);
      #endif
      
      #if defined(WTD_OUTPUT)  
      if(wtd_count <= 10)
      {
        mcp.digitalWrite(WTD_OUTPUT , false);
        delay(500);
        mcp.digitalWrite(WTD_OUTPUT , true);
        delay(500);
         mySerial.print(F("wtd feed ...\n"));
      }
     
      wtd_count++;
      #endif
      #if defined(DC_MOTOR)  
      mcp.digitalWrite(DC_MOTOR_OUTPUT , false);
      delay(500);
      mcp.digitalWrite(DC_MOTOR_OUTPUT , true);
      delay(500);
      mySerial.print(F("DC_MOTOR test done ...\n"));
      #endif
      #if defined(LIGHT_SENSOR)  
      if(mcp.digitalRead(LIGHT_SENSOR_INPUT) == false)
      {
         mySerial.println(F("mcp.digitalRead(LIGHT_SENSOR_INPUT)"));
      }
      #endif
      #if defined(BUTTON_EX)  
      if(mcp.digitalRead(BUTTON_EX_INPUT) == false)
      {
         mySerial.println(F("mcp.digitalRead(BUTTON_EX_INPUT)"));
      }
      #endif
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
