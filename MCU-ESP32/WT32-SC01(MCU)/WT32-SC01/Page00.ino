void Page00_Init()
{   
   Button_Page00_Setting.Init((480 - 250) / 2 ,60 , 250, 200, "Setting Mode", "Setting Mode");
}

void Page00_Refresh()
{
   tft.fillScreen(TFT_WHITE);
   Button_Page00_Setting.Set_State(0); 
}
void Page00_Draw()
{
   Button_Page00_Setting.Draw(tft , touchPos);
}
