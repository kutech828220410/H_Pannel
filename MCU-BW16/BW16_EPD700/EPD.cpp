#include "EPD.h"
#include "Arduino.h"
#include <SPI.h>
void EPD::Init()
{
    this -> framebuffer = (byte*) malloc(EPD_WIDTH * EPD_HEIGHT / 2);
    pinMode(this -> PIN_CS, OUTPUT);
    pinMode(this -> PIN_RST, OUTPUT);
    pinMode(this -> PIN_DC, OUTPUT);
    pinMode(this -> PIN_BUSY, INPUT); 
    
    digitalWrite(this -> PIN_CS , HIGH);
   
    delay(20);

    this -> Wakeup();
    
}
/******************************************************************************
function :  Sends the image buffer in RAM to e-Paper and displays
parameter:
******************************************************************************/
void EPD::EPD_7IN3F_Display(const UBYTE *image) 
{
    unsigned long i,j;
    
    SendCommand(0x10);
    for(i=0; i< EPD_HEIGHT; i++) 
    {
        for(j = 0; j < EPD_WIDTH; j++) 
        {
            SendData(image[j + EPD_WIDTH*2 * i]);
        }
    }
    
//    RefreshCanvas();
}
/******************************************************************************
function :  Sends the part image buffer in RAM to e-Paper and displays
parameter:
******************************************************************************/
void EPD::EPD_7IN3F_Display_part(const UBYTE *image, UWORD xstart, UWORD ystart, 
                                        UWORD image_width, UWORD image_heigh)
{
    unsigned long i,j;
    SPI_Begin();
    SendCommand(0x10);
    for(i = 0; i < EPD_HEIGHT; i++) 
    {
        for(j = 0; j < EPD_WIDTH; j++) 
        {
            if(i < image_heigh+ystart && i >= ystart && j < (image_width+xstart)/2 && j >= xstart/2) 
            {
              SendData(pgm_read_byte(&image[(j - xstart/2) + (image_width / 2*(i - ystart))]));
            }
            else 
            {
              SendData(0x11);
            }
        }
    }
    RefreshCanvas();
    SPI_End();
}
/******************************************************************************
function :  show 7 kind of color block
parameter:
******************************************************************************/
void EPD::EPD_7IN3F_Show7Block(void)
{
    unsigned long i, j, k;
    unsigned char const Color_seven[8] = 
    {EPD_7IN3F_BLACK, EPD_7IN3F_BLUE, EPD_7IN3F_GREEN, EPD_7IN3F_ORANGE,
    EPD_7IN3F_RED, EPD_7IN3F_YELLOW, EPD_7IN3F_WHITE, EPD_7IN3F_WHITE};
    SPI_Begin();
    SendCommand(0x10);
    for(i=0; i<240; i++) {
        for(k = 0 ; k < 4; k ++) {
            for(j = 0 ; j < 100; j ++) {
                SendData((Color_seven[k]<<4) |Color_seven[k]);
            }
        }
    }
    
    for(i=0; i<240; i++) {
        for(k = 4 ; k < 8; k ++) {
            for(j = 0 ; j < 100; j ++) {
                SendData((Color_seven[k]<<4) |Color_seven[k]);
            }
        }
    }
    RefreshCanvas();
    SPI_End();
}

bool EPD::GetReady()
{
   return (digitalRead(this -> PIN_BUSY) == 1);
}
void EPD::Clear(UBYTE color)
{
    SPI_Begin();
    SendCommand(0x10);
   
    for(int j = 0; j < EPD_HEIGHT; j++) 
    {
        for(int i = 0; i < EPD_WIDTH; i++) 
        {
            SendData((color<<4)|color);           
        }
    }
    RefreshCanvas();
    SPI_End();
}
void EPD::SendSPI(char* framebuffer ,int size, int offset)
{
  
//   SendCommand(0x10);
   for (int i = offset; i < size; i++) 
   {
      SendData(*(framebuffer + i));
   }
//   SPI_End();
}
void EPD::DrawFrame_BW()
{
    //send black data
    SPI_Begin();
    for (int j = 0; j < EPD_HEIGHT / 2; j++) 
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
    SendCommand(0x10);
    for (int j = 0; j < EPD_HEIGHT / 2; j++) 
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

//   SendCommand(0x04); // POWER_ON
//   WaitUntilIdle();
//
//    //Second setting 
//   SendCommand(0x06);
//   SendData(0x6F);
//   SendData(0x1F);
//   SendData(0x17);
//   SendData(0x49);
//
//   SendCommand(0x12); // DISPLAY_REFRESH
//   SendData(0x00);
//   WaitUntilIdle();
//    
//   SendCommand(0x02); // POWER_OFF
//   SendData(0X00);
//   WaitUntilIdle();


   SendCommand(0x04);  // POWER_ON
   WaitUntilIdle();
   delay(10);
   SendCommand(0x12);  // DISPLAY_REFRESH
   SendData(0x00);
   WaitUntilIdle();
   delay(10);
   SendCommand(0x02);  // POWER_OFF
   SendData(0x00);
   WaitUntilIdle();
   delay(200);
   SendCommand(0x04);  // POWER_ON
   WaitUntilIdle();
   delay(10);
   SendCommand(0x12);  // DISPLAY_REFRESH
   SendData(0x00);
   WaitUntilIdle();
   delay(10);
   SendCommand(0x02);  // POWER_OFF
   SendData(0x00);
   WaitUntilIdle();
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
//   SPI.beginTransaction(SPISettings(2000000, MSBFIRST, SPI_MODE0));
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
    digitalWrite(PIN_RST, HIGH);
    delay(20);   
    digitalWrite(PIN_RST, LOW);                //module reset    
    delay(20);
    digitalWrite(PIN_RST, HIGH);
    delay(20);   
}
void EPD::Wakeup()
{
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

    SendCommand(0x01);
    SendData(0x3F);
    SendData(0x00);
    SendData(0x32);
    SendData(0x2A);
    SendData(0x0E);
    SendData(0x2A);

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
    SendData(0x1F);
    SendData(0x22);

    SendCommand(0x08);
    SendData(0x6F);
    SendData(0x1F);
    SendData(0x1F);
    SendData(0x22);

    SendCommand(0x13);    // IPC
    SendData(0x00);
    SendData(0x04);

    SendCommand(0x30);
    SendData(0x3C);

    SendCommand(0x41);     // TSE
    SendData(0x00);

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

    SendCommand(0x82);
    SendData(0x1E); 

    SendCommand(0x84);
    SendData(0x00);

    SendCommand(0x86);    // AGID
    SendData(0x00);

    SendCommand(0xE3);
    SendData(0x2F);

    SendCommand(0xE0);   // CCSET
    SendData(0x00); 

    SendCommand(0xE6);   // TSSET
    SendData(0x00);
    SPI_End();
//    mySerial -> println("Wake up done!");
}
void EPD::WaitUntilIdle()
{
    if(!digitalRead(this -> PIN_BUSY))
    {
       delay(10);
       mySerial -> println("WaitUntilIdle....");
    }
    
 
}
void EPD::Sleep()
{  
    this -> HardwareReset();
//    mySerial -> println("Sleep!");

    //this -> WaitUntilIdle();
    SPI_Begin();
    SendCommand(0x07); 
    SendData(0xA5);
    SPI_End();
}
