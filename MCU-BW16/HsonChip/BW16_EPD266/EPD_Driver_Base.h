#pragma once
#include <SoftwareSerial.h>
#include <SPI.h>
#include "Arduino.h"
#include "Timer.h"
#include "Config.h"


#define BLACK   0x0
#define WHITE   0x1
#define YELLOW  0x1
#define RED     0x3

class EPD_Driver_Base {
public:
    virtual ~EPD_Driver_Base();
    SoftwareSerial *mySerial;
    SemaphoreHandle_t xSpiMutex = NULL; // 互斥鎖指針  
    MyTimer MyTimer_SleepWaitTime;
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
    int sleep_check_time = 90000;
    void Init(SemaphoreHandle_t mutex);
    virtual void melloc_init();
    virtual void Clear();
    virtual void DrawFrame_BW();
    virtual void DrawFrame_RW();
    virtual void RefreshCanvas();
    virtual void Sleep() = 0;
    virtual void BW_Command();
    virtual void RW_Command();
    bool GetReady();
    void Sleep_Check();
    virtual void Wakeup();     
    void SendSPI(char* framebuffer , int size, int offset);
    virtual void SetCursor(int Xstart, int Ystart);
    virtual void SetWindows(int Xstart, int Ystart, int Xend, int Yend);

    void SpiDelay(unsigned char xrate);
    void SpiTransfer(unsigned char value);
    void SendCommand(unsigned char command);
    void SendData(unsigned char data);
    void HardwareReset();
    virtual void WaitUntilIdle();
    virtual void SPI_Begin();
    void SPI_End();
    unsigned char Color_get(unsigned char color);
private:
    
    
};
