#include "KeyBoard123.h"
#include "Button.h"
#include "Fonts.h"
#include "FT62XXTouchScreen.h"
#include <TFT_eSPI.h>

KeyBoard123::KeyBoard123()
{

}

void KeyBoard123::Init()
{
  
  button_Num[1].Init(location_X + Button_Size_X * 0,location_Y + Button_Size_Y * 0, Button_Size_X ,Button_Size_Y ,"1","1");
  button_Num[2].Init(location_X + Button_Size_X * 1,location_Y + Button_Size_Y * 0, Button_Size_X ,Button_Size_Y ,"2","2");
  button_Num[3].Init(location_X + Button_Size_X * 2,location_Y + Button_Size_Y * 0, Button_Size_X ,Button_Size_Y ,"3","3");
  button_Num[4].Init(location_X + Button_Size_X * 0,location_Y + Button_Size_Y * 1, Button_Size_X ,Button_Size_Y ,"4","4");
  button_Num[5].Init(location_X + Button_Size_X * 1,location_Y + Button_Size_Y * 1, Button_Size_X ,Button_Size_Y ,"5","5");
  button_Num[6].Init(location_X + Button_Size_X * 2,location_Y + Button_Size_Y * 1, Button_Size_X ,Button_Size_Y ,"6","6");
  button_Num[7].Init(location_X + Button_Size_X * 0,location_Y + Button_Size_Y * 2, Button_Size_X ,Button_Size_Y ,"7","7");
  button_Num[8].Init(location_X + Button_Size_X * 1,location_Y + Button_Size_Y * 2, Button_Size_X ,Button_Size_Y ,"8","8");
  button_Num[9].Init(location_X + Button_Size_X * 2,location_Y + Button_Size_Y * 2, Button_Size_X ,Button_Size_Y ,"9","9");
  
  button_Num[0].Init(location_X + Button_Size_X * 3,location_Y + Button_Size_Y * 0, Button_Size_X ,Button_Size_Y ,"0","0");
  button_CE.Init(location_X + Button_Size_X * 3,location_Y + Button_Size_Y * 1, Button_Size_X ,Button_Size_Y ,"CE","CE");
  button_Enter.Init(location_X + Button_Size_X * 3,location_Y + Button_Size_Y * 2, Button_Size_X ,Button_Size_Y ,"Enter","Enter");
  
  Value_buf = 0;
  Value = 0;
  this -> _TEXT = "";
  this -> TEXT_Buf = " ";
}
void KeyBoard123::DrawText(TFT_eSPI tft)
{
   if(_TEXT.length() > 5)
   {
     this -> _TEXT = this -> _TEXT.substring(0,5);
   }
   if( this -> TEXT_Buf != this -> _TEXT)
   {
     Value_buf = _TEXT.toInt();
     _TEXT = String(Value_buf);
     int Text_Height = 25;
     int Text_Width = tft.textWidth(this -> _TEXT,FONT4);
     tft.fillRect(this->location_X, this->location_Y - Text_Height - 15, Button_Size_X * 4, Text_Height + 14, TFT_BLACK);
     tft.drawRect(this->location_X, this->location_Y - Text_Height - 15, Button_Size_X * 4, Text_Height + 14, TFT_YELLOW);
     tft.setTextColor(TFT_WHITE , TFT_BLACK); 
     tft.drawString(this -> _TEXT, this->location_X + Button_Size_X * 4  - Text_Width - 10, this->location_Y - Text_Height + 9  - 15,FONT4);
     this -> TEXT_Buf = this -> _TEXT;
   }
  
}

int KeyBoard123::Show(TFT_eSPI tft , TouchPoint touchPos)
{
  
  this -> DrawText(tft);
  for(int i = 0 ; i < 10 ; i++)
  {
     if(button_Num[i].Draw(tft , touchPos ) == 1)
     {
         this -> _TEXT += button_Num[i].Text_ON;
     }        
  }
  if(button_CE.Draw(tft , touchPos ) == 1)
  {
     this -> _TEXT = "0";
  }
  if(button_Cancel.Draw(tft , touchPos ) == 1)
  {
    
  }
  if(button_Enter.Draw(tft , touchPos ) == 1)
  {
     if(Value_buf > 0)
     {
         Value = Value_buf;
         return 1;     
     }
  }
  return 0;
}
