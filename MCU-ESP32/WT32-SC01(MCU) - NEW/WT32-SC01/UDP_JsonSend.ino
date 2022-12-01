#include "Arduino.h"
#define INPUT_LOCK 34
#define OUTPUT_LED_RED 5
#define OUTPUT_LED_GREEN 25
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
         doc["Version"] = Version;
         doc["Input"] = Input;
         doc["Output"] = Output;
         doc["Input_dir"] = Input_dir;
         doc["Output_dir"] = Output_dir;
         //doc["keyBoard123_value"] = keyBoard123.Value;
         doc["Touch_xPos"] = touchPos.xPos;
         doc["Touch_yPos"] = touchPos.yPos;
         doc["Touch_touched"] = touchPos.touched;
         doc["WS2812_State"] = (MyLED_WS2812.BlinkTime != 0);
         doc["INPUT_LOCK"] = digitalRead(INPUT_LOCK);
         doc["OUTPUT_LED_RED"] = digitalRead(OUTPUT_LED_RED);
         doc["OUTPUT_LED_GREEN"] = digitalRead(OUTPUT_LED_GREEN);
         doc["Screen_Page"] = Screen_Page;
         doc["ScreenPage_Init"] = flag_ScreenPage_Init;
         JsonOutput = "";
         serializeJson(doc, JsonOutput);
         Send_StringTo(JsonOutput, ServerIp, Serverport);
         flag_JsonSend = false;
         cnt_UDP_Send++;
      }
  }
  if(cnt_UDP_Send == 3)
  {
     cnt_UDP_Send = 65535;
  }
}
