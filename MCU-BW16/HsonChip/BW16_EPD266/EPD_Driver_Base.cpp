#include "Arduino.h"
#include <SPI.h>
#include "epd_driver_base.h"

EPD_Driver_Base::~EPD_Driver_Base() {}
void EPD_Driver_Base::Init(SemaphoreHandle_t mutex)
{
    xSpiMutex = mutex;
    
    pinMode(this -> PIN_CS, OUTPUT);
    pinMode(this -> PIN_BUSY, INPUT);     
    digitalWrite(this -> PIN_CS , HIGH);
    #ifdef MCP23008
    mySerial->println("define MCP23008 , set RST(6),D/C(7) PIN mode...");
    _mcp ->pinMode(PIN_RST , OUTPUT);
    _mcp ->pinMode(PIN_DC , OUTPUT);
    mySerial->println("define MCP23008 , set RST(GPA6),D/C(GPA7) PIN done!");
    #else
    pinMode(this -> PIN_RST, OUTPUT);
    pinMode(this -> PIN_DC, OUTPUT);
    #endif
    
    
    delay(20);
    melloc_init();
    this -> Wakeup();  
}
void EPD_Driver_Base::SendSPI(char* framebuffer ,int size, int offset)
{
  
   SPI_Begin();
   for (int i = offset; i < size; i++) 
   {
      SendData(*(framebuffer + i));
   }
   SPI_End();
}
void EPD_Driver_Base::SetCursor(int Xstart, int Ystart)
{
    SendCommand(0x4E); // SET_RAM_X_ADDRESS_COUNTER
    SendData(Xstart & 0x1F);

    SendCommand(0x4F); // SET_RAM_Y_ADDRESS_COUNTER
    SendData(Ystart & 0xFF);
    SendData((Ystart >> 8) & 0x01);  
}
void EPD_Driver_Base::Sleep_Check()
{
   if(this -> SetToSleep)
   {     
       this -> MyTimer_SleepWaitTime.StartTickTime(sleep_check_time);
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
void EPD_Driver_Base::SetWindows(int Xstart, int Ystart, int Xend, int Yend)
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

bool EPD_Driver_Base::GetReady()
{
   return (digitalRead(this -> PIN_BUSY) == 1);
}

void EPD_Driver_Base::SendCommand(unsigned char command)
{
   #ifdef MCP23008
   if(PIN_DC_buf == true)
   {
      _mcp ->digitalWrite(PIN_DC, false);
      PIN_DC_buf = false;
   }
   #else
   digitalWrite(this -> PIN_DC, LOW);
   #endif 
   SpiTransfer(command);
}
void EPD_Driver_Base::SendData(unsigned char data)
{
   #ifdef MCP23008
   if(PIN_DC_buf != true)
   {
      _mcp ->digitalWrite(PIN_DC, true);
      PIN_DC_buf = true;
   }
   #else
   digitalWrite(this -> PIN_DC, HIGH);
   #endif
   SpiTransfer(data);
}

void EPD_Driver_Base::SPI_End()
{
//   SPI.endTransaction();
   digitalWrite(this -> PIN_CS , LOW); 
}
void EPD_Driver_Base::SpiTransfer(unsigned char value)
{  
   digitalWrite(this -> PIN_CS , LOW);
   SPI.transfer(value);
   digitalWrite(this -> PIN_CS , HIGH);
}
void EPD_Driver_Base::HardwareReset()
{
   #ifdef MCP23008
   mySerial->println("define MCP23008 , set RST(6) LOW...");
   _mcp ->digitalWrite(PIN_RST, LOW);
   delay(10);
   mySerial->println("define MCP23008 , set RST(6) HIGH...");
   _mcp ->digitalWrite(PIN_RST, HIGH);
   delay(10);
   #else
   digitalWrite(this -> PIN_RST, LOW);
   delay(10);
   digitalWrite(this -> PIN_RST, HIGH);
   delay(10); 
   #endif 
     
}
