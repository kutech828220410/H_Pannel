#include "EPD.h"
#include "Arduino.h"
#include <SPI.h>

unsigned char WS_20_30[159] =
{                      
0x80, 0x66, 0x0,  0x0,  0x0,  0x0,  0x0,  0x0,  0x40, 0x0,  0x0,  0x0,
0x10, 0x66, 0x0,  0x0,  0x0,  0x0,  0x0,  0x0,  0x20, 0x0,  0x0,  0x0,
0x80, 0x66, 0x0,  0x0,  0x0,  0x0,  0x0,  0x0,  0x40, 0x0,  0x0,  0x0,
0x10, 0x66, 0x0,  0x0,  0x0,  0x0,  0x0,  0x0,  0x20, 0x0,  0x0,  0x0,
0x0,  0x0,  0x0,  0x0,  0x0,  0x0,  0x0,  0x0,  0x0,  0x0,  0x0,  0x0,
0x14, 0x8,  0x0,  0x0,  0x0,  0x0,  0x1,          
0xA,  0xA,  0x0,  0xA,  0xA,  0x0,  0x1,          
0x0,  0x0,  0x0,  0x0,  0x0,  0x0,  0x0,          
0x0,  0x0,  0x0,  0x0,  0x0,  0x0,  0x0,          
0x0,  0x0,  0x0,  0x0,  0x0,  0x0,  0x0,          
0x0,  0x0,  0x0,  0x0,  0x0,  0x0,  0x0,          
0x0,  0x0,  0x0,  0x0,  0x0,  0x0,  0x0,          
0x0,  0x0,  0x0,  0x0,  0x0,  0x0,  0x0,          
0x14, 0x8,  0x0,  0x1,  0x0,  0x0,  0x1,          
0x0,  0x0,  0x0,  0x0,  0x0,  0x0,  0x1,          
0x0,  0x0,  0x0,  0x0,  0x0,  0x0,  0x0,          
0x0,  0x0,  0x0,  0x0,  0x0,  0x0,  0x0,          
0x44, 0x44, 0x44, 0x44, 0x44, 0x44, 0x0,  0x0,  0x0,      
0x22, 0x17, 0x41, 0x0,  0x32, 0x36
};  

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
    WaitUntilIdle();
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
    SendCommand(0x92);
    //send red data
    this -> RW_Command();
    for (int j = 0; j < EPD_HEIGHT; j++) 
    {
        for (int i = 0; i < EPD_WIDTH; i++) {
            SendData(0xFF);
        }
    }
    SendCommand(0x92);
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
    SendCommand(0x92);
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
    SendCommand(0x92);
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
       this -> MyTimer_SleepWaitTime.StartTickTime(30000);
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
   SendCommand(0x13);
} 
void EPD::RW_Command()
{
   SendCommand(0x10);
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
   digitalWrite(this -> PIN_RST, HIGH);
   delay(20);
   digitalWrite(this -> PIN_RST, LOW);                //module reset    
   delay(1);
   digitalWrite(this -> PIN_RST, HIGH);
   delay(20);   
}
void EPD::Wakeup()
{
    this -> MyTimer_SleepWaitTime.TickStop();  
    this -> MyTimer_SleepWaitTime.StartTickTime(10000);
    mySerial -> println("Wake up!");
    this -> SetToSleep = false;
    SPI_Begin();
    this -> HardwareReset();
    SendCommand(0x04);//power on
    WaitUntilIdle();

    
    SendCommand(0x00);//panel setting
    SendData(0x0f);//default data
    SendData(0x89);//128x296,Temperature sensor, boost and other related timing settings

    SendCommand(0x61);//Display resolution setting
    SendData (0x80);
    SendData (0x01);
    SendData (0x28);

    SendCommand(0X50);//VCOM AND DATA INTERVAL SETTING      
    SendData(0x77);//WBmode:VBDF 17|D7 VBDW 97 VBDB 57   
                            //WBRmode:VBDF F7 VBDW 77 VBDB 37  VBDR B7
}
void EPD::WaitUntilIdle()
{
   while(1) {  //=1 BUSY
    if(digitalRead(PIN_BUSY)==LOW) 
      break;
    SendCommand(0x71);
    delay(5);
  }
  delay(5);
}
void EPD::Sleep()
{  
    SendCommand(0x02); // POWER_OFF
    WaitUntilIdle();
    SendCommand(0x07); // DEEP_SLEEP
    SendData(0xA5); // check code
}
