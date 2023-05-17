#include "Timer.h"
#include "Input.h"
#include "Arduino.h"


void MyInput::Init(int PIN_Num)
{
   this -> PIN_NUM = PIN_Num ;
   if(PIN_Num != -1)pinMode(PIN_Num, INPUT_PULLUP);
}

void MyInput::Set_toggle(bool value)
{
   this -> flag_toogle = value ;
   printf("set input flag %d  , pin : %d\n" , value , this -> PIN_NUM);
}
void MyInput::GetState(int Time)
{
  this -> OnDelayTime = Time;
  this -> OffDelayTime = Time;
  MyInput::GetState();
}
void MyInput::GetState()
{
  if(PIN_NUM == -1)return;
  int PIN = this -> PIN_NUM;
  this -> flag_state = digitalRead(this -> PIN_NUM);
  if(this -> flag_toogle)
  {
    this -> flag_state = !(this -> flag_state);
  }
  if(this -> flag_state)
  {
     this -> cnt_off = 1;
     if(this -> cnt_on == 1)
     {
        myTimer.TickStop();
        myTimer.StartTickTime(this -> OnDelayTime);
        this -> cnt_on ++;
     }
     if(this -> cnt_on == 2)
     {
        if(myTimer.IsTimeOut())
        {
           this -> State = this -> flag_state;
           if(Input_ON != nullptr) Input_ON();
           //Serial.printf("Input ON PIN : %d , OnDelayTime : %d\n",PIN,OnDelayTime);
           this -> cnt_on ++;
        }
     }
  }
  else
  {
     this -> cnt_on = 1;
     if(this -> cnt_off == 1)
     {
        myTimer.TickStop();
        myTimer.StartTickTime(this -> OffDelayTime);
        this -> cnt_off ++;
     }
     if(this -> cnt_off == 2)
     {
        if(myTimer.IsTimeOut())
        {
           this -> State = this -> flag_state;
           if(Input_OFF != nullptr) Input_OFF();
           //Serial.printf("Input OFF PIN : %d , OnDelayTime : %d\n",PIN,OffDelayTime);
           this -> cnt_off ++;
        }
     }
  }
  
  
}
