#include "Timer.h"
#define UART1_RX_SIZE 1024
byte UART1_RX[UART1_RX_SIZE];
int UART1_len;
MyTimer MyTimer_UART1;
String str_distance = "999";
int LaserDistance = -999;
bool flag_UART1_Init = false;
void serial2Event()
{
  if(!flag_UART1_Init)
  {
     if(!wiFiConfig.Get_IsLocker())
     {
        mySerial.println("mySerial2 init done!!");
        mySerial2.begin(9600);         
     }
     
     flag_UART1_Init = true;
  }
  
  if (mySerial2.available())
  {
    if(UART1_len >= UART1_RX_SIZE)
    {
       mySerial.println("mySerial2 buffer overrange!!");
       UART1_len = 0;
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

    if (UART1_RX[UART1_len - 1] == 0X0A && UART1_RX[UART1_len - 2] == 0X0D && UART1_RX[UART1_len - 3] == 0X0D && UART1_RX[UART1_len - 4] == 0X6D && UART1_RX[UART1_len - 5] == 0X6D)
    {
        
      str_distance = "";
      for (int i = 0 ; i < UART1_len - 5 ; i++)
      {
         str_distance += (char)UART1_RX[i];
      }

      LaserDistance = str_distance.toInt();
//      mySerial.print("LaserDistance : ");
//      mySerial.println(str_distance);
    }
    
    UART1_len = 0;
    for (int i = 0 ; i < UART1_RX_SIZE ; i++)
    {
      UART1_RX[i] = 0;
    }
  }
}
