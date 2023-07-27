// include the necessary libraries
String Version = "Ver 1.2.3";

#include "FT62XXTouchScreen.h"
#include "Fonts.h"
#include "Button.h"
#include <Wire.h>
#include <SPI.h>
#include <TFT_eSPI.h> // Hardware-specific library
#include <WiFi.h>
#include <WiFiUdp.h>
#include <AsyncUDP.h> //引用以使用非同步UDP
#include <EEPROM.h>
#include "WiFiConfig.h"
#include "KeyBoardABC.h"
#include "KeyBoard123.h"
#include "Timer.h"
#include "LED.h"
#include "Output.h"
#include "Input.h"
#include "FastLED.h" 
#include <esp_task_wdt.h>
#include "esp32-hal-cpu.h"
#include "esp_system.h"
#include "esp_wifi.h"
#include "WiFiConfig.h"
#include "ESP32Ping.h"
#include <SPIFFS.h>
#include <SD.h>
#include <ArduinoJson.h>
#include <HTTPClient.h>
#include <HTTPUpdate.h>
#include "MyJPEGDecoder.h"

WiFiConfig wiFiConfig;

#define NUM_WS2812B_CRGB 8             // LED灯珠数量
#define LED_DT 26                // Arduino输出控制信号引脚
#define LED_TYPE WS2812B         // LED灯带型号
#define COLOR_ORDER GRB         // RGB灯珠中红色、绿色、蓝色LED的排列顺序
uint8_t max_bright = 255;       // LED亮度控制变量，可使用数值为 0 ～ 255， 数值越大则光带亮度越高
CRGB WS2812B_CRGB[NUM_WS2812B_CRGB];    
MyLED MyLED_WS2812;
int WS2812_R = 0;
int WS2812_G = 0;
int WS2812_B = 0;

WiFiServer server;
WiFiClient myclient;  
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
byte* Jpegbuffer;
int Jpegbuffer_len = 0;

FT62XXTouchScreen TouchScreen = FT62XXTouchScreen(DISPLAY_HEIGHT, PIN_SDA, PIN_SCL);
KeyBoardABC keyBoardABC = KeyBoardABC();
KeyBoard123 keyBoard123 = KeyBoard123();

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
int UDP_SemdTime = 0;
int Localport = 0;
IPAddress ServerIp;
int Serverport;
String GetwayStr;

int Screen_Page = 0;
int Screen_Page_buf = -1;
bool Screen_Page00_Init = false;
bool Screen_Page01_Init = false;
bool Screen_Page02_Init = false;
bool Screen_Page03_Init = false;
bool Screen_Page10_Init = false;
bool Screen_Page11_Init = false;
bool Screen_Page12_Init = false;
bool Screen_Page13_Init = false;
bool Screen_Page14_Init = false;
bool Screen_Page15_Init = false;
bool Screen_Page20_Init = false;
bool flag_ScreenPage_Init = false;

MyTimer MyTimer_BoardInit;
bool flag_boradInit = false;
bool flag_httpInit = false;
bool flag_JsonSend = false;

TaskHandle_t Core0Task1Handle;
TaskHandle_t Core0Task2Handle;
TaskHandle_t Core0Task3Handle;

TaskHandle_t Core1Task1Handle;
TaskHandle_t Core1Task2Handle;
TaskHandle_t Core1Task3Handle;



