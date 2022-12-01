#ifndef _keyBoardABC_h
#define _keyBoardABC_h
#include <TFT_eSPI.h>
#include "Button.h"
#include "FT62XXTouchScreen.h"
#define MODE_ENG 0
#define MODE_eng 1
#define MODE_SYMBOL 2
 
class KeyBoardABC
{
 public:   
  KeyBoardABC(); 
  void Init();
  String TEXT = "";
  int Show(TFT_eSPI tft , TouchPoint touchPos); 
  void button_touchdown_change_MODE();
  bool IsShown = true;
 private:  
  String _TEXT = "";    
  String TEXT_Buf = " ";
  int MODE = 0;
  void Set_MODE();
  void DrawText(TFT_eSPI tft);
  Button button_ENG[26];
  Button button_eng[26];
  Button button_SYMBOL[26];
  Button button_Num[10];
  Button button_funtinon[4];
  int location_X = 0;
  int location_Y = 100;
  int Button_Size_X = 45;
  int Button_Size_Y = 45;   
  int Size_X = Button_Size_X * 10;
  int Size_Y = Button_Size_Y * 4;
  int MODE_buf = -1;        
};

#endif
 
