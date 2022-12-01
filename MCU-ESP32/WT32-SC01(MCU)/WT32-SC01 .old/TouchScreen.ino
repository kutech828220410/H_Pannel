int touch_X, touch_Y;
uint16_t Istouched = false;
TouchPoint TouchScreen_Read()
{
   TouchPoint touchPos = TouchScreen.read();  
   if(touchPos.xPos == 0 && touchPos.yPos == 0) 
   {
       touchPos.touched = false;
   }
   if(touchPos.touched != Istouched)  
   {
      Istouched = touchPos.touched;
   }
   else
   {
      touchPos.touched = false;
   }
  
   return touchPos;
}
