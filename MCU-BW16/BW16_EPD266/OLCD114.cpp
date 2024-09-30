#include "Timer.h"
#include "Config.h"
#include "OLCD114.h"
#include "arduino.h"
#include "font.h"

bool flag_LCD_init = true;
//OLED的初始化
void OLCD114::Lcd_Init()
{ 
    this->framebuffer = (uint16_t*) malloc(LCD_W * LCD_H * sizeof(uint16_t));
    pinMode(dc,OUTPUT);//设置数字11
    pinMode(cs,OUTPUT);//设置数字12 
    #ifdef MCP23017
    _mcp ->digitalWrite(/*pin = */_mcp -> eGPB7, true);
    printf("(RST)mcp.eGPB7 'true' \n");
    #endif
    LCD_WR_REG(0x11); 
    delay(300);
    OLED_CS_Clr();  
    #ifdef MCP23017
    _mcp ->digitalWrite(/*pin = */_mcp -> eGPB, /*Port Value = */0x80);
    printf("(RST)/*pin = */_mcp -> eGPB, /*Port Value = */0x80 \n");
    #endif
    delay(300);
    //************* Start Initial Sequence **********// 
    LCD_WR_REG(0x36); 
    if(USE_HORIZONTAL==0)LCD_WR_DATA8(0x00);
    else if(USE_HORIZONTAL==1)LCD_WR_DATA8(0xC0);
    else if(USE_HORIZONTAL==2)LCD_WR_DATA8(0x70);
    else LCD_WR_DATA8(0xA0);
    
    LCD_WR_REG(0x3A);
    LCD_WR_DATA8(0x05);
    
    LCD_WR_REG(0xB2);
    LCD_WR_DATA8(0x0C);
    LCD_WR_DATA8(0x0C);
    LCD_WR_DATA8(0x00);
    LCD_WR_DATA8(0x33);
    LCD_WR_DATA8(0x33); 
    
    LCD_WR_REG(0xB7); 
    LCD_WR_DATA8(0x35);  
    
    LCD_WR_REG(0xBB);
    LCD_WR_DATA8(0x19);
    
    LCD_WR_REG(0xC0);
    LCD_WR_DATA8(0x2C);
    
    LCD_WR_REG(0xC2);
    LCD_WR_DATA8(0x01);
    
    LCD_WR_REG(0xC3);
    LCD_WR_DATA8(0x12);   
    
    LCD_WR_REG(0xC4);
    LCD_WR_DATA8(0x20);  
    
    LCD_WR_REG(0xC6); 
    LCD_WR_DATA8(0x0F);    
    
    LCD_WR_REG(0xD0); 
    LCD_WR_DATA8(0xA4);
    LCD_WR_DATA8(0xA1);
    
    LCD_WR_REG(0xE0);
    LCD_WR_DATA8(0xD0);
    LCD_WR_DATA8(0x04);
    LCD_WR_DATA8(0x0D);
    LCD_WR_DATA8(0x11);
    LCD_WR_DATA8(0x13);
    LCD_WR_DATA8(0x2B);
    LCD_WR_DATA8(0x3F);
    LCD_WR_DATA8(0x54);
    LCD_WR_DATA8(0x4C);
    LCD_WR_DATA8(0x18);
    LCD_WR_DATA8(0x0D);
    LCD_WR_DATA8(0x0B);
    LCD_WR_DATA8(0x1F);
    LCD_WR_DATA8(0x23);
    
    LCD_WR_REG(0xE1);
    LCD_WR_DATA8(0xD0);
    LCD_WR_DATA8(0x04);
    LCD_WR_DATA8(0x0C);
    LCD_WR_DATA8(0x11);
    LCD_WR_DATA8(0x13);
    LCD_WR_DATA8(0x2C);
    LCD_WR_DATA8(0x3F);
    LCD_WR_DATA8(0x44);
    LCD_WR_DATA8(0x51);
    LCD_WR_DATA8(0x2F);
    LCD_WR_DATA8(0x1F);
    LCD_WR_DATA8(0x1F);
    LCD_WR_DATA8(0x20);
    LCD_WR_DATA8(0x23);
    
    LCD_WR_REG(0x21);
    
    LCD_WR_REG(0x11); 
    //Delay (120); 
    
    LCD_WR_REG(0x29); 
    OLED_CS_Set();
    #ifdef MCP23017
    _mcp ->digitalWrite(/*pin = */_mcp -> eGPB, /*Port Value = */0xFF);
    #endif
    flag_LCD_init = true;
}
void OLCD114::OLED_DC_Clr()
{
   digitalWrite(dc,LOW);
}
void OLCD114::OLED_DC_Set()
{
   digitalWrite(dc,HIGH);
}
void OLCD114::OLED_CS_Clr()
{
  if(flag_LCD_init == false) SPI.beginTransaction(SPISettings(5000, MSBFIRST, SPI_MODE0));  // 开始 SPI 事务
  digitalWrite(cs, LOW); //CS 
  if (flag_LCD_init == false)return;
  SPI.beginTransaction(SPISettings(80000000, MSBFIRST, SPI_MODE0));  // 开始 SPI 事务
  #ifdef MCP23017
  _mcp ->digitalWrite(_mcp ->eGPB0 , !(index == 0));
  _mcp ->digitalWrite(_mcp ->eGPB1 , !(index == 1));
  _mcp ->digitalWrite(_mcp ->eGPB2 , !(index == 2));
  _mcp ->digitalWrite(_mcp ->eGPB3 , !(index == 3));
  _mcp ->digitalWrite(_mcp ->eGPB4 , !(index == 4));
  _mcp ->digitalWrite(_mcp ->eGPB5 , !(index == 5));
  _mcp ->digitalWrite(_mcp ->eGPB6 , !(index == 6));
  #endif

  
}
void OLCD114::OLED_CS_Set()
{
   digitalWrite(cs,HIGH);//CS  
  if (flag_LCD_init == false)return;
  #ifdef MCP23017
  _mcp ->digitalWrite(_mcp ->eGPB0 , (index == 0));
  _mcp ->digitalWrite(_mcp ->eGPB1 , (index == 1));
  _mcp ->digitalWrite(_mcp ->eGPB2 , (index == 2));
  _mcp ->digitalWrite(_mcp ->eGPB3 , (index == 3));
  _mcp ->digitalWrite(_mcp ->eGPB4 , (index == 4));
  _mcp ->digitalWrite(_mcp ->eGPB5 , (index == 5));
  _mcp ->digitalWrite(_mcp ->eGPB6 , (index == 6));
  #endif
  SPI.endTransaction();  // 结束 SPI 事务
}
/******************************************************************************
      函数说明：LCD串行数据写入函数
      入口数据：dat  要写入的串行数据
      返回值：  无
******************************************************************************/
void OLCD114::LCD_Writ_Bus(uint8_t dat)
{
  uint8_t i;
  SPI.transfer(dat);
}
/******************************************************************************
      函数说明：LCD写入数据
      入口数据：dat 写入的数据
      返回值：  无
******************************************************************************/
void OLCD114::LCD_WR_DATA8(uint8_t dat)
{
  OLED_DC_Set();
  LCD_Writ_Bus(dat);
}
/******************************************************************************
      函数说明：LCD写入数据
      入口数据：dat 写入的数据
      返回值：  无
******************************************************************************/
void OLCD114::LCD_WR_DATA(short int dat)
{
  OLED_DC_Set();
  LCD_Writ_Bus(dat>>8);
  LCD_Writ_Bus(dat);
}
/******************************************************************************
      函数说明：LCD写入命令
      入口数据：dat 写入的命令
      返回值：  无
******************************************************************************/
void OLCD114::LCD_WR_REG(uint8_t dat)
{
  OLED_DC_Clr();//写命令
  LCD_Writ_Bus(dat);
}

