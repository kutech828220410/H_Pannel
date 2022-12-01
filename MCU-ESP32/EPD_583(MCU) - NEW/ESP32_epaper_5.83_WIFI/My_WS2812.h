#ifndef __My_WS2812_h
#define __My_WS2812_h
#include "Arduino.h"

#define RED   0xFF0000
#define GREEN 0x00FF00
#define BLUE  0x0000FF
#define BLACK 0x000000
#define WHITE 0xFFFFFF

#define getbit(x,y)   ((x) >> (y)&1)
#define setbit(x,y)  x|=(1<<y)
#define clrbit(x,y)  x&=~(1<<y)
#define reversebit(x,y)  x^=(1<<y)
#define getbit(x,y)   ((x) >> (y)&1)

#define R 0         //红色
#define G 1         //绿色
#define B 2         //蓝色
#define BRIGHT 4    //亮度


class My_WS2812
{
   public:   
    int RGB_NUM = 64;
    uint8_t* RGB_bytes;
    int RGB_PIN = 0;   
    void Init();
    void Set_Buffer(int NumOfLED ,byte byte_R ,byte byte_G ,byte byte_B);
    void Send_Data(int32_t Data);
    void Send();
   private:  
    void delay_ns(float ns);
    void GPIO_0();
    void GPIO_1();
    
};

#endif
