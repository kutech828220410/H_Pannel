#include "Arduino.h"
#include "My_WS2812.h"

void My_WS2812::Init()
{
  this -> RGB_bytes = (byte*) malloc(this -> RGB_NUM * 3);
  pinMode(this -> RGB_PIN,OUTPUT);
  digitalWrite(this -> RGB_PIN,LOW);
}
void My_WS2812::delay_ns(float ns)
{
  for(int j; j <( ns); j++) NOP();
}
void My_WS2812::GPIO_0()
{
  digitalWrite(this -> RGB_PIN,HIGH);
  this -> delay_ns(300);
  digitalWrite(this -> RGB_PIN,LOW);
  this -> delay_ns(600);
}
void My_WS2812::GPIO_1()
{
  digitalWrite(this -> RGB_PIN,HIGH);
  this -> delay_ns(600);
  digitalWrite(this -> RGB_PIN,LOW);
  this -> delay_ns(300);
}

void My_WS2812::Send_Data(int32_t Data)
{
  for(int i= 1; i < 25; i++)
  {
    if(getbit(Data , 24 - i))
      this -> GPIO_1();
    else
      this -> GPIO_0();
  }
}
void My_WS2812::Set_Buffer(int NumOfLED ,byte byte_R ,byte byte_G ,byte byte_B)
{
   *(RGB_bytes + NumOfLED * 3 + 0) = byte_R;
   *(RGB_bytes + NumOfLED * 3 + 1) = byte_G;
   *(RGB_bytes + NumOfLED * 3 + 2) = byte_B;
}
void My_WS2812::Send()
{
  uint8_t i,j,k,a;
  for( k = 0; k< 408; k++)
  {
    for(j = 0;j < 3; j++)
    {
      if( j == R )
      {
         a = *(RGB_bytes + k * 3 + 0);
      }
      else if( j == G )
      {
         a = *(RGB_bytes + k * 3 + 1);
      }
      else if( j == B )
      {
         a = *(RGB_bytes + k * 3 + 2);
      }
      a = 255;
      for( i = 1 ; i < 9 ; i++)
      {
        if(getbit(a ,8 - i))//发送1码
        {
          digitalWrite(this -> RGB_PIN,HIGH);
          delayMicroseconds(0.4);
          digitalWrite(this -> RGB_PIN,LOW);
          delayMicroseconds(0.2);
        }
        else//发送0码
        {
          digitalWrite(this -> RGB_PIN,HIGH);
          delayMicroseconds(0.2);
          digitalWrite(this -> RGB_PIN,LOW);
          delayMicroseconds(0.4);
        }
      }
    }
  }
  delayMicroseconds(400);//每帧数据相隔400us
}
