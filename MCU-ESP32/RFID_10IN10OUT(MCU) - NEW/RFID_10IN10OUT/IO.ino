#define INPUT_PIN01 22
#define INPUT_PIN02 23
#define INPUT_PIN03 25
#define INPUT_PIN04 26
#define INPUT_PIN05 32
#define INPUT_PIN06 33
#define INPUT_PIN07 34
#define INPUT_PIN08 35
#define INPUT_PIN09 36
#define INPUT_PIN10 39

#define OUTPUT_PIN01 2
#define OUTPUT_PIN02 4
#define OUTPUT_PIN03 5
#define OUTPUT_PIN04 13
#define OUTPUT_PIN05 14
#define OUTPUT_PIN06 15
#define OUTPUT_PIN07 18
#define OUTPUT_PIN08 19
#define OUTPUT_PIN09 21
#define OUTPUT_PIN10 27

MyOutput MyOutput_PIN01;
MyOutput MyOutput_PIN02;
MyOutput MyOutput_PIN03;
MyOutput MyOutput_PIN04;
MyOutput MyOutput_PIN05;
MyOutput MyOutput_PIN06;
MyOutput MyOutput_PIN07;
MyOutput MyOutput_PIN08;
MyOutput MyOutput_PIN09;
MyOutput MyOutput_PIN10;

MyInput MyInput_PIN01;
MyInput MyInput_PIN02;
MyInput MyInput_PIN03;
MyInput MyInput_PIN04;
MyInput MyInput_PIN05;
MyInput MyInput_PIN06;
MyInput MyInput_PIN07;
MyInput MyInput_PIN08;
MyInput MyInput_PIN09;
MyInput MyInput_PIN10;

int Input_buf = -1;
int Input = 0;
int Output_buf = -1;
int Output = 0;
int Input_dir_buf = -1;
int Input_dir = 0;
int Output_dir_buf = -1;
int Output_dir = 0;
int LED_Input[5];
int LED_Output[5];

