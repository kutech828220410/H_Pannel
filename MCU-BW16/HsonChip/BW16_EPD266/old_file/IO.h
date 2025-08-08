#ifndef IO_H
#define IO_H

#include "Arduino.h"
#include "Output.h"
#include "Input.h" 
#include "WiFiConfig.h"
#include "Global.h"

extern MyOutput MyOutput_PIN01, MyOutput_PIN02, MyOutput_PIN03, MyOutput_PIN04, MyOutput_PIN05;
extern MyOutput MyOutput_PIN06, MyOutput_PIN07, MyOutput_PIN08, MyOutput_PIN09, MyOutput_PIN10;

extern MyInput MyInput_PIN01, MyInput_PIN02, MyInput_PIN03, MyInput_PIN04, MyInput_PIN05;
extern MyInput MyInput_PIN06, MyInput_PIN07, MyInput_PIN08, MyInput_PIN09, MyInput_PIN10;

extern int Input_buf, Input, Output_buf, Output;
extern int Input_dir_buf, Input_dir, Output_dir_buf, Output_dir;


void IO_Init();
void sub_IO_Program();
void Output_Blink();

void Set_Input_dir(byte pin_num, bool value);
void Set_Input_dir(int value);
int Get_Input_dir();

void Set_Output_dir(byte pin_num, bool value);
void Set_Output_dir(int value);
int Get_Output_dir();

int GetInput();
int GetOutput();

void SetOutputTrigger(int value);
void SetOutputPINTrigger(byte pin_num, bool value);
void SetOutput(int value);
void SetOutputPIN(byte pin_num, bool value);

#ifdef MCP23017
void SetOutputEx(int value);
#endif

#endif
