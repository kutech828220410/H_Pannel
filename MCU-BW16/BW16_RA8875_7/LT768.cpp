/********************* COPYRIGHT  ***********************
* File Name       : LT768.c
* Author          : Levetop Electronics
* Version         : V1.0
* Date            : 2017-8-25
* Description     : 操作LT768的寄存器函数
                    具体操作哪个寄存器请看LT768.h文件
*********************************************************/

#include "LT768.h"
#include "Arduino.h"
#include <SPI.h>
#include <SoftwareSerial.h>
void spi_begin()
{
   SPI.beginTransaction(SPISettings(500000, MSBFIRST, SPI_MODE0));
}
void spi_end()
{
   SPI.endTransaction();
}
uint8_t LCD_DataRead(void)
{
  digitalWrite(LT768_PIN_CS, LOW);
  spi_begin();

  SPI.transfer(LT768_DATAREAD);
  uint8_t x = SPI.transfer(0x0);
  spi_end();
  
  digitalWrite(LT768_PIN_CS, HIGH);
  LT768_Serial -> print("SPI data read : 0x");
  LT768_Serial -> println(x ,HEX);
  return x;
}  
uint8_t LCD_StatusRead(void)
{
  digitalWrite(LT768_PIN_CS, LOW);
  spi_begin();

  SPI.transfer(LT768_CMDREAD);
  uint8_t x = SPI.transfer(0x0);
  spi_end();
  
  digitalWrite(LT768_PIN_CS, HIGH);
  LT768_Serial -> print("SPI statu read : 0x");
  LT768_Serial -> println(x ,HEX);
  return x;
}
void LCD_CmdWrite(uint8_t cmd) 
{
  digitalWrite(LT768_PIN_CS, LOW);
  spi_begin();

  SPI.transfer(LT768_CMDWRITE);
  SPI.transfer(cmd);
  spi_end();

  digitalWrite(LT768_PIN_CS, HIGH);
}
void LCD_DataWrite(uint8_t Data)
{
  digitalWrite(LT768_PIN_CS, LOW);
  spi_begin();

  SPI.transfer(LT768_DATAWRITE);
  SPI.transfer(Data);
  spi_end();

  digitalWrite(LT768_PIN_CS, HIGH);
}
//==============================================================================
void LCD_RegisterWrite(unsigned char Cmd,unsigned char Data)
{
  LCD_CmdWrite(Cmd);
  LCD_DataWrite(Data);
}  
//---------------------//
unsigned char LCD_RegisterRead(unsigned char Cmd)
{
  unsigned char temp;
  
  LCD_CmdWrite(Cmd);
  temp=LCD_DataRead();
  return temp;
}

/******************************************************************************/
/*Sub program area                                  */
/******************************************************************************/
//==============================================================================
void Check_Mem_WR_FIFO_not_Full(void)
{
/*  0: Memory Write FIFO is not full.
  1: Memory Write FIFO is full.   */
  do
  {
    
  }
  while( LCD_StatusRead()&0x80 );
}

void Check_2D_Busy(void)
{
  do
  {
    
  }
  while( LCD_StatusRead()&0x08 );
}
void Check_SDRAM_Ready(void)
{
/*  0: SDRAM is not ready for access
  1: SDRAM is ready for access    */  
  unsigned char temp;   
  do
  {
    temp=LCD_StatusRead();
  }
  while( (temp&0x04) == 0x00 );
}

void Check_Busy_Draw(void)
{
  unsigned char temp;
  do
  {
    temp=LCD_StatusRead();
  }
  while(temp&0x08);

}

//[00h]=========================================================================

void LT768_SW_Reset(void)//IC Reset??
{
  unsigned char temp;

  LCD_CmdWrite(0x00);
  temp = LCD_DataRead();
  temp |= 0x01;
  LCD_DataWrite(temp);

    do
    {
      temp = LCD_DataRead();
    }
    while( temp&0x01 );
}

//[01h][01h][01h][01h][01h][01h][01h][01h][01h][01h][01h][01h][01h][01h][01h][01h]

void TFT_16bit(void)
{
/*  00b: 24-bits output.
    01b: 18-bits output, unused pins are set as GPIO.
    10b: 16-bits output, unused pins are set as GPIO.
    11b: LVDS, all 24-bits unused output pins are set as GPIO.*/
  unsigned char temp;
  LCD_CmdWrite(0x01);
  temp = LCD_DataRead();
  temp |= cSetb4;
    temp &= cClrb3;
  LCD_DataWrite(temp);  
}


void Enable_SFlash_SPI(void)
{
/*  Serial Flash SPI Interface Enable/Disable
    0: Disable
    1: Enable*/
  unsigned char temp;
  LCD_CmdWrite(0x01);
  temp = LCD_DataRead();
  temp |= cSetb1;
  LCD_DataWrite(temp);     
}

void Disable_SFlash_SPI(void)
{
/*  Serial Flash SPI Interface Enable/Disable
    0: Disable
    1: Enable*/
  unsigned char temp;
  LCD_CmdWrite(0x01);
  temp = LCD_DataRead();
  temp &= cClrb1;
  LCD_DataWrite(temp);     
}
void Host_Bus_16bit(void)
{
/*  Parallel Host Data Bus Width Selection
    0: 8-bit Parallel Host Data Bus.
    1: 16-bit Parallel Host Data Bus.*/
  unsigned char temp;
  LCD_CmdWrite(0x01);
  temp = LCD_DataRead();
  temp |= cSetb0;
  LCD_DataWrite(temp);
}

//[02h][02h][02h][02h][02h][02h][02h][02h][02h][02h][02h][02h][02h][02h][02h][02h]

void RGB_16b_16bpp(void)
{
  unsigned char temp;
  LCD_CmdWrite(0x02);
  temp = LCD_DataRead();
  temp &= cClrb7;
  temp |= cSetb6;
  LCD_DataWrite(temp);
}

void MemWrite_Left_Right_Top_Down(void)
{
  unsigned char temp;
  LCD_CmdWrite(0x02);
  temp = LCD_DataRead();
  temp &= cClrb2;
  temp &= cClrb1;
  LCD_DataWrite(temp);
}
//[03h][03h][03h][03h][03h][03h][03h][03h][03h][03h][03h][03h][03h][03h][03h][03h]
void Graphic_Mode(void)
{
  unsigned char temp;
  LCD_CmdWrite(0x03);
  temp = LCD_DataRead();
    temp &= cClrb2;
  LCD_DataWrite(temp);
}
void Text_Mode(void)
{
  unsigned char temp;
  LCD_CmdWrite(0x03);
  temp = LCD_DataRead();
    temp |= cSetb2;
  LCD_DataWrite(temp);
}
void Memory_Select_SDRAM(void)
{
  unsigned char temp;
  LCD_CmdWrite(0x03);
  temp = LCD_DataRead();
    temp &= cClrb1;
    temp &= cClrb0; // B
  LCD_DataWrite(temp);
}


