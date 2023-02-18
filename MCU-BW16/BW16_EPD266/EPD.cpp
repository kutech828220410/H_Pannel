#include "EPD.h"
#include "Arduino.h"
#include <SPI.h>
void EPD::Init()
{
    this -> framebuffer = (byte*) malloc(EPD_WIDTH * EPD_HEIGHT);
    pinMode(this -> PIN_CS, OUTPUT);
    pinMode(this -> PIN_RST, OUTPUT);
    pinMode(this -> PIN_DC, OUTPUT);
    pinMode(this -> PIN_BUSY, INPUT); 
    
    digitalWrite(this -> PIN_CS , HIGH);
   
    delay(20);

    this -> Wakeup();
    
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
    SPI_Begin();
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
    SPI_End();
    this -> RefreshCanvas(); 
    WaitUntilIdle();

}
void EPD::DrawFrame_BW()
{
    //send black data
    SPI_Begin();
    this -> BW_Command();

    for (int j = 0; j < EPD_HEIGHT; j++)
    {
        for (int i = 0; i < EPD_WIDTH; i++) 
        {
            SendData(*(framebuffer + j * EPD_WIDTH + i));
        }
    }    
    SPI_End();
}
void EPD::DrawFrame_RW()
{
    //send red data
    SPI_Begin();
    this -> RW_Command();

    for (int j = 0; j < EPD_HEIGHT; j++) 
    {
        for (int i = 0; i < EPD_WIDTH; i++) 
        {
          SendData(*(framebuffer + j * EPD_WIDTH + i));
        }
    }
    SPI_End();
}
void EPD::RefreshCanvas()
{
   SPI_Begin();
   SendCommand(0x20);
   SPI_End();
   this -> SetToSleep = true;
}
void EPD::Sleep_Check()
{
   if(this -> SetToSleep)
   {     
       this -> MyTimer_SleepWaitTime.StartTickTime(10000);
       if(this -> MyTimer_SleepWaitTime.IsTimeOut())
       {
         if(this -> SetToSleep)
         {
             this -> Sleep();
             this -> SetToSleep = false;
         }
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
void EPD::SPI_Begin()
{
   SPI.beginTransaction(SPISettings(2000000, MSBFIRST, SPI_MODE0));
}
void EPD::SPI_End()
{
   SPI.endTransaction();
}
void EPD::SpiTransfer(unsigned char value)
{
   digitalWrite(this -> PIN_CS , LOW);
   SPI.transfer(value);
   digitalWrite(this -> PIN_CS , HIGH);

}
void EPD::HardwareReset()
{
   digitalWrite(this -> PIN_RST, LOW);                //module reset    
   delay(1);
   digitalWrite(this -> PIN_RST, HIGH);
   delay(10);   
}
void EPD::Wakeup()
{
    this -> MyTimer_SleepWaitTime.TickStop();  
    this -> MyTimer_SleepWaitTime.StartTickTime(10000);
//    mySerial -> println("Wake up!");
    this -> SetToSleep = false;
    this -> HardwareReset();
    SPI_Begin();
    WaitUntilIdle();
    SendCommand(0x12);//soft  reset
    WaitUntilIdle();


    SPI_Begin();
    SendCommand(0x11); //data entry mode       
    SendData(0x03);
  
    SetWindows(0, 0, EPD_WIDTH * 8 -1, EPD_HEIGHT-1);
    
    SendCommand(0x21); //  Display update control
    SendData(0x00);
    SendData(0x80);  
  
    SetCursor(0, 0);
    SPI_End();
}
void EPD::WaitUntilIdle()
{
   //delay(50);
    while(digitalRead(PIN_BUSY) == HIGH) {      //LOW: idle, HIGH: busy
//      mySerial -> println("Watting for PIN_BUSY!");
        //delay(10);
    }
    //delay(50);
}
void EPD::Sleep()
{  
    SendCommand(0x10);
    SendData(0x01);
}
