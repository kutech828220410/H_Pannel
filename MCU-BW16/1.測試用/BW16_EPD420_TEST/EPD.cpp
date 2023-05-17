#include "EPD.h"
#include "Arduino.h"
#include <SPI.h>
void EPD::Init()
{
    //Psram_heap_init();
    //this -> framebuffer = (byte*) malloc(EPD_WIDTH * EPD_HEIGHT);
    //framebuffer = frame_buf;
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
    //BW RW 11,00,10,01
    //send black data
    SPI_Begin();
    this -> BW_Command();
    int EPD_HEIGHT_div = EPD_HEIGHT / 4;
    for (int j = 0; j < EPD_HEIGHT; j++)
    {
        if( (j > EPD_HEIGHT_div * 0) && (j <= EPD_HEIGHT_div * 1))
        {
           for (int i = 0; i < EPD_WIDTH; i++) SendData(0xFF);
        }
        else if( (j > EPD_HEIGHT_div * 1) && (j <= EPD_HEIGHT_div * 2))
        {
           for (int i = 0; i < EPD_WIDTH; i++) SendData(0x00);
        }
        else if( (j > EPD_HEIGHT_div * 2) && (j <= EPD_HEIGHT_div * 3))
        {
           for (int i = 0; i < EPD_WIDTH; i++) SendData(0xFF);
        }
        else if( (j > EPD_HEIGHT_div * 3) && (j <= EPD_HEIGHT_div * 4))
        {
           for (int i = 0; i < EPD_WIDTH; i++) SendData(0x00);
        }
    }
    
    //send red data
    this -> RW_Command();
    for (int j = 0; j < EPD_HEIGHT; j++) 
    {
        if( (j > EPD_HEIGHT_div * 0) && (j <= EPD_HEIGHT_div * 1))
        {
           for (int i = 0; i < EPD_WIDTH; i++) SendData(0xFF);
        }
        else if( (j > EPD_HEIGHT_div * 1) && (j <= EPD_HEIGHT_div * 2))
        {
           for (int i = 0; i < EPD_WIDTH; i++) SendData(0x00);
        }
        else if( (j > EPD_HEIGHT_div * 2) && (j <= EPD_HEIGHT_div * 3))
        {
           for (int i = 0; i < EPD_WIDTH; i++) SendData(0x00);
        }
        else if( (j > EPD_HEIGHT_div * 3) && (j <= EPD_HEIGHT_div * 4))
        {
           for (int i = 0; i < EPD_WIDTH; i++) SendData(0xFF);
        }
    }
    SPI_End();
    this -> RefreshCanvas(); 
    //WaitUntilIdle();

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
   SendCommand(DISPLAY_REFRESH);
   SPI_End();
   this -> SetToSleep = true;
}
void EPD::Sleep_Check()
{
   if(this -> SetToSleep)
   {     
       this -> MyTimer_SleepWaitTime.StartTickTime(60000);
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
void EPD::SendSPI(char* framebuffer ,int size, int offset)
{
   SPI_Begin();
   for (int i = offset; i < size + offset; i++) 
   {
      SendData(*(framebuffer + i));
   }
   SPI_End();
}
void EPD::BW_Command()
{
   SPI_Begin();
   SendCommand(DATA_START_TRANSMISSION_1);
   
   SPI_End();
} 
void EPD::RW_Command()
{
   SPI_Begin();
   SendCommand(DATA_START_TRANSMISSION_2);
   SPI_End();
} 
void EPD::WhiteScreen_ALL(const unsigned char *BW_datas,const unsigned char *R_datas)
{
   BW_Command();
   SPI_Begin();
   for (int i = 0; i < EPD_HEIGHT * EPD_WIDTH; i++) 
   {
      SendData(*(BW_datas + i));
   }
   SPI_End();
   RW_Command();
   SPI_Begin();
   for (int i = 0; i < EPD_HEIGHT * EPD_WIDTH; i++) 
   {
      SendData(*(R_datas + i));
   }
   SPI_End();
   RefreshCanvas();
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
//    pinMode(this -> PIN_MOSI, OUTPUT);
//    pinMode(this -> PIN_SCK, OUTPUT);
   SPI.beginTransaction(SPISettings(20000000, MSBFIRST, SPI_MODE0));
}
void EPD::SPI_End()
{
   SPI.endTransaction();
}
void EPD::SpiTransfer(unsigned char value)
{
   digitalWrite(this -> PIN_CS , LOW);
//   unsigned char tempCom;
//   unsigned char scnt;
//   tempCom = value;
//   for(scnt = 0 ; scnt < 8 ; scnt++)
//   {
//       if(tempCom & 0x80)
//       {
//           digitalWrite(this -> PIN_MOSI , HIGH);
//       }
//       else
//       {
//           digitalWrite(this -> PIN_MOSI , LOW);
//       }
//       digitalWrite(this -> PIN_SCK , LOW);
//       digitalWrite(this -> PIN_SCK , HIGH);
//       tempCom = tempCom << 1;
//   }
   SPI.transfer(value);
   digitalWrite(this -> PIN_CS , HIGH);

}
void EPD::HardwareReset()
{
   delay(10);
   digitalWrite(this -> PIN_RST, LOW);                //module reset    
   delay(10);
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
    SendCommand(POWER_ON);
    WaitUntilIdle();
 
    SendCommand(PANEL_SETTING);
    SendData(0x0F);     // LUT from OTP
   
    SPI_End();
}
void EPD::WaitUntilIdle()
{
   //delay(50);
    while(digitalRead(PIN_BUSY) == HIGH) {      //LOW: idle, HIGH: busy
      delay(10);
    }
    //delay(50);
}
void EPD::Sleep()
{  
    mySerial -> println("Sleep!");
    SPI_Begin();
    SendCommand(VCOM_AND_DATA_INTERVAL_SETTING);
    SendData(0xF7);     // border floating
    SendCommand(POWER_OFF);
    WaitUntilIdle();
    SendCommand(DEEP_SLEEP);
    SendData(0xA5);     // check code
    SPI_End();
}