//[05h]=========================================================================
//[06h]=========================================================================
//[07h]=========================================================================
//[08h]=========================================================================
//[09h]=========================================================================
//[0Ah]=========================================================================
//[0Bh]=========================================================================
//[0Ch]=========================================================================
//[0Eh]=========================================================================

void Select_Main_Window_16bpp(void)
{
/*
Main Window Color Depth Setting
00b: 8-bpp generic TFT, i.e. 256 colors.
01b: 16-bpp generic TFT, i.e. 65K colors.
1xb: 24-bpp generic TFT, i.e. 1.67M colors.
*/
  unsigned char temp;
  LCD_CmdWrite(0x10);
  temp = LCD_DataRead();
    temp &= cClrb3;
    temp |= cSetb2;
  LCD_DataWrite(temp);
}


//[12h]=========================================================================

void PCLK_Falling(void)
{
/*
PCLK Inversion
0: PDAT, DE, HSYNC etc. Drive(/ change) at PCLK falling edge.
1: PDAT, DE, HSYNC etc. Drive(/ change) at PCLK rising edge.
*/
  unsigned char temp;
  LCD_CmdWrite(0x12);
  temp = LCD_DataRead();
    temp |= cSetb7;
  LCD_DataWrite(temp);
}
void Display_ON(void)
{
/*  
Display ON/OFF
0b: Display Off.
1b: Display On.
*/
  unsigned char temp;
  
  LCD_CmdWrite(0x12);
  temp = LCD_DataRead();
  temp |= cSetb6;
  LCD_DataWrite(temp);
}

void VSCAN_T_to_B(void)
{
/*  
Vertical Scan direction
0 : From Top to Bottom
1 : From bottom to Top
PIP window will be disabled when VDIR set as 1.
*/
  unsigned char temp;
  
  LCD_CmdWrite(0x12);
  temp = LCD_DataRead();
  temp &= cClrb3;
  LCD_DataWrite(temp);
}
void PDATA_Set_RGB(void)
{
/*  
parallel PDATA[23:0] Output Sequence
000b : RGB.
001b : RBG.
010b : GRB.
011b : GBR.
100b : BRG.
101b : BGR.
*/
  unsigned char temp;
  
  LCD_CmdWrite(0x12);
  temp = LCD_DataRead();
    temp &=0xf8;
  LCD_DataWrite(temp);
}

////[13h]=========================================================================
void HSYNC_Low_Active(void)
{
/*  
HSYNC Polarity
0 : Low active.
1 : High active.
*/
  unsigned char temp;
  
  LCD_CmdWrite(0x13);
  temp = LCD_DataRead();
  temp &= cClrb7;
  LCD_DataWrite(temp);
}

void VSYNC_Low_Active(void)
{
/*  
VSYNC Polarity
0 : Low active.
1 : High active.
*/
  unsigned char temp;
  
  LCD_CmdWrite(0x13);
  temp = LCD_DataRead();
  temp &= cClrb6; 
  LCD_DataWrite(temp);
}

void DE_High_Active(void)
{
/*  
DE Polarity
0 : High active.
1 : Low active.
*/
  unsigned char temp;
  
  LCD_CmdWrite(0x13);
  temp = LCD_DataRead();
  temp &= cClrb5;
  LCD_DataWrite(temp);
}

//[14h][15h][1Ah][1Bh]=========================================================================

