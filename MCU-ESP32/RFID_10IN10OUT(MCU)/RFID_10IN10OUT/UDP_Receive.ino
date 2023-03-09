#include "Arduino.h"
#include <AsyncUDP.h>
AsyncUDP udp; 
char UdpRead[4096];
int UdpRead_len;
bool UDP_ISConnented = false;
IPAddress packet_IP;

WiFiUDP Udp;  

void onPacketCallBack()
{
  UdpRead_len = Udp.parsePacket(); //獲取當前隊首資料包長度
  
  if (UdpRead_len > 0)                     //如果有資料可用
  {
     Serial.print("UdpRead_len : ");
     Serial.println(String(UdpRead_len));
     Udp.read(UdpRead, UdpRead_len);
     packet_IP = Udp.remoteIP();  
     if(flag_udp_232back)Serial.print("UdpRead_len : ");
     if(flag_udp_232back)Serial.println(String(UdpRead_len));
     if(flag_udp_232back)Serial.printf("remoteIP: %d \n",Udp.remoteIP());
     if(flag_udp_232back)Serial.printf("remotePort: %d \n",Udp.remotePort());

     if(*(UdpRead) == 2)
     {
        if(*(UdpRead + UdpRead_len - 1) == 3)
        {
          if(*(UdpRead + 1) == 'B')
          {                  
              if(flag_udp_232back)Serial.println("OTAUpdate");                               
              Get_Checksum_UDP();
              
              OTAUpdate_IP = IpAddress2String(wiFiConfig.Get_Server_IPAdressClass());
              //OTAUpdate(OTAUpdate_IP , 8080); 
              flag_OTAUpdate = true;
          }
          else if(*(UdpRead + 1) == '9')
          {               
              if(flag_udp_232back)Serial.println("ESP Restart");     
              Get_Checksum_UDP();
              ESP.restart();    
                                                                    
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
              
              if(flag_udp_232back)Serial.printf("Server IP : %d.%d.%d.%d\n", (byte)IPA,(byte)IPB,(byte)IPC,(byte)IPD);
              if(flag_udp_232back)Serial.printf("Server Port : %d",port);

              wiFiConfig.Set_Server_IPAdress((byte)IPA,(byte)IPB,(byte)IPC,(byte)IPD);
              wiFiConfig.Set_Serverport(port);
              Get_Checksum_UDP();
          }
          else if(*(UdpRead + 1) == 'M')
          {                  
              int IPA = *(UdpRead + 2);
              int IPB = *(UdpRead + 3);
              int IPC = *(UdpRead + 4);
              int IPD = *(UdpRead + 5);
 
              
              if(flag_udp_232back)Serial.printf("Geteway : %d.%d.%d.%d\n", (byte)IPA,(byte)IPB,(byte)IPC,(byte)IPD);

              wiFiConfig.Set_Gateway((byte)IPA,(byte)IPB,(byte)IPC,(byte)IPD);
              Get_Checksum_UDP();
          }
          else if(*(UdpRead + 1) == 'N')
          {                  
              int port_L = *(UdpRead + 2);
              int port_H = *(UdpRead + 3);
              int port= port_L | (port_H << 8);
 
              
              if(flag_udp_232back)Serial.printf("LocalPort : %d\n", port);

              wiFiConfig.Set_Localport(port);
              Get_Checksum_UDP();
          }
          else if(*(UdpRead + 1) == 'I')
          {                  
              if(flag_udp_232back)Serial.println("Set_JsonStringSend");     
              flag_JsonSend = true;                          
              Get_Checksum_UDP();
          }
          else if(*(UdpRead + 1) == 'F')
          {                  
              int ms_L = *(UdpRead + 2);
              int ms_H = *(UdpRead + 3);  
              
              int ms= ms_L | (ms_H << 8);

              if(flag_udp_232back)Serial.println("Set_UDP_SendTime");
              if(flag_udp_232back)Serial.print("ms[");
              if(flag_udp_232back)Serial.print(ms);
              if(flag_udp_232back)Serial.println("]");

              wiFiConfig.Set_UDP_SemdTime(ms);                                           
              Get_Checksum_UDP();
          }
          else if(*(UdpRead + 1) == 'J')
          {    
              int value_L = *(UdpRead + 2);
              int value_H = *(UdpRead + 3);             
              int value= value_L | (value_H << 8);              
              if(flag_udp_232back)Serial.printf("Set_Output : %d\n" , value);
              SetOutput(value);
              Get_Checksum_UDP();
          }
          else if(*(UdpRead + 1) == 'K')
          {                  
             int pin_num = *(UdpRead + 2);
             int value = *(UdpRead + 3);
             if(flag_udp_232back)Serial.printf("Set_Output : [PIN Num]%d [value]%d\n" , pin_num , value);
             SetOutputPIN(pin_num , (value == 1));
             Get_Checksum_UDP();
          }
          else if(*(UdpRead + 1) == 'P')
          {    
              int value_L = *(UdpRead + 2);
              int value_H = *(UdpRead + 3);             
              int value= value_L | (value_H << 8);              
              if(flag_udp_232back)Serial.printf("Set_OutputTrigger : %d\n" , value);
              SetOutputTrigger(value);
              Get_Checksum_UDP();
          }
          else if(*(UdpRead + 1) == 'Q')
          {                  
             int pin_num = *(UdpRead + 2);
             int value = *(UdpRead + 3);
             if(flag_udp_232back)Serial.printf("Set_OutputPINTrigger : [PIN Num]%d [value]%d\n" , pin_num , value);
             SetOutputPINTrigger(pin_num , (value == 1));
             Get_Checksum_UDP();
          }
          else if(*(UdpRead + 1) == 'R')
          {                  
             int pin_num = *(UdpRead + 2);
             int value = *(UdpRead + 3);
             if(flag_udp_232back)Serial.printf("Set_Input_dir : [PIN Num]%d [value]%d\n" , pin_num , value);
             Set_Input_dir(pin_num , (value == 1));
             int temp = Get_Input_dir();
             if(flag_udp_232back)Serial.printf("Get_Input_dir : [value]%d\n" , temp);
             wiFiConfig.Set_Input_dir(temp);
             Get_Checksum_UDP();
          }
          else if(*(UdpRead + 1) == 'S')
          {                  
             int pin_num = *(UdpRead + 2);
             int value = *(UdpRead + 3);
             if(flag_udp_232back)Serial.printf("Set_Output_dir : [PIN Num]%d [value]%d\n" , pin_num , value);
             Set_Output_dir(pin_num , (value == 1));
             int temp = Get_Output_dir();
             if(flag_udp_232back)Serial.printf("Get_Output_dir : [value]%d\n" , temp);
             wiFiConfig.Set_Output_dir(temp);
             Get_Checksum_UDP();
          }
          else if(*(UdpRead + 1) == 'T')
          {                  
             int index = *(UdpRead + 2);
             int value = *(UdpRead + 3);
             if(flag_udp_232back)Serial.printf("Set_RFID_Enable : [index]%d [value]%d\n" , index , value);
             Set_RFID_Enable(index , (value == 1));
             Get_Checksum_UDP();
          }
          else if(*(UdpRead + 1) == 'U')
          {                  
             int station = *(UdpRead + 2);
             if(flag_udp_232back)Serial.printf("Set_RFID_Beep : [station]%d \n" , station);
             Set_Beep(station);
             Get_Checksum_UDP();
          }
          else if(*(UdpRead + 1) == 'V')
          {                  
             if(flag_udp_232back)Serial.printf("Get_LEDSetting  \n");
             byte send_byte[10];
             for(int i = 0 ; i < 5 ; i ++)
             {
                send_byte[i] = wiFiConfig.Get_LED_InputPIN(i);
             }
              for(int i = 0 ; i < 5 ; i ++)
             {
                send_byte[i + 5] = wiFiConfig.Get_LED_OutputPIN(i);
             }
             Send_Bytes(send_byte, 10 ,Udp.remoteIP(), wiFiConfig.localport);    
          }
           else if(*(UdpRead + 1) == 'X')
          {                  
             int input = GetInput();
             int output = GetOutput();
             
             byte send_byte[4];
             send_byte[0] = input >> 0;
             send_byte[1] = input >> 8;
             send_byte[2] = output >> 0;
             send_byte[3] = output >> 8;


             if(flag_udp_232back)Serial.printf("Get_IO [input]:%d ,[output]:%d\n" ,input ,output);
             Send_Bytes(send_byte, 4 ,Udp.remoteIP(), wiFiConfig.localport);    
          }
          else if(*(UdpRead + 1) == 'W')
          {                  
             if(flag_udp_232back)Serial.printf("Set_LEDSetting  \n");            
             byte temp;
             for(int i = 0 ; i < 5 ; i ++)
             {
                temp = *(UdpRead + 2 + i);
                wiFiConfig.Set_LED_InputPIN(i ,temp);
                if(flag_udp_232back)Serial.printf("inputPIN : %d  \n",temp); 
             }
              for(int i = 0 ; i < 5 ; i ++)
             {
                temp = *(UdpRead + 7 + i);
                wiFiConfig.Set_LED_OutputPIN(i ,temp);
                if(flag_udp_232back)Serial.printf("outputPIN : %d  \n",temp); 
             }
             Get_Checksum_UDP();
          }
          Udp.flush();
          if(flag_udp_232back)Serial.println("接收到結束碼[3]");
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
   String str = String(checksum);
   if(str.length() < 3) str = "0" + str;
   if(str.length() < 3) str = "0" + str;
   if(str.length() < 3) str = "0" + str;
   if(flag_udp_232back)Serial.println("Checksum String : " + str);
   if(flag_udp_232back)Serial.printf("Checksum Byte : %d \n" , checksum);
   Send_StringTo(str ,Udp.remoteIP() ,wiFiConfig.localport);
}
void Connect_UDP(int localport)
{
    Udp.begin(localport);
    //Serial.printf("UDP lisen : %D \n" ,localport);
}
void Send_Bytes(uint8_t *value ,int Size ,IPAddress RemoteIP ,int RemotePort)
{ 
   Udp.beginPacket(RemoteIP, RemotePort); //準備傳送資料
   Udp.write(value, Size); //複製資料到傳送快取
   Udp.endPacket();            //傳送資料
//   if(flag_udp_232back)Serial.write(str_checksum ,3);
//   if(flag_udp_232back)Serial.println("");
}
void Send_String(String str ,int remoteUdpPort)
{  
   Udp.beginPacket(Udp.remoteIP(), remoteUdpPort); //準備傳送資料
   Udp.print(str); //複製資料到傳送快取
   Udp.endPacket();            //傳送資料
  //udp.broadcastTo(buffer  ,remoteUdpPort);
}
void Send_StringTo(String str ,IPAddress RemoteIP ,int RemotePort)
{  
   Udp.beginPacket(RemoteIP, RemotePort); //準備傳送資料
   Udp.print(str); //複製資料到傳送快取
   Udp.endPacket();            //傳送資料
  //udp.broadcastTo(buffer  ,remoteUdpPort);
}

String IpAddress2String(const IPAddress& ipAddress)
{
  return String(ipAddress[0]) + String(".") +\
  String(ipAddress[1]) + String(".") +\
  String(ipAddress[2]) + String(".") +\
  String(ipAddress[3])  ; 
}

void OTAUpdate(String IP, int Port)
{
    int retry = 0;
    while(true)
    {
      esp_task_wdt_reset(); 
      if(retry >= 3) break;
      WiFiClient client;
      
      if(flag_udp_232back)Serial.printf("ServerIP : %d  Port : %d\n" , IP ,Port);
      esp_task_wdt_reset(); 
      t_httpUpdate_return ret = httpUpdate.update(client, IP, Port , "/update.bin");
      switch (ret) 
      {
        case HTTP_UPDATE_FAILED:
          Serial.printf("HTTP_UPDATE_FAILED Error (%d): %s\n", httpUpdate.getLastError(), httpUpdate.getLastErrorString().c_str());
          break;
  
        case HTTP_UPDATE_NO_UPDATES:
          Serial.println("HTTP_UPDATE_NO_UPDATES");
          break;
  
        case HTTP_UPDATE_OK:
          {
            wiFiConfig.Set_IsUpdate(true);
            Serial.println("HTTP_UPDATE_OK");
            return;
          }
          
          break;
      }
      esp_task_wdt_reset(); 
      retry++;
      delay(1000);
    }
    
    
}
