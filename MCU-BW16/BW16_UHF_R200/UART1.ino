#include "Timer.h"
#define UART1_RX_SIZE 256
static byte UART1_RX[UART0_RX_SIZE];
int UART1_len = 0;
MyTimer MyTimer_UART1;
void serialEvent1()
{
  if (mySerial_R200.available())
  {
    UART1_RX[UART1_len] = mySerial_R200.read();
    String HEX_0 =  String(UART1_RX[UART1_len], HEX); 
    mySerial.println(HEX_0);
    UART1_len++;
    MyTimer_UART1.TickStop();
    MyTimer_UART1.StartTickTime(2);
  }
  if (MyTimer_UART1.IsTimeOut())
  {
    MyTimer_UART1.TickStop();
    MyTimer_UART1.StartTickTime(1000);
    
  }

  UART1_len = 0;
  for (int i = 0 ; i < UART1_RX_SIZE ; i++)
  {
    UART1_RX[i] = 0;
  }
}
