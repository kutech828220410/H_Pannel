#line 1 "C:\\Users\\Evan\\source\\repos\\H_Pannel\\MCU-BW16\\BW16_EPD266\\Output.h"
#ifndef __Output_h
#define __Output_h
#include "Timer.h"
#include "Config.h"
typedef void (*OutputHandle) (void);
#ifdef MCP23017
#include "DFRobot_MCP23017.h"


#define cSetb0    0x0001
#define cSetb1    0x0002
#define cSetb2    0x0004
#define cSetb3    0x0008
#define cSetb4    0x0010
#define cSetb5    0x0020
#define cSetb6    0x0040
#define cSetb7    0x0080
#define cSetb8    0x0080
#define cSetb9    0x0080
#define cSetb10    0x0080
#define cSetb11    0x0080
#define cSetb12    0x0080
#define cSetb13    0x0080
#define cSetb14    0x0080
#define cSetb15    0x0080


class MyOutput
{
   public:
   DFRobot_MCP23017 *_mcp;
   bool flag_mcp = false;
   bool flag_toogle = false;
   bool Trigger = false;
   bool State = false;
   bool State_ON = false;
   bool State_OFF = false;
   bool BlinkEnable = false;
   void Init(int PIN_Num);
   void Init(int PIN_Num ,DFRobot_MCP23017& mcp);
   void Init(int PIN_Num , bool flag_toogle);
   void Set_toggle(bool value);
   void Set_State(bool ON_OFF);
   void Set_StateEx(bool ON_OFF);
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
#else

class MyOutput
{
   public:
   bool flag_toogle = false;
   bool Trigger = false;
   bool State = false;
   bool ADC_Mode = false;
   void Init(int PIN_Num);
   void Init(int PIN_Num_I,int PIN_Num_O);
   void Init(int PIN_Num , bool flag_toogle);
   
   void Set_toggle(bool value);
   void Set_State(bool ON_OFF);
   void ADC_Trigger(int time_ms);
   void Blink(int Time);
   void Blink();
   int OnDelayTime = -1;
   int OnDelayTime_buf = -2;
   int ADC_OnDelayTime = -1;
   int ADC_OnDelayTime_buf = -2;
   OutputHandle Output_ON = nullptr;
   OutputHandle Output_OFF = nullptr;
   private:  
   bool state = false;
   bool GetLogic(bool value);
   void Normal_Blink();
   void ADC_Blink();
   int PIN_NUM = -1;
   int PIN_NUM_INPUT = -1;
   int cnt = 254;
   MyTimer myTimer;
};
#endif



#endif
