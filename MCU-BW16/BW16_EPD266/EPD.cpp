#include "EPD.h"
#include "Arduino.h"
#include <SPI.h>
void EPD::Init(SemaphoreHandle_t mutex)
{
    xSpiMutex = mutex;
    framebuffer = (byte*) malloc(EPD_WIDTH * EPD_HEIGHT);
    buffer_max = EPD_WIDTH * EPD_HEIGHT;
    pinMode(this -> PIN_CS, OUTPUT);
    pinMode(this -> PIN_RST, OUTPUT);
    pinMode(this -> PIN_DC, OUTPUT);
    pinMode(this -> PIN_BUSY, INPUT); 
    
    digitalWrite(this -> PIN_CS , HIGH);
   
    delay(20);

    this -> Wakeup();
    
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
    if (xSemaphoreTake(xSpiMutex, pdMS_TO_TICKS(2000)) == pdTRUE) 
    {
      //send black data
      SPI_Begin();
      if(EPD_TYPE == "EPD579G")
      {
           mySerial -> println("EPD579G Clear function start...");
           byte color = 0x2;
           int Width4 = EPD_WIDTH;
           int Width8 = EPD_WIDTH / 2;
           int Height = EPD_HEIGHT;
           BW_Command();
           for (int j = 0; j < Height / 2; j++) 
           {
              for (int i = 0; i < Width8; i++) 
              {
                  SendData((color<<6) | (color<<4) | (color<<2) | color);
              }
      
              for (int i = 0; i < Width8; i++) 
              {
                  SendData((color<<6) | (color<<4) | (color<<2) | color);
              }
           }
           RW_Command();
           for (int j = 0; j < Height / 2; j++) 
           {
              for (int i = 0; i < Width8; i++) 
              {
                  SendData((color<<6) | (color<<4) | (color<<2) | color);
              }
      
              for (int i = 0; i < Width8; i++) 
              {
                  SendData((color<<6) | (color<<4) | (color<<2) | color);
              }
           }
      }
      else if(EPD_TYPE == "EPD213_BRW_V0")
      {
           mySerial -> println("EPD213_BRW_V0 Clear function start...");
           byte color = 0x0;
           SendCommand(0x10);
           for (int j = 0; j < 250; j++) 
           {
              if(j >= 0 && j < 80)color = WHITE;
              if(j >= 80 && j < 160)color = BLACK;
              if(j >= 160 && j < 250)color = RED;
              for (int i = 0; i < 31; i++) 
              {
                  SendData((color << 6) | (color << 4) | (color << 2) | color);
              }
           }
      }
       else if(EPD_TYPE == "DEPG0579RYT158FxX")
      {
            Wakeup();
            mySerial -> println("DEPG0579RYT158FxX Clear function start...");
            int i, j;
            SendCommand(0xA2);
            SendData(0x01); // M
            SendCommand(0x10); // DTM1
            for (i = 0; i < 272; i++)
            {
              for (j = 0; j < 99; j++)
              {
                if(j >= 0 && j < 25) SendData(0x00);//黑
                if(j >= 25 && j < 50) SendData(0xff);//紅
                if(j >= 50 && j < 75) SendData(0xaa);//黃
                if(j >= 75 && j < 99) SendData(0x55);//白
               
              }
            }
            SendCommand(0xA2);
            SendData(0x02);  // S
            SendCommand(0x10); // DTM1
            for (i = 0; i < 272; i++)
            {
              for (j = 0; j < 99; j++)
              {
                if(j >= 0 && j < 25) SendData(0x00);
                if(j >= 25 && j < 50) SendData(0xff);
                if(j >= 50 && j < 75) SendData(0xaa);
                if(j >= 75 && j < 99) SendData(0x55);
              }
            }
      }
      else if(EPD_TYPE == "EPD579B")
      {
          mySerial -> println("EPD579B Clear function start...");
          //發送M-Part BW資料
          SendCommand(0x24);
          for (int j = 0; j < EPD_HEIGHT; j++) 
          {
              for (int i = 0; i < EPD_WIDTH / 2; i++) 
              {
                  SendData(0x00);
              }
          }
          //發送M-Part RED資料
          SendCommand(0X26);
          for (int j = 0; j < EPD_HEIGHT ; j++) 
          {
              for (int i = 0; i < EPD_WIDTH / 2; i++) 
              {
                  SendData(0x00);
              }
          }    
          //發送S-Part BW資料
          SendCommand(0xA4);
          for (int j = 0; j < EPD_HEIGHT; j++) 
          {
              for (int i = 0; i < EPD_WIDTH / 2; i++) 
              {
                  SendData(0x00);
              }
          }
          //發送S-Part RED資料
          SendCommand(0XA6);
          for (int j = 0; j < EPD_HEIGHT; j++) 
          {
              for (int i = 0; i < EPD_WIDTH / 2; i++) 
              {
                  SendData(0x00);
              }
          }
      }
      else
      {
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
      }
      
      SPI_End();
      mySerial -> println("Clear function done...");
      
      RefreshCanvas(); 
      
       // WaitUntilIdle();
      xSemaphoreGive(xSpiMutex);
    }
    

}
void EPD::DrawFrame_BW()
{
    if (xSemaphoreTake(xSpiMutex, pdMS_TO_TICKS(2000)) == pdTRUE) 
    {
      //send black data
      SPI_Begin();
      if(EPD_TYPE == "EPD579G")
      {
         int Width4 = EPD_WIDTH;
         int Width8 = EPD_WIDTH / 2;
         int Height = EPD_HEIGHT;
         BW_Command();
         for (int j = 0; j < Height / 2; j++) 
         {
            for (int i = 0; i < Width8; i++) 
            {
                SendData(pgm_read_byte(&framebuffer[i + j * Width4]));
            }
    
            for (int i = 0; i < Width8; i++) 
            {
                SendData(pgm_read_byte(&framebuffer[i + (Height - j - 1) * Width4]));
            }
         }
         RW_Command();
         for (int j = 0; j < Height / 2; j++) 
         {
            for (int i = 0; i < Width8; i++) 
            {
                SendData(pgm_read_byte(&framebuffer[j * Width4 + i + Width8]));
            }
    
            for (int i = 0; i < Width8; i++) 
            {
                SendData(pgm_read_byte(&framebuffer[(Height - j - 1) * Width4 + i + Width8]));
            }
         }
      }
      else if(EPD_TYPE == "DEPG0579RYT158FxX")
      {
          int i, j;
          byte tempOriginal;
          int tempcol = 0;
          int templine = 0;
          int templine1 = 271;
          
          SendCommand(0xA2);
          SendData(0x01); // M
          SendCommand(0x10);
          for (i = 0; i < 26928; i++)
          {
            tempOriginal = *(framebuffer + templine * 99 * 2 + tempcol + 99);
            tempcol++;
          
            if (tempcol > 99)
            {
              templine1 = 271 - templine;
              tempOriginal = *(framebuffer + templine1 * 99 * 2 + tempcol + 99 - 99 - 1);
            }
            if (tempcol >= 99 * 2)
            {
              templine++;
              tempcol = 0;
            }
            SendData(tempOriginal);
          }
          tempcol = 0;
          templine = 0;
          templine1 = 271;
          
          SendCommand(0xA2);
          SendData(0x02); // S
          SendCommand(0x10);
          for (i = 0; i < 26928; i++)
          {
            tempOriginal = *(framebuffer + templine * 99 * 2 + tempcol);
            tempcol++;
            if (tempcol > 99)
            {
              templine1 = 271 - templine;
              tempOriginal = *(framebuffer + templine1 * 99 * 2 + tempcol - 99 - 1);
            }
            if (tempcol >= 99 * 2)
            {
              templine++;
              tempcol = 0;
            }
            SendData(tempOriginal);
          }

      }
      else if(EPD_TYPE == "EPD579B")
      {
        //發送M-Part BW資料
        SendCommand(0x24);
        for (int j = 0; j < 272; j++) 
        {
            for (int i = 0; i < 50; i++) 
            {
                SendData(*(framebuffer + j * 99 + i));
            }
        }
        
        //發送S-Part BW資料
        SendCommand(0xA4);
        for (int j = 0; j < 272; j++) 
        {
            for (int i = 0; i < 50; i++) 
            {
                SendData(*(framebuffer + j * 99 + i + 49));
            }
        }
      }
      else
      {
        this -> BW_Command();
  
        for (int j = 0; j < EPD_HEIGHT; j++)
        {
            for (int i = 0; i < EPD_WIDTH; i++) 
            {
                SendData(*(framebuffer + j * EPD_WIDTH + i));
            }
        } 
      }            
      SPI_End();
      xSemaphoreGive(xSpiMutex);
    }
    
}
void EPD::DrawFrame_RW()
{
    if (xSemaphoreTake(xSpiMutex, pdMS_TO_TICKS(2000)) == pdTRUE) 
    {
      //send red data
      SPI_Begin();
      if(EPD_TYPE == "EPD579G")
      {
         int Width4 = EPD_WIDTH;
         int Width8 = EPD_WIDTH / 2;
         int Height = EPD_HEIGHT;
         BW_Command();
         for (int j = 0; j < Height / 2; j++) 
         {
            for (int i = 0; i < Width8; i++) 
            {
                SendData(pgm_read_byte(&framebuffer[i + j * Width4]));
            }
    
            for (int i = 0; i < Width8; i++) 
            {
                SendData(pgm_read_byte(&framebuffer[i + (Height - j - 1) * Width4]));
            }
         }
         RW_Command();
         for (int j = 0; j < Height / 2; j++) 
         {
            for (int i = 0; i < Width8; i++) 
            {
                SendData(pgm_read_byte(&framebuffer[j * Width4 + i + Width8]));
            }
    
            for (int i = 0; i < Width8; i++) 
            {
                SendData(pgm_read_byte(&framebuffer[(Height - j - 1) * Width4 + i + Width8]));
            }
         }
      }
      else if(EPD_TYPE == "EPD579B")
      {

        //發送M-Part RW資料
        SendCommand(0x26);
        for (int j = 0; j < 272; j++) 
        {
            for (int i = 0; i < 50; i++) 
            {
               SendData(*(framebuffer + j * 99 + i));
            }
        }
        
        //發送S-Part RW資料
        SendCommand(0xA6);
        for (int j = 0; j < 272; j++) 
        {
            for (int i = 0; i < 50; i++) 
            {
                SendData(*(framebuffer + j * 99 + i + 49));
            }
        }
      }
      else
      {
         RW_Command();
  
         for (int j = 0; j < EPD_HEIGHT; j++) 
         {
            for (int i = 0; i < EPD_WIDTH; i++) 
            {
              SendData(*(framebuffer + j * EPD_WIDTH + i));
            }
         }
      }     
      SPI_End();
      xSemaphoreGive(xSpiMutex);
    }
    
}
void EPD::RefreshCanvas()
{ 
     if(EPD_TYPE == "EPD420" || EPD_TYPE == "EPD420_D")
     {
       SPI_Begin();
       SendCommand(0x22);
       SendData(0xF7);
       SendCommand(0x20);
       SPI_End();
     }
     else if(EPD_TYPE == "EPD579B")
     {
       mySerial -> println("EPD579B (RefreshCanvas)function start...");
       SPI_Begin();
       SendCommand(0x22);
       SendData(0xD7);
       SendCommand(0x20);
       SPI_End();
       mySerial -> println("EPD579B (RefreshCanvas)function done...");
     }
     else if(EPD_TYPE == "EPD583")
     {
       SPI_Begin();
       SendCommand(0x12);
       SPI_End();
     }
     else if(EPD_TYPE == "DEPG0579RYT158FxX")
     {
       mySerial -> println("DEPG0579RYT158FxX (RefreshCanvas)function start...");
       SPI_Begin();
       SendCommand(0xA2); 
       SendData(0x00);         
       SendCommand(0x12); 
       SendData(0x01);
       SPI_End();
       mySerial -> println("DEPG0579RYT158FxX (RefreshCanvas)function done...");
     }
     else if(EPD_TYPE == "EPD579G")
     {
       mySerial -> print(EPD_TYPE);
       mySerial -> println(" (RefreshCanvas)function start...");
       SPI_Begin();
       SendCommand(0x04);
       delay(200);
       SendCommand(0x12);
       SendData(0x00);
       SPI_End();
       mySerial -> print(EPD_TYPE);
       mySerial -> println(" (RefreshCanvas)function done...");
     }
     else if(EPD_TYPE == "EPD213_BRW_V0")
     {
       mySerial -> print(EPD_TYPE);
       mySerial -> println(" (RefreshCanvas)function start...");
       SPI_Begin();
       SendCommand(0x04);
       WaitUntilIdle();
       SendCommand(0x12);
       SendData(0x00);
       WaitUntilIdle();
       SendCommand(0x02);
       SendData(0x00);
       WaitUntilIdle();
       SPI_End();
       mySerial -> print(EPD_TYPE);
       mySerial -> println(" (RefreshCanvas)function done...");
     }
     else
     {
       SPI_Begin();
       SendCommand(0x20);
       SPI_End();
     }
     
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
   this -> MyTimer_SleepWaitTime.TickStop();  
   this -> MyTimer_SleepWaitTime.StartTickTime(90000);
   this -> SetToSleep = false;
   if(EPD_TYPE == "EPD583")
   {
     SendCommand(0x10);
   }
   else if(EPD_TYPE == "EPD579G")
   {
     SendCommand(0xA2);  //********************
     SendData(0x02);
     SendCommand(0x10);
   }
   else if(EPD_TYPE == "EPD213_BRW_V0")
   {
     SendCommand(0x10);
   }
   else
   {
     SendCommand(0x24);
   }
} 
void EPD::RW_Command()
{
   this -> MyTimer_SleepWaitTime.TickStop();  
   this -> MyTimer_SleepWaitTime.StartTickTime(90000);
   this -> SetToSleep = false;
   
   if(EPD_TYPE == "EPD583")
   {
     SendCommand(0x13);
   }
   else if(EPD_TYPE == "EPD579G")
   {
     SendCommand(0xA2);  //********************
     SendData(0x01);
     SendCommand(0x10);
   }
   else if(EPD_TYPE == "EPD213_BRW_V0")
   {
     SendCommand(0x10);
   }
   else
   {
     SendCommand(0x26);
   }
   
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
//   SPI.beginTransaction(SPISettings(2000000, MSBFIRST, SPI_MODE0));
   if(EPD_TYPE == "EPD583")SPI.beginTransaction(SPISettings(10000000, MSBFIRST, SPI_MODE0));
   else if(EPD_TYPE == "EPD579G" || EPD_TYPE == "EPD579B" || EPD_TYPE == "DEPG0579RYT158FxX")SPI.beginTransaction(SPISettings(2000000, MSBFIRST, SPI_MODE0));
   else SPI.beginTransaction(SPISettings(80000000, MSBFIRST, SPI_MODE0));    
}
void EPD::SPI_End()
{
//   SPI.endTransaction();
   digitalWrite(this -> PIN_CS , LOW); 
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
   delay(10);
   digitalWrite(this -> PIN_RST, HIGH);
   delay(10);   
}
void EPD::Wakeup()
{
    this -> MyTimer_SleepWaitTime.TickStop();  
    this -> MyTimer_SleepWaitTime.StartTickTime(90000);
//    mySerial -> println(" Wake up ...");
    this -> SetToSleep = false;
    this -> HardwareReset();
 

    if (xSemaphoreTake(xSpiMutex, pdMS_TO_TICKS(2000)) == pdTRUE || true) 
    {
        SPI_Begin();
        if(EPD_TYPE == "EPD420" || EPD_TYPE == "EPD420_D")
        {
          mySerial -> println("EPD420 Init...");
          SPI_Begin();
          WaitUntilIdle();
          SendCommand(0x12);//soft  reset
          WaitUntilIdle();
          SendCommand(0x3C); //BorderWavefrom
          SendData(0x05);  
      
          SendCommand(0x18); //Read built-in temperature sensor
          SendData(0x80); 
      
          SendCommand(0x11); //data entry mode       
          SendData(0x03);
      
          SendCommand(0x44); //set Ram-X address start/end position   
          SendData(0x00);
          SendData(400/8-1);
      
          SendCommand(0x45); //set Ram-Y address start/end position          
          SendData(0x00);
          SendData(0x00); 
          SendData((300-1)%256);    
          SendData((300-1)/256);
      
          SendCommand(0x4E);   // set RAM x address count to 0;
          SendData(0x00);
          SendCommand(0x4F);   // set RAM y address count to 0X199;    
          SendData(0x00);    
          SendData(0x00);
        }
        else if(EPD_TYPE == "EPD583")
        {
          mySerial -> println("EPD583 Init...");
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
          SendData(0x02);    //source 648
          SendData(0x88);
          SendData(0x01);    //gate 480
          SendData(0xe0);
        
          SendCommand(0X15);    
          SendData(0x00);   
        
          SendCommand(0X50);      //VCOM AND DATA INTERVAL SETTING
          SendData(0x11);
          SendData(0x07);
        
          SendCommand(0X60);      //TCON SETTING
          SendData(0x22);
        }
        else if(EPD_TYPE == "DEPG0579RYT158FxX")
        {
          
          mySerial -> println("DEPG0579RYT158FxX Init...");
          WaitUntilIdle();
          SendCommand(0xE6);
          SendData(0x16);
          SendCommand(0xE0);
          SendData(0x03);
          delay(10);
          SendCommand(0xA5);
          WaitUntilIdle();
          
          SendCommand(0xA2);
          SendData(0x01); // M
          SendCommand(0x00);
          SendData(0x03);
          SendData(0x29);
          SendCommand(0xA2);
          SendData(0x00);
          
          SendCommand(0xA2);
          SendData(0x02); // S
          SendCommand(0x00);
          SendData(0x07);
          SendData(0x29);
          SendCommand(0xA2);
          SendData(0x00);
          
          SendCommand(0x01);
          SendData(0x07);
          SendData(0x00);
          SendData(0x28);
          SendData(0x78);
          SendData(0x24);
          SendData(0x2C);
          
          /* Extend VGL discharging time start */
          SendCommand(0x03); // POFS
          SendData(0x10);
          SendData(0x54);
          SendData(0x00);
          /* Extend VGL discharging time end */
          
          SendCommand(0x06);
          SendData(0xC0);
          SendData(0xC0);
          SendData(0xC0);
          
          SendCommand(0x30); // frame go with waveform
          SendData(0x02);
          
          SendCommand(0x41);
          SendData(0x00);
          
          SendCommand(0x50);
          SendData(0x37);
          
          SendCommand(0x60);
          SendData(0x02);
          SendData(0x02);
          
          SendCommand(0x61);
          SendData(0x01);
          SendData(0x8c);
          SendData(0x01);
          SendData(0x10);
          
          SendCommand(0x65);
          SendData(0x00);
          SendData(0x00);
          SendData(0x00);
          SendData(0x00);
          
          SendCommand(0x82); 
          SendData(0xaC);
          
          SendCommand(0xE7);
          SendData(0x1C);
          
          SendCommand(0xE3);
          SendData(0x77);
          
          //   SendCommand(0xE0);
          //   SendData(0x00);
          
          SendCommand(0xFF);
          SendData(0xA5);
          
          /* improve pwm start */
          SendCommand(0xEF);
          SendData(0X01);
          SendData(0X1E);
          SendData(0X06);
          SendData(0X1E);
          SendData(0X0E);
          SendData(0x1C);
          SendData(0x21);
          SendData(0X10);
          /* improve pwm end */
          
          SendCommand(0xDB);
          SendData(0x00);
          
          SendCommand(0xF9);
          SendData(0x00);
          
          SendCommand(0xCF);
          SendData(0x00);
          
          SendCommand(0xDF);
          SendData(0x3F);
          
          SendCommand(0xFD);
          SendData(0x01);
          
          SendCommand(0xE8);
          SendData(0x00);
          SendCommand(0xDC);
          SendData(0x00);
          
          SendCommand(0xDD);
          SendData(0x01);
          
          /* improve pwm start */
          SendCommand(0xDE);
          SendData(0x15);
          /* improve pwm end  */
          
          SendCommand(0xFF);
          SendData(0xE3);
          
          SendCommand(0xE9);
          SendData(0x01);
          SendCommand(0x04); //Power on
          WaitUntilIdle();
        }
        else if(EPD_TYPE == "EPD579B")
        {
          mySerial -> println("EPD579B Init...");
          SPI_Begin(); 
          WaitUntilIdle();
          SendCommand(0x12);  // POWER ON
          WaitUntilIdle();
      
          SendCommand(0x18);  
          SendData(0x80); 
      
          SendCommand(0x3C);  
          SendData(0x01);      
      
          SendCommand(0x0C);  
          SendData(0x8B); 
          SendData(0x9C); 
          SendData(0xE4); 
          SendData(0x0F); 
      
          SendCommand(0x03);  
          SendData(0x17);   
      
      
              
          SendCommand(0x01);  // Driver Output Control
          SendData(0x0F);
          SendData(0x01);
          SendData(0x0E);
          delay(100);
          SendCommand(0x21);  // Display Update Control
          SendData(0x00);
          SendData(0x10);
          delay(100);
          
          
          
          SendCommand(0x3C);  // 波形控制
          SendData(0x01);
          SendCommand(0x2C);  // 波形控制
          SendData(0x38);

          SendCommand(0x11);  // Data Entry Mode
          SendData(0x01);  // 確保方向正確
          delay(100);
          SendCommand(0x44);  // Set RAM X Address
          SendData(0x00);     // X Start Address
          SendData(0x31);     // X End Address 
          
          SendCommand(0x45);  // Set RAM Y Address
          SendData(0x0F);     // Y Start L
          SendData(0x01);     // Y Start H
          SendData(0x00);     // Y End L 
          SendData(0x00);     // Y End H
      
          SendCommand(0x91);  
          SendData(0x00);  
          
          SendCommand(0xC4);  // Set X Address Counter
          SendData(0x31);
          SendData(0x00);
      
          SendCommand(0xC5);  // Set X Address Counter
          SendData(0x0F);
          SendData(0x01);
          SendData(0x00);
          SendData(0x00);
          
          SendCommand(0x4E);  // Set X Address Counter
          SendData(0x00);
          
          SendCommand(0x4F);  // Set Y Address Counter
          SendData(0x0F);
          SendData(0x01);
      
          SendCommand(0xCE);  // Set X Address Counter
          SendData(0x31);
          
          SendCommand(0xCF);  // Set Y Address Counter
          SendData(0x0F);
          SendData(0x01);
          SPI_End();
        }
        else if(EPD_TYPE == "EPD579G")
        {
          mySerial -> println("EPD579G Init...");
          SPI_Begin(); 
          SendCommand(0xE0);
          SendData(0x01);
          
          SendCommand(0xA5);
          delay(500);
//          WaitUntilIdle();
          //--------------------------------------//  
          SendCommand(0xA2); //Master
          SendData(0x01);
          
          SendCommand(0x00);
          SendData(0x03);
          SendData(0x29);
          SendCommand(0xA2); //Slave
          SendData(0x00);
      
          //-------------------------------------//  
       
          SendCommand(0xA2); //Slave
          SendData(0x02);
          SendCommand(0x00);
          SendData(0x07);
          SendData(0x29);
          
          SendCommand(0xA2);
          SendData(0x00);
          //-------------------------------------//
          
          SendCommand(0x01);
          SendData(0x07);
          SendData(0x00);
          SendData(0x28);
          SendData(0x78);
          SendData(0x24);
          SendData(0x2C);
          
          SendCommand(0x03);
          SendData(0x10);
          SendData(0x54);
          SendData(0x00);
          
          SendCommand(0x06);
          SendData(0xC0);
          SendData(0xC0);
          SendData(0xC0);
        
          SendCommand(0x30);
          SendData(0x02);
        
          SendCommand(0x41);
          SendData(0x00);
        
          SendCommand(0x50);
          SendData(0x37);
          
          SendCommand(0x60);
          SendData(0x02);
          SendData(0x02); 
          
          SendCommand(0x61);
          SendData(0x01);
          SendData(0x8C); 
          SendData(0x01); 
          SendData(0x10);
          
          SendCommand(0x65);
          SendData(0x00);
          SendData(0x00);
          SendData(0x00);
          SendData(0x00);
          
          SendCommand(0xE7);
          SendData(0x1C);
          
          SendCommand(0xE3);
          SendData(0x77);
          
          SendCommand(0xFF);
          SendData(0xA5);
          
          SendCommand(0xEF);
          SendData(0X01);
          SendData(0X1E);
          SendData(0X06);
          SendData(0X1E);
          SendData(0X0E);
          SendData(0x1C);
          SendData(0x21);
          SendData(0X10);
          
          SendCommand(0xDB);   
          SendData(0x00);
          
          SendCommand(0xF9);
          SendData(0x00);
          
          SendCommand(0xCF);   
          SendData(0x00);
          
          SendCommand(0xDF);   
          SendData(0x3F);
          
          SendCommand(0xFD);   
          SendData(0x01);
          
          SendCommand(0xE8);
          SendData(0x00);
          
          SendCommand(0xDC);
          SendData(0x00);
          SendCommand(0xDD);
          SendData(0x01);
          SendCommand(0xDE);
          SendData(0x15);
        
          SendCommand(0xFF);
          SendData(0xE3);

          SendCommand(0xE9);
          SendData(0xE1);
          
          SPI_End();
        }
        else if(EPD_TYPE == "EPD213_BRW_V0")
        {
          mySerial -> println("EPD213_BRW_V0 Init...");
          SPI_Begin(); 
          SendCommand(0x00);                                                                                                                                                                                 
          SendData(0x07);
          SendData(0x29);
        
          SendCommand(0x01);
          SendData(0x07);
         
          SendCommand(0x03);
          SendData(0x10);
          SendData(0x54);
          SendData(0x44);
          
          SendCommand(0x06);
          SendData(0x40);
          SendData(0x40);
          SendData(0x40);
          
          SendCommand(0x30);
          SendData(0x08);
        
          SendCommand(0x41);
          SendData(0x00);
        
          SendCommand(0x50);
          SendData(0x37);
          
          SendCommand(0x60);
          SendData(0x03);
          SendData(0x03);
          
          SendCommand(0x61);
          SendData(0x00);
          SendData(0x7C);
          SendData(0x00);
          SendData(0xFA);
          
          SendCommand(0x65);
          SendData(0x00);
          SendData(0x00);
          SendData(0x00);
          SendData(0x00);
         
          SendCommand(0xE7);
          SendData(0x1C);
          
          SendCommand(0xE3);
          SendData(0x00);
          
          SendCommand(0xE0);
          SendData(0x00); 
        
        
          
          SendCommand(0xFF);        
          SendData(0xA5);       
          
          SendCommand(0xEF);        // PWM Set
          SendData(0x02);     
          SendData(0x88);  
          SendData(0x04); 
          SendData(0x1A); 
          SendData(0X06); 
          SendData(0X15); 
          SendData(0X08);
          SendData(0X22);
          
          SendCommand(0xDA);        
          SendData(0X08);     
          
          SendCommand(0xE8);   
          SendData(0X08);     
          
          SendCommand(0xDC);//CPCK EN
          SendData(0X01);
          
          SendCommand(0xDD);//CPCK EN
          SendData(0X08); 
          
          SendCommand(0xDE);//CPCK EN
          SendData(0X3C); 
        
          SendCommand(0xFF);        // Exit Test command    ***********
          SendData(0xE3);  
          SPI_End();
        }
        else
        {
          SPI_Begin();
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
          SPI_End();
        }
        xSemaphoreGive(xSpiMutex);
    }
    
    
}
void EPD::WaitUntilIdle()
{
   //delay(50);
   if(EPD_TYPE == "EPD583")
   {
      delay(1);
      mySerial -> println("Wait EPD BUSY!");
      unsigned char busy;
      SPI_Begin();
      do
      {        
         
         SendCommand(0x71);
         
         busy = digitalRead(this -> PIN_BUSY);
      }
      while(busy);    
      delay(200); 
      mySerial -> println("Wait EPD BUSY release!");
   }
   else if(EPD_TYPE == "EPD579B")
   {
      byte busy;
      do
      {
        busy = digitalRead(PIN_BUSY);
        delay(10);   
      }
      while(busy);   
   }
   else if(EPD_TYPE == "DEPG0579RYT158FxX")
   {
      int retry = 0 ;
      while(true)
      {
         if(retry >= 300)
         {
            mySerial -> print("e-Paper (DEPG0579RYT158FxX) read pin busy timeout...\r\n ");
            break;
         }
         if(digitalRead(PIN_BUSY) == HIGH)
         {
            break;
         }
         delay(10);
         retry++;
      }
   }
   else if(EPD_TYPE == "EPD579G" || EPD_TYPE == "EPD213_BRW_V0")
   {
      mySerial -> print("e-Paper busy H\r\n ");
      while(digitalRead(PIN_BUSY) == LOW) 
      {      
          delay(20);
      } 
      mySerial -> print("e-Paper busy release H\r\n "); 
   }
   else
   {
     while(digitalRead(PIN_BUSY) == HIGH) 
     {  
         delay(20);
     } 
   }
   

}
unsigned char EPD::Color_get(unsigned char color)
{
    unsigned datas;
    switch(color)
    {
      case 0x00:
        datas=WHITE;  
        break;    
      case 0x01:
        datas=YELLOW;
        break;
      case 0x02:
        datas=RED;
        break;    
      case 0x03:
        datas=BLACK;
        break;      
      default:
        break;      
    }
     return datas;
}
void EPD::Sleep()
{  
    if (xSemaphoreTake(xSpiMutex, pdMS_TO_TICKS(2000)) == pdTRUE) 
    {
        if(EPD_TYPE == "EPD583")
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
//        else if(EPD_TYPE == "EPD579G" || EPD_TYPE == "EPD213_BRW_V0")
//        {
//          mySerial -> println("Sleep ....");
//          SPI_Begin();
//          SendCommand(0x07); 
//          SendData(0xA5);
//          SPI_End();
//        }
        else if(EPD_TYPE == "EPD579B")
        {
          SPI_Begin();
          SendCommand(0X10);    //deep sleep
          SendData(0x03);
          SPI_End();
        }     
        else if(EPD_TYPE == "DEPG0579RYT158FxX")
        {
          SPI_Begin();
          SendCommand(0X12);    //deep sleep
          SendData(0x01);
          delay(50);
          SendCommand(0x02);    
          SendData(0x01);
          SPI_End();
        }
        else
        {
          SPI_Begin();
          SendCommand(0x10);
          SendData(0x01);
          SPI_End();
        }
        xSemaphoreGive(xSpiMutex);
    }
   
    
}