void LCD_HorizontalWidth_VerticalHeight(unsigned short WX,unsigned short HY)
{
/*
[14h] Horizontal Display Width Setting Bit[7:0]
[15h] Horizontal Display Width Fine Tuning (HDWFT) [3:0]
The register specifies the LCD panel horizontal display width in
the unit of 8 pixels resolution.
Horizontal display width(pixels) = (HDWR + 1) * 8 + HDWFTR

[1Ah] Vertical Display Height Bit[7:0]
Vertical Display Height(Line) = VDHR + 1
[1Bh] Vertical Display Height Bit[10:8]
Vertical Display Height(Line) = VDHR + 1
*/
  unsigned char temp;
  if(WX<8)
    {
  LCD_CmdWrite(0x14);
  LCD_DataWrite(0x00);
    
  LCD_CmdWrite(0x15);
  LCD_DataWrite(WX);
    
    temp=HY-1;
  LCD_CmdWrite(0x1A);
  LCD_DataWrite(temp);
      
  temp=(HY-1)>>8;
  LCD_CmdWrite(0x1B);
  LCD_DataWrite(temp);
  }
  else
  {
    temp=(WX/8)-1;
  LCD_CmdWrite(0x14);
  LCD_DataWrite(temp);
    
    temp=WX%8;
  LCD_CmdWrite(0x15);
  LCD_DataWrite(temp);
    
    temp=HY-1;
  LCD_CmdWrite(0x1A);
  LCD_DataWrite(temp);
      
  temp=(HY-1)>>8;
  LCD_CmdWrite(0x1B);
  LCD_DataWrite(temp);
  }
}
////[16h][17h]=========================================================================
void LCD_Horizontal_Non_Display(unsigned short WX)
{
/*
[16h] Horizontal Non-Display Period(HNDR) Bit[4:0]
This register specifies the horizontal non-display period. Also
called back porch.
Horizontal non-display period(pixels) = (HNDR + 1) * 8 + HNDFTR

[17h] Horizontal Non-Display Period Fine Tuning(HNDFT) [3:0]
This register specifies the fine tuning for horizontal non-display
period; it is used to support the SYNC mode panel. Each level of
this modulation is 1-pixel.
Horizontal non-display period(pixels) = (HNDR + 1) * 8 + HNDFTR
*/
  unsigned char temp;
  if(WX<8)
  {
  LCD_CmdWrite(0x16);
  LCD_DataWrite(0x00);
    
  LCD_CmdWrite(0x17);
  LCD_DataWrite(WX);
  }
  else
  {
    temp=(WX/8)-1;
  LCD_CmdWrite(0x16);
  LCD_DataWrite(temp);
    
    temp=WX%8;
  LCD_CmdWrite(0x17);
  LCD_DataWrite(temp);
  } 
}
//[18h]=========================================================================
void LCD_HSYNC_Start_Position(unsigned short WX)
{
/*
[18h] HSYNC Start Position[4:0]
The starting position from the end of display area to the
beginning of HSYNC. Each level of this modulation is 8-pixel.
Also called front porch.
HSYNC Start Position(pixels) = (HSTR + 1)x8
*/
  unsigned char temp;
  if(WX<8)
  {
  LCD_CmdWrite(0x18);
  LCD_DataWrite(0x00);
  }
  else
  {
    temp=(WX/8)-1;
  LCD_CmdWrite(0x18);
  LCD_DataWrite(temp);  
  }
}
//[19h]=========================================================================
void LCD_HSYNC_Pulse_Width(unsigned short WX)
{
/*
[19h] HSYNC Pulse Width(HPW) [4:0]
The period width of HSYNC.
HSYNC Pulse Width(pixels) = (HPW + 1)x8
*/
  unsigned char temp;
  if(WX<8)
  {
  LCD_CmdWrite(0x19);
  LCD_DataWrite(0x00);
  }
  else
  {
    temp=(WX/8)-1;
  LCD_CmdWrite(0x19);
  LCD_DataWrite(temp);  
  }
}
//[1Ch][1Dh]=========================================================================
void LCD_Vertical_Non_Display(unsigned short HY)
{
/*
[1Ch] Vertical Non-Display Period Bit[7:0]
Vertical Non-Display Period(Line) = (VNDR + 1)

[1Dh] Vertical Non-Display Period Bit[9:8]
Vertical Non-Display Period(Line) = (VNDR + 1)
*/
  unsigned char temp;
    temp=HY-1;
  LCD_CmdWrite(0x1C);
  LCD_DataWrite(temp);

  LCD_CmdWrite(0x1D);
  LCD_DataWrite(temp>>8);
}
//[1Eh]=========================================================================
void LCD_VSYNC_Start_Position(unsigned short HY)
{
/*
[1Eh] VSYNC Start Position[7:0]
The starting position from the end of display area to the beginning of VSYNC.
VSYNC Start Position(Line) = (VSTR + 1)
*/
  unsigned char temp;
    temp=HY-1;
  LCD_CmdWrite(0x1E);
  LCD_DataWrite(temp);
}
//[1Fh]=========================================================================
void LCD_VSYNC_Pulse_Width(unsigned short HY)
{
/*
[1Fh] VSYNC Pulse Width[5:0]
The pulse width of VSYNC in lines.
VSYNC Pulse Width(Line) = (VPWR + 1)
*/
  unsigned char temp;
    temp=HY-1;
  LCD_CmdWrite(0x1F);
  LCD_DataWrite(temp);
}
//[20h][21h][22h][23h]=========================================================================
void Main_Image_Start_Address(unsigned long Addr) 
{
/*
[20h] Main Image Start Address[7:2]
[21h] Main Image Start Address[15:8]
[22h] Main Image Start Address [23:16]
[23h] Main Image Start Address [31:24]
*/
  LCD_RegisterWrite(0x20,Addr);
  LCD_RegisterWrite(0x21,Addr>>8);
  LCD_RegisterWrite(0x22,Addr>>16);
  LCD_RegisterWrite(0x23,Addr>>24);
}
//[24h][25h]=========================================================================
void Main_Image_Width(unsigned short WX)  
{
/*
[24h] Main Image Width [7:0]
[25h] Main Image Width [12:8]
Unit: Pixel.
It must be divisible by 4. MIW Bit [1:0] tie to ¨0〃 internally.
The value is physical pixel number. Maximum value is 8188 pixels
*/
  LCD_RegisterWrite(0x24,WX);
  LCD_RegisterWrite(0x25,WX>>8);
}
//[26h][27h][28h][29h]=========================================================================
void Main_Window_Start_XY(unsigned short WX,unsigned short HY)  
{
/*
[26h] Main Window Upper-Left corner X-coordination [7:0]
[27h] Main Window Upper-Left corner X-coordination [12:8]
Reference Main Image coordination.
Unit: Pixel
It must be divisible by 4. MWULX Bit [1:0] tie to ¨0〃 internally.
X-axis coordination plus Horizontal display width cannot large than 8188.

[28h] Main Window Upper-Left corner Y-coordination [7:0]
[29h] Main Window Upper-Left corner Y-coordination [12:8]
Reference Main Image coordination.
Unit: Pixel
Range is between 0 and 8191.
*/
  LCD_RegisterWrite(0x26,WX);
  LCD_RegisterWrite(0x27,WX>>8);

  LCD_RegisterWrite(0x28,HY);
  LCD_RegisterWrite(0x29,HY>>8);
}
//[2Ah][2Bh][2Ch][2Dh]=========================================================================
//[2Eh][2Fh][30h][31h]=========================================================================
//[32h][33h]=========================================================================
//[34h][35h][36h][37h]=========================================================================
//[38h][39h][3Ah][3Bh]=========================================================================
//[44h]=========================================================================
//[50h][51h][52h][53h]=========================================================================
void Canvas_Image_Start_address(unsigned long Addr) 
{
/*
[50h] Start address of Canvas [7:0]
[51h] Start address of Canvas [15:8]
[52h] Start address of Canvas [23:16]
[53h] Start address of Canvas [31:24]
*/
  LCD_RegisterWrite(0x50,Addr);
  LCD_RegisterWrite(0x51,Addr>>8);
  LCD_RegisterWrite(0x52,Addr>>16);
  LCD_RegisterWrite(0x53,Addr>>24);
}
//[54h][55h]=========================================================================
void Canvas_image_width(unsigned short WX)  
{
/*
[54h] Canvas image width [7:2]
[55h] Canvas image width [12:8]
*/
  LCD_RegisterWrite(0x54,WX);
  LCD_RegisterWrite(0x55,WX>>8);
}
//[56h][57h][58h][59h]=========================================================================
void Active_Window_XY(unsigned short WX,unsigned short HY)  
{
/*
[56h] Active Window Upper-Left corner X-coordination [7:0]
[57h] Active Window Upper-Left corner X-coordination [12:8]
[58h] Active Window Upper-Left corner Y-coordination [7:0]
[59h] Active Window Upper-Left corner Y-coordination [12:8]
*/
  LCD_RegisterWrite(0x56,WX);
  LCD_RegisterWrite(0x57,WX>>8);
  
  LCD_RegisterWrite(0x58,HY);
  LCD_RegisterWrite(0x59,HY>>8);
}
////[5Ah][5Bh][5Ch][5Dh]=========================================================================
void Active_Window_WH(unsigned short WX,unsigned short HY)  
{
/*
[5Ah] Width of Active Window [7:0]
[5Bh] Width of Active Window [12:8]
[5Ch] Height of Active Window [7:0]
[5Dh] Height of Active Window [12:8]
*/
  LCD_RegisterWrite(0x5A,WX);
  LCD_RegisterWrite(0x5B,WX>>8);
 
  LCD_RegisterWrite(0x5C,HY);
  LCD_RegisterWrite(0x5D,HY>>8);
}
//5Eh]=========================================================================
void Memory_XY_Mode(void) 
{
/*
Canvas addressing mode
0: Block mode (X-Y coordination addressing)
1: linear mode
*/
  unsigned char temp;

  LCD_CmdWrite(0x5E);
  temp = LCD_DataRead();
  temp &= cClrb2;
  LCD_DataWrite(temp);
}
void Memory_16bpp_Mode(void)  
{
/*
Canvas imageˇs color depth & memory R/W data width
In Block Mode:
00: 8bpp
01: 16bpp
1x: 24bpp
In Linear Mode:
X0: 8-bits memory data read/write.
X1: 16-bits memory data read/write
*/
  unsigned char temp;

  LCD_CmdWrite(0x5E);
  temp = LCD_DataRead();
  temp &= cClrb1;
  temp |= cSetb0;
  LCD_DataWrite(temp);
}

