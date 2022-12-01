#include "EPD.h"
#include "Arduino.h"
#include <SPI.h>

void EPD::Init()
{
    this -> framebuffer = (byte*) malloc(EPD_WIDTH * EPD_HEIGHT);
    //this -> framebuffer = (byte *)ps_calloc(sizeof(byte), EPD_WIDTH * EPD_HEIGHT);
    pinMode(this -> PIN_CS, OUTPUT);
    pinMode(this -> PIN_RST, OUTPUT);
    pinMode(this -> PIN_DC, OUTPUT);
    pinMode(this -> PIN_BUSY, INPUT); 
    pinMode(this -> PIN_MOSI, OUTPUT);
    pinMode(this -> PIN_SCK , OUTPUT);
    
    digitalWrite(this -> PIN_CS , HIGH);
    digitalWrite(this -> PIN_SCK, LOW);
    
    delay(20);
    //SPI.begin();
    //SPI.beginTransaction(SPISettings(2000000, MSBFIRST, SPI_MODE0));
    //this -> Wakeup();
    
}
void EPD::SetCursor(int Xstart, int Ystart)
{
    SendCommand(0x4E); // SET_RAM_X_ADDRESS_COUNTER
    SendData(Xstart & 0x1F);

    SendCommand(0x4F); // SET_RAM_Y_ADDRESS_COUNTER
    SendData(Ystart & 0xFF);
    SendData((Ystart >> 8) & 0x01);  
}
void EPD::SetWindows(int Xstart, int Ystart, int Xend, int Yend)
{
    SendCommand(0x44); // SET_RAM_X_ADDRESS_START_END_POSITION
    SendData((Xstart>>3) & 0x1F);
    SendData((Xend>>3) & 0x1F);
  
    SendCommand(0x45); // SET_RAM_Y_ADDRESS_START_END_POSITION
    SendData(Ystart & 0xFF);
    SendData((Ystart >> 8) & 0x01);
    SendData(Yend & 0xFF);
    SendData((Yend >> 8) & 0x01);  
}
bool EPD::GetReady()
{
   return (digitalRead(this -> PIN_BUSY) == 1);
}
void EPD::Clear()
{
    //send black data
    this -> BW_Command();
    for (int j = 0; j < EPD_HEIGHT; j++)
    {
        for (int i = 0; i < EPD_WIDTH; i++) {
            SendData(0x00);
        }
    }
    
    //send red data
    this -> RW_Command();
    for (int j = 0; j < EPD_HEIGHT; j++) 
    {
        for (int i = 0; i < EPD_WIDTH; i++) {
            SendData(0xFF);
        }
    }
    this -> RefreshCanvas(); 
    WaitUntilIdle();

}
void EPD::DrawFrame_BW()
{
    //send black data
    this -> BW_Command();
    for (int j = 0; j < EPD_HEIGHT; j++)
    {
        for (int i = 0; i < EPD_WIDTH; i++) 
        {
            SendData(*(framebuffer + j * EPD_WIDTH + i));
        }
    }    
}
void EPD::DrawFrame_RW()
{
    //send red data
    this -> RW_Command();
    for (int j = 0; j < EPD_HEIGHT; j++) 
    {
        for (int i = 0; i < EPD_WIDTH; i++) 
        {
          SendData(*(framebuffer + j * EPD_WIDTH + i));
        }
    }
}
void EPD::RefreshCanvas()
{
   SendCommand(0x20);
   this -> SetToSleep = true;
}
void EPD::Sleep_Check()
{
   if(this -> SetToSleep)
   {     
       this -> MyTimer_SleepWaitTime.StartTickTime(1000);
       if(this -> MyTimer_SleepWaitTime.IsTimeOut())
       {
         this -> Sleep();
         this -> MyTimer_SleepWaitTime.TickStop();       
       }     
   }   
}
void EPD::BW_Command()
{
   SendCommand(0x24);
} 
void EPD::RW_Command()
{
   SendCommand(0x26);
} 

void EPD::SendCommand(unsigned char command)
{
   digitalWrite(this -> PIN_DC, LOW);
   SpiTransfer(command);
}
void EPD::SendData(unsigned char data)
{
   digitalWrite(this -> PIN_DC, HIGH);
   SpiTransfer(data);
}
void EPD::SpiTransfer(unsigned char value)
{
   digitalWrite(this -> PIN_CS , LOW);
//SPI.beginTransaction(spi_settings);
   for (int i = 0; i < 8; i++)
   {
      if ((value & 0x80) == 0) digitalWrite(this -> PIN_MOSI, LOW); 
      else                    digitalWrite(this -> PIN_MOSI, HIGH);

      value <<= 1;
      digitalWrite(this -> PIN_SCK, HIGH);     
      digitalWrite(this -> PIN_SCK, LOW);
   }
   //SPI.transfer(value);
   digitalWrite(this -> PIN_CS , HIGH);
}
void EPD::HardwareReset()
{
   digitalWrite(PIN_RST, HIGH);
   delay(200);
   digitalWrite(PIN_RST, LOW);
   delay(2);
   digitalWrite(PIN_RST, HIGH);
   delay(200);  
}
void EPD::Wakeup()
{
    this -> SetToSleep = false;
    this -> HardwareReset();
    WaitUntilIdle();
    SendCommand(0x12);//soft  reset
    WaitUntilIdle();

    SendCommand(0x11); //data entry mode       
    SendData(0x03);
  
    SetWindows(0, 0, EPD_WIDTH * 8 -1, EPD_HEIGHT-1);
    
    SendCommand(0x21); //  Display update control
    SendData(0x00);
    SendData(0x80);  
  
    SetCursor(0, 0);
    WaitUntilIdle();
}
void EPD::WaitUntilIdle()
{
    //delay(50);
    while(digitalRead(PIN_BUSY) == HIGH) {      //LOW: idle, HIGH: busy
        //delay(10);
    }
    //delay(50);
}
void EPD::Sleep()
{
    SendCommand(0x10);
    SendData(0x01); 
}
