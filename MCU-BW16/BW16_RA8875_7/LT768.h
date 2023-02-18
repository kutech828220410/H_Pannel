/******************** COPYRIGHT  ********************
* File Name       : LT768.h
* Author          : Levetop Electronics
* Version         : V1.0
* Date            : 2017-8-25
* Description     : 操作LT768的寄存器函数
****************************************************/

#ifndef _LT768_h
#define _LT768_h

#include "LT768.h"
#include "lcd.h"
#include <SoftwareSerial.h>
#define LT768_PIN_CS PA15

#define LT768_DATAWRITE 0x80 ///< See datasheet
#define LT768_DATAREAD 0xC0  ///< See datasheet
#define LT768_CMDWRITE 0x00  ///< See datasheet
#define LT768_CMDREAD 0x40   ///< See datasheet

#define cSetb0    0x01
#define cSetb1    0x02
#define cSetb2    0x04
#define cSetb3    0x08
#define cSetb4    0x10
#define cSetb5    0x20
#define cSetb6    0x40
#define cSetb7    0x80

#define cClrb0    0xfe
#define cClrb1    0xfd
#define cClrb2    0xfb
#define cClrb3    0xf7
#define cClrb4    0xef
#define cClrb5    0xdf
#define cClrb6    0xbf
#define cClrb7    0x7f

SoftwareSerial *LT768_Serial;
void LCD_RegisterWrite(unsigned char Cmd,unsigned char Data);
unsigned char LCD_RegisterRead(unsigned char Cmd);


//void LT768_initial(void);
////Set PLL
//void LT768_PLL_Initial(void); 
////Set SDRAM 
//void LT768_SDRAM_initail(void);


//**Staus**//
void Check_Mem_WR_FIFO_not_Full(void);
void Check_2D_Busy(void);
void Check_SDRAM_Ready(void);

void Check_Busy_Draw(void);

//**[00h]**//
void LT768_SW_Reset(void);
void TFT_16bit(void);

void Enable_SFlash_SPI(void);
void Disable_SFlash_SPI(void);
void Host_Bus_16bit(void);
//**[02h]**//


void RGB_16b_16bpp(void);

void MemWrite_Left_Right_Top_Down(void);

void Graphic_Mode(void);
void Text_Mode(void);
void Memory_Select_SDRAM(void);


void Select_Main_Window_16bpp(void);

void PCLK_Falling(void);
void Display_ON(void);

void VSCAN_T_to_B(void);
void VSCAN_B_to_T(void);
void PDATA_Set_RGB(void);


//**[13h]**//
void HSYNC_Low_Active(void);

void VSYNC_Low_Active(void);

void DE_High_Active(void);

void LCD_HorizontalWidth_VerticalHeight(unsigned short WX,unsigned short HY);
//**[16h][17h]**//
void LCD_Horizontal_Non_Display(unsigned short WX);
//**[18h]**//
void LCD_HSYNC_Start_Position(unsigned short WX);
//**[19h]**//
void LCD_HSYNC_Pulse_Width(unsigned short WX);
//**[1Ch][1Dh]**//
void LCD_Vertical_Non_Display(unsigned short HY);
//**[1Eh]**//
void LCD_VSYNC_Start_Position(unsigned short HY);
//**[1Fh]**//
void LCD_VSYNC_Pulse_Width(unsigned short HY);
//**[20h][21h][22h][23h]**//
void Main_Image_Start_Address(unsigned long Addr);
//**[24h][25h]**//          
void Main_Image_Width(unsigned short WX);             
//**[26h][27h][28h][29h]**//
void Main_Window_Start_XY(unsigned short WX,unsigned short HY); 
//**[2Ah][2Bh][2Ch][2Dh]**//
void PIP_Display_Start_XY(unsigned short WX,unsigned short HY);
//**[2Eh][2Fh][30h][31h]**//
void PIP_Image_Start_Address(unsigned long Addr);
//**[32h][33h]**//
void PIP_Image_Width(unsigned short WX);
//**[34h][35h][36h][37h]**//
void PIP_Window_Image_Start_XY(unsigned short WX,unsigned short HY);
//**[38h][39h][3Ah][3Bh]**//

