#ifndef __EPD_h
#define __EPD_h

#include "Arduino.h"
#include "Timer.h"
#define EPD_WIDTH 100
#define EPD_HEIGHT 300

#define PANEL_SETTING                               0x00
#define POWER_SETTING                               0x01
#define POWER_OFF                                   0x02
#define POWER_OFF_SEQUENCE_SETTING                  0x03
#define POWER_ON                                    0x04
#define POWER_ON_MEASURE                            0x05
#define BOOSTER_SOFT_START                          0x06
#define DEEP_SLEEP                                  0x07
#define DATA_START_TRANSMISSION_1                   0x10
#define DATA_STOP                                   0x11
#define DISPLAY_REFRESH                             0x12
#define DATA_START_TRANSMISSION_2                   0x13
#define LUT_FOR_VCOM                                0x20 
#define LUT_WHITE_TO_WHITE                          0x21
#define LUT_BLACK_TO_WHITE                          0x22
#define LUT_WHITE_TO_BLACK                          0x23
#define LUT_BLACK_TO_BLACK                          0x24
#define PLL_CONTROL                                 0x30
#define TEMPERATURE_SENSOR_COMMAND                  0x40
#define TEMPERATURE_SENSOR_SELECTION                0x41
#define TEMPERATURE_SENSOR_WRITE                    0x42
#define TEMPERATURE_SENSOR_READ                     0x43
#define VCOM_AND_DATA_INTERVAL_SETTING              0x50
#define LOW_POWER_DETECTION                         0x51
#define TCON_SETTING                                0x60
#define RESOLUTION_SETTING                          0x61
#define GSST_SETTING                                0x65
#define GET_STATUS                                  0x71
#define AUTO_MEASUREMENT_VCOM                       0x80
#define READ_VCOM_VALUE                             0x81
#define VCM_DC_SETTING                              0x82
#define PARTIAL_WINDOW                              0x90
#define PARTIAL_IN                                  0x91
#define PARTIAL_OUT                                 0x92
#define PROGRAM_MODE                                0xA0
#define ACTIVE_PROGRAMMING                          0xA1
#define READ_OTP                                    0xA2
#define POWER_SAVING                                0xE3

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
