void Page02_Init()
{
   
   String _SSID = "SSID : " + wiFiConfig.Get_SSID_Str();
   String _Password = "Password : " + wiFiConfig.Get_Password_Str();
   String _ServerIP = "ServerIP : " + wiFiConfig.Get_Server_IPAdress_Str();
   String _ServerPort = "ServerPort : " + wiFiConfig.Get_Serverport_Str();
   Button_Page02_SSID.Init(0 ,0 , 480, 45, _SSID, _SSID);
   Button_Page02_Password.Init(0 ,45 , 480, 45, _Password, _Password);
   Button_Page02_ServerIP.Init(0 ,90 , 480, 45, _ServerIP, _ServerIP);
   Button_Page02_ServerPort.Init(0 ,135 , 480, 45, _ServerPort, _ServerPort);
     
   Screen_Button_Page02_IP.Init(0 ,320 - 45 , 150, 45,"IP","IP");   
   Screen_Button_Page02_SSID.Init(160 ,320 - 45 , 150, 45,"SSID","SSID");
   Screen_Button_Page02_SSID.BGColor_OFF = TFT_BLACK;
   Screen_Button_Page02_SSID.FontColor_OFF = TFT_WHITE;
   Screen_Button_Page02_Function.Init(320 ,320 - 45 , 150, 45,"Function","Function");
   
   Button_Page02_SSID.ButtonPressDown = Button_Page02_SSID_PressDown;
   Button_Page02_Password.ButtonPressDown = Button_Page02_Password_PressDown;
   Button_Page02_ServerIP.ButtonPressDown = Button_Page02_ServerIP_PressDown;
   Button_Page02_ServerPort.ButtonPressDown = Button_Page02_ServerPort_PressDown;  
   
   Screen_Button_Page02_IP.ButtonPressDown = Screen_Button_Page02_IP_PressDown;
   Screen_Button_Page02_SSID.ButtonPressDown = Screen_Button_Page02_SSID_PressDown;
   Screen_Button_Page02_Function.ButtonPressDown = Screen_Button_Page02_Function_PressDown;
}
void Page02_Refresh()
{
   tft.fillScreen(TFT_WHITE);
   String _SSID = "SSID : " + wiFiConfig.Get_SSID_Str();
   String _Password = "Password : " + wiFiConfig.Get_Password_Str();
   String _ServerIP = "ServerIP : " + wiFiConfig.Get_Server_IPAdress_Str();
   String _ServerPort = "ServerPort : " + wiFiConfig.Get_Serverport_Str();
   Button_Page02_SSID.Set_Text(_SSID);
   Button_Page02_Password.Set_Text(_Password);
   Button_Page02_ServerIP.Set_Text(_ServerIP);
   Button_Page02_ServerPort.Set_Text(_ServerPort);
   
   Button_Page02_SSID.Set_State(0);
   Button_Page02_Password.Set_State(0);
   Button_Page02_ServerIP.Set_State(0);
   Button_Page02_ServerPort.Set_State(0);
   Screen_Button_Page02_IP.Set_State(0);  
   Screen_Button_Page02_SSID.Set_State(0);  
   Screen_Button_Page02_Function.Set_State(0);  
}
void Page02_Draw()
{
   //TouchPoint touchPos = TouchScreen_Read();
     
   Button_Page02_SSID.Draw(tft , touchPos);
   Button_Page02_Password.Draw(tft , touchPos); 
   Button_Page02_ServerIP.Draw(tft , touchPos); 
   Button_Page02_ServerPort.Draw(tft , touchPos); 
   Screen_Button_Page02_IP.Draw(tft , touchPos);  
   Screen_Button_Page02_SSID.Draw(tft , touchPos);  
   Screen_Button_Page02_Function.Draw(tft , touchPos);
}


