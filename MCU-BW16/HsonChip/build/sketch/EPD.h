#line 1 "C:\\Users\\Evan\\source\\repos\\H_Pannel\\MCU-BW16\\BW16_EPD266\\EPD.h"
#ifndef __EPD_h
#define __EPD_h

#include "Arduino.h"
#include "Timer.h"
#include "Config.h"



#define  BLACK   0x0
#define  WHITE   0x1
#define  YELLOW   0x1
#define  RED     0x3

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
  long buffer_max = 0;
  void Init(SemaphoreHandle_t mutex);
  void Clear();
  void SetCursor(int Xstart, int Ystart);
  void SetWindows(int Xstart, int Ystart, int Xend, int Yend);
  void SPI_Begin();
  void SPI_End();
  void SendSPI(char* framebuffer , int size, int offset);
  void DrawFrame_BW();
  void DrawFrame_RW();
  void RefreshCanvas();
  void BW_Command();
  void RW_Command();
  bool GetReady();
  void Sleep_Check();
  void Sleep();
  void Wakeup();
  unsigned char Color_get(unsigned char color);
  
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

  void EPD420_init();
  void EPD583_init();
  void EPD7IN3E_init();
  void DEPG0579RYT158FxX_init();
  void EPD579B_init();
  void EPD579G_init();
  void EPD213_BRW_V0_init();
  
  SemaphoreHandle_t xSpiMutex = NULL; // 互斥鎖指針
};

#endif