//[5Fh][60h][61h][62h]=========================================================================
void Goto_Pixel_XY(unsigned short WX,unsigned short HY) 
{
/*
[Write]: Set Graphic Read/Write position
[Read]: Current Graphic Read/Write position
Read back is Read position or Write position depends on
REG[5Eh] bit3, Select to read back Graphic Read/Write position.
When DPRAM Linear mode:Graphic Read/Write Position [31:24][23:16][15:8][7:0]
When DPRAM Active window mode:Graphic Read/Write 
Horizontal Position [12:8][7:0], 
Vertical Position [12:8][7:0].
Reference Canvas image coordination. Unit: Pixel
*/
  LCD_RegisterWrite(0x5F,WX);
  LCD_RegisterWrite(0x60,WX>>8);
  
  LCD_RegisterWrite(0x61,HY);
  LCD_RegisterWrite(0x62,HY>>8);
}
//[63h][64h][65h][66h]=========================================================================
//[67h]=========================================================================
/*
[bit7]Draw Line / Triangle Start Signal
Write Function
0 : Stop the drawing function.
1 : Start the drawing function.
Read Function
0 : Drawing function complete.
1 : Drawing function is processing.
[bit5]Fill function for Triangle Signal
0 : Non fill.
1 : Fill.
[bit1]Draw Triangle or Line Select Signal
0 : Draw Line
1 : Draw Triangle
*/
//[68h][69h][6Ah][6Bh]=========================================================================
//线起点

void Square_Start_XY(unsigned short WX,unsigned short HY)
{
/*
[68h] Draw Line/Square/Triangle Start X-coordination [7:0]
[69h] Draw Line/Square/Triangle Start X-coordination [12:8]
[6Ah] Draw Line/Square/Triangle Start Y-coordination [7:0]
[6Bh] Draw Line/Square/Triangle Start Y-coordination [12:8]
*/
  LCD_CmdWrite(0x68);
  LCD_DataWrite(WX);

  LCD_CmdWrite(0x69);
  LCD_DataWrite(WX>>8);

  LCD_CmdWrite(0x6A);
  LCD_DataWrite(HY);

  LCD_CmdWrite(0x6B);
  LCD_DataWrite(HY>>8);
}
void Square_End_XY(unsigned short WX,unsigned short HY)
{
/*
[6Ch] Draw Line/Square/Triangle End X-coordination [7:0]
[6Dh] Draw Line/Square/Triangle End X-coordination [12:8]
[6Eh] Draw Line/Square/Triangle End Y-coordination [7:0]
[6Fh] Draw Line/Square/Triangle End Y-coordination [12:8]
*/
  LCD_CmdWrite(0x6C);
  LCD_DataWrite(WX);

  LCD_CmdWrite(0x6D);
  LCD_DataWrite(WX>>8);

  LCD_CmdWrite(0x6E);
  LCD_DataWrite(HY);

  LCD_CmdWrite(0x6F);
  LCD_DataWrite(HY>>8);
}
//[76h]=========================================================================
/*
[bit7]
Draw Circle / Ellipse / Square /Circle Square Start Signal 
Write Function
0 : Stop the drawing function.
1 : Start the drawing function.
Read Function
0 : Drawing function complete.
1 : Drawing function is processing.
[bit6]
Fill the Circle / Ellipse / Square / Circle Square Signal
0 : Non fill.
1 : fill.
[bit5 bit4]
Draw Circle / Ellipse / Square / Ellipse Curve / Circle Square Select
00 : Draw Circle / Ellipse
01 : Draw Circle / Ellipse Curve
10 : Draw Square.
11 : Draw Circle Square.
[bit1 bit0]
Draw Circle / Ellipse Curve Part Select
00 : 
01 : 
10 : 
11 : 
*/
void Start_Square_Fill(void)
{
  LCD_CmdWrite(0x76);
  LCD_DataWrite(0xE0);//B1110_XXXX
  Check_Busy_Draw();
}

//[77h]~[7Eh]=========================================================================

