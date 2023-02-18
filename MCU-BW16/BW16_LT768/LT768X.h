#ifndef _LT768X_h
#define _LT768X_h

#include "LT768X.h"
#include <SPI.h>
#include <SoftwareSerial.h>
#define LT768_PIN_CS PA15
#define LT768_PIN_RST PA27

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



class LT768X
{
   public:

   void Init(SoftwareSerial *lT768_Serial);
   SoftwareSerial *LT768X_Serial;  
   void fillScreen(uint16_t color);
   void Draw_Picture(int x,int y,int w,int h,unsigned long numbers,const unsigned char *datap);
   void MemoryWrite_Position(uint16_t X,uint16_t Y);
   void Draw_Picture_test(void);
   void Set_Backlight(uint8_t bl);
   void Host_Bus_16bit(void);
   void TFT_16bit(void);
   void RGB_16b_16bpp(void);
   void MemWrite_Left_Right_Top_Down(void);
   void Graphic_Mode(void);
   void Graphic_Mode_8876(void);
   void Text_Mode(void);
   void Memory_Select_SDRAM(void);
   void Select_Main_Window_16bpp(void);
   void PCLK_Falling(void);
   void Display_ON(void);
   void Color_Bar(void);  
   void VSCAN_T_to_B(void);
   void PDATA_Set_RGB(void);
   void HSYNC_Low_Active(void);
   void VSYNC_Low_Active(void);
   void DE_High_Active(void);
   void LCD_SetCursor(uint16_t Xpos, uint16_t Ypos);
   void LCD_WriteRAM_Prepare(void);
   void Active_Window_XY(unsigned short WX,unsigned short HY);
   void Active_Window_WH(unsigned short WX,unsigned short HY);
   void Active_Window(uint16_t XL,uint16_t XR ,uint16_t YT ,uint16_t YB);
   void LCD_HorizontalWidth_VerticalHeight(unsigned short WX,unsigned short HY);
   void LCD_Horizontal_Non_Display(unsigned short WX);
   void LCD_HSYNC_Start_Position(unsigned short WX);
   void LCD_HSYNC_Pulse_Width(unsigned short WX);
   void LCD_Vertical_Non_Display(unsigned short HY);
   void LCD_VSYNC_Start_Position(unsigned short HY);
   void LCD_VSYNC_Pulse_Width(unsigned short HY);
   void Main_Image_Start_Address(unsigned long Addr);
   void Main_Image_Width(unsigned short WX);
   void Main_Window_Start_XY(unsigned short WX,unsigned short HY);
   void Start_SFI_DMA(void);
   void Check_Busy_SFI_DMA(void);
   void Canvas_Image_Start_address(unsigned long Addr);
   void Canvas_image_width(unsigned short WX);
   void Memory_XY_Mode(void);
   void Memory_16bpp_Mode(void); 

   void Select_PWM0(void);
   void Select_PWM1_is_ErrorFlag(void);
   void Select_PWM1(void);
   
   void Set_PWM_Prescaler_1_to_256(unsigned short WX);
   void Select_PWM1_Clock_Divided_By_1(void); 
   void Select_PWM1_Clock_Divided_By_2(void); 
   void Select_PWM1_Clock_Divided_By_4(void); 
   void Select_PWM1_Clock_Divided_By_8(void); 
   void Select_PWM0_Clock_Divided_By_1(void); 
   void Select_PWM0_Clock_Divided_By_2(void); 
   void Select_PWM0_Clock_Divided_By_4(void); 
   void Select_PWM0_Clock_Divided_By_8(void); 
   
   void Start_PWM1(void); 
   void Stop_PWM1(void); 
   void Start_PWM0(void); 
   void Stop_PWM0(void); 

   void Set_Timer0_Compare_Buffer(unsigned short WX);
   void Set_Timer0_Count_Buffer(unsigned short WX);
   void Set_Timer1_Compare_Buffer(unsigned short WX);
   void Set_Timer1_Count_Buffer(unsigned short WX);

   void Background_color_65k(uint16_t temp);
   void Foreground_color_65k(uint16_t temp);
   
   void PWM0_Init(unsigned char on_off,unsigned char Clock_Divided,unsigned char Prescalar,unsigned short Count_Buffer,unsigned short Compare_Buffer);   
   void PWM1_Init(unsigned char on_off,unsigned char Clock_Divided,unsigned char Prescalar,unsigned short Count_Buffer,unsigned short Compare_Buffer);   

   void Start_Line(void);
   void Start_Triangle(void);
   void Start_Triangle_Fill(void);
   void Square_Start_XY(unsigned short WX,unsigned short HY);
   void Square_End_XY(unsigned short WX,unsigned short HY);
   void Start_Circle_or_Ellipse(void);
   void Start_Circle_or_Ellipse_Fill(void);
   void Start_Left_Down_Curve(void);
   void Start_Left_Up_Curve(void);
   void Start_Right_Up_Curve(void);
   void Start_Right_Down_Curve(void);
   void Start_Left_Down_Curve_Fill(void);
   void Start_Left_Up_Curve_Fill(void);
   void Start_Right_Up_Curve_Fill(void);
   void Start_Right_Down_Curve_Fill(void);
   void Start_Square(void);
   void Start_Square_Fill(void);
   void Start_Circle_Square(void);
   void Start_Circle_Square_Fill(void);



   
   void spi_begin(void);
   void spi_end(void);
   uint8_t LCD_DataRead(void);
   uint8_t LCD_StatusRead(bool show);
   uint8_t LCD_StatusRead(void);
   void LCD_CmdWrite(uint8_t cmd);
   void LCD_DataWrite(uint8_t cmd);
   void LCD_RegisterWrite(uint8_t Cmd,uint8_t Data);
   uint8_t LCD_RegisterRead(uint8_t Cmd);   


   
   private:
    
   unsigned char CCLK;    // LT768的内核时钟频率    
   unsigned char MCLK;    // SDRAM的时钟频率
   unsigned char SCLK;    // LCD的扫描时钟频率
   
   void Check_Busy_Draw(void);
   void Check_SDRAM_Ready(void);
   void Check_Busy(void);
   void Check_Busy_Fast(void);
   void SoftReset(void);
   void HardwardReset(void);
   bool CheckSystem(void);
   void PLL_Initial(void);
   void SDRAM_initail(unsigned char mclk);
   void Set_LCD_Panel(void);
     
};
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



#endif
