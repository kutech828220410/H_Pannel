#include "LT768X.h"
#include "Arduino.h"
#include <SPI.h>
#include <SoftwareSerial.h>


void LT768X::Init(SoftwareSerial *lT768_Serial)
{
   LT768X_Serial = lT768_Serial;
   delay(100);                    //delay for LT768 power on
   HardwardReset();
   CheckSystem();
   PLL_Initial();
   SDRAM_initail(MCLK);
   Set_LCD_Panel();
}
void LT768X::Set_Backlight(uint8_t bl) //设置背光亮度，范围0~100
{
  Set_PWM_Prescaler_1_to_256(1);
  Select_PWM0_Clock_Divided_By_1();
  Select_PWM0();                
  Set_Timer0_Count_Buffer(6000); 
  Set_Timer0_Compare_Buffer(bl*60);
  Start_PWM0();               
}
void LT768X::fillScreen(uint16_t color)
{
  Foreground_color_65k(color);
  Square_Start_XY(0,0);
  Square_End_XY(LCD_XSIZE_TFT,LCD_YSIZE_TFT);
  Start_Square_Fill(); 
  Foreground_color_65k(color);

}

void LT768X::HardwardReset(void)
{ 
   pinMode(LT768_PIN_RST, OUTPUT);
   pinMode(LT768_PIN_CS, OUTPUT);

   digitalWrite(LT768_PIN_CS, HIGH);
   digitalWrite(LT768_PIN_RST, LOW);
   delay(100);
   digitalWrite(LT768_PIN_RST, HIGH);
   delay(100);
   LT768X_Serial -> println("LT768_HW_Reset done!");
}
bool LT768X::CheckSystem(void)
{
  uint8_t i = 0;
  uint8_t temp = 0;
  bool system_ok = false;
  LT768X_Serial -> println("LT768 checking system.....");
  while(true)
  {
     if(system_ok)
     {
       LT768X_Serial -> println("LT768 checking system sucess!");
       return true;
     }
     if(i > 5) 
     {       
       LT768X_Serial -> println("LT768 checking system failed!");
       return false;
     }
     if((LCD_StatusRead()&0x02)==0x00)
     {
       delay(1);                  
       LCD_CmdWrite(0x01);
       delay(1);                  
       temp =LCD_DataRead(); 
       if((temp & 0x80)==0x80)       //检测CCR寄存器PLL是否准备好
       {
          system_ok = true;
       }
       else
       {
          delay(1);
          LCD_CmdWrite(0x01);
          delay(1); 
          LCD_DataWrite(0x80);
       }
     }
     else
     {
       i++;
       system_ok = false;
     }
  }
  LT768X_Serial -> println("LT768 checking system failed!");
  return true;
}
void LT768X::PLL_Initial(void) 
{    
  unsigned int  temp = 0,temp1 =0 ;
  
  unsigned short lpllOD_SCLK, lpllOD_cclk, lpllOD_mclk;
  unsigned short lpllR_SCLK, lpllR_cclk, lpllR_mclk;
  unsigned short lpllN_SCLK, lpllN_cclk, lpllN_mclk;
  
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
  
  lpllOD_SCLK = 3;
  lpllOD_cclk = 2;
  lpllOD_mclk = 2;
  lpllR_SCLK  = 2;
  lpllR_cclk  = 2;
  lpllR_mclk  = 2;
  lpllN_mclk  = MCLK;      
  lpllN_cclk  = CCLK;    
  lpllN_SCLK  = 2*SCLK; 
  
#endif

#if XI_8M   
  
  lpllOD_SCLK = 3;
  lpllOD_cclk = 2;
  lpllOD_mclk = 2;
  lpllR_SCLK  = 2;
  lpllR_cclk  = 4;
  lpllR_mclk  = 4;
  lpllN_mclk  = MCLK;      
  lpllN_cclk  = CCLK;    
  lpllN_SCLK  = SCLK; 
  
#endif

#if XI_10M  
  
  lpllOD_SCLK = 3;
  lpllOD_cclk = 2;
  lpllOD_mclk = 2;
  lpllR_SCLK  = 5;
  lpllR_cclk  = 5;
  lpllR_mclk  = 5;
  lpllN_mclk  = MCLK;      
  lpllN_cclk  = CCLK;    
  lpllN_SCLK  = 2*SCLK; 
  
#endif

#if XI_12M  
  
  lpllOD_SCLK = 3;
  lpllOD_cclk = 2;
  lpllOD_mclk = 2;
  lpllR_SCLK  = 3;
  lpllR_cclk  = 6;
  lpllR_mclk  = 6;
  lpllN_mclk  = MCLK;      
  lpllN_cclk  = CCLK;    
  lpllN_SCLK  = SCLK; 
  
#endif    
      
  LCD_CmdWrite(0x05);
  LCD_DataWrite((lpllOD_SCLK<<6) | (lpllR_SCLK<<1) | ((lpllN_SCLK>>8)&0x1));
  LCD_CmdWrite(0x07);
  LCD_DataWrite((lpllOD_mclk<<6) | (lpllR_mclk<<1) | ((lpllN_mclk>>8)&0x1));
  LCD_CmdWrite(0x09);
  LCD_DataWrite((lpllOD_cclk<<6) | (lpllR_cclk<<1) | ((lpllN_cclk>>8)&0x1));

  LCD_CmdWrite(0x06);
  LCD_DataWrite(lpllN_SCLK);
  LCD_CmdWrite(0x08);
  LCD_DataWrite(lpllN_mclk);
  LCD_CmdWrite(0x0a);
  LCD_DataWrite(lpllN_cclk);
      
  LCD_CmdWrite(0x00);
  delay(1);
  LCD_DataWrite(0x80);

  delay(1);  //单PLL铆
}
void LT768X::SDRAM_initail(unsigned char mclk)
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

