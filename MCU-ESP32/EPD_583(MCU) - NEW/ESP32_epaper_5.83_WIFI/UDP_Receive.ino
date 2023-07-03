#include "Arduino.h"
#define UDP_BUFFER_SIZE 60000
#define UDP_RX_BUFFER_SIZE 4096
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

IPAddress packet_IP;

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
     if(flag_UDP0_packet ==false)
     {
        packet_UDP1 = Udp1.parsePacket();
        if(packet_UDP1 > 0) flag_UDP1_packet = true;
     }
  
     
     if(packet_UDP0 > 0 || packet_UDP1 > 0)
     {
        int len = 0;
        if(packet_UDP0 > 0)
        {
            remoteIP = Udp.remoteIP();
            remotePort = Udp.remotePort();       
            len = Udp.read(UdpRead_buf, UDP_RX_BUFFER_SIZE - 1);
            Serial.println("UDP <0> have packet");
        }
        if(packet_UDP1 > 0)
        {
            remoteIP = Udp1.remoteIP();
            remotePort = Udp1.remotePort();
            len = Udp1.read(UdpRead_buf, UDP_RX_BUFFER_SIZE - 1);
            Serial.println("UDP <1> have packet");
        }
        if(flag_UDP_header)
        {
            if(len != 4)
            {
               Serial.println("Received header length error!!");
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
            Serial.print("Received header length : ");
            Serial.println(UDPcheck_len);
        }
        else
        {
          for(int i = 0 ; i < len ; i++)
          {
             *(UdpRead + UdpRead_len + i) = *(UdpRead_buf + i);
          }
          UdpRead_len += len;
          Serial.print("Received len sumary :");
          Serial.println(UdpRead_len);
          if(UDPcheck_len == UdpRead_len)
          {
             if(*(UdpRead + UdpRead_len - 1) == 3)
             {
                flag_UDP_RX_OK = true;
                Serial.println("Received End code!!");
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
             Serial.println("-----RETRY!!-----");
             Send_StringTo("RETRY" ,remoteIP, remotePort);
          }
          break;
        }        
     }
  }
  
  if (flag_UDP_RX_OK)                     //如果有資料可用
  {
     Serial.print("UdpRead_len : ");
     Serial.println(String(UdpRead_len));
 
     if(flag_udp_232back)Serial.print("UdpRead_len : ");
     if(flag_udp_232back)Serial.println(String(UdpRead_len));
     if(flag_udp_232back)Serial.printf("remoteIP:");
     if(flag_udp_232back)Serial.println(remoteIP);
     if(flag_udp_232back)Serial.printf("remotePort:");
     if(flag_udp_232back)Serial.println(remotePort);

     if(*(UdpRead) == 2)
     {
        if(flag_udp_232back)Serial.println("接收到起始碼[2]");
        if(*(UdpRead + UdpRead_len - 1) == 3)
        {
          if(*(UdpRead + 1) == 'a' && UdpRead_len == 3)
          {  
            if(flag_udp_232back)Serial.printf("EPD Sleep\n");        
            epd.Sleep();
            Get_Checksum_UDP();          
          }
          
          else if(*(UdpRead + 1) == 'b' && UdpRead_len == 3)
          {
            if(flag_udp_232back)Serial.printf("EPD Wakeup\n");
            Get_Checksum_UDP();
            epd.Wakeup();        
          }  
          else if(*(UdpRead + 1) == 'c' && UdpRead_len == 3)
          {
            if(flag_udp_232back)Serial.printf("EPD DrawFrame_RW\n");
            epd.DrawFrame_RW();
            Get_Checksum_UDP();         
          } 
          else if(*(UdpRead + 1) == 'd' && UdpRead_len == 3)
          {
            if(flag_udp_232back)Serial.printf("EPD DrawFrame_BW\n");
            epd.DrawFrame_BW();
            Get_Checksum_UDP();           
          }  
          
          else if(*(UdpRead + 1) == 'f' && UdpRead_len == 3)
          {
            if(flag_udp_232back)Serial.printf("EPD RefreshCanvas\n");
            Get_Checksum_UDP();
            epd.RefreshCanvas();
                    }  
          else if (*(UdpRead + 1) == 'e')
          {
            int len = UdpRead_len - 7;
            int startpo_L = (*(UdpRead + 2)) | (*(UdpRead + 3) << 8);
            int startpo_H = (*(UdpRead + 4)) | (*(UdpRead + 5) << 8);
            long startpo = startpo_L | (startpo_H << 16);
            for(int i = 0 ; i < len ; i ++)
            {
               *(epd.framebuffer +startpo + i) = *(UdpRead + 6 + i);
            }
            if(flag_udp_232back)Serial.printf("EPD framebuffer\n");
            if(flag_udp_232back)Serial.printf("len : %d\n" ,len);
            if(flag_udp_232back)Serial.printf("startpo : %d\n" ,startpo);
            Get_Checksum_UDP();
          }
          else if (*(UdpRead + 1) == 'L')
          {
            int len = UdpRead_len - 5;
            int startpo = (*(UdpRead + 2)) | (*(UdpRead + 3) << 8);
            int numofLED = len / 3 ;
            int startLED = startpo / 3;         
            if(flag_udp_232back)Serial.printf("Set WS2812 Buffer\n");
            if(flag_udp_232back)Serial.printf("len : %d\n", len);
            if(flag_udp_232back)Serial.printf("startpo : %d\n", startpo);
            if(flag_udp_232back)Serial.printf("numofLED : %d\n", numofLED);
            if(flag_udp_232back)Serial.printf("startLED : %d\n", startLED);
            
            for(int i = 0 ; i < numofLED ; i++)
            {
               WS2812B_CRGB_BUF[i * 3 + startLED + 0] = *(UdpRead + 4 + i * 3 + 0);     // 将光带上第1个LED灯珠的RGB数值中R数值设置为255
               WS2812B_CRGB_BUF[i * 3 + startLED + 1] = *(UdpRead + 4 + i * 3 + 1);   // 将光带上第1个LED灯珠的RGB数值中G数值设置为255
               WS2812B_CRGB_BUF[i * 3 + startLED + 2] = *(UdpRead + 4 + i * 3 + 2);      // 将光带上第1个LED灯珠的RGB数值中B数值设置为0      
            }     
                     
            Get_Checksum_UDP();
          }
          else if (*(UdpRead + 1) == 'O')
          {           
            int num_L = *(UdpRead + 2);
            int num_H = *(UdpRead + 3);
            int num= num_L | (num_H << 8);
            if(num > NUM_WS2812B_CRGB * 3) num = NUM_WS2812B_CRGB * 3;
            if(flag_udp_232back)Serial.printf("Get WS2812 Buffer\n");
            if(flag_udp_232back)Serial.printf("num : %d\n", num);
//            if(flag_udp_232back)
//            {
//                for(int i = 0 ; i < num ; i++)
//                {
//                  Serial.printf("i : %d\n", WS2812B_CRGB_BUF[i]);
//                }
//            }
            Send_Bytes(WS2812B_CRGB_BUF, num ,remoteIP, remotePort);                    
          }
          else if(*(UdpRead + 1) == 'B')
          {                  
              if(flag_udp_232back)Serial.println("OTAUpdate");                               
              Get_Checksum_UDP();
              OTAUpdate_IP = IpAddress2String(wiFiConfig.Get_Server_IPAdressClass());
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
   Send_StringTo(str ,remoteIP ,remotePort);
}
void Connect_UDP(int localport)
{
    Udp.begin(localport);
    Udp1.begin(29500);
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
   Udp.beginPacket(remoteIP, remotePort); //準備傳送資料
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
      if(!flag_OTAUpdate)break;
      esp_task_wdt_reset();
      if(retry >= 3) break;
      WiFiClient client;
      
      if(flag_udp_232back)Serial.printf("ServerIP : %d  Port : %d\n" , IP ,Port);
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
      retry++;
      delay(1000);
    }
    
    
}
