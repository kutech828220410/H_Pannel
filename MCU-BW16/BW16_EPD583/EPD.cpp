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
bool EPD::GetReady()
{
   return (digitalRead(this -> PIN_BUSY) == 1);
}
void EPD::Clear()
{
    //send black data
    SPI_Begin();
    SendCommand(0x10);
    for (int j = 0; j < EPD_HEIGHT; j++)
    {
        for (int i = 0; i < EPD_WIDTH; i++) {
            SendData(0xFF);
        }
    }
    
    //send red data
    SendCommand(0x13);
    for (int j = 0; j < EPD_HEIGHT; j++) 
    {
        for (int i = 0; i < EPD_WIDTH; i++) {
            SendData(0x00);
        }
    }
    SPI_End();
    this -> RefreshCanvas(); 
    WaitUntilIdle();

}
void EPD::SendSPI(char* framebuffer ,int size, int offset)
{
   SPI_Begin();
   for (int i = offset; i < size; i++) 
   {
      SendData(*(framebuffer + i));
   }
   SPI_End();
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
   SendCommand(0x12);
   SPI_End();
   this -> SetToSleep = true;
}
void EPD::Sleep_Check()
{
   if(this -> SetToSleep)
   {     
       this -> MyTimer_SleepWaitTime.StartTickTime(90000);
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
   this -> SetToSleep = false;
   this -> MyTimer_SleepWaitTime.TickStop();  
   this -> MyTimer_SleepWaitTime.StartTickTime(90000);
   SPI_Begin();
   SendCommand(0x10);
   SPI_End();
} 
void EPD::RW_Command()
{
   this -> SetToSleep = false;
   this -> MyTimer_SleepWaitTime.TickStop();  
   this -> MyTimer_SleepWaitTime.StartTickTime(90000);
   SPI_Begin();
   SendCommand(0x13);
   SPI_End();
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
   SPI.beginTransaction(SPISettings(10000000, MSBFIRST, SPI_MODE0));
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
    this -> MyTimer_SleepWaitTime.StartTickTime(90000);
//    mySerial -> println("Wake up!");
    this -> SetToSleep = false;
    this -> HardwareReset();
    SPI_Begin();
    SendCommand(0x01);      //POWER SETTING
    SendData (0x07);    //VGH=20V,VGL=-20V
    SendData (0x07);    //VGH=20V,VGL=-20V
    SendData (0x3f);    //VDH=15V
    SendData (0x3f);    //VDL=-15V
    SendCommand(0x04); //POWER ON
   
    WaitUntilIdle();
    
    SendCommand(0X00);      //PANNEL SETTING
    SendData(0x0F);   //KW-3f   KWR-2F  BWROTP 0f BWOTP 1f
  
    SendCommand(0x61);          //tres      
    SendData (0x02);    //source 648
    SendData (0x88);
    SendData (0x01);    //gate 480
    SendData (0xe0);
  
    SendCommand(0X15);    
    SendData(0x00);   
  
    SendCommand(0X50);      //VCOM AND DATA INTERVAL SETTING
    SendData(0x11);
    SendData(0x07);
  
    SendCommand(0X60);      //TCON SETTING
    SendData(0x22);
    SPI_End();
}
void EPD::WaitUntilIdle()
{
    delay(1);
    mySerial -> println("Wait EPD BUSY!");
    unsigned char busy;
    do
    {        
       SPI_Begin();
       SendCommand(0x71);
       SPI_End();
       busy = digitalRead(this -> PIN_BUSY);
    }
    while(busy);    
    delay(200); 
    mySerial -> println("Wait EPD BUSY release!");
    
}
void EPD::Sleep()
{  
    this -> HardwareReset();
//    mySerial -> println("Sleep!");
    SPI_Begin();
    SendCommand(0x02); 
    SPI_End();  
    //this -> WaitUntilIdle();
    SPI_Begin();
    SendCommand(0x07); 
    SendData(0xA5);
    SPI_End();
}
