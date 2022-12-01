#ifndef __KeyBoard123_h
#define __KeyBoard123_h
#include <TFT_eSPI.h>
#include "Button.h"
#include "FT62XXTouchScreen.h"


class KeyBoard123
{
 public:   
  int Value = 0;
  int Value_buf = 0;
  KeyBoard123(); 
  void Init();
  String TEXT = "";
  int Show(TFT_eSPI tft , TouchPoint touchPos); 
  String _TEXT = "";    
  String TEXT_Buf = " ";
  bool IsShown = false;
 private:  
  void DrawText(TFT_eSPI tft);
  Button button_Num[10];
  Button button_CE;
  Button button_Cancel;
  Button button_Enter;
  int location_X = 90;
  int location_Y = 95;
  int Button_Size_X = 75;
  int Button_Size_Y = 75;   
  int Size_X = Button_Size_X * 10;
  int Size_Y = Button_Size_Y * 4;
};



#endif
