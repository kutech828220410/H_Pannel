#line 1 "C:\\Users\\Evan\\source\\repos\\H_Pannel\\MCU-BW16\\BW16_EPD266\\Timer.h"
#ifndef __Timer_h
#define __Timer_h
#include "Arduino.h"
class MyTimer
{
   public:   
    void StartTickTime(long TickTime);
    void TickStop();
    bool IsTimeOut();
    
    
   private:  
    long start_Time = 0;
    long TickTime = 0;
    bool OnTick = false;
    
};

#endif