void setup()
{
  Serial.begin(115200);   
  setCpuFrequencyMhz(240); 
  Serial.setRxBufferSize(1024);  
  Serial.printf("CPU Frenqency is: %d  Mhz \n",getCpuFrequencyMhz());
  pinMode(27, OUTPUT);
  
  if(!SPIFFS.begin(true))
  {
     Serial.println("無法掛載 SPIFFS 檔案系統!");
  }
  else
  {
    Serial.printf("SPIFFS 總空間大小: %d\n", SPIFFS.totalBytes());
    Serial.printf("SPIFFS 使用空間大小: %d\n", SPIFFS.usedBytes());
  }
  framebuffer = (uint16_t *)ps_calloc(sizeof(uint16_t), DISPLAY_WIDTH * DISPLAY_HEIGHT);
  Jpegbuffer = (byte*)ps_calloc(sizeof(byte),45000);
  
  tft.init();
  tft.fillScreen(TFT_WHITE);
  tft.setRotation(1);
  TouchScreen.begin();

  
  wiFiConfig.SetTFT(tft);
  wiFiConfig.Init(Version);
  Localport = wiFiConfig.Get_Localport();
  Serverport = wiFiConfig.Get_Serverport();
  ServerIp = wiFiConfig.Get_Server_IPAdressClass();
  UDP_SemdTime = wiFiConfig.Get_UDP_SemdTime();
  
  GetwayStr = wiFiConfig.Get_Gateway_Str();
  
  
  pinMode(TFT_BL, OUTPUT);
  digitalWrite(TFT_BL, 1);
  
  
  MyTimer_BoardInit.StartTickTime(3000);
  LEDS.addLeds<LED_TYPE, LED_DT, COLOR_ORDER>(WS2812B_CRGB, NUM_WS2812B_CRGB);  // 初始化光带 
  FastLED.setBrightness(max_bright);                            // 设置光带亮度
  FastLED.clear();
  FastLED.show();
  MyLED_WS2812.BlinkTime = 0;
  MyLED_WS2812.LED_ON = WS2812_LED_ON;
  MyLED_WS2812.LED_OFF = WS2812_LED_OFF;
  
  keyBoardABC.Init();
  keyBoard123.Init();
  Page00_Init();
  Page01_Init();
  Page02_Init();
  Page03_Init();
  IO_Init();
  
 
  xTaskCreatePinnedToCore(Core0Task1,"Core0Task1",10000,NULL,1,&Core0Task1Handle,0); 
  xTaskCreatePinnedToCore(Core0Task2,"Core0Task2",10000,NULL,1,&Core0Task2Handle,0); 
  xTaskCreatePinnedToCore(Core0Task3,"Core0Task3",10000,NULL,1,&Core0Task3Handle,0); 
  
  xTaskCreatePinnedToCore(Core1Task1,"Core1Task1",10000,NULL,1,&Core1Task1Handle,1); 
  xTaskCreatePinnedToCore(Core1Task2,"Core1Task2",10000,NULL,1,&Core1Task2Handle,1); 
  xTaskCreatePinnedToCore(Core1Task3,"Core1Task3",10000,NULL,1,&Core1Task3Handle,1); 
}

TouchPoint touchPos;
extern bool touchNow = false;
int xPos_buf = 0;
int yPos_buf = 0;

void loop() 
{     
  
    if(MyTimer_BoardInit.IsTimeOut() && !flag_boradInit)
    {                                                   
        touchPos = TouchScreen_Read();
        touchPos.touched = true;
        Serial.printf("X: %d, Y: %d , touched: %d\n", touchPos.xPos, touchPos.yPos, touchPos.touched);
        if(!Button_Page00_Setting.IsPress(touchPos) && ( wiFiConfig.Get_PC_Restart() == 0))
        {           
           wiFiConfig.WIFI_Connenct();
           if(WiFi.status() == WL_CONNECTED)
           {                              
              Connect_UDP(Localport);
              Screen_Page10_Init = true;  
              //tft.fillScreen(TFT_WHITE);
           }
           else
           {
              Screen_Page03_Init = true;  
           }
        }
        else
        {
           Screen_Page03_Init = true;  
        }
        
        wiFiConfig.Set_PC_Restart(false);     
         
        flag_boradInit = true;
    }
    if(flag_boradInit)
    {      
      touchPos = TouchScreen_Read();
      if (touchPos.touched) 
      {
         if(flag_udp_232back)Serial.printf("X: %d, Y: %d , touched: %d\n", touchPos.xPos, touchPos.yPos, touchPos.touched);        
         flag_PlayBeep = true;        
      }
      if((touchPos.xPos != xPos_buf)||(touchPos.yPos != yPos_buf))
      {
         xPos_buf = touchPos.xPos;
         yPos_buf = touchPos.yPos;
         flag_JsonSend = true;
      }
    }
    if(WiFi.status() == WL_CONNECTED)
    {
       if(!flag_httpInit)
       {
          wiFiConfig.httpInit();
          flag_httpInit = true;
       }
       wiFiConfig.HandleClient();    
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
                if(Screen_Page == 10)
                {
                  wiFiConfig.WIFI_Connenct();
                  Connect_UDP(Localport);
                  Screen_Page10_Init = true;
                }               
            }
                           
        }
        PlayBeep();
        esp_task_wdt_reset();
        delay(1);
    }  
}
void Core0Task2( void * pvParameters )
{
    for(;;)
    {
       sub_IO_Program();
       esp_task_wdt_reset();
       delay(1);
    }  
}

