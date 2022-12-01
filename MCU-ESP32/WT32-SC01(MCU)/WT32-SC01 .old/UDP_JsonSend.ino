#include "Arduino.h"
#define INPUT_LOCK 34
#define OUTPUT_LED_RED 5
#define OUTPUT_LED_GREEN 25
int cnt_UDP_Send = 65535;
MyTimer UDP_Send_Timer;
DynamicJsonDocument doc(1024);
String JsonOutput = "";

void sub_UDP_Send()
{
  if(cnt_UDP_Send == 65535)
  {
      if(flag_boradInit)cnt_UDP_Send = 1;
  }
  if(cnt_UDP_Send == 1)
  {  
      UDP_Send_Timer.TickStop();
      UDP_Send_Timer.StartTickTime(200);
      cnt_UDP_Send++;
  }
  if(cnt_UDP_Send == 2)
  {      
      if(UDP_Send_Timer.IsTimeOut())
      {
         doc["IP"] = wiFiConfig.Get_IPAdress_Str();
         doc["Port"] = wiFiConfig.Get_Localport();
         doc["Version"] = Version;
         doc["Touch_yPos"] = touchPos.xPos;
         doc["Touch_xPos"] = touchPos.yPos;
         doc["Touch_touched"] = touchPos.touched;
         doc["INPUT_LOCK"] = digitalRead(INPUT_LOCK);
         doc["OUTPUT_LED_RED"] = digitalRead(OUTPUT_LED_RED);
         doc["OUTPUT_LED_GREEN"] = digitalRead(OUTPUT_LED_GREEN);
         doc["Screen_Page"] = Screen_Page;
         doc["Command"] = "Test";    
         JsonOutput = "";
         serializeJson(doc, JsonOutput);
//         Send_StringTo(JsonOutput,ServerIp,Serverport);
         cnt_UDP_Send++;
      }
  }
  if(cnt_UDP_Send == 3)
  {
     cnt_UDP_Send = 65535;
  }
}
