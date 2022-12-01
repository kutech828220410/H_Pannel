#include "Button.h"
#include "FT62XXTouchScreen.h"
#include <TFT_eSPI.h>
 Button::Button()
 {
  
 }
 Button::Button(int location_X,int location_Y,int Size_X,int Size_Y, String Text_ON, String Text_OFF)
 {
     this -> location_X = location_X;
     this -> location_Y = location_Y;
     this -> Size_X = Size_X;
     this -> Size_Y = Size_Y;
     this -> Text_ON = Text_ON;
     this -> Text_OFF = Text_OFF;
     this -> state = 0;
     this -> state_buf = -1;
 }
 void Button::Init(int location_X,int location_Y,int Size_X,int Size_Y, String Text_ON, String Text_OFF)
 {
     this -> location_X = location_X;
     this -> location_Y = location_Y;
     this -> Size_X = Size_X;
     this -> Size_Y = Size_Y;
     this -> Text_ON = Text_ON;
     this -> Text_OFF = Text_OFF;
     this -> state = 0;
     this -> state_buf = -1;
 }
 int Button::Draw(TFT_eSPI tft, TouchPoint touchPos)
 {
    if(IsPress(touchPos))state = BUTTON_ON;
    else state = BUTTON_OFF;
    return this -> Button::Draw(tft);        
 }
 int Button::Draw(TFT_eSPI tft)
 {
    if(state_buf != state)
    {
      if(state == BUTTON_ON)
       {
           this -> Draw_ON_State(tft);
           if(ButtonPressDown != nullptr) ButtonPressDown();
           
       }
       else if(state == BUTTON_OFF)
       {
           this -> Draw_OFF_State(tft);
           if(ButtonPressUp != nullptr) ButtonPressUp();
       }
       state_buf  = state ;
       return state;
    } 
    return -1;         
 }
 void Button::Draw_OFF_State(TFT_eSPI tft)
 {
     tft.setCursor(0, 0);
     int Text_Width =  tft.textWidth(Text_OFF, FONT_SIZE_OFF);
     int Text_Height = tft.fontHeight(FONT_SIZE_OFF);
     int Text_location_X = location_X + (Size_X / 2) - (Text_Width / 2);
     int Text_location_Y = location_Y + (Size_Y / 2) - (Text_Height / 2) ;
     tft.fillRect(location_X, location_Y, Size_X, Size_Y, BGColor_OFF);
     
     tft.drawRect(location_X, location_Y, Size_X, Size_Y, TFT_BLACK);
     tft.setTextColor(FontColor_OFF , BGColor_OFF); 
     tft.drawString(Text_OFF, Text_location_X, Text_location_Y,FONT_SIZE_OFF);
 } 
 void Button::Draw_ON_State(TFT_eSPI tft)
 {
     tft.setCursor(0, 0);
     int Text_Width =  tft.textWidth(Text_ON, FONT_SIZE_ON);
     int Text_Height = tft.fontHeight(FONT_SIZE_ON);
     int Text_location_X = location_X + (Size_X / 2) - (Text_Width / 2);
     int Text_location_Y = location_Y + (Size_Y / 2) - (Text_Height / 2) ;
     tft.fillRect(location_X, location_Y, Size_X, Size_Y, BGColor_ON);
     tft.drawRect(location_X, location_Y, Size_X, Size_Y, TFT_BLACK);
     tft.setTextColor(FontColor_ON , BGColor_ON); 
     tft.drawString(Text_ON, Text_location_X, Text_location_Y,FONT_SIZE_ON);
 }
 
 void Button::Set_Text(String text)
 {
    this -> Text_ON = text;
    this -> Text_OFF = text;
 }
  void Button::Set_State(uint16_t state)
 {
    this -> state = state;
    this -> state_buf = -1;
 }
 void Button::Set_Font_SIZE(uint16_t ON_SIZE, uint16_t OFF_SIZE)
 {
    this -> FONT_SIZE_ON = ON_SIZE;
    this -> FONT_SIZE_OFF = OFF_SIZE;
 }
 bool Button::IsPress( TouchPoint touchPos )
 {
    int posX = touchPos.xPos; 
    int posY = touchPos.yPos;
    if(!touchPos.touched)
    {      
      return false;
    }
    if((posX >= location_X) && (posX <= location_X + Size_X))
    {
      if((posY >= location_Y) && (posY <= location_Y + Size_Y))
      {
         return true;
      }
    }
    return false;
 }