MyTimer MyTimer_Touched500ms;
MyTimer MyTimer_Touched2000ms;
void Core0Task3( void * pvParameters )
{
    for(;;)
    {
        if (touchNow)
        {
           if(MyTimer_Touched500ms.IsTimeOutPulse())
           {
               MyLED_WS2812.BlinkTime = 0;
               flag_JsonSend = true;
           }
           if(MyTimer_Touched2000ms.IsTimeOutPulse() && Screen_Page == 10)
           {
               Screen_Page10_Init = true;
           }
        }
        else
        {
           MyTimer_Touched500ms.TickStop();
           MyTimer_Touched500ms.StartTickTime(500);
           MyTimer_Touched2000ms.TickStop();
           MyTimer_Touched2000ms.StartTickTime(2000);
        }
        
        if(flag_boradInit)
        {
          if(Screen_Page00_Init)
          {
            Screen_Page = 0;
            Page00_Refresh();
            Screen_Page00_Init = false;
            flag_JsonSend = true;
          }              
          if(Screen_Page01_Init)
          {
            Screen_Page = 1;
            Page01_Refresh();
            Screen_Page01_Init = false;
            flag_JsonSend = true;
          }
          if(Screen_Page02_Init)
          {
            Screen_Page = 2;
            Page02_Refresh(); 
            Screen_Page02_Init = false;
            flag_JsonSend = true;
          }
          if(Screen_Page03_Init)
          {
            Screen_Page = 3;
            Page03_Refresh(); 
            Screen_Page03_Init = false;
            flag_JsonSend = true;
          }
    
          if(Screen_Page10_Init)
          {     
            keyBoard123.Init();      
            tft.fillScreen(TFT_WHITE);  
            Screen_Page10_Init = false;
            Screen_Page = 10;
            flag_ScreenPage_Init = true; 
            flag_JsonSend = true;
          }        
          if(Screen_Page11_Init)
          {      
            tft.fillScreen(TFT_WHITE);   
            flag_ScreenPage_Init = true;
            Screen_Page11_Init = false;
            Screen_Page = 11;
            flag_JsonSend = true;
          }
          if(Screen_Page12_Init)
          {     
            tft.fillScreen(TFT_WHITE);   
            flag_ScreenPage_Init = true;
            Screen_Page12_Init = false;
            Screen_Page = 12;
            flag_JsonSend = true;
          }
          if(Screen_Page13_Init)
          {        
            tft.fillScreen(TFT_WHITE);   
            flag_ScreenPage_Init = true;
            Screen_Page13_Init = false;
            Screen_Page = 13;
            flag_JsonSend = true;
          }
          if(Screen_Page14_Init)
          {        
            tft.fillScreen(TFT_WHITE);   
            flag_ScreenPage_Init = true;
            Screen_Page14_Init = false;
            Screen_Page = 14;
            flag_JsonSend = true;
          }
          if(Screen_Page15_Init)
          {       
            tft.fillScreen(TFT_WHITE);   
            flag_ScreenPage_Init = true;
            Screen_Page15_Init = false;
            Screen_Page = 15;
            flag_JsonSend = true;
          }
          if(Screen_Page20_Init)
          {  
            keyBoard123.Init();     
            tft.fillScreen(TFT_WHITE);               
            Screen_Page = 20;
            flag_ScreenPage_Init = true;
            Screen_Page20_Init = false;          
            flag_JsonSend = true;
          }
          
          
          if(Screen_Page == 1)
          {
            Page01_Draw();
          }
          if(Screen_Page == 2)
          {
            Page02_Draw();
          }
          if(Screen_Page == 3)
          {
            Page03_Draw();     
          }
          if(Screen_Page == 20 && Screen_Page20_Init == false)
          {
            if(keyBoard123.Show(tft , touchPos) == 1)
            {
               MyLED_WS2812.BlinkTime = 0;
               flag_JsonSend = true;
            }
          }
      }
      if(Screen_Page == 0)
      {
        Page00_Draw();
      } 
      esp_task_wdt_reset();
      delay(1);
    }  
}

