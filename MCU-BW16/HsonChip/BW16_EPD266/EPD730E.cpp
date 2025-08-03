#include "Arduino.h"
#include <SPI.h>
#include "EPD730E.h"

void EPD730E::melloc_init() 
{
    mySerial->print("epd malloc : ");
    mySerial->print(96000);
    mySerial->println(" bytes");

    framebuffer = (byte*) malloc(96000);
    buffer_max = 96000;
}

void EPD730E::Clear() 
{
    SPI_Begin();
    SendCommand(0x10);
   
    for(int i = 0; i < 192000; i++) 
    {
        SendData((EPD_7IN3E_GREEN<<4)|EPD_7IN3E_GREEN);           
    }
    RefreshCanvas();
    SPI_End();
}

void EPD730E::DrawFrame_BW() 
{
    SPI_Begin();
    SendCommand(0x10);
    SPI_End();
}

void EPD730E::DrawFrame_RW() 
{
    SPI_Begin();
    for (int i = 0; i < 96000; i++) 
    {
      SendData(*(framebuffer + i));
    }
    SPI_End();
}

void EPD730E::RefreshCanvas() 
{
    SPI_Begin();
    SendCommand(0x04); // POWER_ON
    WaitUntilIdle();
    
    //Second setting 
    SendCommand(0x06);
    SendData(0x6F);
    SendData(0x1F);
    SendData(0x17);
    SendData(0x49);
    
    SendCommand(0x12); // DISPLAY_REFRESH
    SendData(0x00);
    WaitUntilIdle();
    
    SendCommand(0x02); // POWER_OFF
    SendData(0X00);
    WaitUntilIdle();  
}

void EPD730E::Sleep() 
{
    SPI_Begin();
    SendCommand(0X02); // DEEP_SLEEP
    SendData(0x00);
    WaitUntilIdle();
    
    SendCommand(0x07); // DEEP_SLEEP
    SendData(0XA5);
    SPI_End();
}

void EPD730E::BW_Command() 
{
    
}

void EPD730E::RW_Command() 
{
   
}

void EPD730E::Wakeup() 
{
    mySerial -> println("EPD7IN3E Init...");
    this -> MyTimer_SleepWaitTime.TickStop();  
    this -> MyTimer_SleepWaitTime.StartTickTime(90000);
//    mySerial -> println("Wake up!");
    this -> SetToSleep = false;
    this -> HardwareReset();
    
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
    
    SendCommand(0x01);//
    SendData(0x3F);
    
    SendCommand(0x00);  
    SendData(0x5F);
    SendData(0x69);
    
    SendCommand(0x03);
    SendData(0x00);
    SendData(0x54);
    SendData(0x00);
    SendData(0x44); 
    
    SendCommand(0x05);
    SendData(0x40);
    SendData(0x1F);
    SendData(0x1F);
    SendData(0x2C);
    
    SendCommand(0x06);
    SendData(0x6F);
    SendData(0x1F);
    SendData(0x17);
    SendData(0x49);
    
    SendCommand(0x08);
    SendData(0x6F);
    SendData(0x1F);
    SendData(0x1F);
    SendData(0x22);
    
    SendCommand(0x30);
    SendData(0x03);
    
    SendCommand(0x50);
    SendData(0x3F);
    
    SendCommand(0x60);
    SendData(0x02);
    SendData(0x00);
    
    SendCommand(0x61);
    SendData(0x03);
    SendData(0x20);
    SendData(0x01); 
    SendData(0xE0);
    
    SendCommand(0x84);
    SendData(0x01);
    
    SendCommand(0xE3);
    SendData(0x2F);
    
    SendCommand(0x04);     //PWR on  
    SPI_End();
    mySerial -> println("EPD7IN3E done...");
}


void EPD730E::WaitUntilIdle() 
{
    if(!digitalRead(this -> PIN_BUSY))
    {
       delay(10);
       mySerial -> println("WaitUntilIdle....");
    }
}

void EPD730E::SPI_Begin() 
{
    SPI.beginTransaction(SPISettings(10000000, MSBFIRST, SPI_MODE0));
}

void EPD730E::SetCursor(int Xstart, int Ystart)
{
  
}
void EPD730E::SetWindows(int Xstart, int Ystart, int Xend, int Yend)
{
  
}
