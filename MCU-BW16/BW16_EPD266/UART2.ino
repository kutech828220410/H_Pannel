#include "Timer.h"
#define UART1_RX_SIZE 256
byte UART1_RX[UART1_RX_SIZE];
int UART1_len;
MyTimer MyTimer_UART1;
String str_distance = "999";
String str_TOF10120 = "";
int LaserDistance = -999;
int LASER_ON_cnt = 0;
bool LASER_ON = false;
bool LASER_ON_buf = false;
bool flag_UART1_Init = false;
int LASER_ON_num = 0;
void serial2Event()
{
  if(!flag_UART1_Init)
  {
     mySerial.println("mySerial2 init done!!");
     mySerial2.begin(9600);    
     
     flag_UART1_Init = true;
  }
  
  if (mySerial2.available())
  {
    if(UART1_len >= UART1_RX_SIZE)
    {
       mySerial.println("mySerial2 buffer overrange!!");
       UART1_len = 0;
       str_TOF10120 = "";
    }
    else
    {
       
       UART1_RX[UART1_len] = mySerial2.read();
       UART1_len++;
       MyTimer_UART1.TickStop();
       MyTimer_UART1.StartTickTime(2);
    }
    
  }
  if (MyTimer_UART1.IsTimeOut())
  {
    MyTimer_UART1.TickStop();
    MyTimer_UART1.StartTickTime(1000);
//    for (int i = 0 ; i < UART1_len ; i++)
//    {
//       str_TOF10120 += (char)UART1_RX[i];
//    }
//    mySerial.println(str_TOF10120);
    if (UART1_RX[UART1_len - 1] == 0X0A && UART1_RX[UART1_len - 2] == 0X0D && UART1_RX[UART1_len - 3] == 0X0D && UART1_RX[UART1_len - 4] == 0X6D && UART1_RX[UART1_len - 5] == 0X6D)
    {
        
      str_distance = "";
      for (int i = 0 ; i < UART1_len - 5 ; i++)
      {
         str_distance += (char)UART1_RX[i];
      }

      LaserDistance = str_distance.toInt();
      if((LaserDistance <= LASER_D_MAX) && (LaserDistance >= LASER_D_MIN))
      {
         LASER_ON_cnt++;
      }
      else
      {
         LASER_ON_cnt = 0;
         LASER_ON = false;
      }
      if(LASER_ON_cnt >= 2)
      {
         LASER_ON = true;
      }
      else
      {
         LASER_ON = false;
      }
      if(LASER_ON == true)
      {
         flag_JsonSend = true;
      }
      if(LASER_ON_buf != LASER_ON)
      {
         LASER_ON_buf = LASER_ON;
         if(LASER_ON)
         {
           LASER_ON_num++;
         }
         flag_JsonSend = true;
      }
//      mySerial.print("LaserDistance : ");
//      mySerial.println(str_distance);
    }
    
    UART1_len = 0;
    str_TOF10120 = "";
    for (int i = 0 ; i < UART1_RX_SIZE ; i++)
    {
      UART1_RX[i] = 0;
    }
  }
}