void Button_Page02_SSID_PressDown()
{
  while(true)
  {
    if(keyBoardABC.Show(tft , touchPos) == 1)
    {
       touchPos.touched = false;
       break;
    }
    esp_task_wdt_reset();
    delay(1);
  }
  Screen_Page02_Init = true;

  String str_SSID = keyBoardABC.TEXT;
  String str = "SSID : " + keyBoardABC.TEXT;
  Button_Page02_SSID.Text_ON = str;
  Button_Page02_SSID.Text_OFF = str;
  wiFiConfig.Set_SSID(str_SSID);
  Serial.println(str);
}
void Button_Page02_Password_PressDown()
{
  while(true)
  {
    if(keyBoardABC.Show(tft , touchPos) == 1)
    {
       touchPos.touched = false;
       break;
    }
    esp_task_wdt_reset();
    delay(1);
  }
  Screen_Page02_Init = true;

  String str_Password = keyBoardABC.TEXT;
  String str = "Password : " + str_Password;
  Button_Page02_Password.Text_ON = str;
  Button_Page02_Password.Text_OFF = str;
  wiFiConfig.Set_Password(str_Password);
  Serial.println(str);
}
void Button_Page02_ServerIP_PressDown()
{
  while(true)
  {
    if(keyBoardABC.Show(tft , touchPos) == 1)
    {
       touchPos.touched = false;
       break;
    }
    esp_task_wdt_reset();
    delay(1);
  }
  
  Screen_Page02_Init = true;
  String IPA,IPB,IPC,IPD;

  int index = 0;
  int str_num = 0;
  String Text = keyBoardABC.TEXT;
  while(true)
  {
    index = Text.indexOf('.');
    if(index != -1)
    {     
      if( str_num == 0)IPA = Text.substring(0 ,index );
      if( str_num == 1)IPB = Text.substring(0 ,index );
      if( str_num == 2)IPC = Text.substring(0 ,index );
      Text = Text.substring(index + 1 , Text.length());
      str_num++;
    }
    else
    {
      IPD = Text;
      str_num++;
      break;
    }
  }
  int iPA = 999;
  int iPB = 999;
  int iPC = 999;
  int iPD = 999;  
  bool flag_OK = true ;
  iPA = Get_value(IPA);
  iPB = Get_value(IPB);
  iPC = Get_value(IPC);
  iPD = Get_value(IPD);
  if(iPA < 0 || iPA > 255) flag_OK = false ;
  if(iPB < 0 || iPB > 255) flag_OK = false ;
  if(iPC < 0 || iPC > 255) flag_OK = false ;
  if(iPD < 0 || iPD > 255) flag_OK = false ;
  if( str_num != 4 ) flag_OK = false;
  if(flag_OK)
  {
    String ip_str = "ServerIP : " + String(iPA) + "." + String(iPB) + "." + String(iPC) + "." + String(iPD);
    Button_Page02_ServerIP.Text_ON = ip_str;
    Button_Page02_ServerIP.Text_OFF = ip_str;

    wiFiConfig.Set_Server_IPAdress((byte)iPA,(byte)iPB,(byte)iPC,(byte)iPD);
    Serial.println(ip_str);
  }
  else
  {
    Serial.println("IPAdress write error!");
  }
}
void Button_Page02_ServerPort_PressDown()
{
  while(true)
  {
    if(keyBoardABC.Show(tft , touchPos) == 1)
    {
       touchPos.touched = false;
       break;
    }
    esp_task_wdt_reset();
    delay(1);
  }
  Screen_Page02_Init = true;

  String Str_Port = keyBoardABC.TEXT;
  int _port = Get_value(Str_Port);
  if( _port > 0 )
  {
    String str = "ServerPort : " + String(_port);
    Button_Page02_ServerPort.Text_ON = str;
    Button_Page02_ServerPort.Text_OFF = str;
    wiFiConfig.Set_Serverport(_port);
    Serial.println(str);
  }
  else
  {
    Serial.println("Port write error!");
  }
}

void Screen_Button_Page02_IP_PressDown()
{
   Screen_Page01_Init = true;
}
void Screen_Button_Page02_SSID_PressDown()
{
   
}
void Screen_Button_Page02_Function_PressDown()
{
   Screen_Page03_Init = true;
}