bool flag_Init = false;
void IO_Init()
{       

    Input_dir = wiFiConfig.Get_Input_dir();
    Set_Input_dir(Input_dir);
    Output_dir = wiFiConfig.Get_Output_dir();
    Set_Output_dir(Output_dir);
    for(int i = 0 ; i < 5 ; i ++)
    {
       LED_Input[i] = wiFiConfig.Get_LED_InputPIN(i);
       LED_Output[i] = wiFiConfig.Get_LED_OutputPIN(i);
    }
    
    MyOutput_PIN01.Init(OUTPUT_PIN01);
    MyOutput_PIN02.Init(OUTPUT_PIN02);
    MyOutput_PIN03.Init(OUTPUT_PIN03);
    MyOutput_PIN04.Init(OUTPUT_PIN04);
    MyOutput_PIN05.Init(OUTPUT_PIN05);
    MyOutput_PIN06.Init(OUTPUT_PIN06);
    MyOutput_PIN07.Init(OUTPUT_PIN07);
    MyOutput_PIN08.Init(OUTPUT_PIN08);
    MyOutput_PIN09.Init(OUTPUT_PIN09);
    MyOutput_PIN10.Init(OUTPUT_PIN10);
    
    MyInput_PIN01.Init(INPUT_PIN01);
    MyInput_PIN02.Init(INPUT_PIN02);
    MyInput_PIN03.Init(INPUT_PIN03);
    MyInput_PIN04.Init(INPUT_PIN04);
    MyInput_PIN05.Init(INPUT_PIN05);
    MyInput_PIN06.Init(INPUT_PIN06);
    MyInput_PIN07.Init(INPUT_PIN07);
    MyInput_PIN08.Init(INPUT_PIN08);
    MyInput_PIN09.Init(INPUT_PIN09);
    MyInput_PIN10.Init(INPUT_PIN10);

  
}
bool flag_WL_CONNECTED = false;
bool flag_WL_DISCONNECTED = false;
void sub_IO_Program()
{

    if(!flag_Init)
    {
       IO_Init();
       flag_Init = true;
    }
    for(int i = 0 ; i < 5 ; i ++)
    {
       LED_Input[i] = wiFiConfig.LED_InputPIN[i];
       LED_Output[i] = wiFiConfig.LED_OutputPIN[i];
    }
    Input = GetInput();
    OutputLED_Blink();
    Output = GetOutput();
    Input_dir = Get_Input_dir();
    Output_dir = Get_Output_dir();
    
    if(WiFi.status() == WL_CONNECTED)
    {
       if(flag_WL_CONNECTED)
       {
         // Set_Output_dir(wiFiConfig.Get_Output_dir());
          flag_WL_CONNECTED = false;
       }
       
       flag_WL_DISCONNECTED = true;
    }
    else
    {
       if(flag_WL_DISCONNECTED)
       {
//          SetOutputPINTrigger(1 , true);
//          SetOutputPINTrigger(2 , true);
//          SetOutputPINTrigger(3 , true);
//          SetOutputPINTrigger(4 , true);
//          SetOutputPINTrigger(5 , true);
//          SetOutputPINTrigger(6 , true);
//          SetOutputPINTrigger(7 , true);
//          SetOutputPINTrigger(8 , true);
//          SetOutputPINTrigger(9 , true);
//          SetOutputPINTrigger(10 , true);
//          Set_Output_dir(0xFFFF);
          flag_WL_DISCONNECTED = false;
       }
       flag_WL_CONNECTED = true;
    }
    Output_Blink();

    
    if(Input_buf != Input)
    {       
       if(flag_udp_232back)Serial.printf("Input : %d\n" , Input);
       Input_buf = Input;
       flag_JsonSend = true;
    }
    if(Output_buf != Output)
    {       
       if(flag_udp_232back)Serial.printf("Output : %d\n" , Output);
       Output_buf = Output;
       flag_JsonSend = true;
    }
    if(Input_dir_buf != Input_dir)
    {       
       if(flag_udp_232back)Serial.printf("Input_dir : %d\n" , Input_dir);
       Input_dir_buf = Input_dir;
       flag_JsonSend = true;
    }
    if(Output_dir_buf != Output_dir)
    {       
       if(flag_udp_232back)Serial.printf("Output_dir : %d\n" , Output_dir);
       Output_dir_buf = Output_dir;
       flag_JsonSend = true;
    }
}
void OutputLED_Blink()
{
   int input_index = -1;
   int output_index = -1;
   MyOutput_PIN01.BlinkEnable = false;
   MyOutput_PIN02.BlinkEnable = false;
   MyOutput_PIN03.BlinkEnable = false;
   MyOutput_PIN04.BlinkEnable = false;
   MyOutput_PIN05.BlinkEnable = false;
   for(int i = 0 ; i < 5 ; i++)
   {
      input_index = LED_Input[i];
      output_index = LED_Output[i];
      if(input_index < 0 || input_index >=10) continue;
      if(output_index < 0 || output_index >=10) continue;
      if(output_index == 0) MyOutput_PIN01.BlinkEnable = ((Input >> input_index) % 2 == 0);
      else if(output_index == 1) MyOutput_PIN02.BlinkEnable = ((Input >> input_index) % 2 == 0);
      else if(output_index == 2) MyOutput_PIN03.BlinkEnable = ((Input >> input_index) % 2 == 0);
      else if(output_index == 3) MyOutput_PIN04.BlinkEnable = ((Input >> input_index) % 2 == 0);
      else if(output_index == 4) MyOutput_PIN05.BlinkEnable = ((Input >> input_index) % 2 == 0);
      else if(output_index == 5) MyOutput_PIN06.BlinkEnable = ((Input >> input_index) % 2 == 0);
      else if(output_index == 6) MyOutput_PIN07.BlinkEnable = ((Input >> input_index) % 2 == 0);
      else if(output_index == 7) MyOutput_PIN08.BlinkEnable = ((Input >> input_index) % 2 == 0);
      else if(output_index == 8) MyOutput_PIN09.BlinkEnable = ((Input >> input_index) % 2 == 0);
      else if(output_index == 9) MyOutput_PIN10.BlinkEnable = ((Input >> input_index) % 2 == 0);
   }
}
void Output_Blink()
{
    MyOutput_PIN01.Blink(200);
    MyOutput_PIN02.Blink(200);
    MyOutput_PIN03.Blink(200);
    MyOutput_PIN04.Blink(200);
    MyOutput_PIN05.Blink(200);
    MyOutput_PIN06.Blink(200);
    MyOutput_PIN07.Blink(200);
    MyOutput_PIN08.Blink(200);
    MyOutput_PIN09.Blink(200);
    MyOutput_PIN10.Blink(200);
}
void Set_Input_dir(byte pin_num , bool value)
{
    if(pin_num == 0) MyInput_PIN01.Set_toggle(value);
    else if(pin_num == 1) MyInput_PIN02.Set_toggle(value);
    else if(pin_num == 2) MyInput_PIN03.Set_toggle(value);
    else if(pin_num == 3) MyInput_PIN04.Set_toggle(value);
    else if(pin_num == 4) MyInput_PIN05.Set_toggle(value);
    else if(pin_num == 5) MyInput_PIN06.Set_toggle(value);
    else if(pin_num == 6) MyInput_PIN07.Set_toggle(value);
    else if(pin_num == 7) MyInput_PIN08.Set_toggle(value);
    else if(pin_num == 8) MyInput_PIN09.Set_toggle(value);
    else if(pin_num == 9) MyInput_PIN10.Set_toggle(value);
}
void Set_Input_dir(int value)
{
    if(((value >> 0) % 2 ) ==  1) MyInput_PIN01.Set_toggle(true);
    else MyInput_PIN01.Set_toggle(false);
    if(((value >> 1) % 2 ) ==  1) MyInput_PIN02.Set_toggle(true);
    else MyInput_PIN02.Set_toggle(false);
    if(((value >> 2) % 2 ) ==  1) MyInput_PIN03.Set_toggle(true);
    else MyInput_PIN03.Set_toggle(false);
    if(((value >> 3) % 2 ) ==  1) MyInput_PIN04.Set_toggle(true);
    else MyInput_PIN04.Set_toggle(false);
    if(((value >> 4) % 2 ) ==  1) MyInput_PIN05.Set_toggle(true);
    else MyInput_PIN05.Set_toggle(false);
    if(((value >> 5) % 2 ) ==  1) MyInput_PIN06.Set_toggle(true);
    else MyInput_PIN06.Set_toggle(false);
    if(((value >> 6) % 2 ) ==  1) MyInput_PIN07.Set_toggle(true);
    else MyInput_PIN07.Set_toggle(false);
    if(((value >> 7) % 2 ) ==  1) MyInput_PIN08.Set_toggle(true);
    else MyInput_PIN08.Set_toggle(false);
    if(((value >> 8) % 2 ) ==  1) MyInput_PIN09.Set_toggle(true);
    else MyInput_PIN09.Set_toggle(false);
    if(((value >> 9) % 2 ) ==  1) MyInput_PIN10.Set_toggle(true);
    else MyInput_PIN10.Set_toggle(false);
}
int Get_Input_dir()
{
    int temp = 0;
    if(MyInput_PIN01.flag_toogle) temp += (1 << 0);
    if(MyInput_PIN02.flag_toogle) temp += (1 << 1);
    if(MyInput_PIN03.flag_toogle) temp += (1 << 2);
    if(MyInput_PIN04.flag_toogle) temp += (1 << 3);
    if(MyInput_PIN05.flag_toogle) temp += (1 << 4);
    if(MyInput_PIN06.flag_toogle) temp += (1 << 5);
    if(MyInput_PIN07.flag_toogle) temp += (1 << 6);
    if(MyInput_PIN08.flag_toogle) temp += (1 << 7);
    if(MyInput_PIN09.flag_toogle) temp += (1 << 8);
    if(MyInput_PIN10.flag_toogle) temp += (1 << 9);
    return temp;
}
void Set_Output_dir(byte pin_num , bool value)
{
    if(pin_num == 0) MyOutput_PIN01.Set_toggle(value);
    else if(pin_num == 1) MyOutput_PIN02.Set_toggle(value);
    else if(pin_num == 2) MyOutput_PIN03.Set_toggle(value);
    else if(pin_num == 3) MyOutput_PIN04.Set_toggle(value);
    else if(pin_num == 4) MyOutput_PIN05.Set_toggle(value);
    else if(pin_num == 5) MyOutput_PIN06.Set_toggle(value);
    else if(pin_num == 6) MyOutput_PIN07.Set_toggle(value);
    else if(pin_num == 7) MyOutput_PIN08.Set_toggle(value);
    else if(pin_num == 8) MyOutput_PIN09.Set_toggle(value);
    else if(pin_num == 9) MyOutput_PIN10.Set_toggle(value);
}
void Set_Output_dir(int value)
{
    if(((value >> 0) % 2 ) ==  1) MyOutput_PIN01.Set_toggle(true);
    else MyOutput_PIN01.Set_toggle(false);
    if(((value >> 1) % 2 ) ==  1) MyOutput_PIN02.Set_toggle(true);
    else MyOutput_PIN02.Set_toggle(false);
    if(((value >> 2) % 2 ) ==  1) MyOutput_PIN03.Set_toggle(true);
    else MyOutput_PIN03.Set_toggle(false);
    if(((value >> 3) % 2 ) ==  1) MyOutput_PIN04.Set_toggle(true);
    else MyOutput_PIN04.Set_toggle(false);
    if(((value >> 4) % 2 ) ==  1) MyOutput_PIN05.Set_toggle(true);
    else MyOutput_PIN05.Set_toggle(false);
    if(((value >> 5) % 2 ) ==  1) MyOutput_PIN06.Set_toggle(true);
    else MyOutput_PIN06.Set_toggle(false);
    if(((value >> 6) % 2 ) ==  1) MyOutput_PIN07.Set_toggle(true);
    else MyOutput_PIN07.Set_toggle(false);
    if(((value >> 7) % 2 ) ==  1) MyOutput_PIN08.Set_toggle(true);
    else MyOutput_PIN08.Set_toggle(false);
    if(((value >> 8) % 2 ) ==  1) MyOutput_PIN09.Set_toggle(true);
    else MyOutput_PIN09.Set_toggle(false);
    if(((value >> 9) % 2 ) ==  1) MyOutput_PIN10.Set_toggle(true);
    else MyOutput_PIN10.Set_toggle(false);
}
int Get_Output_dir()
{
    int temp = 0;
    if(MyOutput_PIN01.flag_toogle) temp += (1 << 0);
    if(MyOutput_PIN02.flag_toogle) temp += (1 << 1);
    if(MyOutput_PIN03.flag_toogle) temp += (1 << 2);
    if(MyOutput_PIN04.flag_toogle) temp += (1 << 3);
    if(MyOutput_PIN05.flag_toogle) temp += (1 << 4);
    if(MyOutput_PIN06.flag_toogle) temp += (1 << 5);
    if(MyOutput_PIN07.flag_toogle) temp += (1 << 6);
    if(MyOutput_PIN08.flag_toogle) temp += (1 << 7);
    if(MyOutput_PIN09.flag_toogle) temp += (1 << 8);
    if(MyOutput_PIN10.flag_toogle) temp += (1 << 9);
    return temp;

}

