#include "RA8871.h"
#include "Arduino.h"
#include <SPI.h>

bool RA8875::Init()
{
   pinMode(PIN_CS, OUTPUT);
   digitalWrite(PIN_CS, HIGH);
   pinMode(PIN_RST, OUTPUT);
  
   LCD_Reset();
   readReg(0x00);
   uint8_t x ;
   mySerial -> println("RA8875 IO Init done!");
   displayOn(true);
   readReg(0x12);
//   writeReg(0x88 ,0x07);
//   writeReg(0x89 ,0x03);
//   delay(1);
//   writeReg(0x01 ,0x01);
//   writeReg(0x01 ,0x00);
//   delay(100);
//   writeReg(0x10 ,0x3C);
//   writeReg(0x10 ,0x0C);
//   writeReg(0x12 ,0x00);
//   writeReg(0x13 ,0x00);
//
//   writeReg(0x14 ,0x27);
//   writeReg(0x15 ,0x02);
//   writeReg(0x16 ,0x03);
//   writeReg(0x17 ,0x01);
//   writeReg(0x18 ,0x03);
//   
//   writeReg(0x19 ,0xef);
//   writeReg(0x1a ,0x00);
//   writeReg(0x1b ,0x0f);
//   writeReg(0x1c ,0x00);
//   writeReg(0x1d ,0x0e);
//   writeReg(0x1e ,0x06);
//   writeReg(0x1f ,0x01);
//
//   writeReg(0x28 ,0x02);
//
//   
//   
//   ActiveWindow(0,799,0,479);
//   Memory_Clear_with_Font_BgColor();
//   Text_Background_Color(0xFC);
//   Memory_Clear();
//   
//   writeReg(0x01 ,0x80);
//   
//   PWM1config(true, RA8875_PWM_CLK_DIV1); // PWM output for backlight
//   PWM1out(0xFF);
//
//  // With hardware accelleration this is instant
//   fillScreen(RA8875_YELLOW);
   return true;
   
}
void RA8875::ActiveWindow(uint16_t XL ,uint16_t XR ,uint16_t YT ,uint16_t YB)
{
   uint16_t temp;
   temp = XL;
   writeReg(0x30 ,temp);
   temp = XL >> 8;
   writeReg(0x31 ,temp);
   temp = XR;
   writeReg(0x34 ,temp);
   temp = XR >> 8;
   writeReg(0x35 ,temp); 

   temp = YT;
   writeReg(0x32 ,temp); 
   temp = YT >> 8;
   writeReg(0x33 ,temp); 
   temp = YB;
   writeReg(0x36 ,temp); 
   temp = YB >> 8;
   writeReg(0x37 ,temp); 
   
} 

void RA8875::Memory_Clear_with_Font_BgColor(void)
{ 
   uint8_t temp;
   readReg(0x8e);
   temp |= cSetD0;
   writeReg(0x8e , temp); //MCLR
}
void RA8875::Text_Background_Color(uint16_t color)
{ 
   uint8_t temp;
   temp = color;
   writeReg(0x43 , temp);

} 

void RA8875::Memory_Clear(void)
{
   uint8_t temp;
   readReg(0x8e);
   temp |= cSetD7;
   writeReg(0x8e , temp); //MCLR
   Chk_Busy();
} 
void RA8875::displayOn(boolean on)
{
  uint8_t temp;
  readReg(0x12);
  if(on) temp |= cSetD6;
  else temp &= cClrD6;
  writeReg(0x12 , temp); //MCLR
  Chk_Busy();

}
void RA8875::GPIOX(boolean on)
{
  if (on)
    writeReg(RA8875_GPIOX, 1);
  else
    writeReg(RA8875_GPIOX, 0);
}
void RA8875::PWM1config(boolean on, uint8_t clock) 
{
  if (on) {
    writeReg(RA8875_P1CR, RA8875_P1CR_ENABLE | (clock & 0xF));
  } else {
    writeReg(RA8875_P1CR, RA8875_P1CR_DISABLE | (clock & 0xF));
  }
}
void RA8875::fillScreen(uint16_t color)
{
  rectHelper(0, 0, _width - 1, _height - 1, color, true);
}

