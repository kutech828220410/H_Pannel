// include the necessary libraries
#include "FT62XXTouchScreen.h"
#include "Fonts.h"
#include "Button.h"
#include <TFT_eSPI.h> // Hardware-specific library
#include <WiFi.h>
#include <WiFiUdp.h>
#include <AsyncUDP.h> //引用以使用非同步UDP
#include "WiFiConfig.h"
#include "KeyBoardABC.h"
#include "Timer.h"
#include "LED.h"
#include "FastLED.h" 
#include <esp_task_wdt.h>
#include "esp32-hal-cpu.h"
#include "esp_system.h"
#include "WiFiConfig.h"

#include <ArduinoJson.h>
//#include <HTTPClient.h>
//#include <HTTPUpdate.h>

String Version = "Ver 1.0.7";
WiFiConfig wiFiConfig;

#define NUM_LEDS 8             // LED灯珠数量
#define LED_DT 26                // Arduino输出控制信号引脚
#define LED_TYPE WS2812B         // LED灯带型号
#define COLOR_ORDER GRB         // RGB灯珠中红色、绿色、蓝色LED的排列顺序
uint8_t max_bright = 255;       // LED亮度控制变量，可使用数值为 0 ～ 255， 数值越大则光带亮度越高
CRGB leds[NUM_LEDS];
MyLED MyLED_WS2812;
int WS2812_R = 0;
int WS2812_G = 0;
int WS2812_B = 0;


TFT_eSPI tft = TFT_eSPI();       // Invoke custom library
bool flag_PlayBeep;

#define DISPLAY_WIDTH  480
#define DISPLAY_HEIGHT 320
#define ST7796_DRIVER 1
#define TFT_WIDTH  480
#define TFT_HEIGHT 320
#define USE_HSPI_PORT 1
#define PIN_SDA 18
#define PIN_SCL 19
#define TFT_MISO 12
#define TFT_MOSI 13
#define TFT_SCLK 14
#define TFT_CS   15
#define TFT_DC   21
#define TFT_RST  22
#define TFT_BL   23

uint16_t *framebuffer;
int framebuffer_len = DISPLAY_WIDTH * DISPLAY_HEIGHT;

FT62XXTouchScreen TouchScreen = FT62XXTouchScreen(DISPLAY_HEIGHT, PIN_SDA, PIN_SCL);
KeyBoardABC _KeyBoardABC = KeyBoardABC();

Button Button_Page00_Setting;

Button Button_Page01_IPAdress;
Button Button_Page01_Subnet;
Button Button_Page01_Gateway;
Button Button_Page01_Dns;
Button Button_Page01_MacAdress;
Button Button_Page01_Port;
Button Screen_Button_Page01_IP;
Button Screen_Button_Page01_SSID;
Button Screen_Button_Page01_Function;

Button Button_Page02_SSID;
Button Button_Page02_Password;
Button Button_Page02_ServerIP;
Button Button_Page02_ServerPort;
Button Screen_Button_Page02_IP;
Button Screen_Button_Page02_SSID;
Button Screen_Button_Page02_Function;

Button Button_Page03_Connect;
Button Button_Page03_UDPTest;
Button Button_Page03_Update;
Button Screen_Button_Page03_IP;
Button Screen_Button_Page03_SSID;
Button Screen_Button_Page03_Function;

bool flag_udp_232back = true;
bool Wifi_Init = false;
int Localport = 0;
IPAddress ServerIp;
int Serverport;

