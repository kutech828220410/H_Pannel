#include "KeyBoardABC.h"
#include "Button.h"
#include "Fonts.h"
#include "FT62XXTouchScreen.h"
#include <TFT_eSPI.h>
 KeyBoardABC::KeyBoardABC()
 {

 }
 void KeyBoardABC::Init()
 {
    
    for(int i = 0 ; i < 10 ; i++)
    {
        if(i ==0)button_Num[i].Init(location_X + Button_Size_X * i,location_Y + Button_Size_Y * 1,Button_Size_X,Button_Size_Y ,"1","1");
        if(i ==1)button_Num[i].Init(location_X + Button_Size_X * i,location_Y + Button_Size_Y * 1,Button_Size_X,Button_Size_Y ,"2","2");
        if(i ==2)button_Num[i].Init(location_X + Button_Size_X * i,location_Y + Button_Size_Y * 1,Button_Size_X,Button_Size_Y ,"3","3");
        if(i ==3)button_Num[i].Init(location_X + Button_Size_X * i,location_Y + Button_Size_Y * 1,Button_Size_X,Button_Size_Y ,"4","4");
        if(i ==4)button_Num[i].Init(location_X + Button_Size_X * i,location_Y + Button_Size_Y * 1,Button_Size_X,Button_Size_Y ,"5","5");
        if(i ==5)button_Num[i].Init(location_X + Button_Size_X * i,location_Y + Button_Size_Y * 1,Button_Size_X,Button_Size_Y ,"6","6");
        if(i ==6)button_Num[i].Init(location_X + Button_Size_X * i,location_Y + Button_Size_Y * 1,Button_Size_X,Button_Size_Y ,"7","7");
        if(i ==7)button_Num[i].Init(location_X + Button_Size_X * i,location_Y + Button_Size_Y * 1,Button_Size_X,Button_Size_Y ,"8","8");
        if(i ==8)button_Num[i].Init(location_X + Button_Size_X * i,location_Y + Button_Size_Y * 1,Button_Size_X,Button_Size_Y ,"9","9");
        if(i ==9)button_Num[i].Init(location_X + Button_Size_X * i,location_Y + Button_Size_Y * 1,Button_Size_X,Button_Size_Y ,"0","0");
    }
    for(int i = 0 ; i < 26 ; i++)
    {
      if(i < 10)
      {      
        if(i ==0)button_ENG[i].Init(location_X + Button_Size_X * i,location_Y + Button_Size_Y * 2,Button_Size_X,Button_Size_Y ,"A","A");
        if(i ==1)button_ENG[i].Init(location_X + Button_Size_X * i,location_Y + Button_Size_Y * 2,Button_Size_X,Button_Size_Y ,"B","B");
        if(i ==2)button_ENG[i].Init(location_X + Button_Size_X * i,location_Y + Button_Size_Y * 2,Button_Size_X,Button_Size_Y ,"C","C");
        if(i ==3)button_ENG[i].Init(location_X + Button_Size_X * i,location_Y + Button_Size_Y * 2,Button_Size_X,Button_Size_Y ,"D","D");
        if(i ==4)button_ENG[i].Init(location_X + Button_Size_X * i,location_Y + Button_Size_Y * 2,Button_Size_X,Button_Size_Y ,"E","E");
        if(i ==5)button_ENG[i].Init(location_X + Button_Size_X * i,location_Y + Button_Size_Y * 2,Button_Size_X,Button_Size_Y ,"F","F");
        if(i ==6)button_ENG[i].Init(location_X + Button_Size_X * i,location_Y + Button_Size_Y * 2,Button_Size_X,Button_Size_Y ,"G","G");
        if(i ==7)button_ENG[i].Init(location_X + Button_Size_X * i,location_Y + Button_Size_Y * 2,Button_Size_X,Button_Size_Y ,"H","H");
        if(i ==8)button_ENG[i].Init(location_X + Button_Size_X * i,location_Y + Button_Size_Y * 2,Button_Size_X,Button_Size_Y ,"I","I");
        if(i ==9)button_ENG[i].Init(location_X + Button_Size_X * i,location_Y + Button_Size_Y * 2,Button_Size_X,Button_Size_Y ,"J","J");
      }
      else if(i >= 10 && i < 20)
      {
        if(i ==10)button_ENG[i].Init(location_X + Button_Size_X * (i - 10),location_Y + Button_Size_Y * 3,Button_Size_X,Button_Size_Y ,"K","K");
        if(i ==11)button_ENG[i].Init(location_X + Button_Size_X * (i - 10),location_Y + Button_Size_Y * 3,Button_Size_X,Button_Size_Y ,"L","L");
        if(i ==12)button_ENG[i].Init(location_X + Button_Size_X * (i - 10),location_Y + Button_Size_Y * 3,Button_Size_X,Button_Size_Y ,"M","M");
        if(i ==13)button_ENG[i].Init(location_X + Button_Size_X * (i - 10),location_Y + Button_Size_Y * 3,Button_Size_X,Button_Size_Y ,"N","N");
        if(i ==14)button_ENG[i].Init(location_X + Button_Size_X * (i - 10),location_Y + Button_Size_Y * 3,Button_Size_X,Button_Size_Y ,"O","O");
        if(i ==15)button_ENG[i].Init(location_X + Button_Size_X * (i - 10),location_Y + Button_Size_Y * 3,Button_Size_X,Button_Size_Y ,"P","P");
        if(i ==16)button_ENG[i].Init(location_X + Button_Size_X * (i - 10),location_Y + Button_Size_Y * 3,Button_Size_X,Button_Size_Y ,"Q","Q");
        if(i ==17)button_ENG[i].Init(location_X + Button_Size_X * (i - 10),location_Y + Button_Size_Y * 3,Button_Size_X,Button_Size_Y ,"R","R");
        if(i ==18)button_ENG[i].Init(location_X + Button_Size_X * (i - 10),location_Y + Button_Size_Y * 3,Button_Size_X,Button_Size_Y ,"S","S");
        if(i ==19)button_ENG[i].Init(location_X + Button_Size_X * (i - 10),location_Y + Button_Size_Y * 3,Button_Size_X,Button_Size_Y ,"T","T");
      }
      else if(i >= 20 && i < 26)
      {
      
        if(i ==20)button_ENG[i].Init(location_X + Button_Size_X * (i - 20),location_Y + Button_Size_Y * 4,Button_Size_X,Button_Size_Y ,"U","U");
        if(i ==21)button_ENG[i].Init(location_X + Button_Size_X * (i - 20),location_Y + Button_Size_Y * 4,Button_Size_X,Button_Size_Y ,"V","V");
        if(i ==22)button_ENG[i].Init(location_X + Button_Size_X * (i - 20),location_Y + Button_Size_Y * 4,Button_Size_X,Button_Size_Y ,"W","W");
        if(i ==23)button_ENG[i].Init(location_X + Button_Size_X * (i - 20),location_Y + Button_Size_Y * 4,Button_Size_X,Button_Size_Y ,"X","X");
        if(i ==24)button_ENG[i].Init(location_X + Button_Size_X * (i - 20),location_Y + Button_Size_Y * 4,Button_Size_X,Button_Size_Y ,"Y","Y");
        if(i ==25)button_ENG[i].Init(location_X + Button_Size_X * (i - 20),location_Y + Button_Size_Y * 4,Button_Size_X,Button_Size_Y ,"Z","Z");   
      }           
    }
    for(int i = 0 ; i < 26 ; i++)
    {
      if(i < 10)
      {      
        if(i ==0)button_eng[i].Init(location_X + Button_Size_X * i,location_Y + Button_Size_Y * 2,Button_Size_X,Button_Size_Y ,"a","a");
        if(i ==1)button_eng[i].Init(location_X + Button_Size_X * i,location_Y + Button_Size_Y * 2,Button_Size_X,Button_Size_Y ,"b","b");
        if(i ==2)button_eng[i].Init(location_X + Button_Size_X * i,location_Y + Button_Size_Y * 2,Button_Size_X,Button_Size_Y ,"c","c");
        if(i ==3)button_eng[i].Init(location_X + Button_Size_X * i,location_Y + Button_Size_Y * 2,Button_Size_X,Button_Size_Y ,"d","d");
        if(i ==4)button_eng[i].Init(location_X + Button_Size_X * i,location_Y + Button_Size_Y * 2,Button_Size_X,Button_Size_Y ,"e","e");
        if(i ==5)button_eng[i].Init(location_X + Button_Size_X * i,location_Y + Button_Size_Y * 2,Button_Size_X,Button_Size_Y ,"f","f");
        if(i ==6)button_eng[i].Init(location_X + Button_Size_X * i,location_Y + Button_Size_Y * 2,Button_Size_X,Button_Size_Y ,"g","g");
        if(i ==7)button_eng[i].Init(location_X + Button_Size_X * i,location_Y + Button_Size_Y * 2,Button_Size_X,Button_Size_Y ,"h","h");
        if(i ==8)button_eng[i].Init(location_X + Button_Size_X * i,location_Y + Button_Size_Y * 2,Button_Size_X,Button_Size_Y ,"i","i");
        if(i ==9)button_eng[i].Init(location_X + Button_Size_X * i,location_Y + Button_Size_Y * 2,Button_Size_X,Button_Size_Y ,"j","j");
      }
      else if(i >= 10 && i < 20)
      {
        if(i ==10)button_eng[i].Init(location_X + Button_Size_X * (i - 10),location_Y + Button_Size_Y * 3,Button_Size_X,Button_Size_Y ,"k","k");
        if(i ==11)button_eng[i].Init(location_X + Button_Size_X * (i - 10),location_Y + Button_Size_Y * 3,Button_Size_X,Button_Size_Y ,"l","l");
        if(i ==12)button_eng[i].Init(location_X + Button_Size_X * (i - 10),location_Y + Button_Size_Y * 3,Button_Size_X,Button_Size_Y ,"m","m");
        if(i ==13)button_eng[i].Init(location_X + Button_Size_X * (i - 10),location_Y + Button_Size_Y * 3,Button_Size_X,Button_Size_Y ,"n","n");
        if(i ==14)button_eng[i].Init(location_X + Button_Size_X * (i - 10),location_Y + Button_Size_Y * 3,Button_Size_X,Button_Size_Y ,"o","o");
        if(i ==15)button_eng[i].Init(location_X + Button_Size_X * (i - 10),location_Y + Button_Size_Y * 3,Button_Size_X,Button_Size_Y ,"p","p");
        if(i ==16)button_eng[i].Init(location_X + Button_Size_X * (i - 10),location_Y + Button_Size_Y * 3,Button_Size_X,Button_Size_Y ,"q","q");
        if(i ==17)button_eng[i].Init(location_X + Button_Size_X * (i - 10),location_Y + Button_Size_Y * 3,Button_Size_X,Button_Size_Y ,"r","r");
        if(i ==18)button_eng[i].Init(location_X + Button_Size_X * (i - 10),location_Y + Button_Size_Y * 3,Button_Size_X,Button_Size_Y ,"s","s");
        if(i ==19)button_eng[i].Init(location_X + Button_Size_X * (i - 10),location_Y + Button_Size_Y * 3,Button_Size_X,Button_Size_Y ,"t","t");
      }
      else if(i >= 20 && i < 26)
      {      
        if(i ==20)button_eng[i].Init(location_X + Button_Size_X * (i - 20),location_Y + Button_Size_Y * 4,Button_Size_X,Button_Size_Y ,"u","u");
        if(i ==21)button_eng[i].Init(location_X + Button_Size_X * (i - 20),location_Y + Button_Size_Y * 4,Button_Size_X,Button_Size_Y ,"v","v");
        if(i ==22)button_eng[i].Init(location_X + Button_Size_X * (i - 20),location_Y + Button_Size_Y * 4,Button_Size_X,Button_Size_Y ,"w","w");
        if(i ==23)button_eng[i].Init(location_X + Button_Size_X * (i - 20),location_Y + Button_Size_Y * 4,Button_Size_X,Button_Size_Y ,"x","x");
        if(i ==24)button_eng[i].Init(location_X + Button_Size_X * (i - 20),location_Y + Button_Size_Y * 4,Button_Size_X,Button_Size_Y ,"y","y");
        if(i ==25)button_eng[i].Init(location_X + Button_Size_X * (i - 20),location_Y + Button_Size_Y * 4,Button_Size_X,Button_Size_Y ,"z","z");   
      }           
    }
    for(int i = 0 ; i < 26 ; i++)
    {
      if(i < 10)
      {      
        if(i ==0)button_SYMBOL[i].Init(location_X + Button_Size_X * i,location_Y + Button_Size_Y * 2,Button_Size_X,Button_Size_Y ,"!","!");
        if(i ==1)button_SYMBOL[i].Init(location_X + Button_Size_X * i,location_Y + Button_Size_Y * 2,Button_Size_X,Button_Size_Y ,"@","@");
        if(i ==2)button_SYMBOL[i].Init(location_X + Button_Size_X * i,location_Y + Button_Size_Y * 2,Button_Size_X,Button_Size_Y ,"#","#");
        if(i ==3)button_SYMBOL[i].Init(location_X + Button_Size_X * i,location_Y + Button_Size_Y * 2,Button_Size_X,Button_Size_Y ,"$","$");
        if(i ==4)button_SYMBOL[i].Init(location_X + Button_Size_X * i,location_Y + Button_Size_Y * 2,Button_Size_X,Button_Size_Y ,"%","%");
        if(i ==5)button_SYMBOL[i].Init(location_X + Button_Size_X * i,location_Y + Button_Size_Y * 2,Button_Size_X,Button_Size_Y ,"^","^");
        if(i ==6)button_SYMBOL[i].Init(location_X + Button_Size_X * i,location_Y + Button_Size_Y * 2,Button_Size_X,Button_Size_Y ,"&","&");
        if(i ==7)button_SYMBOL[i].Init(location_X + Button_Size_X * i,location_Y + Button_Size_Y * 2,Button_Size_X,Button_Size_Y ,"*","*");
        if(i ==8)button_SYMBOL[i].Init(location_X + Button_Size_X * i,location_Y + Button_Size_Y * 2,Button_Size_X,Button_Size_Y ,"(","(");
        if(i ==9)button_SYMBOL[i].Init(location_X + Button_Size_X * i,location_Y + Button_Size_Y * 2,Button_Size_X,Button_Size_Y ,")",")");
      }
      else if(i >= 10 && i < 20)
      {
        if(i ==10)button_SYMBOL[i].Init(location_X + Button_Size_X * (i - 10),location_Y + Button_Size_Y * 3,Button_Size_X,Button_Size_Y ,"-","-");
        if(i ==11)button_SYMBOL[i].Init(location_X + Button_Size_X * (i - 10),location_Y + Button_Size_Y * 3,Button_Size_X,Button_Size_Y ,"_","_");
        if(i ==12)button_SYMBOL[i].Init(location_X + Button_Size_X * (i - 10),location_Y + Button_Size_Y * 3,Button_Size_X,Button_Size_Y ,"+","+");
        if(i ==13)button_SYMBOL[i].Init(location_X + Button_Size_X * (i - 10),location_Y + Button_Size_Y * 3,Button_Size_X,Button_Size_Y ,"=","=");
        if(i ==14)button_SYMBOL[i].Init(location_X + Button_Size_X * (i - 10),location_Y + Button_Size_Y * 3,Button_Size_X,Button_Size_Y ,"/","/");
        if(i ==15)button_SYMBOL[i].Init(location_X + Button_Size_X * (i - 10),location_Y + Button_Size_Y * 3,Button_Size_X,Button_Size_Y ,"\\","\\");
        if(i ==16)button_SYMBOL[i].Init(location_X + Button_Size_X * (i - 10),location_Y + Button_Size_Y * 3,Button_Size_X,Button_Size_Y ,"<","<");
        if(i ==17)button_SYMBOL[i].Init(location_X + Button_Size_X * (i - 10),location_Y + Button_Size_Y * 3,Button_Size_X,Button_Size_Y ,">",">");
        if(i ==18)button_SYMBOL[i].Init(location_X + Button_Size_X * (i - 10),location_Y + Button_Size_Y * 3,Button_Size_X,Button_Size_Y ,"[","[");
        if(i ==19)button_SYMBOL[i].Init(location_X + Button_Size_X * (i - 10),location_Y + Button_Size_Y * 3,Button_Size_X,Button_Size_Y ,"]","]");
      }
      else if(i >= 20 && i < 26)
      {    
        if(i ==20)button_SYMBOL[i].Init(location_X + Button_Size_X * (i - 20),location_Y + Button_Size_Y * 4,Button_Size_X,Button_Size_Y ,".",".");
        if(i ==21)button_SYMBOL[i].Init(location_X + Button_Size_X * (i - 20),location_Y + Button_Size_Y * 4,Button_Size_X,Button_Size_Y ,",",",");
        if(i ==22)button_SYMBOL[i].Init(location_X + Button_Size_X * (i - 20),location_Y + Button_Size_Y * 4,Button_Size_X,Button_Size_Y ,"?","?");
        if(i ==23)button_SYMBOL[i].Init(location_X + Button_Size_X * (i - 20),location_Y + Button_Size_Y * 4,Button_Size_X,Button_Size_Y ,":",":");
        if(i ==24)button_SYMBOL[i].Init(location_X + Button_Size_X * (i - 20),location_Y + Button_Size_Y * 4,Button_Size_X,Button_Size_Y ,";",";");
        if(i ==25)button_SYMBOL[i].Init(location_X + Button_Size_X * (i - 20),location_Y + Button_Size_Y * 4,Button_Size_X,Button_Size_Y ,"~","~");   
      }           
    }
    int last_X = 0;
    int last_Y = 0;
    last_X = location_X + Button_Size_X * 5;
    last_Y = location_Y + Button_Size_Y * 4;
    last_X += Button_Size_X;
    button_funtinon[0].Init(last_X , last_Y , Button_Size_X,Button_Size_Y ," "," ");
    button_funtinon[0].Set_Font_SIZE(FONT2 , FONT2);
    last_X += Button_Size_X;
    button_funtinon[1].Init(last_X , last_Y , Button_Size_X ,Button_Size_Y ,"back","back");
    button_funtinon[1].Set_Font_SIZE(FONT2 , FONT2);
    last_X += Button_Size_X;
    button_funtinon[2].Init(last_X , last_Y , Button_Size_X ,Button_Size_Y ,"ENT","ENT");
    button_funtinon[2].Set_Font_SIZE(FONT2 , FONT2);
    last_X += Button_Size_X;
    button_funtinon[3].Init(last_X , last_Y , Button_Size_X ,Button_Size_Y ,"<->","<->");
    button_funtinon[3].Set_Font_SIZE(FONT2 , FONT2);
    this -> _TEXT = "";
    this -> TEXT_Buf = " ";
  
 }
 void KeyBoardABC::DrawText(TFT_eSPI tft)
 {
     if(_TEXT.length() > 20)
     {
       this -> _TEXT = this -> _TEXT.substring(0,20);
     }
     if( this -> TEXT_Buf != this -> _TEXT)
     {
       tft.fillRect(this->location_X, this->location_Y, Button_Size_X * 10, Button_Size_Y, TFT_BLACK);
       tft.drawRect(this->location_X, this->location_Y, Button_Size_X * 10, Button_Size_Y, TFT_YELLOW);
       tft.setTextColor(TFT_WHITE , TFT_BLACK); 
       tft.drawString(this -> _TEXT, this->location_X + 10, this->location_Y + 12,FONT4);
       this -> TEXT_Buf = this -> _TEXT;
     }
    
 }

 int KeyBoardABC::Show(TFT_eSPI tft , TouchPoint touchPos)
 {

    this -> DrawText(tft);
    for(int i = 0 ; i < 10 ; i++)
    {
       if(button_Num[i].Draw(tft , touchPos ) == 1)
       {
           this -> _TEXT += button_Num[i].Text_ON;
       }        
    }
    if(this -> MODE == MODE_ENG)
    {
      for(int i = 0 ; i < 26 ; i++)
      {
        if(button_ENG[i].Draw(tft , touchPos ) == 1)
        {
            this -> _TEXT += button_ENG[i].Text_ON;
        }     
      }
    }
    if(this -> MODE == MODE_eng)
    {
      for(int i = 0 ; i < 26 ; i++)
      {
        if(button_eng[i].Draw(tft , touchPos ) == 1)
        {
            this -> _TEXT += button_eng[i].Text_ON;
        }       
      }
    }
    if(this -> MODE == MODE_SYMBOL)
    {
      for(int i = 0 ; i < 26 ; i++)
      {
         if(button_SYMBOL[i].Draw(tft , touchPos ) == 1)
        {
            this -> _TEXT += button_SYMBOL[i].Text_ON;
        }       
      }
    }
    if(button_funtinon[0].Draw(tft , touchPos ) == 1)
    {
      this -> _TEXT += button_funtinon[0].Text_ON;
    } 
    if(button_funtinon[1].Draw(tft , touchPos ) == 1)
    {
      this -> _TEXT  = this -> _TEXT.substring(0,this -> _TEXT.length() -1);
    }
    if(button_funtinon[2].Draw(tft , touchPos ) == 1)
    {
      this -> TEXT  = this -> _TEXT;
      this -> Init();
      return 1;
    }
    if(button_funtinon[3].Draw(tft , touchPos ) == 1)
    {      
       this -> Set_MODE();
    }
    return 0;
 }
 void KeyBoardABC::Set_MODE()
 {
    int _mode = this -> MODE;
    _mode++;
    if(_mode > 2) _mode = 0;
    this -> MODE = _mode;
    if(this -> MODE == MODE_ENG)
    {
      for(int i = 0 ; i < 26 ; i++)
      {
         button_ENG[i].Set_State(0);      
      }
    }
    else if(this -> MODE == MODE_eng)
    {
      for(int i = 0 ; i < 26 ; i++)
      {
         button_eng[i].Set_State(0);      
      }
    }
    else if(this -> MODE == MODE_SYMBOL)
    {
      for(int i = 0 ; i < 26 ; i++)
      {
         button_SYMBOL[i].Set_State(0);      
      }
    }
 }

 
