#ifndef __EPD_h
#define __EPD_h

#include "Arduino.h"
#include "Timer.h"
#define EPD_WIDTH 400
#define EPD_HEIGHT 480
#define UWORD   unsigned int
#define UBYTE   unsigned char
#define UDOUBLE  unsigned long
/**********************************
Color Index
**********************************/
//#define EPD_7IN3F_BLACK   0x0  /// 000
//#define EPD_7IN3F_WHITE   0x1 /// 001
//#define EPD_7IN3F_GREEN   0x2 /// 010
//#define EPD_7IN3F_BLUE    0x3 /// 011
//#define EPD_7IN3F_RED     0x4 /// 100
//#define EPD_7IN3F_YELLOW  0x5 /// 101
//#define EPD_7IN3F_ORANGE  0x6 /// 110
//#define EPD_7IN3F_CLEAN   0x7 /// 111   unavailable  Afterimage

#define EPD_7IN3E_BLACK   0x0   /// 000
#define EPD_7IN3E_WHITE   0x1   /// 001
#define EPD_7IN3E_YELLOW  0x2   /// 010
#define EPD_7IN3E_RED     0x3   /// 011
#define EPD_7IN3E_BLUE    0x5   /// 101
#define EPD_7IN3E_GREEN   0x6   /// 110


#include <SoftwareSerial.h>

class EPD
{
  public:
  SoftwareSerial *mySerial;
  byte* framebuffer;
  bool SetToSleep = false;
  int PIN_SCK = PA14;
  int PIN_MOSI = PA12;
  int PIN_MISO = PA13;
  int PIN_CS = PA15;
  int PIN_RST = PA25;
  int PIN_DC = PA26;
  int PIN_BUSY = PA27;
  
  void Init();
  void EPD_7IN3F_Show7Block();
  void EPD_7IN3F_Display(const UBYTE *image);
  void EPD_7IN3F_Display_part(const UBYTE *image, UWORD xstart, UWORD ystart, 
                                 UWORD image_width, UWORD image_heigh);
  void Clear(UBYTE color);
  void SetCursor(int Xstart, int Ystart);
  void SetWindows(int Xstart, int Ystart, int Xend, int Yend);
  void SPI_Begin();
  void SPI_End();
  void DrawFrame_BW();
  void DrawFrame_RW();
  void RefreshCanvas();
  void BW_Command();
  void RW_Command();
  void SendSPI(char* framebuffer , int size, int offset);
  bool GetReady();
  void Sleep_Check();
  void Sleep();
  void Wakeup();
  private: 
  MyTimer MyTimer_SleepWaitTime;
  void SpiDelay(unsigned char xrate);
  void SpiTransfer(unsigned char value);
  void SendCommand(unsigned char command);
  void SendData(unsigned char data);
  void SendCommandSPI(unsigned char command);
  void SendDataSPI(unsigned char data);
  void HardwareReset();
  void WaitUntilIdle();
};

#endif
