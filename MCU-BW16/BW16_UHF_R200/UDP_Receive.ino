#include "Arduino.h"
#define UDP_BUFFER_SIZE 60000
#define UDP_RX_BUFFER_SIZE 1500
char* UdpRead;
char* UdpRead_buf;
bool UDP_ISConnented = false;
int UdpRead_len = 0;
int UdpRead_len_buf = 0;
long UDPcheck_len = 0;
WiFiUDP Udp; 
bool flag_UDP_RX_BUFFER_Init = false;
bool flag_UDP_RX_OK = false;
bool flag_UDP_header = true;
MyTimer MyTimer_UDP;
MyTimer MyTimer_UDP_RX_TimeOut;
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
  UdpRead_len = 0;
  UDPcheck_len = 0;
  MyTimer_UDP.TickStop();
  MyTimer_UDP.StartTickTime(0);
  while(true)
  {
     int flag_packet = Udp.parsePacket();
     if(flag_packet > 0)
     {
        int len = Udp.read(UdpRead_buf, UDP_RX_BUFFER_SIZE - 1);
        if(flag_UDP_header)
        {
            if(len != 4)
            {
               mySerial.println("Received header length error!!");
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
            mySerial.print("Received header length : ");
            mySerial.println(UDPcheck_len);
        }
        else
        {
          for(int i = 0 ; i < len ; i++)
          {
             *(UdpRead + UdpRead_len + i) = *(UdpRead_buf + i);
          }
          UdpRead_len += len;
          if(UDPcheck_len == UdpRead_len)
          {
             if(*(UdpRead + UdpRead_len - 1) == 3)
             {
                flag_UDP_RX_OK = true;
                mySerial.println("Received End code!!");
                break;
             }              
          }
          
         
        }        
        MyTimer_UDP.TickStop();
        MyTimer_UDP.StartTickTime(100);
     }
     else
     {
        if(MyTimer_UDP.IsTimeOut())
        {
          if(UdpRead_len > 0)
          {
             mySerial.println("-----RETRY!!-----");
             Send_StringTo("RETRY" ,wiFiConfig.server_IPAdress, wiFiConfig.localport);
          }
          break;
        }        
     }
  }

  if (flag_UDP_RX_OK)                     //如果有資料可用
  {
     if(flag_udp_232back) 
     {
        mySerial.print("Received packet of size ");
        mySerial.println(UdpRead_len);
        mySerial.print("From ");
        IPAddress remoteIp = Udp.remoteIP();
        mySerial.print(remoteIp);
        mySerial.print(", port ");
        mySerial.println(Udp.remotePort());
        
     }      
     if(*(UdpRead) == 2)
     {
        if(flag_udp_232back)mySerial.println("接收到起始碼[2]");
        if(*(UdpRead + UdpRead_len - 1) == 3)
        {                                             
          if(*(UdpRead + 1) == 'B')
          {                  
              String serverIP = wiFiConfig.server_IPAdress_str;
              int str_len = serverIP.length() + 1; 
              char serverIP_char_array[str_len];
              serverIP.toCharArray(serverIP_char_array , str_len );
              mySerial.print("Host :");
              mySerial.print(serverIP_char_array);
              mySerial.print(", port :");
              mySerial.println(8080);
              
              int ret = http_update_ota(serverIP_char_array, 8080, RESOURCE);
              printf("[%s] Update task exit\n\r", __FUNCTION__);
              if(!ret)
              {
                printf("[%s] Ready to reboot\n\r", __FUNCTION__); 
                ota_platform_reset();
              }
          }
          else if(*(UdpRead + 1) == 'J')
          {    
              int value_L = *(UdpRead + 2);
              int value_H = *(UdpRead + 3);             
              int value= value_L | (value_H << 8);              
              if(flag_udp_232back)printf("Set_Output : %d\n" , value);
              SetOutput(value);
              Get_Checksum_UDP();
          }
          else if(*(UdpRead + 1) == 'K')
          {                  
             int pin_num = *(UdpRead + 2);
             int value = *(UdpRead + 3);
             if(flag_udp_232back)printf("Set_Output : [PIN Num]%d [value]%d\n" , pin_num , value);
             SetOutputPIN(pin_num , (value == 1));
             Get_Checksum_UDP();
          }
          else if(*(UdpRead + 1) == 'P')
          {    
              int value_L = *(UdpRead + 2);
              int value_H = *(UdpRead + 3);             
              int value= value_L | (value_H << 8);              
              if(flag_udp_232back)printf("Set_OutputTrigger : %d\n" , value);
              SetOutputTrigger(value);
              Get_Checksum_UDP();
          }
          else if(*(UdpRead + 1) == 'Q')
          {                  
             int pin_num = *(UdpRead + 2);
             int value = *(UdpRead + 3);
             if(flag_udp_232back)printf("Set_OutputPINTrigger : [PIN Num]%d [value]%d\n" , pin_num , value);
             SetOutputPINTrigger(pin_num , (value == 1));
             Get_Checksum_UDP();
          }
          else if(*(UdpRead + 1) == 'R')
          {                  
             int pin_num = *(UdpRead + 2);
             int value = *(UdpRead + 3);
             if(flag_udp_232back)printf("Set_Input_dir : [PIN Num]%d [value]%d\n" , pin_num , value);
             Set_Input_dir(pin_num , (value == 1));
             int temp = Get_Input_dir();
             if(flag_udp_232back)printf("Get_Input_dir : [value]%d\n" , temp);
             wiFiConfig.Set_Input_dir(temp);
             Get_Checksum_UDP();
          }
          else if(*(UdpRead + 1) == 'S')
          {                  
             int pin_num = *(UdpRead + 2);
             int value = *(UdpRead + 3);
             if(flag_udp_232back)printf("Set_Output_dir : [PIN Num]%d [value]%d\n" , pin_num , value);
             Set_Output_dir(pin_num , (value == 1));
             int temp = Get_Output_dir();
             if(flag_udp_232back)printf("Get_Output_dir : [value]%d\n" , temp);
             wiFiConfig.Set_Output_dir(temp);
             Get_Checksum_UDP();
          }
          if(flag_udp_232back)
          {
              mySerial.println("接收到結束碼[3]");
              mySerial.println("-------------------------------------------");
          }
           
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
   String str0 = String(checksum);
   if(str0.length() < 3) str0 = "0" + str0;
   if(str0.length() < 3) str0 = "0" + str0;
   if(str0.length() < 3) str0 = "0" + str0;
   if(flag_udp_232back)printf("Checksum String : %d\n",str0);
   if(flag_udp_232back)printf("Checksum Byte : %d \n" , checksum);
   Send_StringTo(str0 ,wiFiConfig.server_IPAdress, wiFiConfig.localport);
   if(flag_udp_232back)printf("Send_StringTo");
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
}
void Send_String(String str ,int remoteUdpPort)
{  
   Udp.beginPacket(Udp.remoteIP(), remoteUdpPort); //準備傳送資料
   Udp.print(str); //複製資料到傳送快取
   Udp.endPacket();            //傳送資料
}
void Send_StringTo(String str ,IPAddress RemoteIP ,int RemotePort)
{  
   Udp.beginPacket(RemoteIP, RemotePort); //準備傳送資料
   Udp.print(str); //複製資料到傳送快取
   Udp.endPacket();            //傳送資料
}

String IpAddress2String(const IPAddress& ipAddress)
{
  return String(ipAddress[0]) + String(".") +\
  String(ipAddress[1]) + String(".") +\
  String(ipAddress[2]) + String(".") +\
  String(ipAddress[3])  ; 
}
