#ifndef __Output_h
#define __Output_h
#include "Timer.h"
typedef void (*OutputHandle) (void);
#include "DFRobot_MCP23017.h"


class MyOutput
{
   public:
   DFRobot_MCP23017 *_mcp;
   bool flag_mcp = false;
   bool flag_toogle = false;
   bool Trigger = false;
   bool State = false;
   bool BlinkEnable = false;
   void Init(int PIN_Num);
   void Init(int PIN_Num ,DFRobot_MCP23017& mcp);
   void Init(int PIN_Num , bool flag_toogle);
   void Set_toggle(bool value);
   void Set_State(bool ON_OFF);
   void Blink(int Time);
   void Blink();
   int OnDelayTime = -1;
   int OnDelayTime_buf = -2;
   OutputHandle Output_ON = nullptr;
   OutputHandle Output_OFF = nullptr;
   private:  
   bool state = false;
   bool GetLogic(bool value);
   int PIN_NUM = -1;
   int cnt = 254;
   MyTimer myTimer;
};


#endif
