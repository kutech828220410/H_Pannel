#ifndef __Timer_h
#define __Timer_h
class MyTimer
{
   public:   
    void StartTickTime(long TickTime);
    void TickStop();
    bool IsTimeOut();
    bool IsTimeOutPulse();
    
   private:  
    long start_Time = 0;
    long TickTime = 0;
    bool OnTick = false;
    
};

#endif