int Screen_Page = 0;
int Screen_Page_buf = -1;
bool Screen_Page00_Init = false;
bool Screen_Page01_Init = false;
bool Screen_Page02_Init = false;
bool Screen_Page03_Init = false;
bool Screen_Page10_Init = false;
TouchPoint touchPos;
MyTimer MyTimer_BoardInit;
bool flag_boradInit = false;
bool flag_httpInit = false;
void setup()
{
      Serial.begin(115200);   
      setCpuFrequencyMhz(80); 
      Serial.setRxBufferSize(1024);  
      Serial.print("CPU Frenqency is: ");
      Serial.print(getCpuFrequencyMhz());
      Serial.println(" Mhz ");
      


      wiFiConfig.Init("");
      Localport = wiFiConfig.Get_Localport();
 
      LEDS.addLeds<LED_TYPE, LED_DT, COLOR_ORDER>(leds, NUM_LEDS);  // 初始化光带 
      FastLED.setBrightness(max_bright);                            // 设置光带亮度
      FastLED.clear();
      FastLED.show();
      MyTimer_BoardInit.StartTickTime(3000);
      xTaskCreatePinnedToCore(Task2code,"Task2code", 10000,NULL,1,NULL,1); 

  
//  if(!SPIFFS.begin(true))
//  {
//     Serial.println("無法掛載 SPIFFS 檔案系統!");
//  }
//  else
//  {
//    Serial.printf("SPIFFS 總空間大小: %d\n", SPIFFS.totalBytes());
//    Serial.printf("SPIFFS 使用空間大小: %d\n", SPIFFS.usedBytes());
//  }

  
//  

//  wiFiConfig.Init(Version);
//  Localport = wiFiConfig.Get_Localport();
//  Serverport = wiFiConfig.Get_Serverport();
//  ServerIp = wiFiConfig.Get_Server_IPAdressClass();
//  wiFiConfig.WIFI_Connenct();
//  
//  tft.init();
//  tft.fillScreen(TFT_WHITE);
//  tft.setRotation(1);
//  TouchScreen.begin();
//  wiFiConfig.SetTFT(tft);
//  pinMode(TFT_BL, OUTPUT);
//  digitalWrite(TFT_BL, 1);
//
// 
//  framebuffer = (uint16_t *)ps_calloc(sizeof(uint16_t), DISPLAY_WIDTH * DISPLAY_HEIGHT);
//  
  
//  _KeyBoardABC.Init();
//  Page00_Init();
//  Page01_Init();
//  Page02_Init();
//  Page03_Init();
//  
  MyTimer_BoardInit.StartTickTime(3000);
//  LEDS.addLeds<LED_TYPE, LED_DT, COLOR_ORDER>(leds, NUM_LEDS);  // 初始化光带 
//  FastLED.setBrightness(max_bright);                            // 设置光带亮度
//  FastLED.clear();
//  FastLED.show();
//  MyLED_WS2812.BlinkTime = 0;
//  MyLED_WS2812.LED_ON = WS2812_LED_ON;
//  MyLED_WS2812.LED_OFF = WS2812_LED_OFF;
  
  xTaskCreatePinnedToCore(Task2code,"Task2code",10000,NULL,1,NULL,1); 

}


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
        else
        {
           
           if(!flag_httpInit)
           {
              wiFiConfig.httpInit();
              flag_httpInit = true;
           }
            wiFiConfig.HandleClient();  
        }
           
    } 
//    if(flag_boradInit)
//    {
//          touchPos = TouchScreen_Read();
//          
//          if (touchPos.touched) 
//          {
//            if(flag_udp_232back)Serial.printf("X: %d, Y: %d , touched: %d\n", touchPos.xPos, touchPos.yPos, touchPos.touched);
//            flag_PlayBeep = true;
//          }
//    }
//    if(Screen_Page00_Init)
//    {
//      Screen_Page = 0;
//      Page00_Refresh();
//      Screen_Page00_Init = false;
//    }              
//    if(Screen_Page01_Init)
//    {
//      Screen_Page = 1;
//      Page01_Refresh();
//      Screen_Page01_Init = false;
//    }
//    if(Screen_Page02_Init)
//    {
//      Screen_Page = 2;
//      Page02_Refresh(); 
//      Screen_Page02_Init = false;
//    }
//    if(Screen_Page03_Init)
//    {
//      Screen_Page = 3;
//      Page03_Refresh(); 
//      Screen_Page03_Init = false;
//    }
//    if(Screen_Page10_Init)
//    {
//      Screen_Page = 10;
//      tft.fillScreen(TFT_WHITE);   
//      Screen_Page10_Init = false;
//    }
//    
 
