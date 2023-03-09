#include "Arduino.h"
int cnt_UDP_Send = 65534;
MyTimer UDP_Send_Timer;
DynamicJsonDocument doc(1024);
String JsonOutput = "";
String RFID01 = "AA";
String RFID02 = "BB";
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
         doc["CardID_01"] = Card_00;
         doc["CardID_02"] = Card_01;
         doc["CardID_03"] = Card_02;
         doc["CardID_04"] = Card_03;
         doc["CardID_05"] = Card_04;
         
         doc["RSSI_01"] = RSSI_00;
         doc["RSSI_02"] = RSSI_01;
         doc["RSSI_03"] = RSSI_02;
         doc["RSSI_04"] = RSSI_03;
         doc["RSSI_05"] = RSSI_04;
     
         JsonOutput = "";
         serializeJson(doc, JsonOutput);
         Send_StringTo(JsonOutput, wiFiConfig.server_IPAdress, wiFiConfig.serverport);
         flag_JsonSend = false;
         cnt_UDP_Send++;
      }
  }
  if(cnt_UDP_Send == 3)
  {
     cnt_UDP_Send = 65535;
  }
}
