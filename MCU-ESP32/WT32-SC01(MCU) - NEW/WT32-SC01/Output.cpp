#include "Timer.h"
#include "Output.h"
#include "Arduino.h"

void MyOutput::Set_toggle(bool value)
{
   this -> flag_toogle = value;
   this -> Set_State(this -> State);
}
void MyOutput::Init(int PIN_Num)
{
   this -> PIN_NUM = PIN_Num ;
   if(PIN_Num != -1)pinMode(PIN_Num, OUTPUT);
   if(PIN_Num != -1)digitalWrite(PIN_Num, this -> GetLogic(false));
}
void MyOutput::Init(int PIN_Num , bool flag_toogle)
{
   this -> flag_toogle = flag_toogle;
   this -> PIN_NUM = PIN_Num ;
   if(PIN_Num != -1)pinMode(PIN_Num, OUTPUT);
   if(PIN_Num != -1)digitalWrite(PIN_Num, this -> GetLogic(false));
}
void MyOutput::Init(int PIN_Num_I,int PIN_Num_O)
{
   this -> PIN_NUM_INPUT = PIN_Num_I ;
   this -> PIN_NUM = PIN_Num_O ;
   if(PIN_Num_O != -1)pinMode(PIN_Num_O, OUTPUT);
   if(PIN_Num_O != -1)digitalWrite(PIN_Num_O, this -> GetLogic(false));
}
void MyOutput::Set_State(bool ON_OFF)
{
    State = ON_OFF;
    if(this -> PIN_NUM != -1)digitalWrite(this -> PIN_NUM, this -> GetLogic(ON_OFF));
}
void MyOutput::Blink(int Time)
{
  this -> OnDelayTime = Time;
  this -> OffDelayTime = Time;
  MyOutput::Blink();
}
bool MyOutput::GetLogic(bool value)
{
  if(this -> flag_toogle == false)
  {
     return value;
  }
  else
  {
    return !value;
  }
}
void MyOutput::Blink()
{
  int PIN = this -> PIN_NUM;

  if( (this -> cnt) == 254)
  {
    State = false;
    if(PIN != -1)digitalWrite(PIN, this -> GetLogic(false));
    (this -> cnt) = 255 ;
  }
  if( this -> OnDelayTime == 1 ) 
  {
    State = true;
    if(PIN != -1)digitalWrite(PIN, this -> GetLogic(true));
    if(Output_ON != nullptr) Output_ON();
    this -> OnDelayTime_buf = OnDelayTime;    
    return;
  }
  if( this -> OffDelayTime == 0 ) 
  {
    State = false;
    if(PIN != -1)digitalWrite(PIN, this -> GetLogic(false));
    if(Output_OFF != nullptr) Output_OFF();
    this -> OffDelayTime_buf = OffDelayTime;   
    return;
  } 
  if( (this -> cnt) == 255)
  {
     Trigger = false;
     (this -> cnt) = 1;
  }
  if( (this -> cnt) == 1)
  {    
    if(BlinkEnable == true) Trigger = true;
    if(Trigger) 
    {
       (this -> cnt) = (this -> cnt) + 1 ;
    }
  }
  if( (this -> cnt) == 2)
  {    
    State = true;
    if(PIN != -1)digitalWrite(PIN, this -> GetLogic(true));
    if(Output_ON != nullptr) Output_ON();
    //Serial.printf("Output ON PIN : %d , OnDelayTime : %d\n",PIN,OnDelayTime);
    myTimer.StartTickTime(this -> OnDelayTime);
    (this -> cnt) = (this -> cnt) + 1 ;
  }
  if( (this -> cnt) == 3)
  {    
    if(PIN_NUM_INPUT == -1)
    {
       (this -> cnt) = (this -> cnt) + 1 ;
    }
    else
    {
       if(digitalRead(PIN_NUM_INPUT))
       {
           myTimer.StartTickTime(this -> OnDelayTime);
          (this -> cnt) = (this -> cnt) + 1 ;
       }
    }
  }
  if( (this -> cnt) == 4)
  {
    if(myTimer.IsTimeOut())
    {
      State = false;
      if(PIN != -1)digitalWrite(PIN, this -> GetLogic(false));
      if(Output_OFF != nullptr) Output_OFF();
      //Serial.printf("Output OFF PIN : %d , OnDelayTime : %d\n",PIN,OnDelayTime);
      myTimer.StartTickTime(this -> OffDelayTime);
      
      (this -> cnt) = (this -> cnt) + 1 ;
    }
  }
  if( (this -> cnt) == 5)
  {
    if(myTimer.IsTimeOut())
    {      
      (this -> cnt) = 255 ;
    }
  }
}
