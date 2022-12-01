#include "Timer.h"
#define UART2_RX_SIZE 128
static byte UART2_RX[UART2_RX_SIZE];
int UART2_len;
MyTimer MyTimer_UART2;
String str_distance = "999";
int LaserDistance = -999;
void serial2Event()
{
  if (Serial2.available())
  {
    UART2_RX[UART2_len] = Serial2.read();
    UART2_len++;
    MyTimer_UART2.TickStop();
    MyTimer_UART2.StartTickTime(2);
  }
  if (MyTimer_UART2.IsTimeOut())
  {
    MyTimer_UART2.TickStop();
    MyTimer_UART2.StartTickTime(1000);
    if (UART2_RX[UART2_len - 1] == 0X0A && UART2_RX[UART2_len - 2] == 0X0D && UART2_RX[UART2_len - 3] == 0X0D && UART2_RX[UART2_len - 4] == 0X6D && UART2_RX[UART2_len - 5] == 0X6D)
    {
      str_distance = "";
      for (int i = 0 ; i < UART2_len - 5 ; i++)
      {
         str_distance += (char)UART2_RX[i];
      }

      LaserDistance = str_distance.toInt();
     
    }
    
    UART2_len = 0;
    for (int i = 0 ; i < UART2_RX_SIZE ; i++)
    {
      UART2_RX[i] = 0;
    }
  }
}
