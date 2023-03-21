#ifndef __Output_h
#define __Output_h
#include "Timer.h"
typedef void (*OutputHandle) (void);
class MyOutput
{
   public:
   bool flag_toogle = false;
   bool Trigger = false;
   bool State = false;
   bool BlinkEnable = false;
   void Init(int PIN_Num);
   void Init(int PIN_Num , bool flag_toogle);
   void Init(int PIN_Num_I,int PIN_Num_O);
   void Set_toggle(bool value);
   void Set_State(bool ON_OFF);
   void Blink(int Time);
   void Blink();
   int OffDelayTime = -1;
   int OffDelayTime_buf = -2;
   int OnDelayTime = -1;
   int OnDelayTime_buf = -2;
   OutputHandle Output_ON = nullptr;
   OutputHandle Output_OFF = nullptr;
   private:  
   bool state = false;
   bool GetLogic(bool value);
   int PIN_NUM = -1;
   int PIN_NUM_INPUT = -1;
   int cnt = 254;
   MyTimer myTimer;
};


#endif