void LT768X::Set_LCD_Panel(void)
{
   TFT_16bit();
   Host_Bus_16bit();
   RGB_16b_16bpp();
   MemWrite_Left_Right_Top_Down();
   Graphic_Mode();
   Select_Main_Window_16bpp();
   PCLK_Falling();
   VSCAN_T_to_B();
   PDATA_Set_RGB();
   HSYNC_Low_Active();
   VSYNC_Low_Active();
   DE_High_Active();


 
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


//[00h]=========================================================================
void LT768X::SoftReset(void)
{
  uint8_t temp;

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
//[01h]=========================================================================
void LT768X::TFT_16bit(void)
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
void LT768X::Host_Bus_16bit(void)
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

//[02h]=========================================================================
void LT768X::RGB_16b_16bpp(void)
{
  unsigned char temp;
  LCD_CmdWrite(0x02);
  temp = LCD_DataRead();
  temp &= cClrb7;
  temp |= cSetb6;
  LCD_DataWrite(temp);
}

void LT768X::MemWrite_Left_Right_Top_Down(void)
{
  unsigned char temp;
  LCD_CmdWrite(0x02);
  temp = LCD_DataRead();
  temp &= cClrb2;
  temp &= cClrb1;
  LCD_DataWrite(temp);
}
//[03h]=========================================================================
void LT768X::Graphic_Mode(void)
{
  unsigned char temp;
  LCD_CmdWrite(0x03);
  temp = LCD_DataRead();
    temp &= cClrb2;
  LCD_DataWrite(temp);
}
void LT768X::Text_Mode(void)
{
  unsigned char temp;
  LCD_CmdWrite(0x03);
  temp = LCD_DataRead();
    temp |= cSetb2;
  LCD_DataWrite(temp);
}
void LT768X::Memory_Select_SDRAM(void)
{
  unsigned char temp;
  LCD_CmdWrite(0x03);
  temp = LCD_DataRead();
    temp &= cClrb1;
    temp &= cClrb0; // B
  LCD_DataWrite(temp);
}


//[04h]=========================================================================
//[05h]=========================================================================
//[06h]=========================================================================
//[07h]=========================================================================
//[08h]=========================================================================
//[09h]=========================================================================
//[0Ah]=========================================================================
//[0Bh]=========================================================================
void LT768X::LCD_SetCursor(uint16_t Xpos, uint16_t Ypos)
{
    LCD_CmdWrite(0x5F);
  LCD_DataWrite(Xpos);  
    LCD_CmdWrite(0x60);    
  LCD_DataWrite(Xpos>>8);
    LCD_CmdWrite(0x61);
  LCD_DataWrite(Ypos);
    LCD_CmdWrite(0x62);    
  LCD_DataWrite(Ypos>>8);
}
void LT768X::LCD_WriteRAM_Prepare(void)
{
  LCD_CmdWrite(0x04); //
}

//[0Ch]=========================================================================
//[0Dh]=========================================================================
//[0Eh]=========================================================================
void LT768X::Select_Main_Window_16bpp(void)
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

void LT768X::PCLK_Falling(void)
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
void LT768X::Display_ON(void)
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

void LT768X::VSCAN_T_to_B(void)
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
void LT768X::PDATA_Set_RGB(void)
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
void LT768X::Color_Bar()
{
  unsigned char temp;
  LCD_CmdWrite(0x12);
  temp = LCD_DataRead();
  temp |= cSetb5;
  LCD_DataWrite(temp);
}
////[13h]=========================================================================
void LT768X::HSYNC_Low_Active(void)
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

void LT768X::VSYNC_Low_Active(void)
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

void LT768X::DE_High_Active(void)
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

void LT768X::LCD_HorizontalWidth_VerticalHeight(unsigned short WX,unsigned short HY)
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
void LT768X::LCD_Horizontal_Non_Display(unsigned short WX)
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
void LT768X::LCD_HSYNC_Start_Position(unsigned short WX)
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
void LT768X::LCD_HSYNC_Pulse_Width(unsigned short WX)
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
void LT768X::LCD_Vertical_Non_Display(unsigned short HY)
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
void LT768X::LCD_VSYNC_Start_Position(unsigned short HY)
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
void LT768X::LCD_VSYNC_Pulse_Width(unsigned short HY)
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
void LT768X::Main_Image_Start_Address(unsigned long Addr)  
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
void LT768X::Main_Image_Width(unsigned short WX)  
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
void LT768X::Main_Window_Start_XY(unsigned short WX,unsigned short HY)  
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

//[50h][51h][52h][53h]=========================================================================
void LT768X::Canvas_Image_Start_address(unsigned long Addr)  
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
void LT768X::Canvas_image_width(unsigned short WX)  
{
/*
[54h] Canvas image width [7:2]
[55h] Canvas image width [12:8]
*/
  LCD_RegisterWrite(0x54,WX);
  LCD_RegisterWrite(0x55,WX>>8);
}

//[56h][57h][58h][59h]=========================================================================
void LT768X::Active_Window_XY(unsigned short WX,unsigned short HY)  
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
void LT768X::Active_Window_WH(unsigned short WX,unsigned short HY)  
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

//[5Eh]=========================================================================
void LT768X::Memory_XY_Mode(void) 
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
void LT768X::Memory_16bpp_Mode(void)  
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
void LT768X::Start_Line(void)
{
  LCD_CmdWrite(0x67);
  LCD_DataWrite(0x80);
  Check_Busy_Draw();
}
void LT768X::Start_Triangle(void)
{
  LCD_CmdWrite(0x67);
  LCD_DataWrite(0x82);//B1000_0010
  Check_Busy_Draw();
}
void LT768X::Start_Triangle_Fill(void)
{

  LCD_CmdWrite(0x67);
  LCD_DataWrite(0xA2);//B1010_0010
  Check_Busy_Draw();
}
//[68h][69h][6Ah][6Bh]=========================================================================
//畫矩形起始位置
void LT768X::Square_Start_XY(unsigned short WX,unsigned short HY)
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
//[6Ch][6Dh][6Eh][6Fh]=========================================================================
//畫矩形結束位置
void LT768X::Square_End_XY(unsigned short WX,unsigned short HY)
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
void LT768X::Start_Circle_or_Ellipse(void)
{
  LCD_CmdWrite(0x76);
  LCD_DataWrite(0x80);//B1000_XXXX
  Check_Busy_Draw();  
}
void LT768X::Start_Circle_or_Ellipse_Fill(void)
{
  LCD_CmdWrite(0x76);
  LCD_DataWrite(0xC0);//B1100_XXXX
  Check_Busy_Draw();  
}
//
void LT768X::Start_Left_Down_Curve(void)
{
  LCD_CmdWrite(0x76);
  LCD_DataWrite(0x90);//B1001_XX00
  Check_Busy_Draw();  
}
void LT768X::Start_Left_Up_Curve(void)
{
  LCD_CmdWrite(0x76);
  LCD_DataWrite(0x91);//B1001_XX01
  Check_Busy_Draw();  
}
void LT768X::Start_Right_Up_Curve(void)
{
  LCD_CmdWrite(0x76);
  LCD_DataWrite(0x92);//B1001_XX10
  Check_Busy_Draw();  
}
void LT768X::Start_Right_Down_Curve(void)
{
  LCD_CmdWrite(0x76);
  LCD_DataWrite(0x93);//B1001_XX11
  Check_Busy_Draw();  
}
//
void LT768X::Start_Left_Down_Curve_Fill(void)
{
  LCD_CmdWrite(0x76);
  LCD_DataWrite(0xD0);//B1101_XX00
  Check_Busy_Draw();
}
void LT768X::Start_Left_Up_Curve_Fill(void)
{
  LCD_CmdWrite(0x76);
  LCD_DataWrite(0xD1);//B1101_XX01
  Check_Busy_Draw();
}
void LT768X::Start_Right_Up_Curve_Fill(void)
{
  LCD_CmdWrite(0x76);
  LCD_DataWrite(0xD2);//B1101_XX10
  Check_Busy_Draw();
}
void LT768X::Start_Right_Down_Curve_Fill(void)
{
  LCD_CmdWrite(0x76);
  LCD_DataWrite(0xD3);//B1101_XX11
  Check_Busy_Draw();
}
//
void LT768X::Start_Square(void)
{
  LCD_CmdWrite(0x76);
  LCD_DataWrite(0xA0);//B1010_XXXX
  Check_Busy_Draw();
}
void LT768X::Start_Square_Fill(void)
{
  LCD_CmdWrite(0x76);
  LCD_DataWrite(0xE0);//B1110_XXXX
  Check_Busy_Draw();
}
void LT768X::Start_Circle_Square(void)
{
  LCD_CmdWrite(0x76);
  LCD_DataWrite(0xB0);//B1011_XXXX
  Check_Busy_Draw();  
}
void LT768X::Start_Circle_Square_Fill(void)
{
  LCD_CmdWrite(0x76);
  LCD_DataWrite(0xF0);//B1111_XXXX
  Check_Busy_Draw();  
}

//[84h]=========================================================================
void LT768X::Set_PWM_Prescaler_1_to_256(unsigned short WX)
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
void LT768X::Select_PWM1_Clock_Divided_By_1(void)
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
void LT768X::Select_PWM1_Clock_Divided_By_2(void)
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
void LT768X::Select_PWM1_Clock_Divided_By_4(void)
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
void LT768X::Select_PWM1_Clock_Divided_By_8(void)
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
void LT768X::Select_PWM0_Clock_Divided_By_1(void)
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
void LT768X::Select_PWM0_Clock_Divided_By_2(void)
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
void LT768X::Select_PWM0_Clock_Divided_By_4(void)
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
void LT768X::Select_PWM0_Clock_Divided_By_8(void)
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
//[85h].[bit1][bit0]=========================================================================
/*
XPWM[0] pin function control
0X: XPWM[0] becomes GPIO-C[7]
10: XPWM[0] enabled and controlled by PWM timer 0
11: XPWM[0] output core clock
*/

void LT768X::Select_PWM0(void)
{
  unsigned char temp;
  
  LCD_CmdWrite(0x85);
  temp = LCD_DataRead();
  temp |= cSetb1;
  temp &= cClrb0;
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
void LT768X::Select_PWM1_is_ErrorFlag(void)
{
  unsigned char temp;
  
  LCD_CmdWrite(0x85);
  temp = LCD_DataRead();
  temp &= cClrb3;
  LCD_DataWrite(temp);
}
void LT768X::Select_PWM1(void)
{
  unsigned char temp;
  
  LCD_CmdWrite(0x85);
  temp = LCD_DataRead();
  temp |= cSetb3;
  temp &= cClrb2;
  LCD_DataWrite(temp);
}
//[86h]=========================================================================
//[86h]PWM1
void LT768X::Start_PWM1(void)
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
void LT768X::Stop_PWM1(void)
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
void LT768X::Start_PWM0(void)
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
void LT768X::Stop_PWM0(void)
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
//[88h][89h]=========================================================================
void LT768X::Set_Timer0_Compare_Buffer(unsigned short WX)
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
void LT768X::Set_Timer0_Count_Buffer(unsigned short WX)
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
void LT768X::Set_Timer1_Compare_Buffer(unsigned short WX)
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
void LT768X::Set_Timer1_Count_Buffer(unsigned short WX)
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
void LT768X::Start_SFI_DMA(void)
{
  unsigned char temp;
  LCD_CmdWrite(0xB6);
  temp = LCD_DataRead();
    temp |= cSetb0;
  LCD_DataWrite(temp);
}

void LT768X::Check_Busy_SFI_DMA(void)
{
  LCD_CmdWrite(0xB6);
  do
  {   
  }while((LCD_DataRead()&0x01)==0x01);
}

//[D2h][D3h][D4h]======================================================================
void LT768X::Foreground_color_65k(uint16_t temp)
{
  LCD_CmdWrite(0xD2);
  LCD_DataWrite(temp>>8);

  LCD_CmdWrite(0xD3);
  LCD_DataWrite(temp>>3);

  LCD_CmdWrite(0xD4);
  LCD_DataWrite(temp<<3);
}

//[D5h][D6h][D7h]======================================================================
void LT768X::Background_color_65k(uint16_t temp)
{
  LCD_CmdWrite(0xD5);
  LCD_DataWrite(temp>>8);

  LCD_CmdWrite(0xD6);
  LCD_DataWrite(temp>>3);
 
  LCD_CmdWrite(0xD7);
  LCD_DataWrite(temp<<3);
}

void LT768X::PWM0_Init
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

void LT768X::PWM1_Init
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
void LT768X::Active_Window(uint16_t XL,uint16_t XR ,uint16_t YT ,uint16_t YB)
{
  uint8_t temp;
    //setting active window X
  temp=XL;   
    LCD_CmdWrite(0x30);//HSAW0
  LCD_DataWrite(temp);
  temp=XL>>8;   
    LCD_CmdWrite(0x31);//HSAW1     
  LCD_DataWrite(temp);

  temp=XR;   
    LCD_CmdWrite(0x34);//HEAW0
  LCD_DataWrite(temp);
  temp=XR>>8;   
    LCD_CmdWrite(0x35);//HEAW1     
  LCD_DataWrite(temp);

    //setting active window Y
  temp=YT;   
    LCD_CmdWrite(0x32);//VSAW0
  LCD_DataWrite(temp);
  temp=YT>>8;   
    LCD_CmdWrite(0x33);//VSAW1     
  LCD_DataWrite(temp);

  temp=YB;   
    LCD_CmdWrite(0x36);//VEAW0
  LCD_DataWrite(temp);
  temp=YB>>8;   
    LCD_CmdWrite(0x37);//VEAW1     
  LCD_DataWrite(temp);
}

/*
显示自定义图片
x:X坐标地址
x:Y坐标地址
w:显示图片的宽度
h:显示图片的高度
numbers:显示图片的字节数
*datap：图片数组的指针
注意：推荐用“image2lcd”软件生成图片数组，数组格式是高位在前，RGB565格式
*/
void LT768X::Draw_Picture(int x,int y,int w,int h,unsigned long numbers,const unsigned char *datap)
{  
   unsigned long i;  
   Graphic_Mode();

   Main_Image_Start_Address(0);
   Main_Image_Width(LCD_XSIZE_TFT);
   Main_Window_Start_XY(0,0);
   Select_Main_Window_16bpp();
   
   Canvas_Image_Start_address(0);  
   Canvas_image_width(LCD_XSIZE_TFT);
   Memory_16bpp_Mode();
   Memory_Select_SDRAM();  
   Active_Window_WH(LCD_XSIZE_TFT,LCD_YSIZE_TFT);
   Active_Window_XY(0,0);
   
   LCD_WriteRAM_Prepare();
   
   digitalWrite(LT768_PIN_CS, LOW);
   spi_begin();
   for(i = 0; i < numbers;)
   {    
    digitalWrite(LT768_PIN_CS, LOW);
    SPI.transfer(LT768_DATAWRITE);
    SPI.transfer(datap[i]);
    digitalWrite(LT768_PIN_CS, HIGH);
    digitalWrite(LT768_PIN_CS, LOW);
    Check_Busy_Fast();
    digitalWrite(LT768_PIN_CS, HIGH);
    i = i + 1;
   }
   spi_end();
   digitalWrite(LT768_PIN_CS, HIGH);
   Graphic_Mode();

   Display_ON();
 
}
void LT768X::Draw_Picture_test(void)
{
  //********内存写入测试********
  Active_Window(150,229,0,79);//设置工作窗口大小  
  MemoryWrite_Position(150,0);//内存写入位置
  LCD_CmdWrite(0x02);//写入寄存器数据
  int i = 6400;  //80X80点阵数据量
  while(i--)
  {
    LCD_DataWrite(0xff);//黄色
    Check_Busy();
    LCD_DataWrite(0xe0);
    Check_Busy();
  }
}
void LT768X::MemoryWrite_Position(uint16_t X,uint16_t Y)
{
  uint8_t temp;

  temp=X;   
  LCD_CmdWrite(0x46);
  LCD_DataWrite(temp);
  temp=X>>8;   
  LCD_CmdWrite(0x47);    
  LCD_DataWrite(temp);

  temp=Y;   
  LCD_CmdWrite(0x48);
  LCD_DataWrite(temp);
  temp=Y>>8;   
  LCD_CmdWrite(0x49);    
  LCD_DataWrite(temp);
}


void LT768X::Check_Busy_Draw(void)
{
  LT768X_Serial -> println("Check_Busy_Draw ....");
  do
  {
    
  }
  while( LCD_StatusRead()&0x08 );
  LT768X_Serial -> println("Check_Busy_Draw done!");
}
void LT768X::Check_Busy_Fast(void)
{
  uint8_t x;
  do
  {
    SPI.transfer(LT768_CMDREAD);
    x = SPI.transfer(0x0);
  }
  while( bitRead(x , 7));   
}
void LT768X::Check_Busy(void)
{
  unsigned char temp; 
  do
  {
    //LT768X_Serial -> println("Check_Busy ....");
    temp = LCD_StatusRead(false);
  }
  while( bitRead(temp , 7));
  //LT768X_Serial -> println("Check_Busy done ....");
}
void LT768X::Check_SDRAM_Ready(void)
{
/*  0: SDRAM is not ready for access
  1: SDRAM is ready for access    */  
  unsigned char temp;   
  do
  {
    temp = LCD_StatusRead();
  }
  while( (temp&0x04) == 0x00 );
}

void LT768X::spi_begin()
{
   SPI.beginTransaction(SPISettings(20000000, MSBFIRST, SPI_MODE0));
}
void LT768X::spi_end()
{
   SPI.endTransaction();
}
uint8_t LT768X::LCD_DataRead(void)
{
  digitalWrite(LT768_PIN_CS, LOW);
  spi_begin();

  SPI.transfer(LT768_DATAREAD);
  uint8_t x = SPI.transfer(0x0);
  spi_end();
  
  digitalWrite(LT768_PIN_CS, HIGH);
  LT768X_Serial -> print("SPI data read : 0x");
  LT768X_Serial -> println(x ,HEX);
  return x;
}  
uint8_t LT768X::LCD_StatusRead(void)
{
  return LCD_StatusRead(false);
}
uint8_t LT768X::LCD_StatusRead(bool show)
{
  digitalWrite(LT768_PIN_CS, LOW);
  spi_begin();

  SPI.transfer(LT768_CMDREAD);
  uint8_t x = SPI.transfer(0x0);
  spi_end();
  
  digitalWrite(LT768_PIN_CS, HIGH);
  if(show)LT768X_Serial -> print("SPI statu read : 0x");
  if(show)LT768X_Serial -> println(x ,HEX);
  return x;
}

void LT768X::LCD_CmdWrite(uint8_t cmd) 
{
  digitalWrite(LT768_PIN_CS, LOW);
  spi_begin();

  SPI.transfer(LT768_CMDWRITE);
  SPI.transfer(cmd);
  spi_end();

  digitalWrite(LT768_PIN_CS, HIGH);
}
void LT768X::LCD_DataWrite(uint8_t Data)
{
  digitalWrite(LT768_PIN_CS, LOW);
  spi_begin();

  SPI.transfer(LT768_DATAWRITE);
  SPI.transfer(Data);
  spi_end();

  digitalWrite(LT768_PIN_CS, HIGH);
}
void LT768X::LCD_RegisterWrite(uint8_t Cmd,uint8_t Data)
{
  LCD_CmdWrite(Cmd);
  LCD_DataWrite(Data);
}  
uint8_t LT768X::LCD_RegisterRead(uint8_t Cmd)
{
  uint8_t temp;
  
  LCD_CmdWrite(Cmd);
  temp=LCD_DataRead();
  return temp;
}