void Canvas_Image_Start_address(unsigned long Addr);
//**[54h][55h]**//
void Canvas_image_width(unsigned short WX);
//**[56h][57h][58h][59h]**//
void Active_Window_XY(unsigned short WX,unsigned short HY);
//**[5Ah][5Bh][5Ch][5Dh]**//
void Active_Window_WH(unsigned short WX,unsigned short HY);
//**[5E]**//

void Memory_XY_Mode(void);
void Memory_16bpp_Mode(void);

void Goto_Pixel_XY(unsigned short WX,unsigned short HY);
  
void Square_Start_XY(unsigned short WX,unsigned short HY);    
void Square_End_XY(unsigned short WX,unsigned short HY);    
//**[76h]**//

void Start_Square_Fill(void);

////////////////////////////////////////////////////////////////////////
////**** [ Function : PWM ] ****////
//**[84h]**//
void Set_PWM_Prescaler_1_to_256(unsigned short WX);
//**[85h]**//
void Select_PWM1_Clock_Divided_By_1(void);
void Select_PWM1_Clock_Divided_By_2(void);
void Select_PWM1_Clock_Divided_By_4(void);
void Select_PWM1_Clock_Divided_By_8(void);
void Select_PWM0_Clock_Divided_By_1(void);
void Select_PWM0_Clock_Divided_By_2(void);
void Select_PWM0_Clock_Divided_By_4(void);
void Select_PWM0_Clock_Divided_By_8(void);
//[85h].[bit3][bit2]
void Select_PWM1_is_ErrorFlag(void);
void Select_PWM1(void);
void Select_PWM0(void);

void Start_PWM1(void);
void Stop_PWM1(void);
//[86h]PWM0
void Start_PWM0(void);
void Stop_PWM0(void);
//**[87h]**//
void Set_Timer0_Compare_Buffer(unsigned short WX);
//**[8Ah][8Bh]**//
void Set_Timer0_Count_Buffer(unsigned short WX);
//**[8Ch][8Dh]**//
void Set_Timer1_Compare_Buffer(unsigned short WX);
//**[8Eh][8Fh]**//
void Set_Timer1_Count_Buffer(unsigned short WX);


////////////////////////////////////////////////////////////////////////
////**** [ Function : Serial Flash ] ****////


//REG[B6h] Serial flash DMA Controller REG (DMA_CTRL) 
void Start_SFI_DMA(void);
void Check_Busy_SFI_DMA(void);

//REG[B7h] Serial Flash/ROM Controller Register (SFL_CTRL) 
void Select_SFI_0(void);
void Select_SFI_1(void);
void Select_SFI_DMA_Mode(void);
void Select_SFI_Dual_Mode0(void);
//REG[BB] SPI Clock period (SPIDIV) 
void SPI_Clock_Period(unsigned char temp);


//**[BCh][BDh][BEh][BFh]**//
void SFI_DMA_Source_Start_Address(unsigned long Addr);
//**[C0h][C1h][C2h][C3h]**//
void SFI_DMA_Destination_Upper_Left_Corner(unsigned short WX,unsigned short HY);
void SFI_DMA_Transfer_Width_Height(unsigned short WX,unsigned short HY);
//**[CAh][CBh]**//
void SFI_DMA_Source_Width(unsigned short WX);

void Foreground_color_65k(unsigned short temp);


//unsigned char CCLK;    // LT768的内核时钟频率    
//unsigned char MCLK;    // SDRAM的时钟频率
//unsigned char SCLK;    // LCD的扫描时钟频率

//外部晶振
#define XI_4M            0
#define XI_8M            0
#define XI_10M           1
#define XI_12M           0

//分辨率
#define LCD_XSIZE_TFT    800  
#define LCD_YSIZE_TFT    480
//参数
#define LCD_VBPD     20
#define LCD_VFPD     12
#define LCD_VSPW     3
#define LCD_HBPD     140
#define LCD_HFPD     160
#define LCD_HSPW       20