//[84h]=========================================================================
void Set_PWM_Prescaler_1_to_256(unsigned short WX)
{
/*
PWM Prescaler Register
These 8 bits determine prescaler value for Timer 0 and 1.
Time base is ¨Core_Freq / (Prescaler + 1)〃
*/
  WX=WX-1;
  LCD_CmdWrite(0x84);
  LCD_DataWrite(WX);
}
//[85h]=========================================================================
void Select_PWM1_Clock_Divided_By_1(void)
{
/*
Select MUX input for PWM Timer 1.
00 = 1; 01 = 1/2; 10 = 1/4 ; 11 = 1/8;
*/
  unsigned char temp;
  
  LCD_CmdWrite(0x85);
  temp = LCD_DataRead();
  temp &= cClrb7;
  temp &= cClrb6;
  LCD_DataWrite(temp);
}
void Select_PWM1_Clock_Divided_By_2(void)
{
/*
Select MUX input for PWM Timer 1.
00 = 1; 01 = 1/2; 10 = 1/4 ; 11 = 1/8;
*/
  unsigned char temp;
  
  LCD_CmdWrite(0x85);
  temp = LCD_DataRead();
  temp &= cClrb7;
  temp |= cSetb6;
  LCD_DataWrite(temp);
}
void Select_PWM1_Clock_Divided_By_4(void)
{
/*
Select MUX input for PWM Timer 1.
00 = 1; 01 = 1/2; 10 = 1/4 ; 11 = 1/8;
*/
  unsigned char temp;
  
  LCD_CmdWrite(0x85);
  temp = LCD_DataRead();
  temp |= cSetb7;
  temp &= cClrb6;
  LCD_DataWrite(temp);
}
void Select_PWM1_Clock_Divided_By_8(void)
{
/*
Select MUX input for PWM Timer 1.
00 = 1; 01 = 1/2; 10 = 1/4 ; 11 = 1/8;
*/
  unsigned char temp;
  
  LCD_CmdWrite(0x85);
  temp = LCD_DataRead();
  temp |= cSetb7;
  temp |= cSetb6;
  LCD_DataWrite(temp);
}
void Select_PWM0_Clock_Divided_By_1(void)
{
/*
Select MUX input for PWM Timer 0.
00 = 1; 01 = 1/2; 10 = 1/4 ; 11 = 1/8;
*/
  unsigned char temp;
  
  LCD_CmdWrite(0x85);
  temp = LCD_DataRead();
  temp &= cClrb5;
  temp &= cClrb4;
  LCD_DataWrite(temp);
}
void Select_PWM0_Clock_Divided_By_2(void)
{
/*
Select MUX input for PWM Timer 0.
00 = 1; 01 = 1/2; 10 = 1/4 ; 11 = 1/8;
*/
  unsigned char temp;
  
  LCD_CmdWrite(0x85);
  temp = LCD_DataRead();
  temp &= cClrb5;
  temp |= cSetb4;
  LCD_DataWrite(temp);
}
void Select_PWM0_Clock_Divided_By_4(void)
{
/*
Select MUX input for PWM Timer 0.
00 = 1; 01 = 1/2; 10 = 1/4 ; 11 = 1/8;
*/
  unsigned char temp;
  
  LCD_CmdWrite(0x85);
  temp = LCD_DataRead();
  temp |= cSetb5;
  temp &= cClrb4;
  LCD_DataWrite(temp);
}
void Select_PWM0_Clock_Divided_By_8(void)
{
/*
Select MUX input for PWM Timer 0.
00 = 1; 01 = 1/2; 10 = 1/4 ; 11 = 1/8;
*/
  unsigned char temp;
  
  LCD_CmdWrite(0x85);
  temp = LCD_DataRead();
  temp |= cSetb5;
  temp |= cSetb4;
  LCD_DataWrite(temp);
}
//[85h].[bit3][bit2]=========================================================================
/*
XPWM[1] pin function control
0X: XPWM[1] output system error flag (REG[00h] bit[1:0], Scan bandwidth insufficient + Memory access out of range)
10: XPWM[1] enabled and controlled by PWM timer 1
11: XPWM[1] output oscillator clock
//If XTEST[0] set high, then XPWM[1] will become panel scan clock input.
*/
void Select_PWM1_is_ErrorFlag(void)
{
  unsigned char temp;
  
  LCD_CmdWrite(0x85);
  temp = LCD_DataRead();
  temp &= cClrb3;
  LCD_DataWrite(temp);
}
void Select_PWM1(void)
{
  unsigned char temp;
  
  LCD_CmdWrite(0x85);
  temp = LCD_DataRead();
  temp |= cSetb3;
  temp &= cClrb2;
  LCD_DataWrite(temp);
}
//[85h].[bit1][bit0]=========================================================================
/*
XPWM[0] pin function control
0X: XPWM[0] becomes GPIO-C[7]
10: XPWM[0] enabled and controlled by PWM timer 0
11: XPWM[0] output core clock
*/

void Select_PWM0(void)
{
  unsigned char temp;
  
  LCD_CmdWrite(0x85);
  temp = LCD_DataRead();
  temp |= cSetb1;
  temp &= cClrb0;
  LCD_DataWrite(temp);
}
//[86h]=========================================================================
//[86h]PWM1
void Start_PWM1(void)
{
/*
PWM Timer 1 start/stop
Determine start/stop for Timer 1. 
0 = Stop 
1 = Start for Timer 1
*/
  unsigned char temp;
  LCD_CmdWrite(0x86);
  temp = LCD_DataRead();
  temp |= cSetb4;
  LCD_DataWrite(temp);
}
void Stop_PWM1(void)
{
/*
PWM Timer 1 start/stop
Determine start/stop for Timer 1. 
0 = Stop 
1 = Start for Timer 1
*/
  unsigned char temp;
  LCD_CmdWrite(0x86);
  temp = LCD_DataRead();
  temp &= cClrb4;
  LCD_DataWrite(temp);
}
//[86h]PWM0
void Start_PWM0(void)
{
/*
PWM Timer 0 start/stop
Determine start/stop for Timer 0. 
0 = Stop 
1 = Start for Timer 0
*/
  unsigned char temp;
  LCD_CmdWrite(0x86);
  temp = LCD_DataRead();
  temp |= cSetb0;
  LCD_DataWrite(temp);
}
void Stop_PWM0(void)
{
/*
PWM Timer 0 start/stop
Determine start/stop for Timer 0. 
0 = Stop 
1 = Start for Timer 0
*/
  unsigned char temp;
  LCD_CmdWrite(0x86);
  temp = LCD_DataRead();
  temp &= cClrb0;
  LCD_DataWrite(temp);
}
//[87h]=========================================================================
//[88h][89h]=========================================================================
void Set_Timer0_Compare_Buffer(unsigned short WX)
{
/*
Timer 0 compare buffer register
Compare buffer register total has 16 bits.
When timer counter equal or less than compare buffer register will cause PWM out
high level if inv_on bit is off.
*/
  LCD_CmdWrite(0x88);
  LCD_DataWrite(WX);
  LCD_CmdWrite(0x89);
  LCD_DataWrite(WX>>8);
}
//[8Ah][8Bh]=========================================================================
void Set_Timer0_Count_Buffer(unsigned short WX)
{
/*
Timer 0 count buffer register
Count buffer register total has 16 bits.
When timer counter equal to 0 will cause PWM timer reload Count buffer register if reload_en bit set as enable.
It may read back timer counterˇs real time value when PWM timer start.
*/
  LCD_CmdWrite(0x8A);
  LCD_DataWrite(WX);
  LCD_CmdWrite(0x8B);
  LCD_DataWrite(WX>>8);
}
//[8Ch][8Dh]=========================================================================
void Set_Timer1_Compare_Buffer(unsigned short WX)
{
/*
Timer 0 compare buffer register
Compare buffer register total has 16 bits.
When timer counter equal or less than compare buffer register will cause PWM out
high level if inv_on bit is off.
*/
  LCD_CmdWrite(0x8C);
  LCD_DataWrite(WX);
  LCD_CmdWrite(0x8D);
  LCD_DataWrite(WX>>8);
}
//[8Eh][8Fh]=========================================================================
void Set_Timer1_Count_Buffer(unsigned short WX)
{
/*
Timer 0 count buffer register
Count buffer register total has 16 bits.
When timer counter equal to 0 will cause PWM timer reload Count buffer register if reload_en bit set as enable.
It may read back timer counterˇs real time value when PWM timer start.
*/
  LCD_CmdWrite(0x8E);
  LCD_DataWrite(WX);
  LCD_CmdWrite(0x8F);
  LCD_DataWrite(WX>>8);
}