/******************************************************************************
      函数说明：设置起始和结束地址
      入口数据：x1,x2 设置列的起始和结束地址
                y1,y2 设置行的起始和结束地址
      返回值：  无
******************************************************************************/
void OLCD114::LCD_Address_Set(uint8_t x1,uint8_t y1,uint8_t x2,uint8_t y2)
{
  
  if(USE_HORIZONTAL==0)
  {
    LCD_WR_REG(0x2a);//列地址设置
    LCD_WR_DATA(x1+52);
    LCD_WR_DATA(x2+52);
    LCD_WR_REG(0x2b);//行地址设置
    LCD_WR_DATA(y1+40);
    LCD_WR_DATA(y2+40);
    LCD_WR_REG(0x2c);//储存器写
  }
  else if(USE_HORIZONTAL==1)
  {
    LCD_WR_REG(0x2a);//列地址设置
    LCD_WR_DATA(x1+53);
    LCD_WR_DATA(x2+53);
    LCD_WR_REG(0x2b);//行地址设置
    LCD_WR_DATA(y1+40);
    LCD_WR_DATA(y2+40);
    LCD_WR_REG(0x2c);//储存器写
  }
  else if(USE_HORIZONTAL==2)
  {
    LCD_WR_REG(0x2a);//列地址设置
    LCD_WR_DATA(x1+40);
    LCD_WR_DATA(x2+40);
    LCD_WR_REG(0x2b);//行地址设置
    LCD_WR_DATA(y1+53);
    LCD_WR_DATA(y2+53);
    LCD_WR_REG(0x2c);//储存器写
  }
  else
  {
    LCD_WR_REG(0x2a);//列地址设置
    LCD_WR_DATA(x1+40);
    LCD_WR_DATA(x2+40);
    LCD_WR_REG(0x2b);//行地址设置
    LCD_WR_DATA(y1+52);
    LCD_WR_DATA(y2+52);
    LCD_WR_REG(0x2c);//储存器写
  }
}

