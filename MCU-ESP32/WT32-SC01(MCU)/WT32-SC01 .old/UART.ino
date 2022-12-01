//#include "Timer.h"
//#include "Arduino.h"
//#define UART0_RX_SIZE 1024
//static byte UART0_RX[UART0_RX_SIZE];
//int UART0_len;
//MyTimer MyTimer_UART0;
//void serialEvent()
//{
//
//  if (Serial.available())
//  {
//    UART0_RX[UART0_len] = Serial.read();
//    UART0_len++;
//    MyTimer_UART0.TickStop();
//    MyTimer_UART0.StartTickTime(2);
//  }
//  if (MyTimer_UART0.IsTimeOut())
//  {
//    MyTimer_UART0.TickStop();
//    MyTimer_UART0.StartTickTime(1000);
//    if(UART0_RX[0] == 'W')
//    {
////        File file = SPIFFS.open("/test.txt","w");
////        int len = UART0_len -1;
////        if(!file)
////        {
////          Serial.printf("SPIFFS 無法開啟檔案!\n");
////        }
////        else
////        {
////          Serial.printf("檔案大小 : %d\n",framebuffer_len * 2);
////          byte temp_L = 0;
////          byte temp_H = 0;
////          for(int i = 0 ; i < framebuffer_len ; i++)
////          {
////             temp_L =  (*(framebuffer + i)) >> 0;
////             temp_H =  (*(framebuffer + i)) >> 8;
////             file.write(temp_L);
////             file.write(temp_H);
////          }
////          file.close();
////          Serial.printf("寫入檔案完成!\n");
////        }
////        
//    }
//    else if(UART0_RX[0] == 'R')
//    {
////        File file = SPIFFS.open("/test.txt","r");
////        int len = UART0_len -1;
////        if(!file)
////        {
////          Serial.printf("SPIFFS 無法開啟檔案!\n");
////        }
////        else
////        {
////          Serial.printf("檔案大小 : %d\n",file.size());
////          int temp = 0;
////          int temp_L = 0;
////          int temp_H = 0;
////          int x_L = file.read();
////          int x_H = file.read();
////          int y_L = file.read();
////          int y_H = file.read();
////          int width_L = file.read();
////          int width_H = file.read();
////          int height_L = file.read();
////          int height_H = file.read();                   
////          int Fontcolor_L = file.read();
////          int Fontcolor_H = file.read();
////          int Forecolor_L = file.read();
////          int Forecolor_H = file.read();
////          int x = x_L | (x_H << 8);
////          int y = y_L | (y_H << 8);
////          int width = width_L | (width_H << 8);
////          int height = height_L | (height_H << 8);
////          int Fontcolor = Fontcolor_L | (Fontcolor_H << 8);
////          int Forecolor = Forecolor_L | (Forecolor_H << 8);
////          
////          Serial.print("x[");
////          Serial.print(x);
////          Serial.println("]");
////          Serial.print("y[");
////          Serial.print(y);
////          Serial.println("]");
////          Serial.print("width[");
////          Serial.print(width);
////          Serial.println("]");
////          Serial.print("height[");
////          Serial.print(height);
////          Serial.println("]");
////          Serial.print("Fontcolor[");
////          Serial.print(Fontcolor);
////          Serial.println("]");
////          Serial.print("Forecolor[");
////          Serial.print(Forecolor);
////          Serial.println("]");
////          
////          for(int i = 0 ; i < (width * height) ; i++)
////          {
////              temp_L = file.read();
////              temp_H = file.read();
////              (*(framebuffer + i)) = (temp_H << 8) | temp_L;
////          }
////          file.close();
////          tft.pushImage(x,y,width,height,framebuffer);
////          Serial.printf("讀取檔案完成!\n");
////        }
//    }
//    UART0_len = 0;
//    for (int i = 0 ; i < UART0_RX_SIZE ; i++)
//    {
//      UART0_RX[i] = 0;
//    }
//  }
//
//}
//bool Check_Station(byte L, byte H)
//{
//  int station = L | (H << 8);
//  return (wiFiConfig.Get_Station() == station);
//}
//
//
//void Get_Checksum()
//{
//  byte checksum = 0;
//  for (int i = 0 ; i < UART0_len; i ++)
//  {
//    checksum += UART0_RX[i];
//  }
//  int checksum_2 = checksum / 100;
//  int checksum_1 = (checksum - checksum_2 * 100) / 10 ;
//  int checksum_0 = (checksum - checksum_2 * 100 - checksum_1 * 10) ;
//  byte str_checksum[3] = {(checksum_2 + 48), (checksum_1 + 48), (checksum_0 + 48)};
//  Serial.write(str_checksum , 3);
//  Serial.flush();
//
//}