#define color256_black   0x00
#define color256_white   0xff
#define color256_red     0xe0
#define color256_green   0x1c
#define color256_blue    0x03
#define color256_yellow  color256_red|color256_green
#define color256_cyan    color256_green|color256_blue
#define color256_purple  color256_red|color256_blue
 
#define color65k_black   0x0000
#define color65k_white   0xffff
#define color65k_red     0xf800
#define color65k_green   0x07e0
#define color65k_blue    0x001f
#define color65k_yellow  color65k_red|color65k_green
#define color65k_cyan    color65k_green|color65k_blue
#define color65k_purple  color65k_red|color65k_blue

#define color65k_grayscale1    2113
#define color65k_grayscale2    2113*2
#define color65k_grayscale3    2113*3
#define color65k_grayscale4    2113*4
#define color65k_grayscale5    2113*5
#define color65k_grayscale6    2113*6
#define color65k_grayscale7    2113*7
#define color65k_grayscale8    2113*8
#define color65k_grayscale9    2113*9
#define color65k_grayscale10   2113*10
#define color65k_grayscale11   2113*11
#define color65k_grayscale12   2113*12
#define color65k_grayscale13   2113*13
#define color65k_grayscale14   2113*14
#define color65k_grayscale15   2113*15
#define color65k_grayscale16   2113*16
#define color65k_grayscale17   2113*17
#define color65k_grayscale18   2113*18
#define color65k_grayscale19   2113*19
#define color65k_grayscale20   2113*20
#define color65k_grayscale21   2113*21
#define color65k_grayscale22   2113*22
#define color65k_grayscale23   2113*23
#define color65k_grayscale24   2113*24
#define color65k_grayscale25   2113*25
#define color65k_grayscale26   2113*26
#define color65k_grayscale27   2113*27
#define color65k_grayscale28   2113*28
#define color65k_grayscale29   2113*29
#define color65k_grayscale30   2113*30

 
#define color16M_black   0x00000000
#define color16M_white   0x00ffffff
#define color16M_red     0x00ff0000
#define color16M_green   0x0000ff00
#define color16M_blue    0x000000ff
#define color16M_yellow  color16M_red|color16M_green
#define color16M_cyan    color16M_green|color16M_blue
#define color16M_purple  color16M_red|color16M_blue


/* LCD color */
#define White          0xFFFF
#define Black          0x0000
#define Grey           0xF7DE
#define Blue           0x001F
#define Blue2          0x051F
#define Red            0xF800
#define Magenta        0xF81F
#define Green          0x07E0
#define Cyan           0x7FFF
#define Yellow         0xFFE0

#define Line0          0
#define Line1          24
#define Line2          48
#define Line3          72
#define Line4          96
#define Line5          120
#define Line6          144
#define Line7          168
#define Line8          192
#define Line9          216
#define Line10         240
#define Line11         264
#define Line12         288
#define Line13         312
#define Line14         336
#define Line15         360
#define Line16         384
#define Line17         408
#define Line18         432
#define Line19         456
#define Line20         480
#define Line21         504
#define Line22         528
#define Line23         552
#define Line24         576
#define Line25         600



void LT768_Init(void);

void LT768_DrawSquare_Fill(unsigned short X1,unsigned short Y1,unsigned short X2,unsigned short Y2,unsigned long ForegroundColor);


void LT768_DMA_24bit_Block(unsigned char SCS,unsigned char Clk,unsigned short X1,unsigned short Y1 ,unsigned short X_W,unsigned short Y_H,unsigned short P_W,unsigned long Addr);

/*  PWM */
void LT768_PWM0_Init(unsigned char on_off,unsigned char Clock_Divided,unsigned char Prescalar,unsigned short Count_Buffer,unsigned short Compare_Buffer);
void LT768_PWM1_Init(unsigned char on_off,unsigned char Clock_Divided,unsigned char Prescalar,unsigned short Count_Buffer,unsigned short Compare_Buffer);
void LT768_PWM0_Duty(unsigned short Compare_Buffer);
void LT768_PWM1_Duty(unsigned short Compare_Buffer);
#endif