/******************************************************************************
      函数说明：LCD清屏函数
      入口数据：无
      返回值：  无
******************************************************************************/
void OLCD114::LCD_Clear(short int Color)
{
  OLED_CS_Clr();  
  short int i,j;    
  LCD_Address_Set(0,0,LCD_W-1,LCD_H-1);
    for(i=0;i<LCD_H;i++)
    {
       for (j=0;j<LCD_W;j++)
        {
          LCD_WR_DATA(Color);
        }

    }
 OLED_CS_Set();
}


/******************************************************************************
      函数说明：LCD显示汉字
      入口数据：x,y   起始坐标
      返回值：  无
******************************************************************************/
void OLCD114::LCD_DrawPoint(short int x,short int y,short int color)
{
  LCD_Address_Set(x,y,x,y);//设置光标位置 
  LCD_WR_DATA(color);
} 
/******************************************************************************
      函数说明：LCD画一个大的点
      入口数据：x,y   起始坐标
      返回值：  无
******************************************************************************/
void OLCD114::LCD_DrawPoint_big(short int x,short int y,short int color)
{
  LCD_Fill(x-1,y-1,x+1,y+1,color);
} 
/******************************************************************************
      函数说明：在指定区域填充颜色
      入口数据：xsta,ysta   起始坐标
                xend,yend   终止坐标
      返回值：  无
******************************************************************************/
void OLCD114::LCD_Fill(short int xsta,short int ysta,short int xend,short int yend,short int color)
{          
  short int i,j; 
  LCD_Address_Set(xsta,ysta,xend,yend);      //设置光标位置 
  for(i=ysta;i<=yend;i++)
  {                               
    for(j=xsta;j<=xend;j++)
    {
      LCD_WR_DATA(color);//设置光标位置     
    }     
  }                   
}
/******************************************************************************
      函数说明：画线
      入口数据：x1,y1   起始坐标
                x2,y2   终止坐标
      返回值：  无
******************************************************************************/
void OLCD114::LCD_DrawLine(short int x1,short int y1,short int x2,short int y2,short int color)
{
  short int t; 
  int xerr=0,yerr=0,delta_x,delta_y,distance;
  int incx,incy,uRow,uCol;
  delta_x=x2-x1; //计算坐标增量 
  delta_y=y2-y1;
  uRow=x1;//画线起点坐标
  uCol=y1;
  if(delta_x>0)incx=1; //设置单步方向 
  else if (delta_x==0)incx=0;//垂直线 
  else {incx=-1;delta_x=-delta_x;}
  if(delta_y>0)incy=1;
  else if (delta_y==0)incy=0;//水平线 
  else {incy=-1;delta_y=-delta_x;}
  if(delta_x>delta_y)distance=delta_x; //选取基本增量坐标轴 
  else distance=delta_y;
  for(t=0;t<distance+1;t++)
  {
    LCD_DrawPoint(uRow,uCol,color);//画点
    xerr+=delta_x;
    yerr+=delta_y;
    if(xerr>distance)
    {
      xerr-=distance;
      uRow+=incx;
    }
    if(yerr>distance)
    {
      yerr-=distance;
      uCol+=incy;
    }
  }
}
/******************************************************************************
      函数说明：画矩形
      入口数据：x1,y1   起始坐标
                x2,y2   终止坐标
      返回值：  无
******************************************************************************/
void OLCD114::LCD_DrawRectangle(short int x1, short int y1, short int x2, short int y2,short int color)
{
  LCD_DrawLine(x1,y1,x2,y1,color);
  LCD_DrawLine(x1,y1,x1,y2,color);
  LCD_DrawLine(x1,y2,x2,y2,color);
  LCD_DrawLine(x2,y1,x2,y2,color);
}
/******************************************************************************
      函数说明：画圆
      入口数据：x0,y0   圆心坐标
                r       半径
      返回值：  无
******************************************************************************/
void OLCD114::Draw_Circle(short int x0,short int y0,uint8_t r,short int color)
{
  int a,b;
  int di;
  a=0;b=r;    
  while(a<=b)
  {
    LCD_DrawPoint(x0-b,y0-a,color);             //3           
    LCD_DrawPoint(x0+b,y0-a,color);             //0           
    LCD_DrawPoint(x0-a,y0+b,color);             //1                
    LCD_DrawPoint(x0-a,y0-b,color);             //2             
    LCD_DrawPoint(x0+b,y0+a,color);             //4               
    LCD_DrawPoint(x0+a,y0-b,color);             //5
    LCD_DrawPoint(x0+a,y0+b,color);             //6 
    LCD_DrawPoint(x0-b,y0+a,color);             //7
    a++;
    if((a*a+b*b)>(r*r))//判断要画的点是否过远
    {
      b--;
    }
  }
}