//[B6h]=========================================================================
void Start_SFI_DMA(void)
{
  unsigned char temp;
  LCD_CmdWrite(0xB6);
  temp = LCD_DataRead();
    temp |= cSetb0;
  LCD_DataWrite(temp);
}

void Check_Busy_SFI_DMA(void)
{
  LCD_CmdWrite(0xB6);
  do
  {   
  }while((LCD_DataRead()&0x01)==0x01);
}


//[B7h]=========================================================================
void Select_SFI_0(void)
{
/*[bit7]
Serial Flash/ROM I/F # Select
0: Serial Flash/ROM 0 I/F is selected.
1: Serial Flash/ROM 1 I/F is selected.
*/
  unsigned char temp;
  LCD_CmdWrite(0xB7);
  temp = LCD_DataRead();
    temp &= cClrb7;
  LCD_DataWrite(temp);
}
void Select_SFI_1(void)
{
/*[bit7]
Serial Flash/ROM I/F # Select
0: Serial Flash/ROM 0 I/F is selected.
1: Serial Flash/ROM 1 I/F is selected.
*/
  unsigned char temp;
  LCD_CmdWrite(0xB7);
  temp = LCD_DataRead();
    temp |= cSetb7;
  LCD_DataWrite(temp);
}

void Select_SFI_DMA_Mode(void)
{
/*[bit6]
Serial Flash /ROM Access Mode
0: Font mode ? for external cgrom
1: DMA mode ? for cgram , pattern , bootstart image or osd
*/
  unsigned char temp;
  LCD_CmdWrite(0xB7);
  temp = LCD_DataRead();
    temp |= cSetb6;
  LCD_DataWrite(temp);
}

void Select_SFI_Dual_Mode0(void)
{
  unsigned char temp;
  LCD_CmdWrite(0xB7);
  temp = LCD_DataRead();
  temp &= 0xFC;
    temp |= cSetb1;
  LCD_DataWrite(temp);
}

//REG[BB] SPI Clock period (SPIDIV) 
void SPI_Clock_Period(unsigned char temp)
{
   LCD_CmdWrite(0xBB);
   LCD_DataWrite(temp);
} 

//[BCh][BDh][BEh][BFh]=========================================================================
void SFI_DMA_Source_Start_Address(unsigned long Addr)
{
/*
DMA Source START ADDRESS
This bits index serial flash address [7:0][15:8][23:16][31:24]
*/
  LCD_CmdWrite(0xBC);
  LCD_DataWrite(Addr);
  LCD_CmdWrite(0xBD);
  LCD_DataWrite(Addr>>8);
  LCD_CmdWrite(0xBE);
  LCD_DataWrite(Addr>>16);
  LCD_CmdWrite(0xBF);
  LCD_DataWrite(Addr>>24);
}
//[C0h][C1h][C2h][C3h]=========================================================================

//[C0h][C1h][C2h][C3h]=========================================================================
void SFI_DMA_Destination_Upper_Left_Corner(unsigned short WX,unsigned short HY)
{
/*
C0h
This register defines DMA Destination Window Upper-Left corner 
X-coordination [7:0] on Canvas area. 
When REG DMACR bit 1 = 1 (Block Mode) 
This register defines Destination address [7:2] in SDRAM. 
C1h
When REG DMACR bit 1 = 0 (Linear Mode) 
This register defines DMA Destination Window Upper-Left corner 
X-coordination [12:8] on Canvas area. 
When REG DMACR bit 1 = 1 (Block Mode) 
This register defines Destination address [15:8] in SDRAM.
C2h
When REG DMACR bit 1 = 0 (Linear Mode) 
This register defines DMA Destination Window Upper-Left corner
Y-coordination [7:0] on Canvas area. 
When REG DMACR bit 1 = 1 (Block Mode) 
This register defines Destination address [23:16] in SDRAM. 
C3h
When REG DMACR bit 1 = 0 (Linear Mode) 
This register defines DMA Destination Window Upper-Left corner 
Y-coordination [12:8] on Canvas area. 
When REG DMACR bit 1 = 1 (Block Mode) 
This register defines Destination address [31:24] in SDRAM. 
*/
 
  LCD_CmdWrite(0xC0);
  LCD_DataWrite(WX);
  LCD_CmdWrite(0xC1);
  LCD_DataWrite(WX>>8);
 
  LCD_CmdWrite(0xC2);
  LCD_DataWrite(HY);
  LCD_CmdWrite(0xC3);
  LCD_DataWrite(HY>>8);
}



//[C6h][C7h][C8h][C9h]=========================================================================

void SFI_DMA_Transfer_Width_Height(unsigned short WX,unsigned short HY)
{
/*
When REG DMACR bit 1 = 0 (Linear Mode)
DMA Transfer Number [7:0][15:8][23:16][31:24]

When REG DMACR bit 1 = 1 (Block Mode)
DMA Block Width [7:0][15:8]
DMA Block HIGH[7:0][15:8]
*/
  LCD_CmdWrite(0xC6);
  LCD_DataWrite(WX);
  LCD_CmdWrite(0xC7);
  LCD_DataWrite(WX>>8);

  LCD_CmdWrite(0xC8);
  LCD_DataWrite(HY);
  LCD_CmdWrite(0xC9);
  LCD_DataWrite(HY>>8);
}
//[CAh][CBh]=========================================================================
void SFI_DMA_Source_Width(unsigned short WX)
{
/*
DMA Source Picture Width [7:0][12:8]
Unit: pixel
*/
  LCD_CmdWrite(0xCA);
  LCD_DataWrite(WX);
  LCD_CmdWrite(0xCB);
  LCD_DataWrite(WX>>8);
} 
void Foreground_color_65k(unsigned short temp)
{
    LCD_CmdWrite(0xD2);
  LCD_DataWrite(temp>>8);
 
    LCD_CmdWrite(0xD3);
  LCD_DataWrite(temp>>3);
  
    LCD_CmdWrite(0xD4);
  LCD_DataWrite(temp<<3);
}




