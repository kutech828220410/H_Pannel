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

String Version = "Ver 1.0.0";
char ssid[] = "EPD420";  //Set the AP's SSID
char pass[] = "00000000";     //Set the AP's password
char channel[] = "1";         //Set the AP's channel
int status = WL_IDLE_STATUS;  //Set the Wifi radio's status
int ssid_status = 0;          //Set SSID status, 1 hidden, 0 not hidden

void setup() 
{


    mySerial.begin(115200); 
    MyLED_IS_Connented.Init(SYSTEM_LED_PIN);   
    MyTimer_BoardInit.StartTickTime(3000);
    // check for the presence of the shield:
    if (WiFi.status() == WL_NO_SHIELD) 
    {
        mySerial.println("WiFi shield not present");
        while (true);
    }

    while (status != WL_CONNECTED) 
    {
        mySerial.print("Attempting to start AP with SSID: ");
        mySerial.println(ssid);
        status = WiFi.apbegin(ssid, pass, "6");  
        delay(5000);
    }
    //xTaskCreate(Core0Task1,"Core0Task1", 1024,NULL,1,&Core0Task1Handle);

}
bool flag_connected = false;
void loop() 
{
   
   if(MyTimer_BoardInit.IsTimeOut() && !flag_boradInit)
   {     
       mySerial.begin(115200);   
        
       wiFiConfig.mySerial = &mySerial;
       epd.mySerial = &mySerial;
       //wiFiConfig.Init(Version);
       SPI.begin(); //SCLK, MISO, MOSI, SS
       myWS2812.Init(NUM_WS2812B_CRGB);
       epd.Init(); 
       epd.Wakeup();
       epd.Clear();
       server.begin();
       xTaskCreate(Core0Task2,"Core0Task2", 1024,NULL,1,&Core0Task2Handle);
       flag_boradInit = true;
   }
   if(flag_boradInit)
   {
      serialEvent();      
      if(flag_WS2812B_Refresh)
      {
          myWS2812.Show();
        
          flag_WS2812B_Refresh = false;
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
bool flag_MyTimer_server_init = false;
bool flag_server_init = false;
void Core0Task2( void * pvParameters )
{
    for(;;)
    {      
       
       if(flag_boradInit)
       {
              if(!flag_MyTimer_server_init)
              {
                 MyTimer_ServerOnInit.StartTickTime(3000);                 
                 flag_MyTimer_server_init = true;
              }
              if(MyTimer_ServerOnInit.IsTimeOut() && !flag_server_init)
              {
                 
                 flag_server_init = true;
              }
              if(flag_server_init)
              {
                WiFiClient client = server.available();     // listen for incoming clients

                if (client)
                {                               // if you get a client,
                    mySerial.println("new client");           // print a message out the serial port
                    String currentLine = "";                // make a String to hold incoming data from the client
                    while (client.connected()) {            // loop while the client's connected
                        if (client.available()) {           // if there's bytes to read from the client,
                            char c = client.read();         // read a byte, then
                            mySerial.write(c);                // print it out the serial monitor
                            if (c == '\n') 
                            {           
                                if (currentLine.length() == 0)
                                {
            
                                    client.println("HTTP/1.1 200 OK");
                                    client.println("Content-type:text/html");
                                    //client.println("Connection: close");
                                    client.println();
                                    client.println("<!DOCTYPE html>");
                                    client.println("<html lang='zh-hans'>");
                                    client.println("<head>");
                                    client.println("<title>電子紙屏刷新</title>");
                                    client.println("<meta charset='utf-8' />");
                                    client.println("</head>");
                                    client.println("<body>");
                                
                                    client.println("<h1 name='超連結' style='color:green;'>電子紙屏幕刷新</h1>");
                                    
                                    client.println(" <p style='font-size:40px;font-weight:bold'>型式【A】</p>");
                                    client.println("<a href='/H' style='font-size:30px;'>刷新</a>");
                                    client.println(" <p style='font-size:40px;font-weight:bold'>型式【B】</p>");
                                    client.println("<a href='/L' style='font-size:30px;'>刷新</a>");
                                    
  
                                    client.println("</body>");
                                    client.println("</html>");
                                    client.println();
                                    break;
                                } 
                                else
                                {    
                                    currentLine = "";
                                }
                            } 
                            else if 
                            (c != '\r')
                            { 
                                currentLine += c; 
                            }
    
                            if (currentLine.endsWith("GET /H")) 
                            {
                                epd.WhiteScreen_ALL(gImage_BW01,gImage_RW01);                             
                            }
                            if (currentLine.endsWith("GET /L"))
                            {
                                epd.WhiteScreen_ALL(gImage_BW02,gImage_RW02); 
//                                client.println("<script>document.location.href='http://" + wiFiConfig.Get_IPAdress_Str() + "';</script>");                 
                            }
                        }
                    }
                   
                                       
                    client.stop();
                    mySerial.println("client disonnected");
                }
              }
              
         
       }
          
       delay(10);
    }
    
}