//    if(MyTimer_BoardInit.IsTimeOut() && !flag_boradInit)
//    {                                      
//       
//        
//        touchPos = TouchScreen_Read();
//        touchPos.touched = true;
//        Serial.printf("X: %d, Y: %d , touched: %d\n", touchPos.xPos, touchPos.yPos, touchPos.touched);
//        if(!Button_Page00_Setting.IsPress(touchPos) && ( wiFiConfig.Get_PC_Restart() == 0))
//        {           
//           wiFiConfig.WIFI_Connenct();
//           if(WiFi.status() == WL_CONNECTED)
//           {                              
//              Connect_UDP(Localport);
//              Screen_Page10_Init = true;  
//              tft.fillScreen(TFT_WHITE);
//           }
//           else
//           {
//              Screen_Page03_Init = true;  
//           }
//        }
//        else
//        {
//           Screen_Page03_Init = true;  
//        }
//        
//        wiFiConfig.Set_PC_Restart(false);
//        flag_boradInit = true;
//    }
//    flag_boradInit =true;
//    if(flag_boradInit)
//    {        
//        if(WiFi.status() != WL_CONNECTED)
//        {
//            if(Screen_Page == 10)
//            {
//              wiFiConfig.WIFI_Connenct();
//              Connect_UDP(Localport);
//            }
//           
//        }
//        else
//        {
//           
//           if(!flag_httpInit)
//           {
//              wiFiConfig.httpInit();
//              flag_httpInit = true;
//           }
//            wiFiConfig.HandleClient();  
//        }
//           
//    }
    
//    if(Screen_Page == 0)
//    {
//      Page00_Draw();
//    }
//    if(Screen_Page == 1)
//    {
//      Page01_Draw();
//    }
//    if(Screen_Page == 2)
//    {
//      Page02_Draw();
//    }
//    if(Screen_Page == 3)
//    {
//      Page03_Draw();     
//    }
   
}

void Task2code( void * pvParameters )
{
    for(;;)
    {

        if(flag_boradInit)
     {

       FastLED.show();        

     if( WiFi.status() == WL_CONNECTED  )
     {
       onPacketCallBack();
     }
     }

       
//        if(flag_boradInit)
//        {
//           serialEvent();
//           MyLED_WS2812.Blink();
//           FastLED.show();
//           if( WiFi.status() == WL_CONNECTED)
//           {
//             onPacketCallBack();
//             sub_UDP_Send();
//           }
//           
//        }
//        PlayBeep();

    }  
}

void WS2812_LED_ON()
{
    for(int i = 0 ; i < NUM_LEDS ; i++)
    {
       leds[i].r = WS2812_R;     // 将光带上第1个LED灯珠的RGB数值中R数值设置为255
       leds[i].g = WS2812_G;   // 将光带上第1个LED灯珠的RGB数值中G数值设置为255
       leds[i].b = WS2812_B;      // 将光带上第1个LED灯珠的RGB数值中B数值设置为0      
    } 
}
void WS2812_LED_OFF()
{
    for(int i = 0 ; i < NUM_LEDS ; i++)
    {
       leds[i].r = 0;     // 将光带上第1个LED灯珠的RGB数值中R数值设置为255
       leds[i].g = 0;   // 将光带上第1个LED灯珠的RGB数值中G数值设置为255
       leds[i].b = 0;      // 将光带上第1个LED灯珠的RGB数值中B数值设置为0      
    } 
}
bool IsDigit(char num)
{
  if(num>=48 && num<= 57)return true;
  return false;
}
int Ascii(char num)
{
  return (num - 48);
}
int Get_value(String str_value)
{
   int value = -1;
   char buf[6] ;
   if(str_value.length() < 5 ) str_value = "0" + str_value;
   if(str_value.length() < 5 ) str_value = "0" + str_value;
   if(str_value.length() < 5 ) str_value = "0" + str_value; 
   if(str_value.length() < 5 ) str_value = "0" + str_value; 
   if(str_value.length() < 5 ) str_value = "0" + str_value; 
   str_value.toCharArray(buf,6);
   if(IsDigit(buf[0]) && IsDigit(buf[1]) && IsDigit(buf[2]) && IsDigit(buf[3]) && IsDigit(buf[4]))
   {
     value = Ascii(buf[0]) * 10000 + Ascii(buf[1]) * 1000 + Ascii(buf[2]) * 100 + Ascii(buf[3]) * 10 + Ascii(buf[4]) * 1;
   }
   else
   {
        value = -3;
   }
    
   return value;
}
