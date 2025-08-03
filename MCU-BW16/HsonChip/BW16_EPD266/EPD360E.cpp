#include "Arduino.h"
#include <SPI.h>
#include "EPD360E.h"

void EPD360E::melloc_init() 
{
    mySerial->print("epd malloc : ");
    mySerial->print(60000);
    mySerial->println(" bytes");

    framebuffer = (byte*) malloc(60000);
    buffer_max = 60000;
}
void EPD360E::Show7Block(void)
{
    SPI_Begin();
    unsigned long j, k;
    unsigned char const Color_seven[6] = 
    {EPD_3IN6E_BLACK, EPD_3IN6E_YELLOW, EPD_3IN6E_RED, EPD_3IN6E_BLUE, EPD_3IN6E_GREEN, EPD_3IN6E_WHITE};

    SendCommand(0x10);
    for(k = 0 ; k < 6; k ++) {
        for(j = 0 ; j < 20000; j ++) {
            SendData((Color_seven[k]<<4) |Color_seven[k]);
        }
    }
    RefreshCanvas();
    SPI_End();
}
void EPD360E::Clear() 
{
    SPI_Begin();
    SendCommand(0x10);
   
    for(int i = 0; i < 60000 * 2; i++) 
    {
        SendData((EPD_3IN6E_GREEN<<4)|EPD_3IN6E_GREEN);           
    }
    RefreshCanvas();
    SPI_End();
}

void EPD360E::DrawFrame_BW() 
{
    SPI_Begin();
    SendCommand(0x10);
    SPI_End();
}

void EPD360E::DrawFrame_RW() 
{
    SPI_Begin();
    for (int i = 0; i < 60000; i++) 
    {
      SendData(*(framebuffer + i));
    }
    SPI_End();
}

void EPD360E::RefreshCanvas() 
{
    SPI_Begin();
    SendCommand(0x02); // POWER_OFF
    SendData(0X00);
    WaitUntilIdle(); 

    SendCommand(0x04); // POWER_ON
    WaitUntilIdle();
    delay(200);

    //Second setting 
    SendCommand(0x06);
    SendData(0x6F);
    SendData(0x1F);
    SendData(0x17);
    SendData(0x27);
    delay(200);

    SendCommand(0x12); // DISPLAY_REFRESH
    SendData(0x00);
    WaitUntilIdle();

    SendCommand(0x02); // POWER_OFF
    SendData(0X00);
    WaitUntilIdle();
    
}

void EPD360E::Sleep() 
{
    SPI_Begin();
    SendCommand(0x07); // DEEP_SLEEP
    SendData(0XA5);
    SPI_End();
}

void EPD360E::BW_Command() 
{
    
}

void EPD360E::RW_Command() 
{
   
}

void EPD360E::Wakeup() 
{
    mySerial -> println("EPD3IN6E Init...");
    this -> MyTimer_SleepWaitTime.TickStop();  
    this -> MyTimer_SleepWaitTime.StartTickTime(90000);
//    mySerial -> println("Wake up!");
    this -> SetToSleep = false;    
    HardwareReset();
    delay(20);
    WaitUntilIdle();
    SPI_Begin();
    SendCommand(0xAA);    // CMDH
    SendData(0x49);
    SendData(0x55);
    SendData(0x20);
    SendData(0x08);
    SendData(0x09);
    SendData(0x18);

    SendCommand(0x01);
    SendData(0x3F);

    SendCommand(0x00);
    SendData(0x5F);
    SendData(0x69);

    SendCommand(0x05);
    SendData(0x40);
    SendData(0x1F);
    SendData(0x1F);
    SendData(0x2C);

    SendCommand(0x08);
    SendData(0x6F);
    SendData(0x1F);
    SendData(0x1F);
    SendData(0x22);

    SendCommand(0x06);
    SendData(0x6F);
    SendData(0x1F);
    SendData(0x17);
    SendData(0x17);

    SendCommand(0x03);
    SendData(0x00);
    SendData(0x54);
    SendData(0x00);
    SendData(0x44); 

    SendCommand(0x60);
    SendData(0x02);
    SendData(0x00);

    SendCommand(0x30);
    SendData(0x08);

    SendCommand(0x50);
    SendData(0x3F);

    SendCommand(0x61);
    SendData(0x01);
    SendData(0x90);
    SendData(0x02); 
    SendData(0x58);

    SendCommand(0xE3);
    SendData(0x2F);

    SendCommand(0x84);
    SendData(0x01);
    WaitUntilIdle();
    SPI_End();
    mySerial -> println("EPD3IN6E done...");
}


void EPD360E::WaitUntilIdle() 
{
    if(!digitalRead(this -> PIN_BUSY))
    {
       delay(10);
       mySerial -> println("WaitUntilIdle....");
    }
}

void EPD360E::SPI_Begin() 
{
    SPI.beginTransaction(SPISettings(10000000, MSBFIRST, SPI_MODE0));
}

void EPD360E::SetCursor(int Xstart, int Ystart)
{
  
}
void EPD360E::SetWindows(int Xstart, int Ystart, int Xend, int Yend)
{
  
}
