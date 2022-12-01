void Page03_Init()
{

   Button_Page03_Connect.Init(0 ,270 - 45 , 150, 45,"Connect","Connect");   
   Button_Page03_Update.Init(160 ,270 - 45 , 150, 45,"Update","Update");   
   Button_Page03_UDPTest.Init(320 ,270 - 45 , 150, 45,"UDPTest","UDPTest");   
   
   
   Screen_Button_Page03_IP.Init(0 ,320 - 45 , 150, 45,"IP","IP");   
   Screen_Button_Page03_SSID.Init(160 ,320 - 45 , 150, 45,"SSID","SSID");
   Screen_Button_Page03_Function.Init(320 ,320 - 45 , 150, 45,"Function","Function");
   Screen_Button_Page03_Function.BGColor_OFF = TFT_BLACK;
   Screen_Button_Page03_Function.FontColor_OFF = TFT_WHITE;
  
   Screen_Button_Page03_IP.ButtonPressDown = Screen_Button_Page03_IP_PressDown;
   Screen_Button_Page03_SSID.ButtonPressDown = Screen_Button_Page03_SSID_PressDown;
   Screen_Button_Page03_Function.ButtonPressDown = Screen_Button_Page03_Function_PressDown;
   Button_Page03_Connect.ButtonPressDown = Button_Page03_Connect_PressDown;
   Button_Page03_UDPTest.ButtonPressDown = Button_Page03_UDPTest_PressDown;
   Button_Page03_Update.ButtonPressDown = Button_Page03_Update_PressDown;
}
void Page03_Refresh()
{
   tft.fillScreen(TFT_WHITE);
   Button_Page03_Connect.Set_State(0);  
   Button_Page03_UDPTest.Set_State(0);  
   Button_Page03_Update.Set_State(0);  
   Screen_Button_Page03_IP.Set_State(0);  
   Screen_Button_Page03_SSID.Set_State(0);  
   Screen_Button_Page03_Function.Set_State(0);  
}
void Page03_Draw()
{
   //TouchPoint touchPos = TouchScreen_Read();
   
   Button_Page03_Connect.Draw(tft, touchPos);
   Button_Page03_UDPTest.Draw(tft, touchPos);
   Button_Page03_Update.Draw(tft, touchPos);
   Screen_Button_Page03_IP.Draw(tft , touchPos);  
   Screen_Button_Page03_SSID.Draw(tft , touchPos);  
   Screen_Button_Page03_Function.Draw(tft , touchPos);

   tft.drawString(Version , 400 , 0,FONT2);
  
}
void Screen_Button_Page03_IP_PressDown()
{
   Screen_Page01_Init = true;
}
void Screen_Button_Page03_SSID_PressDown()
{
   Screen_Page02_Init = true;
}
void Screen_Button_Page03_Function_PressDown()
{
   
}
void Button_Page03_Connect_PressDown()
{
  wiFiConfig.Init(Version);
  wiFiConfig.WIFI_Connenct();
  Connect_UDP(Localport);
}
void Button_Page03_UDPTest_PressDown()
{
  if(WiFi.status() == WL_CONNECTED)
  {
    IPAddress ip = wiFiConfig.Get_Server_IPAdressClass();
    int port = wiFiConfig.Get_Serverport();
    Send_StringTo("TEST", ip, port);
  }
  
}
void Button_Page03_Update_PressDown()
{
  if(WiFi.status() == WL_CONNECTED)
  {
    OTAUpdate();
  }
}


void OTAUpdate()
{
//    int retry = 0;
//    while(true)
//    {
//      if(retry >= 3) break;
//      WiFiClient client;
//      t_httpUpdate_return ret = httpUpdate.update(client, wiFiConfig.Get_Server_IPAdress_Str(), 80 , "/update.bin");
//      switch (ret) 
//      {
//        case HTTP_UPDATE_FAILED:
//          Serial.printf("HTTP_UPDATE_FAILED Error (%d): %s\n", httpUpdate.getLastError(), httpUpdate.getLastErrorString().c_str());
//          break;
//  
//        case HTTP_UPDATE_NO_UPDATES:
//          Serial.println("HTTP_UPDATE_NO_UPDATES");
//          break;
//  
//        case HTTP_UPDATE_OK:
//          {
//            wiFiConfig.Set_IsUpdate(true);
//            Serial.println("HTTP_UPDATE_OK");
//            return;
//          }
//          
//          break;
//      }
//      retry++;
//      delay(1000);
//    }
    
    
}
