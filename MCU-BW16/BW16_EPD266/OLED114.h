#ifndef __OLED114_h
#define __OLED114_h
#include "Timer.h"
#include <SPI.h>
#include "Arduino.h"
#include <SoftwareSerial.h>

typedef void (*OutputHandle) (void);
#define USE_HORIZONTAL 2  //设置横屏或者竖屏显示 0或1为竖屏 2或3为横屏

#if USE_HORIZONTAL==0||USE_HORIZONTAL==1
#define LCD_W 135
#define LCD_H 240

#else
#define LCD_W 240
#define LCD_H 135
#endif

//

//颜色
#define WHITE            0xFFFF
#define BLACK            0x0000
#define BLUE             0x001F  
#define GREEN            0x07E0
#define RED              0xF800

#define BRED             0XF81F
#define GRED             0XFFE0
#define GBLUE            0X07FF

#define MAGENTA          0xF81F

#define CYAN             0x7FFF
#define YELLOW           0xFFE0
#define BROWN            0XBC40 //棕色
#define BRRED            0XFC07 //棕红色
#define GRAY             0X8430 //灰色
//GUI颜色

#define DARKBLUE         0X01CF //深蓝色
#define LIGHTBLUE        0X7D7C //浅蓝色  
#define GRAYBLUE         0X5458 //灰蓝色
//以上三色为PANEL的颜色 
 
#define LIGHTGREEN       0X841F //浅绿色
#define LGRAY            0XC618 //浅灰色(PANNEL),窗体背景色

#define LGRAYBLUE        0XA651 //浅灰蓝色(中间层颜色)
#define LBBLUE           0X2B12 //浅棕蓝色(选择条目的反色)




class OLED114
{
   public:
   uint16_t* framebuffer;
   SoftwareSerial *mySerial;
   int index = 0;
   int dc=PA27;//定义数字接口11
   int cs=PA15;//定义数字接口12
   void Lcd_Init();
   void LCD_Writ_Bus(uint8_t dat);
   void LCD_WR_DATA8(uint8_t dat);
   void LCD_WR_DATA(short int dat);
   void LCD_WR_REG(uint8_t dat);
   void LCD_Clear(short int Color);
   void LCD_ShowChinese(short int x,short int y,uint8_t index,uint8_t size,short int color);
   void LCD_DrawPoint(short int x,short int y,short int color);
   void LCD_Fill(short int xsta,short int ysta,short int xend,short int yend,short int color);
   void LCD_DrawLine(short int x1,short int y1,short int x2,short int y2,short int color);
   void LCD_DrawRectangle(short int x1, short int y1, short int x2, short int y2,short int color);
   void Draw_Circle(short int x0,short int y0,uint8_t r,short int color);
   void LCD_ShowChar(short int x,short int y,uint8_t num,short int color);
   void LCD_ShowString(short int x,short int y,const char *p,short int color);
   u32 mypow(uint8_t m,uint8_t n);
   void LCD_ShowNum(short int x,short int y,short int num,uint8_t len,short int color);
   void LCD_ShowNum1(short int x,short int y,float num,uint8_t len,short int color);
   void LCD_ShowPicture(short int x1,short int y1,short int x2,short int y2);
   void LCD_ShowPicture();
   void LCD_Address_Set(uint8_t x1,uint8_t y1,uint8_t x2,uint8_t y2);
   void LCD_DrawPoint_big(short int x,short int y,short int color);
   void OLED_CS_Clr();
   void OLED_CS_Set();
   void OLED_DC_Clr();
   void OLED_DC_Set();

};

#endif
