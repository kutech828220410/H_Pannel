#ifndef __EPD_h
#define __EPD_h

#include "Arduino.h"
#include "Timer.h"
#define EPD_WIDTH 81
#define EPD_HEIGHT 480

class EPD
{
  public:
  byte* framebuffer;
  bool SetToSleep = false;
  int PIN_SCK = 13;
  int PIN_MOSI = 14;
  int PIN_CS = 15;
  int PIN_RST = 26;
  int PIN_DC = 27;
  int PIN_BUSY = 25;
  
  void Init();
  void Clear();
  void DrawFrame_BW();
  void DrawFrame_RW();
  void RefreshCanvas();
  void BW_Command();
  void RW_Command();
  bool GetReady();
  void Sleep_Check();
  void Sleep();
  void Wakeup();
  private:
  MyTimer MyTimer_SleepWaitTime;
  void SpiTransfer(unsigned char value);
  void SendCommand(unsigned char command);
  void SendData(unsigned char data);
  void HardwareReset();
  void WaitUntilIdle();
};

#endif
