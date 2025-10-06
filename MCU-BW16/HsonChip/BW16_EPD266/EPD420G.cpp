#include "Arduino.h"
#include <SPI.h>
#include "EPD420G.h"

void EPD420G::melloc_init() 
{
    mySerial->print("epd malloc : ");
    mySerial->print(30000);
    mySerial->println(" bytes");

    framebuffer = (byte*) malloc(30000);
    buffer_max = 30000;
}

void EPD420G::Clear() 
{
    SPI_Begin();
    SendCommand(0x10);

    int Width, Height;
    Width = 100;
    Height = 300;

    SendCommand(0x10);
    for (int j = 0; j < Height / 2; j++) 
    {
        for (int i = 0; i < Width; i++) 
        {
            SendData((EPD_420G_YELLOW << 6) | (EPD_420G_YELLOW << 4) | (EPD_420G_YELLOW << 2) | EPD_420G_YELLOW);
        }
    }    
    for (int j = 0; j < Height / 2; j++) 
    {
        for (int i = 0; i < Width; i++) 
        {
            SendData((EPD_420G_RED << 6) | (EPD_420G_RED << 4) | (EPD_420G_RED << 2) | EPD_420G_RED);
        }
    } 
    
    RefreshCanvas();
    SPI_End();
}

void EPD420G::DrawFrame_BW() 
{
    SPI_Begin();
    SendCommand(0x10);
    for (int i = 0; i < 30000; i++) 
    {
      SendData(*(framebuffer + i));
    }
    SPI_End();
}

void EPD420G::DrawFrame_RW() 
{
    SPI_Begin();
    SendCommand(0x10);
    for (int i = 0; i < 30000; i++) 
    {
      SendData(*(framebuffer + i));
    }
    SPI_End();
}

void EPD420G::RefreshCanvas() 
{
    SPI_Begin();
    SendCommand(0x12); // DISPLAY_REFRESH
    SendData(0x00);
    WaitUntilIdle();

    
}

void EPD420G::Sleep() 
{
    SPI_Begin();
    SendCommand(0x07); // DEEP_SLEEP
    SendData(0XA5);
    SPI_End();
}

void EPD420G::BW_Command() 
{
    
}

void EPD420G::RW_Command() 
{
   
}

void EPD420G::Wakeup() 
{
    mySerial -> println("EPD420G Init...");
    this -> MyTimer_SleepWaitTime.TickStop();  
    this -> MyTimer_SleepWaitTime.StartTickTime(90000);
    this -> SetToSleep = false;    
    HardwareReset();
    delay(100);
    SPI_Begin();
    SendCommand(0x4D);
    SendData(0x78);

    SendCommand(0x00);  
    SendData(0x0F); 
    SendData(0x29);

    SendCommand(0x06);
    SendData(0x0d); 
    SendData(0x12); 
    SendData(0x24);     
    SendData(0x25);     
    SendData(0x12);       
    SendData(0x29);     
    SendData(0x10);

    SendCommand(0x30);
    SendData(0x08); 

    SendCommand(0x50);
    SendData(0x37); 

    SendCommand(0x61); //0x61
    SendData(400/256);    
    SendData(400%256);    
    SendData(300/256);   
    SendData(300%256);   

    SendCommand(0xae); 
    SendData(0xcf);

    SendCommand(0xb0); 
    SendData(0x13);

    SendCommand(0xbd); 
    SendData(0x07);

    SendCommand(0xbe); 
    SendData(0xfe);

    SendCommand(0xE9); 
    SendData(0x01);

    SendCommand(0x04);
    WaitUntilIdle();
    SPI_End();
    mySerial -> println("EPD420G done...");
}


void EPD420G::WaitUntilIdle() 
{
    while(!digitalRead(this -> PIN_BUSY))
    {
       delay(10);
       mySerial -> println("WaitUntilIdle....");
    }
}

void EPD420G::SPI_Begin() 
{
    SPI.beginTransaction(SPISettings(10000000, MSBFIRST, SPI_MODE0));
}

void EPD420G::SetCursor(int Xstart, int Ystart)
{
  
}
void EPD420G::SetWindows(int Xstart, int Ystart, int Xend, int Yend)
{
  
}
