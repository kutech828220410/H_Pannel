#ifndef __MyWS2812_h
#define __MyWS2812_h

#include "Arduino.h"

#define Zero 0b11100000
#define One 0b11111000

#define WS2812_BITNUM 24
class MyWS2812
{
   public:   
    int PIN_CS = PB3;
    void Init(int NumOfLed , SemaphoreHandle_t mutex);   
    void SetRGB(int lednum ,byte R, byte G, byte B);
    byte* rgbBuffer;
    byte* GetRGB();
    void Show(byte bytes[] , int numOfLed);
    void Show();
    void ClearBytes();
    int numOfLed = 0;
    double brightness = 100;
    bool IsON(int lednum);
   private:  
    
    int offset = 5;
    bool flag_IS_ON = false;
    byte rgbBytesBuffer[500 *24];
    byte rgb_light[500];
    byte rgb_light_curent[500];
    void RGBConvert2812Bytes(int lednum ,byte R, byte G, byte B);
    void RGBConvert2812Bytes(byte R, byte G, byte B, byte* bytes);
    SemaphoreHandle_t xSpiMutex = NULL; // 互斥鎖指針

    
};



#endif
