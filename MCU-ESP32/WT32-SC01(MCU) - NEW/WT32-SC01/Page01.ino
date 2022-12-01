
void Page01_Init()
{

   String ipAdress = "IPAdress : " + wiFiConfig.Get_IPAdress_Str();
   String subnet = "Subnet : " + wiFiConfig.Get_Subnet_Str();
   String gateway = "Gateway : " + wiFiConfig.Get_Gateway_Str();
   String dns = "Dns : " + wiFiConfig.Get_DNS_Str();
   String Port = "localport : " + wiFiConfig.Get_Localport_Str();
   String MacAdress = "MacAdress :" + WiFi.macAddress();
   
   Button_Page01_IPAdress.Init(0 ,0 , 480, 45, ipAdress, ipAdress);
   Button_Page01_Subnet.Init(0 ,45 , 480, 45, subnet, subnet);
   Button_Page01_Gateway.Init(0 ,90 , 480, 45, gateway, gateway);
   Button_Page01_Dns.Init(0 ,135 , 480, 45, dns, dns);
   Button_Page01_Port.Init(0 ,180 , 480, 45, Port, Port);
   Button_Page01_MacAdress.Init(0 , 225 , 480 , 45 , MacAdress , MacAdress);
   
   
   Screen_Button_Page01_IP.Init(0 ,320 - 45 , 150, 45,"IP","IP");   
   Screen_Button_Page01_IP.BGColor_OFF = TFT_BLACK;
   Screen_Button_Page01_IP.FontColor_OFF = TFT_WHITE;
   Screen_Button_Page01_SSID.Init(160 ,320 - 45 , 150, 45,"SSID","SSID");
   Screen_Button_Page01_Function.Init(320 ,320 - 45 , 150, 45,"Function","Function");
   
   Button_Page01_IPAdress.ButtonPressDown = Button_Page01_IPAdress_PressDown;
   Button_Page01_Subnet.ButtonPressDown = Button_Page01_Subnet_PressDown;
   Button_Page01_Gateway.ButtonPressDown = Button_Page01_Gateway_PressDown;
   Button_Page01_Dns.ButtonPressDown = Button_Page01_Dns_PressDown;
   Button_Page01_Port.ButtonPressDown = Button_Page01_Port_PressDown;
   
   Screen_Button_Page01_IP.ButtonPressDown = Screen_Button_Page01_IP_PressDown;
   Screen_Button_Page01_SSID.ButtonPressDown = Screen_Button_Page01_SSID_PressDown;
   Screen_Button_Page01_Function.ButtonPressDown = Screen_Button_Page01_Function_PressDown;
}
void Page01_Refresh()
{
   tft.fillScreen(TFT_WHITE);

   String ipAdress = "IPAdress : " + wiFiConfig.Get_IPAdress_Str();
   String subnet = "Subnet : " + wiFiConfig.Get_Subnet_Str();
   String gateway = "Gateway : " + wiFiConfig.Get_Gateway_Str();
   String dns = "Dns : " + wiFiConfig.Get_DNS_Str();
   String Port = "localport : " + wiFiConfig.Get_Localport_Str();
   String MacAdress = "MacAdress :" + WiFi.macAddress();
   Button_Page01_IPAdress.Set_Text(ipAdress);
   Button_Page01_Subnet.Set_Text(subnet);
   Button_Page01_Gateway.Set_Text(gateway);
   Button_Page01_Dns.Set_Text(dns);
   Button_Page01_Port.Set_Text(Port);
   Button_Page01_MacAdress.Set_Text(MacAdress);
   
   Button_Page01_IPAdress.Set_State(0);
   Button_Page01_Subnet.Set_State(0);
   Button_Page01_Gateway.Set_State(0);
   Button_Page01_Dns.Set_State(0);
   Button_Page01_Port.Set_State(0);  
   Button_Page01_MacAdress.Set_State(0);
        
   Screen_Button_Page01_IP.Set_State(0);  
   Screen_Button_Page01_SSID.Set_State(0);  
   Screen_Button_Page01_Function.Set_State(0);  
}
void Page01_Draw()
{
   //TouchPoint touchPos = TouchScreen_Read();
   Button_Page01_IPAdress.Draw(tft , touchPos);
   Button_Page01_Subnet.Draw(tft , touchPos);
   Button_Page01_Gateway.Draw(tft , touchPos);
   Button_Page01_Dns.Draw(tft , touchPos);       
   Button_Page01_Port.Draw(tft , touchPos);  
   Button_Page01_MacAdress.Draw(tft , touchPos); 
      
   Screen_Button_Page01_IP.Draw(tft , touchPos);
   Screen_Button_Page01_SSID.Draw(tft , touchPos);
   Screen_Button_Page01_Function.Draw(tft , touchPos);
}
void Button_Page01_IPAdress_PressDown()
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
  
  Screen_Page01_Init = true;
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
    String ip_str = "IPAdress : " + String(iPA) + "." + String(iPB) + "." + String(iPC) + "." + String(iPD);
    Button_Page01_IPAdress.Text_ON = ip_str;
    Button_Page01_IPAdress.Text_OFF = ip_str;

    wiFiConfig.Set_IPAdress((byte)iPA,(byte)iPB,(byte)iPC,(byte)iPD);
    Serial.println(ip_str);
  }
  else
  {
    Serial.println("IPAdress write error!");
  }
}
void Button_Page01_Subnet_PressDown()
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
  
  Screen_Page01_Init = true;
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
    String ip_str = "Subnet : " + String(iPA) + "." + String(iPB) + "." + String(iPC) + "." + String(iPD);
    Button_Page01_Subnet.Text_ON = ip_str;
    Button_Page01_Subnet.Text_OFF = ip_str;

    wiFiConfig.Set_Subnet((byte)iPA,(byte)iPB,(byte)iPC,(byte)iPD);
    Serial.println(ip_str);
  }
  else
  {
    Serial.println("Subnet write error!");
  }
}
void Button_Page01_Gateway_PressDown()
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
  
  Screen_Page01_Init = true;
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
    String ip_str = "Gateway : " + String(iPA) + "." + String(iPB) + "." + String(iPC) + "." + String(iPD);
    Button_Page01_Gateway.Text_ON = ip_str;
    Button_Page01_Gateway.Text_OFF = ip_str;

    wiFiConfig.Set_Gateway((byte)iPA,(byte)iPB,(byte)iPC,(byte)iPD);
    Serial.println(ip_str);
  }
  else
  {
    Serial.println("Gateway write error!");
  }
}
void Button_Page01_Dns_PressDown()
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
  
  Screen_Page01_Init = true;
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
    String ip_str = "Dns : " + String(iPA) + "." + String(iPB) + "." + String(iPC) + "." + String(iPD);
    Button_Page01_Dns.Text_ON = ip_str;
    Button_Page01_Dns.Text_OFF = ip_str;

    wiFiConfig.Set_DNS((byte)iPA,(byte)iPB,(byte)iPC,(byte)iPD);
    Serial.println(ip_str);
  }
  else
  {
    Serial.println("Dns write error!");
  }
}
void Button_Page01_Port_PressDown()
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
  Screen_Page01_Init = true;

  String Str_Port = keyBoardABC.TEXT;
  int _port = Get_value(Str_Port);
  if( _port > 0 )
  {
    String str = "Port : " + String(_port);
    Button_Page01_Port.Text_ON = str;
    Button_Page01_Port.Text_OFF = str;
    wiFiConfig.Set_Localport(_port);
    Serial.println(str);
  }
  else
  {
    Serial.println("Port write error!");
  }
}



void Screen_Button_Page01_IP_PressDown()
{
   //udp.broadcastTo("TEST" , 4800);
}
void Screen_Button_Page01_SSID_PressDown()
{
   Screen_Page02_Init = true;
}
void Screen_Button_Page01_Function_PressDown()
{
   Screen_Page03_Init = true;
}
   
