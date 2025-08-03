#pragma once
#include <SoftwareSerial.h>
#include <SPI.h>
#include "Arduino.h"
#include "Timer.h"
#include "Config.h"
#include "EPD_Driver_Base.h"

#define EPD_7IN3E_BLACK   0x0   /// 000
#define EPD_7IN3E_WHITE   0x1   /// 001
#define EPD_7IN3E_YELLOW  0x2   /// 010
#define EPD_7IN3E_RED     0x3   /// 011
#define EPD_7IN3E_BLUE    0x5   /// 101
#define EPD_7IN3E_GREEN   0x6   /// 110

class EPD730E : public EPD_Driver_Base {
public:
    void melloc_init() override;
    void Clear() override;
    void DrawFrame_BW() override;
    void DrawFrame_RW() override;
    void RefreshCanvas() override;
    void Sleep() override;
    void BW_Command() override;
    void RW_Command() override;
    void Wakeup() override;     
    void SetCursor(int Xstart, int Ystart) override;
    void SetWindows(int Xstart, int Ystart, int Xend, int Yend) override;

private:
    void WaitUntilIdle() override;
    void SPI_Begin() override;
};
