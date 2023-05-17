#ifndef __EPD_h
#define __EPD_h

#include "Arduino.h"
#include "Timer.h"
#define EPD_WIDTH 120
#define EPD_HEIGHT 640



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
  void Clear();
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
  void WhiteScreen_ALL(const unsigned char *BW_datas,const unsigned char *R_datas);
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
  void SetLut(const unsigned char* lut);
  void SetLut_by_host(unsigned char *lut);

};

#endif
