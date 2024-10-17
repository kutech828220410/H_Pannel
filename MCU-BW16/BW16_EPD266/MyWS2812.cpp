#include "MyWS2812.h"
#include <SPI.h>

void MyWS2812::Init(int NumOfLed)
{
    pinMode(this -> PIN_CS, OUTPUT);
    numOfLed = NumOfLed;
    rgbBuffer = (byte*) malloc(500 * 3);
    for(int i = 0 ; i < 1500 ;i++)
    {
       *(rgbBuffer + i) = 0;
    }
}
void MyWS2812::SetRGB(int lednum ,byte R, byte G, byte B)
{ 
    *(rgbBuffer + lednum * 3 + 0) = R;
    *(rgbBuffer + lednum * 3 + 1) = G;
    *(rgbBuffer + lednum * 3 + 2) = B;
}
bool MyWS2812::IsON(int lednum)
{ 
    for(int i = 0 ; i < lednum ;i++)
    {
        if(*(rgbBuffer + (i + 0)* 3 + 0) > 0) return true;
        if(*(rgbBuffer + (i + 0)* 3 + 1) > 0) return true;
        if(*(rgbBuffer + (i + 0)* 3 + 2) > 0) return true;
    }
    return flag_IS_ON;     
}

byte* MyWS2812::GetRGB()
{ 
    return rgbBuffer;
}
void MyWS2812::Show()
{    
    
    digitalWrite(this -> PIN_CS , HIGH);
    delay(10);
    SPI.begin(); 
    SPI.beginTransaction(SPISettings(10000000, MSBFIRST, SPI_MODE0));
    for(int i = 0 ; i < offset * 24; i++)
    {
      *(rgbBytesBuffer + i) = 0;
    }
    for(int i = 0 ; i < numOfLed ;i++)
    {
        RGBConvert2812Bytes(i + offset,*(rgbBuffer + i * 3 + 0), *(rgbBuffer + i * 3 + 1), *(rgbBuffer + i * 3 + 2));  
    }      
    SPI.transfer(rgbBytesBuffer , numOfLed * 24 + offset * 24);
    SPI.endTransaction();
    delay(100);
    digitalWrite(this -> PIN_CS , LOW);
}
void MyWS2812::Show(byte bytes[] , int _numOfLed)
{    
    
    digitalWrite(this -> PIN_CS , HIGH);
    delay(10);
    flag_IS_ON = false;
    for(int i = 0 ; i < offset * 24; i++)
    {
      *(rgbBytesBuffer + i) = 0;
    }
    for(int i = 0 ; i < _numOfLed ;i++)
    {
        RGBConvert2812Bytes(i + offset, bytes[i * 3 + 0], bytes[i * 3 + 1], bytes[i * 3 + 2]);  

        if((bytes[i * 3 + 0]) > 0) flag_IS_ON = true;
        if((bytes[i * 3 + 1]) > 0) flag_IS_ON = true;
        if((bytes[i * 3 + 2]) > 0) flag_IS_ON = true;
    }    
    
    SPI.begin(); 
    SPI.beginTransaction(SPISettings(10000000, MSBFIRST, SPI_MODE0));
     
    SPI.transfer(rgbBytesBuffer , _numOfLed * 24 + offset * 24);
    SPI.endTransaction();
    delay(100);
    digitalWrite(this -> PIN_CS , LOW);
}



void MyWS2812::RGBConvert2812Bytes(int lednum ,byte R, byte G, byte B)
{
    
    byte RGB_bytes[24];
    RGBConvert2812Bytes(R, G, B, RGB_bytes);
 
    for(int i = 0 ; i < 24 ; i++)
    {
      *(rgbBytesBuffer + lednum * 24 + i) = *(RGB_bytes + i);
    }
}
void MyWS2812::RGBConvert2812Bytes(byte R, byte G, byte B, byte* bytes)
{
    unsigned char bitnum = 24;
    unsigned long value = 0x00000000;
    R = R * (brightness / 100) ;
    G = G * (brightness / 100) ;
    B = B * (brightness / 100) ;
    value = (((unsigned long)G << 16) | ((unsigned long)R << 8) | ((unsigned long)B));
  
    for (int i = 0; i < bitnum; i++)
    {
      if ((value & 0x800000) != LOW)
      {
         *(bytes + i) = One;
      }
      else                   
      {
         *(bytes + i) = Zero;
      }
      value <<= 1;
    }
}