void Core1Task1( void * pvParameters )
{
    for(;;)
    {       
        if(flag_boradInit)
        {
           serialEvent();  
           if( WiFi.status() == WL_CONNECTED)
           {
             onPacketCallBack();
             sub_UDP_Send();
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
           MyLED_WS2812.Blink();
           FastLED.show();
        } 
        esp_task_wdt_reset();
        delay(1);
    }  
}
void Core1Task3( void * pvParameters )
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
void WS2812_LED_ON()
{
    for(int i = 0 ; i < NUM_WS2812B_CRGB ; i++)
    {
       WS2812B_CRGB[i].r = WS2812_R;     // 将光带上第1个LED灯珠的RGB数值中R数值设置为255
       WS2812B_CRGB[i].g = WS2812_G;   // 将光带上第1个LED灯珠的RGB数值中G数值设置为255
       WS2812B_CRGB[i].b = WS2812_B;      // 将光带上第1个LED灯珠的RGB数值中B数值设置为0      
    } 
}
void WS2812_LED_OFF()
{
    for(int i = 0 ; i < NUM_WS2812B_CRGB ; i++)
    {
       WS2812B_CRGB[i].r = 0;     // 将光带上第1个LED灯珠的RGB数值中R数值设置为255
       WS2812B_CRGB[i].g = 0;   // 将光带上第1个LED灯珠的RGB数值中G数值设置为255
       WS2812B_CRGB[i].b = 0;      // 将光带上第1个LED灯珠的RGB数值中B数值设置为0      
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

#define minimum(a,b)     (((a) < (b)) ? (a) : (b))
#define USE_SPI_BUFFER
//####################################################################################################
// Draw a JPEG on the TFT pulled from a program memory array
//####################################################################################################
void drawArrayJpeg(const uint8_t arrayname[], uint32_t array_size, int x, int y) {

  JpegDec.decodeArray(arrayname, array_size);

  jpegInfo(); // Print information from the JPEG file (could comment this line out)

  jpegRender(x, y);
  
  Serial.println("#########################");
}
//====================================================================================
//   Decode and render the Jpeg image onto the TFT screen
//====================================================================================
void jpegRender(int xpos, int ypos) {

  // retrieve infomration about the image
  uint16_t  *pImg;
  uint16_t mcu_w = JpegDec.MCUWidth;
  uint16_t mcu_h = JpegDec.MCUHeight;
  uint32_t max_x = JpegDec.width;
  uint32_t max_y = JpegDec.height;

  // Jpeg images are draw as a set of image block (tiles) called Minimum Coding Units (MCUs)
  // Typically these MCUs are 16x16 pixel blocks
  // Determine the width and height of the right and bottom edge image blocks
  uint32_t min_w = minimum(mcu_w, max_x % mcu_w);
  uint32_t min_h = minimum(mcu_h, max_y % mcu_h);

  // save the current image block size
  uint32_t win_w = mcu_w;
  uint32_t win_h = mcu_h;

  // record the current time so we can measure how long it takes to draw an image
  uint32_t drawTime = millis();

  // save the coordinate of the right and bottom edges to assist image cropping
  // to the screen size
  max_x += xpos;
  max_y += ypos;

  // read each MCU block until there are no more
#ifdef USE_SPI_BUFFER
  while ( JpegDec.readSwappedBytes()) { // Swap byte order so the SPI buffer can be used
#else
  while ( JpegDec.read()) { // Normal byte order read
#endif
    // save a pointer to the image block
    pImg = JpegDec.pImage;

    // calculate where the image block should be drawn on the screen
    int mcu_x = JpegDec.MCUx * mcu_w + xpos;
    int mcu_y = JpegDec.MCUy * mcu_h + ypos;

    if ( mcu_y < tft.height() )
    {
      // check if the image block size needs to be changed for the right edge
      if (mcu_x + mcu_w <= max_x) win_w = mcu_w;
      else win_w = min_w;

      // check if the image block size needs to be changed for the bottom edge
      if (mcu_y + mcu_h <= max_y) win_h = mcu_h;
      else win_h = min_h;

      // copy pixels into a smaller block
      if (win_w != mcu_w)
      {
        for (int h = 1; h < win_h; h++)
        {
          memcpy(pImg + h * win_w, pImg + h * mcu_w, win_w << 1);
        }
      }
      tft.pushImage(mcu_x, mcu_y, win_w, win_h, pImg);
    }
    else
    {
      JpegDec.abort();
    }
  }

  // calculate how long it took to draw the image
  drawTime = millis() - drawTime; // Calculate the time it took

  // print the results to the serial port
  Serial.print  ("Total render time was    : "); Serial.print(drawTime); Serial.println(" ms");
  Serial.println("=====================================");

}
void jpegInfo() {
  Serial.println(F("==============="));
  Serial.println(F("JPEG image info"));
  Serial.println(F("==============="));
  Serial.print(F(  "Width      :")); Serial.println(JpegDec.width);
  Serial.print(F(  "Height     :")); Serial.println(JpegDec.height);
  Serial.print(F(  "Components :")); Serial.println(JpegDec.comps);
  Serial.print(F(  "MCU / row  :")); Serial.println(JpegDec.MCUSPerRow);
  Serial.print(F(  "MCU / col  :")); Serial.println(JpegDec.MCUSPerCol);
  Serial.print(F(  "Scan type  :")); Serial.println(JpegDec.scanType);
  Serial.print(F(  "MCU width  :")); Serial.println(JpegDec.MCUWidth);
  Serial.print(F(  "MCU height :")); Serial.println(JpegDec.MCUHeight);
  Serial.println(F("==============="));
}
