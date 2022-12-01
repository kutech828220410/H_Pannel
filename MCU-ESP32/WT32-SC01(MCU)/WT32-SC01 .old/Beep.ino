//#define BEEP 0
//MyTimer Beep_Timer;
//
//long cnt_Beep = 65534;
//void BeepInit()
//{
//  ledcSetup(0, 5000, 8);
//  ledcAttachPin(BEEP, 0);
//  ledcWrite(0, 0);
//}
//void PlayBeep()
//{
//  if(cnt_Beep == 65534)
//  {
//     BeepInit();
//     cnt_Beep = 65535;
//  }
//  if(cnt_Beep == 65535)
//  {
//     cnt_Beep = 1;
//  }
//  if(cnt_Beep == 1)
//  {
//     if(flag_PlayBeep)
//     {
//       cnt_Beep++;
//     }     
//  }  
//  if(cnt_Beep == 2)
//  {
//     ledcWrite(BEEP, 128);
//     Beep_Timer.StartTickTime(100);
//     cnt_Beep++;
//  }
//  if(cnt_Beep == 3)
//  {
//    if(Beep_Timer.IsTimeOut())
//    {
//      ledcWrite(BEEP, 0);
//      cnt_Beep++;
//    }    
//  }
//  if(cnt_Beep == 4)
//  {
//     cnt_Beep = 65500;
//  }
//  if(cnt_Beep == 65500)
//  {
//     flag_PlayBeep = false;
//     cnt_Beep = 65535;
//  } 
//}