void RA8875::PWM1out(uint8_t p)
{
  writeReg(RA8875_P1DCR, p);
}
void RA8875::rectHelper(int16_t x, int16_t y, int16_t w, int16_t h,
                                 uint16_t color, bool filled) {
  x = applyRotationX(x);
  y = applyRotationY(y);
  w = applyRotationX(w);
  h = applyRotationY(h);

  /* Set X */
  writeCommand(0x91);
  writeData(x);
  writeCommand(0x92);
  writeData(x >> 8);

  /* Set Y */
  writeCommand(0x93);
  writeData(y);
  writeCommand(0x94);
  writeData(y >> 8);

  /* Set X1 */
  writeCommand(0x95);
  writeData(w);
  writeCommand(0x96);
  writeData((w) >> 8);

  /* Set Y1 */
  writeCommand(0x97);
  writeData(h);
  writeCommand(0x98);
  writeData((h) >> 8);

  /* Set Color */
  writeCommand(0x63);
  writeData((color & 0xf800) >> 11);
  writeCommand(0x64);
  writeData((color & 0x07e0) >> 5);
  writeCommand(0x65);
  writeData((color & 0x001f));

  /* Draw! */
  writeCommand(RA8875_DCR);
  if (filled) {
    writeData(0xB0);
  } else {
    writeData(0x90);
  }

  /* Wait for the command to finish */
  waitPoll(RA8875_DCR, RA8875_DCR_LINESQUTRI_STATUS);
}
int16_t RA8875::applyRotationX(int16_t x) 
{
  switch (_rotation) {
  case 2:
    x = _width - 1 - x;
    break;
  }

  return x;
}

int16_t RA8875::applyRotationY(int16_t y) 
{
  switch (_rotation) {
  case 2:
    y = _height - 1 - y;
    break;
  }

  return y + _voffset;
}
bool RA8875::waitPoll(uint8_t regname, uint8_t waitflag) 
{
  /* Wait for the command to finish */
  while (1)
  {
    uint8_t temp = readReg(regname);
    if (!(temp & waitflag))
      return true;
  }
  return false; // MEMEFIX: yeah i know, unreached! - add timeout?
}
void RA8875::LCD_Reset() 
{
   digitalWrite(PIN_RST, LOW);
   delay(100);
   digitalWrite(PIN_RST, HIGH);
   delay(100);
}
void RA8875::Chk_Busy()
{
  uint8_t temp;   
  do
  {
    temp = readStatu();
  }
  while(( temp & 0x80 )== 0x80);       
}
void RA8875::Chk_BTE_Busy()
{
  uint8_t temp;   
  do
  {
    temp = readData();
  }
  while(( temp & 0x40 ) == 0x40);       
}
void RA8875::Chk_DMA_Busy()
{
  uint8_t temp;   
  do
  {
    writeData(0xBF);
    temp = readData();
  }
  while(( temp & 0x01 )== 0x01);       
}
void RA8875::PLL_init(void)
{
    writeReg(RA8875_PLLC1, RA8875_PLLC1_PLLDIV1 + 11);
    delay(1);
    writeReg(RA8875_PLLC2, RA8875_PLLC2_DIV4);
    delay(1);
}  

void RA8875::writeCommand(uint8_t cmd) 
{
  digitalWrite(PIN_CS, LOW);
  spi_begin();

  SPI.transfer(RA8875_CMDWRITE);
  SPI.transfer(cmd);
  spi_end();

  digitalWrite(PIN_CS, HIGH);
}
void RA8875::writeData(uint8_t Data)
{
  digitalWrite(PIN_CS, LOW);
  spi_begin();

  SPI.transfer(RA8875_DATAWRITE);
  SPI.transfer(Data);
  spi_end();

  digitalWrite(PIN_CS, HIGH);
}
uint8_t RA8875::readData(void)
{
  digitalWrite(PIN_CS, LOW);
  spi_begin();

  SPI.transfer(RA8875_DATAREAD);
  uint8_t x = SPI.transfer(0x0);
  spi_end();
  
  digitalWrite(PIN_CS, HIGH);
  mySerial -> print("SPI data read : 0x");
  mySerial -> println(x ,HEX);
  return x;
}  
uint8_t RA8875::readStatu(void)
{
  digitalWrite(PIN_CS, LOW);
  spi_begin();

  SPI.transfer(RA8875_CMDREAD);
  uint8_t x = SPI.transfer(0x0);
  spi_end();
  
  digitalWrite(PIN_CS, HIGH);
  mySerial -> print("SPI statu read : 0x");
  mySerial -> println(x ,HEX);
  return x;
}

void RA8875::writeReg(uint8_t reg, uint8_t val)
{
  writeCommand(reg);
  writeData(val);
}
uint8_t RA8875::readReg(uint8_t reg) 
{
  uint8_t x;
  writeCommand(reg);
  x = readData();
  
  return x;
}
void RA8875::spi_begin()
{
   SPI.beginTransaction(SPISettings(500000, MSBFIRST, SPI_MODE0));
}
void RA8875::spi_end()
{
   SPI.endTransaction();
}