//复位LT768
void LT768_HW_Reset(void)
{  
  
}

//检查LT768系统
void System_Check_Temp(void)
{
  unsigned char i=0;
  unsigned char temp=0;
  unsigned char system_ok=0;
  do
  {
    if((LCD_StatusRead()&0x02)==0x00)    
    {
      delay(1);                  //若MCU 速度太快，必要r使用
      LCD_CmdWrite(0x01);
      delay(1);                  //若MCU 速度太快，必要r使用
      temp =LCD_DataRead();
      if((temp & 0x80)==0x80)       //检测CCR寄存器PLL是否准备好
      {
        system_ok=1;
        i=0;
      }
      else
      {
        delay(1); //若MCU 速度太快，必要r使用
        LCD_CmdWrite(0x01);
        delay(1); //若MCU 速度太快，必要r使用
        LCD_DataWrite(0x80);
      }
    }
    else
    {
      system_ok=0;
      i++;
    }
    if(system_ok==0 && i==5)
    {
      LT768_HW_Reset(); //note1
      i=0;
    }
  }while(system_ok==0);
}

void LT768_PLL_Initial(void) 
{    
  unsigned int  temp = 0,temp1 =0 ;
  
  unsigned short lpllOD_sclk, lpllOD_cclk, lpllOD_mclk;
  unsigned short lpllR_sclk, lpllR_cclk, lpllR_mclk;
  unsigned short lpllN_sclk, lpllN_cclk, lpllN_mclk;
  
  temp = (LCD_HBPD + LCD_HFPD + LCD_HSPW + LCD_XSIZE_TFT) * (LCD_VBPD + LCD_VFPD + LCD_VSPW+LCD_YSIZE_TFT) * 60;   
  
  temp1=(temp%1000000)/100000;
  if(temp1>=5) 
     temp = temp / 1000000 + 1;
  else temp = temp / 1000000;
  SCLK = temp;
  temp = temp * 3;
  MCLK = temp;
  CCLK = temp;
  
  if(CCLK > 100)  CCLK = 100;
  if(MCLK > 100)  MCLK = 100;
  if(SCLK > 65) SCLK = 65;

#if XI_4M   
  
  lpllOD_sclk = 3;
  lpllOD_cclk = 2;
  lpllOD_mclk = 2;
  lpllR_sclk  = 2;
  lpllR_cclk  = 2;
  lpllR_mclk  = 2;
  lpllN_mclk  = MCLK;      
  lpllN_cclk  = CCLK;    
  lpllN_sclk  = 2*SCLK; 
  
#endif

#if XI_8M   
  
  lpllOD_sclk = 3;
  lpllOD_cclk = 2;
  lpllOD_mclk = 2;
  lpllR_sclk  = 2;
  lpllR_cclk  = 4;
  lpllR_mclk  = 4;
  lpllN_mclk  = MCLK;      
  lpllN_cclk  = CCLK;    
  lpllN_sclk  = SCLK; 
  
#endif

#if XI_10M  
  
  lpllOD_sclk = 3;
  lpllOD_cclk = 2;
  lpllOD_mclk = 2;
  lpllR_sclk  = 5;
  lpllR_cclk  = 5;
  lpllR_mclk  = 5;
  lpllN_mclk  = MCLK;      
  lpllN_cclk  = CCLK;    
  lpllN_sclk  = 2*SCLK; 
  
#endif

#if XI_12M  
  
  lpllOD_sclk = 3;
  lpllOD_cclk = 2;
  lpllOD_mclk = 2;
  lpllR_sclk  = 3;
  lpllR_cclk  = 6;
  lpllR_mclk  = 6;
  lpllN_mclk  = MCLK;      
  lpllN_cclk  = CCLK;    
  lpllN_sclk  = SCLK; 
  
#endif    
      
  LCD_CmdWrite(0x05);
  LCD_DataWrite((lpllOD_sclk<<6) | (lpllR_sclk<<1) | ((lpllN_sclk>>8)&0x1));
  LCD_CmdWrite(0x07);
  LCD_DataWrite((lpllOD_mclk<<6) | (lpllR_mclk<<1) | ((lpllN_mclk>>8)&0x1));
  LCD_CmdWrite(0x09);
  LCD_DataWrite((lpllOD_cclk<<6) | (lpllR_cclk<<1) | ((lpllN_cclk>>8)&0x1));

  LCD_CmdWrite(0x06);
  LCD_DataWrite(lpllN_sclk);
  LCD_CmdWrite(0x08);
  LCD_DataWrite(lpllN_mclk);
  LCD_CmdWrite(0x0a);
  LCD_DataWrite(lpllN_cclk);
      
  LCD_CmdWrite(0x00);
  delay(1);
  LCD_DataWrite(0x80);

  delay(1);  //单PLL铆
}


void LT768_SDRAM_initail(unsigned char mclk)
{
  unsigned short sdram_itv;
  
  LCD_RegisterWrite(0xe0,0x29);      
  LCD_RegisterWrite(0xe1,0x03); //CAS:2=0x02?CAS:3=0x03
  sdram_itv = (64000000 / 8192) / (1000/mclk) ;
  sdram_itv-=2;

  LCD_RegisterWrite(0xe2,sdram_itv);
  LCD_RegisterWrite(0xe3,sdram_itv >>8);
  LCD_RegisterWrite(0xe4,0x01);
  Check_SDRAM_Ready();
  delay(1);
}


