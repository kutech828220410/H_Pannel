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
    this -> Wakeup();
    
}
bool EPD::GetReady()
{
   return (digitalRead(this -> PIN_BUSY) == 1);
}
void EPD::Clear()
{
    //send black data
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
   SendCommand(0x12);
   this -> SetToSleep = true;
}
void EPD::Sleep_Check()
{
   if(this -> SetToSleep)
   {     
       this -> MyTimer_SleepWaitTime.StartTickTime(40000);
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
   SendCommand(0x10);
} 
void EPD::RW_Command()
{
   SendCommand(0x13);
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
   digitalWrite(this -> PIN_RST, LOW);                //module reset    
   delay(1);
   digitalWrite(this -> PIN_RST, HIGH);
   delay(10);   
}
void EPD::Wakeup()
{
    this -> MyTimer_SleepWaitTime.TickStop();  
    this -> MyTimer_SleepWaitTime.StartTickTime(20000);
    Serial.println("Wake up!");
    this -> SetToSleep = false;
    this -> HardwareReset();
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
}
void EPD::WaitUntilIdle()
{
    delay(1);
    Serial.println("Wait EPD BUSY!");
    while(digitalRead(this -> PIN_BUSY) == 0)
    {

    }
    Serial.println("Wait EPD BUSY release!");
//    unsigned char busy;
//    do
//    {        
//       SendCommand(0x71);
//       busy = digitalRead(this -> PIN_BUSY);
//    }
//    while(busy);    
//    delay(200); 
}
void EPD::Sleep()
{  
    this -> HardwareReset();
    Serial.println("Sleep!");
    SendCommand(0x02);   
    this -> WaitUntilIdle();
    SendCommand(0x07); 
    SendData(0xA5);
}