/******************************************************************************
      函数说明：显示数字
      入口数据：m底数，n指数
      返回值：  无
******************************************************************************/
u32 OLCD114::mypow(uint8_t m,uint8_t n)
{
  u32 result=1;
  while(n--)result*=m;    
  return result;
}

/******************************************************************************
      函数说明：显示数字
      入口数据：x,y    起点坐标
                num    要显示的数字
                len    要显示的数字个数
      返回值：  无
******************************************************************************/
void OLCD114::LCD_ShowNum(short int x,short int y,short int num,uint8_t len,short int color)
{           
  uint8_t t,temp;
  uint8_t enshow=0;
  for(t=0;t<len;t++)
  {
    temp=(num/mypow(10,len-t-1))%10;
    if(enshow==0&&t<(len-1))
    {
      if(temp==0)
      {
        LCD_ShowChar(x+8*t,y,' ',color);
        continue;
      }else enshow=1; 
       
    }
    LCD_ShowChar(x+8*t,y,temp+48,color); 
  }
} 
///******************************************************************************
//      函数说明：显示字符
//      入口数据：x,y    起点坐标
//                num    要显示的字符
//                color  颜色
//      返回值：  无
//******************************************************************************/
void OLCD114::LCD_ShowChar(short int x,short int y,uint8_t num,short int color)
{
  uint8_t pos,t,temp;
  short int x1=x;
  if(x>LCD_W-16||y>LCD_H-16)return;     //设置窗口       
  num=num-' ';//得到偏移后的值
  LCD_Address_Set(x,y,x+8-1,y+16-1);      //设置光标位置 
    for(pos=0;pos<16;pos++)
    {
       temp=pgm_read_byte(&asc2_1608[num*16+pos]);   //调用1608字体
//        temp=asc2_1608[(short int)num*16+pos];     //调用1608字体
       for(t=0;t<8;t++)
        {
            if(temp&0x01)LCD_DrawPoint(x+t,y+pos,color);//画一个点
            else LCD_DrawPoint(x+t,y+pos,WHITE);
            temp>>=1;
        }
    }
}

/******************************************************************************
      函数说明：显示数字
      入口数据：x,y    起点坐标
                num    要显示的数字
                len    要显示的数字个数
      返回值：  无
******************************************************************************/
void OLCD114::LCD_ShowNum1(short int x,short int y,float num,uint8_t len,short int color)
{      
  OLED_CS_Clr();       
  uint8_t t,temp;
  uint8_t enshow=0;
  short int num1;
  num1=num*100;
  for(t=0;t<len;t++)
  {
    temp=(num1/mypow(10,len-t-1))%10;
    if(t==(len-2))
    {
      LCD_ShowChar(x+8*(len-2),y,'.',color);
      t++;
      len+=1;
    }
    LCD_ShowChar(x+8*t,y,temp+48,color);
  }
  OLED_CS_Set();
}
/******************************************************************************
      函数说明：显示40x40图片
      入口数据：x,y    起点坐标
      返回值：  无
******************************************************************************/
void OLCD114::LCD_ShowPicture(short int x1,short int y1,short int x2,short int y2)
{
  OLED_CS_Clr();  
  int i,j,temp1,temp2;
  LCD_Address_Set(x1,y1,x2,y2);
  for(i=0;i<1600;i++)
  {
    temp1=pgm_read_byte(&image[i*2+1]);
    temp2=pgm_read_byte(&image[i*2]);
    LCD_WR_DATA8(temp1);
    LCD_WR_DATA8(temp2);
  }    
  OLED_CS_Set(); 
}
void OLCD114::LCD_ShowPicture()
{
  OLED_CS_Clr();  
  short int i,j;    
  LCD_Address_Set(0,0,LCD_W-1,LCD_H-1);
  for( i = 0 ; i < LCD_H * LCD_W; i++)
  {
    LCD_WR_DATA((*(framebuffer + i)));
  }
  OLED_CS_Set();
}