void Set_LCD_Panel(void)
{
  //**[01h]**//   
  TFT_16bit();  
  //TFT_18bit();
  //TFT_24bit(); 
  
  #if STM32_FSMC_8
  Host_Bus_8bit();    //主机总线8bit
  #else
  Host_Bus_16bit(); //主机总线16bit
  #endif
      
  //**[02h]**//
  RGB_16b_16bpp();
  //RGB_16b_24bpp_mode1();
  MemWrite_Left_Right_Top_Down(); 
  //MemWrite_Down_Top_Left_Right();
      
  //**[03h]**//
  Graphic_Mode();
  Memory_Select_SDRAM();
     
  PCLK_Falling();         //REG[12h]:下降沿 
  //PCLK_Rising();
  
  VSCAN_T_to_B();         //REG[12h]:从上到下
  //VSCAN_B_to_T();       //从下到上
  
  PDATA_Set_RGB();        //REG[12h]:Select RGB output
  //PDATA_Set_RBG();
  //PDATA_Set_GRB();
  //PDATA_Set_GBR();
  //PDATA_Set_BRG();
  //PDATA_Set_BGR();

  HSYNC_Low_Active();     //REG[13h]:     
  //HSYNC_High_Active();
  
  VSYNC_Low_Active();     //REG[13h]:     
  //VSYNC_High_Active();
  
  DE_High_Active();       //REG[13h]: 
  //DE_Low_Active();
 
  LCD_HorizontalWidth_VerticalHeight(LCD_XSIZE_TFT ,LCD_YSIZE_TFT); 
  LCD_Horizontal_Non_Display(LCD_HBPD);                             
  LCD_HSYNC_Start_Position(LCD_HFPD);                               
  LCD_HSYNC_Pulse_Width(LCD_HSPW);                                  
  LCD_Vertical_Non_Display(LCD_VBPD);                                 
  LCD_VSYNC_Start_Position(LCD_VFPD);                               
  LCD_VSYNC_Pulse_Width(LCD_VSPW);                                  

  Memory_XY_Mode(); //Block mode (X-Y coordination addressing);块模式
  Memory_16bpp_Mode();  
}

void LT768_initial(void)
{
  LT768_PLL_Initial();
  LT768_SDRAM_initail(MCLK);
  Set_LCD_Panel();
}


void LT768_Init(void)
{
  delay(100);                    //delay for LT768 power on
  LT768_HW_Reset();                 //LT768复位
  System_Check_Temp();              //检测复位是否成功
  delay(100);
  while(LCD_StatusRead()&0x02);     //Initial_Display_test  and  set SW2 pin2 = 1
  LT768_initial();
}


void LT768_DrawSquare_Fill
(
 unsigned short X1                // X1位置
,unsigned short Y1                // Y1位置
,unsigned short X2                // X2位置
,unsigned short Y2                // Y2位置
,unsigned long ForegroundColor    // 背景颜色
)
{
  Foreground_color_65k(ForegroundColor);
  Square_Start_XY(X1,Y1);
  Square_End_XY(X2,Y2);
  Start_Square_Fill();
  Check_2D_Busy();
}


void LT768_DMA_24bit_Block
(
 unsigned char SCS         // 选择外挂的SPI   : SCS：0       SCS：1
,unsigned char Clk         // SPI时钟分频参数 : SPI Clock = System Clock /{(Clk+1)*2}
,unsigned short X1         // 传输到内存X1的位置
,unsigned short Y1         // 传输到内存Y1的位置
,unsigned short X_W        // DMA传输数据的宽度
,unsigned short Y_H        // DMA传输数据的高度
,unsigned short P_W        // 图片的宽度
,unsigned long Addr        // Flash的地址
)
{  

  Enable_SFlash_SPI();                            // 使能SPI功能
  if(SCS == 0)  Select_SFI_0();                   // 选择外挂的SPI0
  if(SCS == 1)  Select_SFI_1();                   // 选择外挂的SPI1
 
                       
  Select_SFI_DMA_Mode();                          // 设置SPI的DMA模式
  SPI_Clock_Period(Clk);                          // 设置SPI的分频系数

  Goto_Pixel_XY(X1,Y1);                           // 在图形模式中设置内存的位置
  SFI_DMA_Destination_Upper_Left_Corner(X1,Y1);   // DMA传输的目的地（内存的位置）
  SFI_DMA_Transfer_Width_Height(X_W,Y_H);         // 设置块数据的宽度和高度
  SFI_DMA_Source_Width(P_W);                      // 设置源数据的宽度
  SFI_DMA_Source_Start_Address(Addr);             // 设置源数据在Flash的地址

  Start_SFI_DMA();                                // 开始DMA传输
  Check_Busy_SFI_DMA();                           // 检测DMA是否传输完成
}


//----------------------------------------------------------------------------------------------------------------------------------

void LT768_PWM0_Init
(
 unsigned char on_off                       // 0：禁止PWM0    1：使能PWM0
,unsigned char Clock_Divided                // PWM时钟分频  取值范围 0~3(1,1/2,1/4,1/8)
,unsigned char Prescalar                    // 时钟分频     取值范围 1~256
,unsigned short Count_Buffer                // 设置PWM的输出周期
,unsigned short Compare_Buffer              // 设置占空比
)
{
   Select_PWM0();
   Set_PWM_Prescaler_1_to_256(Prescalar);

  if(Clock_Divided ==0) Select_PWM0_Clock_Divided_By_1();
  if(Clock_Divided ==1) Select_PWM0_Clock_Divided_By_2();
  if(Clock_Divided ==2) Select_PWM0_Clock_Divided_By_4();
  if(Clock_Divided ==3) Select_PWM0_Clock_Divided_By_8();

  Set_Timer0_Count_Buffer(Count_Buffer);  
  Set_Timer0_Compare_Buffer(Compare_Buffer);  
    
  if (on_off == 1)  Start_PWM0(); 
  if (on_off == 0)  Stop_PWM0();
}


void LT768_PWM0_Duty(unsigned short Compare_Buffer)
{
  Set_Timer0_Compare_Buffer(Compare_Buffer);
}



void LT768_PWM1_Init
(
 unsigned char on_off                       // 0：禁止PWM0    1：使能PWM0
,unsigned char Clock_Divided                // PWM时钟分频  取值范围 0~3(1,1/2,1/4,1/8)
,unsigned char Prescalar                    // 时钟分频     取值范围 1~256
,unsigned short Count_Buffer                // 设置PWM的输出周期
,unsigned short Compare_Buffer              // 设置占空比
)
{
  Select_PWM1();
  Set_PWM_Prescaler_1_to_256(Prescalar);
 
  if(Clock_Divided ==0) Select_PWM1_Clock_Divided_By_1();
  if(Clock_Divided ==1) Select_PWM1_Clock_Divided_By_2();
  if(Clock_Divided ==2) Select_PWM1_Clock_Divided_By_4();
  if(Clock_Divided ==3) Select_PWM1_Clock_Divided_By_8();

  Set_Timer1_Count_Buffer(Count_Buffer); 
  Set_Timer1_Compare_Buffer(Compare_Buffer); 

  if (on_off == 1)  Start_PWM1(); 
  if (on_off == 0)  Stop_PWM1();
}


void LT768_PWM1_Duty(unsigned short Compare_Buffer)
{
  Set_Timer1_Compare_Buffer(Compare_Buffer);
}
