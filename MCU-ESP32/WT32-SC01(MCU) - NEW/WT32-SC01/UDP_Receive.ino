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

IPAddress packet_IP;

byte checksum;
int x = 0;
int y = 0;
int width = 0;
int height = 0;
int BackColor;
int FontColor;
int ForeColor;

String filename_Code = "/Code.txt";


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
               Send_StringTo("RETRY" ,wiFiConfig.server_IPAdress, wiFiConfig.localport);
            }
            break;
          }        
       }
    }
    
    if (flag_UDP_RX_OK)  
    {     
      Udp.read(UdpRead, UDP_BUFFER_SIZE);
      packet_IP = Udp.remoteIP();  
      if(flag_udp_232back)Serial.print("UdpRead_len : ");
      if(flag_udp_232back)Serial.println(String(UdpRead_len));
      if(flag_udp_232back)Serial.printf("remoteIP: %d \n",Udp.remoteIP());
      if(flag_udp_232back)Serial.printf("remotePort: %d \n",Udp.remotePort());
      if(*(UdpRead) == 2)
      {
          if(flag_udp_232back)Serial.println("接收到起始碼[2]");
 
          if(*(UdpRead + UdpRead_len - 1) == 3)
          {
              if(*(UdpRead + 1) == '0')
              {
                if(flag_udp_232back)Serial.println("設定x,y,width,height");
                int x_L = *(UdpRead + 2);
                int x_H = *(UdpRead + 3);
                int y_L = *(UdpRead + 4);
                int y_H = *(UdpRead + 5);
                int width_L = *(UdpRead + 6);
                int width_H = *(UdpRead + 7);
                int height_L = *(UdpRead + 8);
                int height_H = *(UdpRead + 9);
                x = x_L | (x_H << 8);
                y = y_L | (y_H << 8);
                width = width_L | (width_H << 8);
                height = height_L | (height_H << 8);
                if(flag_udp_232back)Serial.print("X[");
                if(flag_udp_232back)Serial.print(x);
                if(flag_udp_232back)Serial.println("]");
                if(flag_udp_232back)Serial.print("Y[");
                if(flag_udp_232back)Serial.print(y);
                if(flag_udp_232back)Serial.println("]");
                if(flag_udp_232back)Serial.print("Width[");
                if(flag_udp_232back)Serial.print(width);
                if(flag_udp_232back)Serial.println("]");
                if(flag_udp_232back)Serial.print("Height[");
                if(flag_udp_232back)Serial.print(height);
                if(flag_udp_232back)Serial.println("]");
                Get_Checksum_UDP();
      
              }
              else if(*(UdpRead + 1) == '1')
              {
                
                int color_L = *(UdpRead + 2);
                int color_H = *(UdpRead + 3);
                BackColor = color_L | (color_H << 8);
                tft.fillRect(x,y,width,height,BackColor);
                if(flag_udp_232back)Serial.println("DrawRect");
                if(flag_udp_232back)Serial.printf("Color : %d \n", BackColor);
                Get_Checksum_UDP();
              }
              else if(*(UdpRead + 1) == '2')
              {
                int _data;
                int len = (UdpRead_len - 7) / 2;
                if(flag_udp_232back)Serial.println("設定GraphicData");
                int startpo_LL = *(UdpRead + 2);
                int startpo_LH = *(UdpRead + 3);
                int startpo_L = startpo_LL | (startpo_LH << 8);
                int startpo_HL = *(UdpRead + 4);
                int startpo_HH = *(UdpRead + 5);
                int startpo_H = startpo_HL | (startpo_HH << 8);
                long startpo = startpo_L | (startpo_H << 16); 
                if(flag_udp_232back)Serial.print("Start Position[");
                if(flag_udp_232back)Serial.print(startpo);
                if(flag_udp_232back)Serial.println("]");
                if(flag_udp_232back)Serial.print("Num Of Data[");
                if(flag_udp_232back)Serial.print(len);
                if(flag_udp_232back)Serial.println("]");        
                
                for(int i = 0 ; i < len ; i ++)
                {
                   _data = (*(UdpRead + 6 + i * 2)) |  (*(UdpRead + 7 + i * 2 ) << 8) ;   
                  *(framebuffer + startpo + i ) = _data;
                   
                }
  
                Get_Checksum_UDP();
              } 
              else if(*(UdpRead + 1) == 'j')
              {
                int len = (UdpRead_len - 2);
                if(flag_udp_232back)Serial.println("Set_Jpegbuffer");

                if(flag_udp_232back)Serial.print("Num Of Data[");
                if(flag_udp_232back)Serial.print(len);
                if(flag_udp_232back)Serial.println("]");        
                
                for(int i = 0 ; i < len ; i ++)
                {
                  *(Jpegbuffer + i ) = *(UdpRead + 2 + i);                  
                }
                Jpegbuffer_len = len;
                drawArrayJpeg(Jpegbuffer, Jpegbuffer_len, 0, 0);
                Get_Checksum_UDP();
              }
              else if(*(UdpRead + 1) == '3')
              {
                  if(flag_udp_232back)Serial.println("DrawCanvas");
                  tft.pushImage(x,y,width,height,framebuffer);
                  Get_Checksum_UDP();
              }
              else if(*(UdpRead + 1) == 'a')
              {
                  if(flag_udp_232back)Serial.println("DrawCanvasEx");
                  int x_L = *(UdpRead + 2);
                  int x_H = *(UdpRead + 3);
                  int y_L = *(UdpRead + 4);
                  int y_H = *(UdpRead + 5);
                  int width_L = *(UdpRead + 6);
                  int width_H = *(UdpRead + 7);
                  int height_L = *(UdpRead + 8);
                  int height_H = *(UdpRead + 9);
                  int _x = x_L | (x_H << 8);
                  int _y = y_L | (y_H << 8);
                  int _width = width_L | (width_H << 8);
                  int _height = height_L | (height_H << 8);
                  tft.pushImage(_x,_y,_width,_height,framebuffer);
                  delay(10);
                  Get_Checksum_UDP();
              }
              else if(*(UdpRead + 1) == '4')
              {                 
                  int x0_L = *(UdpRead + 2);
                  int x0_H = *(UdpRead + 3);
                  int y0_L = *(UdpRead + 4);
                  int y0_H = *(UdpRead + 5);
                  int x1_L = *(UdpRead + 6);
                  int x1_H = *(UdpRead + 7);
                  int y1_L = *(UdpRead + 8);
                  int y1_H = *(UdpRead + 9);
                  int x0 = x0_L | (x0_H << 8);
                  int y0 = y0_L | (y0_H << 8);
                  int x1 = x1_L | (x1_H << 8);
                  int y1 = y1_L | (y1_H << 8);

                  int color_L = *(UdpRead + 10);
                  int color_H = *(UdpRead + 11);
                  int Color = color_L | (color_H << 8);
                  
                  if(flag_udp_232back)Serial.println("drawLine");
                  if(flag_udp_232back)Serial.print("X0[");
                  if(flag_udp_232back)Serial.print(x0);
                  if(flag_udp_232back)Serial.println("]");
                  if(flag_udp_232back)Serial.print("Y0[");
                  if(flag_udp_232back)Serial.print(y0);
                  if(flag_udp_232back)Serial.println("]");
                  if(flag_udp_232back)Serial.print("X1[");
                  if(flag_udp_232back)Serial.print(x1);
                  if(flag_udp_232back)Serial.println("]");
                  if(flag_udp_232back)Serial.print("Y1[");
                  if(flag_udp_232back)Serial.print(y1);
                  if(flag_udp_232back)Serial.println("]");
                  if(flag_udp_232back)Serial.printf("Color : %d \n", Color);
                  tft.drawLine(x0,y0,x1,y1,Color);
                  Get_Checksum_UDP();
              }             
              else if(*(UdpRead + 1) == '5')
              {                  
                  int BlinkTime_L = *(UdpRead + 2);
                  int BlinkTime_H = *(UdpRead + 3);
                  int BlinkTime = BlinkTime_L | (BlinkTime_H << 8);
                  WS2812_R = *(UdpRead + 4);
                  WS2812_G = *(UdpRead + 5);
                  WS2812_B = *(UdpRead + 6);
                  MyLED_WS2812.BlinkTime = BlinkTime;
                  flag_JsonSend = true;
                  if(flag_udp_232back)Serial.println("Set_WS2812");
                  if(flag_udp_232back)Serial.print("BlinkTime[");
                  if(flag_udp_232back)Serial.print(BlinkTime);
                  if(flag_udp_232back)Serial.println("]");
                  if(flag_udp_232back)Serial.print("WS2812_R[");
                  if(flag_udp_232back)Serial.print(WS2812_R);
                  if(flag_udp_232back)Serial.println("]");
                  if(flag_udp_232back)Serial.print("WS2812_G[");
                  if(flag_udp_232back)Serial.print(WS2812_G);
                  if(flag_udp_232back)Serial.println("]");
                  if(flag_udp_232back)Serial.print("WS2812_B[");
                  if(flag_udp_232back)Serial.print(WS2812_B);
                  if(flag_udp_232back)Serial.println("]");
                  Get_Checksum_UDP();
              }
              else if(*(UdpRead + 1) == '6')
              {                  
                int _data;
                
                if(flag_udp_232back)Serial.println("設定GraphicDataEx");
                int UDP_len = (UdpRead_len - 9);
                int len_L = *(UdpRead + 2);
                int len_H = *(UdpRead + 3);
                int startpo_LL = *(UdpRead + 4);
                int startpo_LH = *(UdpRead + 5);
                int startpo_L = startpo_LL | (startpo_LH << 8);
                int startpo_HL = *(UdpRead + 6);
                int startpo_HH = *(UdpRead + 7);
                int startpo_H = startpo_HL | (startpo_HH << 8);
                long startpo = startpo_L | (startpo_H << 16); 
                int len = len_L | (len_H << 8);
                bool flag[8];
                if(flag_udp_232back)Serial.print("Start Position[");
                if(flag_udp_232back)Serial.print(startpo);
                if(flag_udp_232back)Serial.println("]");
                if(flag_udp_232back)Serial.print("Num Of Data[");
                if(flag_udp_232back)Serial.print(len);
                if(flag_udp_232back)Serial.println("]");        
                int index = 0;
                for(int i = 0 ; i < UDP_len ; i ++)
                {
                   _data = *(UdpRead + 8 + i);
                   flag[0] = (((_data >> 0) % 2) == 1);
                   flag[1] = (((_data >> 1) % 2) == 1);
                   flag[2] = (((_data >> 2) % 2) == 1);
                   flag[3] = (((_data >> 3) % 2) == 1);
                   flag[4] = (((_data >> 4) % 2) == 1);
                   flag[5] = (((_data >> 5) % 2) == 1);
                   flag[6] = (((_data >> 6) % 2) == 1);
                   flag[7] = (((_data >> 7) % 2) == 1);
                   
                   for(int k = 0 ; k < 8 ; k++)
                   {
                     if(index == len) break;
                     if(flag[k])
                     {
                        *(framebuffer + startpo + (i * 8) + k) = FontColor;
                     }
                     else
                     {
                        *(framebuffer + startpo + (i * 8) + k) = ForeColor;
                     }
                     index++;
                   }
                   
                }
  
                Get_Checksum_UDP();
              }
              else if(*(UdpRead + 1) == '7')
              {                  
                  int Fontcolor_L = *(UdpRead + 2);
                  int Fontcolor_H = *(UdpRead + 3);
                  FontColor = Fontcolor_L | (Fontcolor_H << 8);
                  int Forecolor_L = *(UdpRead + 4);
                  int Forecolor_H = *(UdpRead + 5);
                  ForeColor = Forecolor_L | (Forecolor_H << 8);

                  if(flag_udp_232back)Serial.println("Set_FontColor");
                  if(flag_udp_232back)Serial.print("FontColor[");
                  if(flag_udp_232back)Serial.print(FontColor);
                  if(flag_udp_232back)Serial.println("]");
                  if(flag_udp_232back)Serial.print("ForeColor[");
                  if(flag_udp_232back)Serial.print(ForeColor);
                  if(flag_udp_232back)Serial.println("]");
                  
                  Get_Checksum_UDP();
              }
              else if(*(UdpRead + 1) == 'b')
              { 
                  int value = 0;                  
                  value = keyBoard123.Value;
                  String str = String(value);
                  if(str.length() < 5) str = "0" + str;
                  if(str.length() < 5) str = "0" + str;
                  if(str.length() < 5) str = "0" + str;
                  if(str.length() < 5) str = "0" + str;
                  if(str.length() < 5) str = "0" + str;
                  Send_StringTo(str ,Udp.remoteIP() ,wiFiConfig.localport);  
                  if(flag_udp_232back)Serial.printf("GetKeyBoardValue value : %d\n" , str);                 
              }
              
              else if(*(UdpRead + 1) == '8')
              {          
                        
                  byte x_L = *(UdpRead + 2);
                  byte x_H = *(UdpRead + 3);
                  byte y_L = *(UdpRead + 4);
                  byte y_H = *(UdpRead + 5);
                  byte width_L = *(UdpRead + 6);
                  byte width_H = *(UdpRead + 7);
                  byte height_L = *(UdpRead + 8);
                  byte height_H = *(UdpRead + 9);                   
                  byte Fontcolor_L = *(UdpRead + 10);
                  byte Fontcolor_H = *(UdpRead + 11);
                  byte Forecolor_L = *(UdpRead + 12);
                  byte Forecolor_H = *(UdpRead + 13);
                 
                  x = x_L | (x_H << 8);
                  y = y_L | (y_H << 8);
                  width = width_L | (width_H << 8);
                  height = height_L | (height_H << 8);
                  int len = (width * height);  
                  
                  File file = SPIFFS.open("/test.txt","w");
                  if(!file)
                  {
                    Serial.printf("SPIFFS 無法開啟檔案!\n");
                  }
                  else
                  {
                    
                    byte temp_L = 0;
                    byte temp_H = 0;
                    file.write(x_L);
                    file.write(x_H);
                    file.write(y_L);
                    file.write(y_H);
                    file.write(width_L);
                    file.write(width_H);
                    file.write(height_L);
                    file.write(height_H);
                    file.write(Fontcolor_L);
                    file.write(Fontcolor_H);
                    file.write(Forecolor_L);
                    file.write(Forecolor_H);
                    for(int i = 0 ; i < len ; i++)
                    {
                       temp_L =  (*(framebuffer + i)) >> 0;
                       temp_H =  (*(framebuffer + i)) >> 8;
                       file.write(temp_L);
                       file.write(temp_H);
                    }
                    Serial.printf("檔案大小 : %d\n",file.size());
                    file.close();
                    Serial.printf("寫入檔案完成!\n");
                  }

                  
                  Get_Checksum_UDP();
              }
              else if(*(UdpRead + 1) == '9')
              {               
                  if(flag_udp_232back)Serial.println("ESP Restart");     
                  //wiFiConfig.Set_PC_Restart(true);
                  Get_Checksum_UDP();
                  tft.fillScreen(TFT_BLACK);   
                  ESP.restart();                                                            
              }
              else if(*(UdpRead + 1) == 'A')
              {                  
                  byte PIN = *(UdpRead + 2);  
                  byte State = *(UdpRead + 3);  

                  if(flag_udp_232back)Serial.println("Set_PIN");
                  if(flag_udp_232back)Serial.print("PIN[");
                  if(flag_udp_232back)Serial.print(PIN);
                  if(flag_udp_232back)Serial.println("]");
                  if(flag_udp_232back)Serial.print("State[");
                  if(flag_udp_232back)Serial.print(State);
                  if(flag_udp_232back)Serial.println("]");
                  
                  pinMode(PIN, OUTPUT);
                  if(State > 0) digitalWrite(PIN, HIGH);  
                  else digitalWrite(PIN, LOW);                                          
                  Get_Checksum_UDP();
              }
              else if(*(UdpRead + 1) == 'B')
              {                  
                  if(flag_udp_232back)Serial.println("OTAUpdate");                               
                  Get_Checksum_UDP();
                  String IP = IpAddress2String(wiFiConfig.Get_Server_IPAdressClass());
                  OTAUpdate(IP , 8080);
              }
              else if(*(UdpRead + 1) == 'C')
              {                                  
                  int page = *(UdpRead + 2);
                  if( page == 0 )
                  {
                     Screen_Page00_Init = true;                    
                  }
                  else if( page == 1 )
                  {
                     Screen_Page01_Init = true;
                  }
                  else if( page == 2 )
                  {
                     Screen_Page02_Init = true;
                  }  
                  else if( page == 3 )
                  {
                     Screen_Page03_Init = true;
                  }  
                  else if( page == 10 )
                  {
                     Screen_Page10_Init = true;
                  }  
                  else if( page == 11 )
                  {
                     Screen_Page11_Init = true;
                  } 
                  else if( page == 12 )
                  {
                     Screen_Page12_Init = true;
                  } 
                  else if( page == 13 )
                  {
                     Screen_Page13_Init = true;
                  } 
                  else if( page == 14 )
                  {
                     Screen_Page14_Init = true;
                  } 
                  else if( page == 15 )
                  {
                     Screen_Page15_Init = true;
                  } 
                  else if( page == 20 )
                  {
                     Screen_Page20_Init = true;
                  } 
                  Get_Checksum_UDP();
                  if(flag_udp_232back)Serial.println("SetToPage");   
                  if(flag_udp_232back)Serial.print("page[");
                  if(flag_udp_232back)Serial.print(page);
                  if(flag_udp_232back)Serial.println("]");
              }
 
              else if(*(UdpRead + 1) == 'E')
              {                  
                  int x_L = *(UdpRead + 2);
                  int x_H = *(UdpRead + 3);
                  int y_L = *(UdpRead + 4);
                  int y_H = *(UdpRead + 5);
                  int width_L = *(UdpRead + 6);
                  int width_H = *(UdpRead + 7);
                  int height_L = *(UdpRead + 8);
                  int height_H = *(UdpRead + 9);
                  int pen_width_L = *(UdpRead + 10);
                  int pen_width_H = *(UdpRead + 11);
                  int color_L = *(UdpRead + 12);
                  int color_H = *(UdpRead + 13);
                  
                  int x= x_L | (x_H << 8);
                  int y = y_L | (y_H << 8);
                  int width = width_L | (width_H << 8);
                  int height = height_L | (height_H << 8);
                  int pen_width = pen_width_L | (pen_width_H << 8);                
                  int Color = color_L | (color_H << 8);

                  if(flag_udp_232back)Serial.println("Set_DrawRect");
                  if(flag_udp_232back)Serial.print("x[");
                  if(flag_udp_232back)Serial.print(x);
                  if(flag_udp_232back)Serial.println("]");
                  if(flag_udp_232back)Serial.print("y[");
                  if(flag_udp_232back)Serial.print(y);
                  if(flag_udp_232back)Serial.println("]");
                  if(flag_udp_232back)Serial.print("width[");
                  if(flag_udp_232back)Serial.print(width);
                  if(flag_udp_232back)Serial.println("]");
                  if(flag_udp_232back)Serial.print("height[");
                  if(flag_udp_232back)Serial.print(height);
                  if(flag_udp_232back)Serial.println("]");
                  if(flag_udp_232back)Serial.print("pen_width[");
                  if(flag_udp_232back)Serial.print(pen_width);
                  if(flag_udp_232back)Serial.println("]");
                  if(flag_udp_232back)Serial.print("Color[");
                  if(flag_udp_232back)Serial.print(Color);
                  if(flag_udp_232back)Serial.println("]");


                  for(int i =0 ; i < pen_width ; i++)
                  {
                     tft.drawRect((x + i),( y + i),(width - i * 2),( height - i * 2), Color);
                  }
                                                           
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
              else if(*(UdpRead + 1) == 'G')
              {                  
                  int enable = *(UdpRead + 2);
    
                  if(flag_udp_232back)Serial.println("Set_Screen_Page_Init");
                  if(flag_udp_232back)Serial.print("enable[");
                  if(flag_udp_232back)Serial.print(enable);
                  if(flag_udp_232back)Serial.println("]");
                  if(enable > 0) flag_ScreenPage_Init = true; 
                  else flag_ScreenPage_Init = false; 

                  flag_JsonSend = true;  
                  Get_Checksum_UDP();
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
              else if(*(UdpRead + 1) == 'I')
              {                  
                  if(flag_udp_232back)Serial.println("Set_JsonStringSend");     
                  flag_JsonSend = true;                          
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
   Send_StringTo(str ,Udp.remoteIP() ,wiFiConfig.localport);
}
void Connect_UDP(int localport)
{
    Udp.begin(localport);
    Serial.printf("UDP lisen : %D \n" ,localport);
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
//
//   if(flag_udp_232back)Serial.print("remoteIP: "+ IpAddress2String(RemoteIP) +" \n");
//   if(flag_udp_232back)Serial.printf("remotePort: %d \n",RemotePort);
   Udp.beginPacket(RemoteIP, RemotePort); //準備傳送資料
   //Udp.print(str + "$" + wiFiConfig.Get_IPAdress_Str()); //複製資料到傳送快取
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