int GetInput()
{
    int temp = 0;
    MyInput_PIN01.GetState(100);
    MyInput_PIN02.GetState(100);
    MyInput_PIN03.GetState(100);
    MyInput_PIN04.GetState(100);
    MyInput_PIN05.GetState(100);
    MyInput_PIN06.GetState(100);
    MyInput_PIN07.GetState(100);
    MyInput_PIN08.GetState(100);
    MyInput_PIN09.GetState(100);
    MyInput_PIN10.GetState(100);
    
    if(MyInput_PIN01.State) temp += (1 << 0);
    if(MyInput_PIN02.State) temp += (1 << 1);
    if(MyInput_PIN03.State) temp += (1 << 2);
    if(MyInput_PIN04.State) temp += (1 << 3);
    if(MyInput_PIN05.State) temp += (1 << 4);
    if(MyInput_PIN06.State) temp += (1 << 5);
    if(MyInput_PIN07.State) temp += (1 << 6);
    if(MyInput_PIN08.State) temp += (1 << 7);
    if(MyInput_PIN09.State) temp += (1 << 8);
    if(MyInput_PIN10.State) temp += (1 << 9);
  
    return temp;
}

int GetOutput()
{
    int temp = 0;
    
    if(MyOutput_PIN01.State && !MyOutput_PIN01.BlinkEnable) temp += (1 << 0);
    if(MyOutput_PIN02.State && !MyOutput_PIN02.BlinkEnable) temp += (1 << 1);
    if(MyOutput_PIN03.State && !MyOutput_PIN03.BlinkEnable) temp += (1 << 2);
    if(MyOutput_PIN04.State && !MyOutput_PIN04.BlinkEnable) temp += (1 << 3);
    if(MyOutput_PIN05.State && !MyOutput_PIN05.BlinkEnable) temp += (1 << 4);
    if(MyOutput_PIN06.State && !MyOutput_PIN06.BlinkEnable) temp += (1 << 5);
    if(MyOutput_PIN07.State && !MyOutput_PIN07.BlinkEnable) temp += (1 << 6);
    if(MyOutput_PIN08.State && !MyOutput_PIN08.BlinkEnable) temp += (1 << 7);
    if(MyOutput_PIN09.State && !MyOutput_PIN09.BlinkEnable) temp += (1 << 8);
    if(MyOutput_PIN10.State && !MyOutput_PIN10.BlinkEnable) temp += (1 << 9);
    return temp;
}
void SetOutputTrigger(int value)
{
    if(((value >> 0) % 2 ) ==  1) MyOutput_PIN01.Trigger = true;
    if(((value >> 1) % 2 ) ==  1) MyOutput_PIN02.Trigger = true;
    if(((value >> 2) % 2 ) ==  1) MyOutput_PIN03.Trigger = true;
    if(((value >> 3) % 2 ) ==  1) MyOutput_PIN04.Trigger = true;
    if(((value >> 4) % 2 ) ==  1) MyOutput_PIN05.Trigger = true;
    if(((value >> 5) % 2 ) ==  1) MyOutput_PIN06.Trigger = true;
    if(((value >> 6) % 2 ) ==  1) MyOutput_PIN07.Trigger = true;
    if(((value >> 7) % 2 ) ==  1) MyOutput_PIN08.Trigger = true;
    if(((value >> 8) % 2 ) ==  1) MyOutput_PIN09.Trigger = true;
    if(((value >> 9) % 2 ) ==  1) MyOutput_PIN10.Trigger = true;
}
void SetOutputPINTrigger(byte pin_num , bool value)
{
    if(pin_num == 0) MyOutput_PIN01.Trigger = true;
    else if(pin_num == 1) MyOutput_PIN02.Trigger = true;
    else if(pin_num == 2) MyOutput_PIN03.Trigger = true;
    else if(pin_num == 3) MyOutput_PIN04.Trigger = true;
    else if(pin_num == 4) MyOutput_PIN05.Trigger = true;
    else if(pin_num == 5) MyOutput_PIN06.Trigger = true;
    else if(pin_num == 6) MyOutput_PIN07.Trigger = true;
    else if(pin_num == 7) MyOutput_PIN08.Trigger = true;
    else if(pin_num == 8) MyOutput_PIN09.Trigger = true;
    else if(pin_num == 9) MyOutput_PIN10.Trigger = true;
 
}
void SetOutput(int value)
{
    if(((value >> 0) % 2 ) ==  0) MyOutput_PIN01.Set_State(true);
    else MyOutput_PIN01.Set_State(false);
    if(((value >> 1) % 2 ) ==  0) MyOutput_PIN02.Set_State(true);
    else MyOutput_PIN02.Set_State(false);
    if(((value >> 2) % 2 ) ==  0) MyOutput_PIN03.Set_State(true);
    else MyOutput_PIN03.Set_State(false);
    if(((value >> 3) % 2 ) ==  0) MyOutput_PIN04.Set_State(true);
    else MyOutput_PIN04.Set_State(false);
    if(((value >> 4) % 2 ) ==  0) MyOutput_PIN05.Set_State(true);
    else MyOutput_PIN05.Set_State(false);
    if(((value >> 5) % 2 ) ==  0) MyOutput_PIN06.Set_State(true);
    else MyOutput_PIN06.Set_State(false);
    if(((value >> 6) % 2 ) ==  0) MyOutput_PIN07.Set_State(true);
    else MyOutput_PIN07.Set_State(false);
    if(((value >> 7) % 2 ) ==  0) MyOutput_PIN08.Set_State(true);
    else MyOutput_PIN08.Set_State(false);
    if(((value >> 8) % 2 ) ==  0) MyOutput_PIN09.Set_State(true);
    else MyOutput_PIN09.Set_State(false);
    if(((value >> 9) % 2 ) ==  0) MyOutput_PIN10.Set_State(true);
    else MyOutput_PIN10.Set_State(false);   
}
void SetOutputPIN(byte pin_num , bool value)
{
    if(pin_num == 0) MyOutput_PIN01.Set_State(value);
    else if(pin_num == 1) MyOutput_PIN02.Set_State(value);
    else if(pin_num == 2) MyOutput_PIN03.Set_State(value);
    else if(pin_num == 3) MyOutput_PIN04.Set_State(value);
    else if(pin_num == 4) MyOutput_PIN05.Set_State(value);
    else if(pin_num == 5) MyOutput_PIN06.Set_State(value);
    else if(pin_num == 6) MyOutput_PIN07.Set_State(value);
    else if(pin_num == 7) MyOutput_PIN08.Set_State(value);
    else if(pin_num == 8) MyOutput_PIN09.Set_State(value);
    else if(pin_num == 9) MyOutput_PIN10.Set_State(value);
}
