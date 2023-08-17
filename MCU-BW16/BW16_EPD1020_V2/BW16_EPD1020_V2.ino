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
#include "demo.h"

#define NUM_WS2812B_CRGB  450
#define NUM_OF_LEDS NUM_WS2812B_CRGB
#define SYSTEM_LED_PIN PA30

bool flag_udp_232back = true;
bool flag_JsonSend = false;
bool flag_writeMode = false;

WiFiServer server(80);

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
MyTimer MyTimer_ServerOnInit;
bool flag_boradInit = false;
MyLED MyLED_IS_Connented;

TaskHandle_t Core0Task1Handle;
TaskHandle_t Core0Task2Handle;
TaskHandle_t Core0Task3Handle;
TaskHandle_t Core0Task4Handle;

SoftwareSerial mySerial(PA8, PA7); // RX, TX
//SoftwareSerial mySerial(PB2, PB1); // RX, TX

String Version = "Ver 1.2.7";
char ssid[] = "EPD1020";  //Set the AP's SSID
char pass[] = "00000000";     //Set the AP's password
void setup() 
{      
    MyTimer_BoardInit.StartTickTime(3000);       
}
bool flag_connected = false;
void loop() 
{
   
   if(MyTimer_BoardInit.IsTimeOut() && !flag_boradInit)
   {     
       mySerial.begin(115200);   
       mySerial.println(Version);   
       wiFiConfig.mySerial = &mySerial;
       epd.mySerial = &mySerial;
       xTaskCreate(Core0Task1,"Core0Task1", 1024,NULL,1,&Core0Task1Handle);
       wiFiConfig.Init(Version);     
       wiFiConfig.Set_Localport(29012);
       wiFiConfig.Set_Serverport(30012);
       Localport = wiFiConfig.Get_Localport();
       Serverport = wiFiConfig.Get_Serverport();
       ServerIp = wiFiConfig.Get_Server_IPAdressClass();
       UDP_SemdTime = wiFiConfig.Get_UDP_SemdTime();
       GetwayStr = wiFiConfig.Get_Gateway_Str();
       MyLED_IS_Connented.Init(SYSTEM_LED_PIN);
       SPI.begin(); //SCLK, MISO, MOSI, SS
       myWS2812.Init(NUM_WS2812B_CRGB);
       epd.Init(); 
         
       flag_boradInit = true;
   }
   if(flag_boradInit)
   {
      
      if(WiFi.status() != WL_CONNECTED)
      {
         wiFiConfig.WIFI_Connenct();
         flag_connected = false;
         
         
      }   
      if(flag_WS2812B_Refresh)
      {
          myWS2812.Show();
        
          flag_WS2812B_Refresh = false;
      }  
      if(WiFi.status() == WL_CONNECTED)
      {
          if(!flag_connected)
          {
             Connect_UDP(Localport);
             server.begin();
             flag_connected = true;
          }
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
          sub_IO_Program();
          MyLED_IS_Connented.Blink();
          if( WiFi.status() == WL_CONNECTED  )
          {
              MyLED_IS_Connented.BlinkTime = 100;      
          }
          else
          {
              MyLED_IS_Connented.BlinkTime = 500;
          }
          
          epd.Sleep_Check();
       }
          
       delay(10);
    }
    
}
//bool flag_MyTimer_server_init = false;
//bool flag_server_init = false;
//void Core0Task2( void * pvParameters )
//{
//    for(;;)
//    {      
//       
//       if(flag_boradInit)
//       {
//          
//         
//          if( WiFi.status() == WL_CONNECTED  )
//          {
//
//              if(!flag_MyTimer_server_init)
//              {
//                 MyTimer_ServerOnInit.StartTickTime(3000);                 
//                 flag_MyTimer_server_init = true;
//              }
//              if(MyTimer_ServerOnInit.IsTimeOut() && !flag_server_init)
//              {
//                 
//                 flag_server_init = true;
//              }
//              if(flag_server_init)
//              {
//                WiFiClient client = server.available();     // listen for incoming clients
//
//                if (client)
//                {                               // if you get a client,
//                    mySerial.println("new client");           // print a message out the serial port
//                    String currentLine = "";                // make a String to hold incoming data from the client
//                    while (client.connected()) {            // loop while the client's connected
//                        if (client.available()) {           // if there's bytes to read from the client,
//                            char c = client.read();         // read a byte, then
//                            mySerial.write(c);                // print it out the serial monitor
//                            if (c == '\n') 
//                            {           
//                                if (currentLine.length() == 0)
//                                {
//            
//                                    client.println("HTTP/1.1 200 OK");
//                                    client.println("Content-type:text/html");
//                                    //client.println("Connection: close");
//                                    client.println();
//                                    client.println("<!DOCTYPE html>");
//                                    client.println("<html lang='zh-hans'>");
//                                    client.println("<head>");
//                                    client.println("<title>電子紙屏刷新</title>");
//                                    client.println("<meta charset='utf-8' />");
//                                    client.println("</head>");
//                                    client.println("<body>");
//                                
//                                    client.println("<h1 name='超連結' style='color:green;'>電子紙屏幕刷新</h1>");
//                                    
//                                    client.println(" <p style='font-size:40px;font-weight:bold'>型式【A】</p>");
//                                    client.println("<a href='/H' style='font-size:30px;'>刷新</a>");
//                                    client.println(" <p style='font-size:40px;font-weight:bold'>型式【B】</p>");
//                                    client.println("<a href='/L' style='font-size:30px;'>刷新</a>");
//                                    
//  
//                                    client.println("</body>");
//                                    client.println("</html>");
//                                    client.println();
//                                    break;
//                                } 
//                                else
//                                {    
//                                    currentLine = "";
//                                }
//                            } 
//                            else if 
//                            (c != '\r')
//                            { 
//                                currentLine += c; 
//                            }
//    
//                            if (currentLine.endsWith("GET /H")) 
//                            {
//                                epd.WhiteScreen_ALL(gImage_BW01,gImage_RW01);                             
//                                client.println("<script>document.location.href='http://" + wiFiConfig.Get_IPAdress_Str() + "';</script>");            
//                            }
//                            if (currentLine.endsWith("GET /L"))
//                            {
//                                epd.WhiteScreen_ALL(gImage_BW02,gImage_RW02); 
//                                client.println("<script>document.location.href='http://" + wiFiConfig.Get_IPAdress_Str() + "';</script>");                 
//                            }
//                        }
//                    }
//                   
//                                       
//                    client.stop();
//                    mySerial.println("client disonnected");
//                }
//              }
//              
//          }
//         
//       }
//          
//       delay(10);
//    }
//    
//}
