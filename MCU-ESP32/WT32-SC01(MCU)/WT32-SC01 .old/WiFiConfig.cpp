#include "WiFiConfig.h"
#include <EEPROM.h>
#include "Arduino.h"
#include "UDP.h"
#include <WiFi.h>
#include <WiFiClient.h>
#include <WebServer.h>
#include <ESPmDNS.h>
#include <Update.h>
#include <Wire.h>
#include <esp_task_wdt.h>
#include <TFT_eSPI.h>
#include "Fonts.h"

String _Version = "1.0.10";
int ConnectWIFI_posy = 0;
WebServer _server(80);
String css =
"<style>#file-input,input{width:100%;height:100px;border-radius:5px;margin:10px auto;font-size:16px}"
"input{background:#fff;border:0;padding:0 15px}body{background:#5398D9;font-family:sans-serif;font-size:14px;color:#4d4d4d}"
"#file-input{padding:0;border:2px solid #ddd;line-height:45px;text-align:left;display:block;cursor:pointer}"
"#bar,#prgbar{background-color:#4d4d4d;border-radius:15px}#bar{background-color:#5398D9;width:0%;height:15px}"
"form{background:#EDF259;max-width:300px;margin:80px auto;padding:30px;border-radius:5px;text-align:center}"
".btn{background:#5398D9;color:#EDF259;cursor:pointer}</style>";
 
String loginIndex =
"<form name='loginForm'>"
"<h2>Chosemaker ESP32 Login</h2>"
"<input name=userid placeholder='Username'> "
"<input name=pwd placeholder='Password' type=Password> "
"<input type=submit onclick=check(this.form) class=btn value=Login></form>"
"<script>"
    "function check(form)"
    "{"
    "if(form.userid.value=='admin' &amp;&amp; form.pwd.value=='admin')"
    "{"
    "window.open('/serverIndex')"
    "}"
    "else"
    "{"
    " alert('Error Password or Username')/*displays error message*/"
    "}"
    "}"
"</script>" + css;
 //"<script src='https://ajax.googleapis.com/ajax/libs/jquery/3.2.1/jquery.min.js'></script>"
String serverIndex =
 "<script src='https://ajax.googleapis.com/ajax/libs/jquery/3.2.1/jquery.min.js'></script>"
 "<form method='POST' action='#' enctype='multipart/form-data' id='upload_form'>"
 "<input type='file' name='update'>"
 "<input type='submit' value='Update "+_Version+"'>"
 "<div id='prg'>0%</div>"
 "<div id='prgbar'><div id='bar'></div></div></form>"
 "<script>"
  "$('form').submit(function(e){"
  "e.preventDefault();"
  "var form = $('#upload_form')[0];"
  "var data = new FormData(form);"
  " $.ajax({"
  "url: '/update',"
  "type: 'POST',"
  "data: data,"
  "contentType: false,"
  "processData:false,"
  "xhr: function() {"
  "var xhr = new window.XMLHttpRequest();"
  "xhr.upload.addEventListener('progress', function(evt) {"
  "if (evt.lengthComputable) {"
  "var per = evt.loaded / evt.total;"
  "$('#prg').html('Running ' + Math.round(per*100) + '%');"
  "$('#bar').css('width',Math.round(per*100) + '%');"
  "}"
  "}, false);"
  "return xhr;"
  "},"
  "success:function(d, s) {"
  "console.log('success!')"
 "},"
 "error: function (a, b, c) {"
 "}"
 "});"
 "});"
 "</script>" + css;
void WiFiConfig::SetTFT(TFT_eSPI& Tft)
{
  this -> tft = &Tft;
}

void WiFiConfig::Init(String Version)
{
    _Version = Version;
    EEPROM.begin(1024);
    
    String _SSID = "SSID : " + this -> Get_SSID_Str();
    String _Password = "Password : " + this -> Get_Password_Str();
    String ipAdress = "IPAdress : " + this -> Get_IPAdress_Str();
    String server_ipAdress = "Server IPAdress : " + this -> Get_Server_IPAdress_Str();
    String subnet = "Subnet : " + this -> Get_Subnet_Str();
    String gateway = "Gateway : " + this -> Get_Gateway_Str();
    String dns = "Dns : " + this -> Get_DNS_Str();
    String _Localport = "Localport : " + this -> Get_Localport_Str();
    String _Serverport = "Serverport : " + this -> Get_Serverport_Str();
    String MacAdress = "MacAdress :" + WiFi.macAddress();   
    String PC_Restart = "PC Restart :" +   String(this -> Get_PC_Restart());
    String IsUpdate = "PC IsUpdate :" +   String(this -> Get_IsUpdate());
    
    Serial.println(ipAdress);
    Serial.println(_Localport);
    Serial.println(subnet);
    Serial.println(gateway);
    Serial.println(dns);   
    Serial.println(server_ipAdress); 
    Serial.println(_Serverport);
    Serial.println(_SSID);
    Serial.println(_Password);   
    Serial.println(PC_Restart);   
    Serial.println(IsUpdate);
    
//    int Text_Height = tft->fontHeight(FONT2);
//    ConnectWIFI_posy = 0;
//    tft->setTextColor(TFT_BLACK , TFT_WHITE); 
//    tft->drawString(_SSID , 0, ConnectWIFI_posy,FONT2);
//    ConnectWIFI_posy+= Text_Height;
//    tft->drawString(_Password , 0, ConnectWIFI_posy,FONT2);
//    ConnectWIFI_posy+= Text_Height;
//    tft->drawString(ipAdress , 0, ConnectWIFI_posy,FONT2);
//    ConnectWIFI_posy+= Text_Height;
//    tft->drawString(subnet , 0, ConnectWIFI_posy,FONT2);
//    ConnectWIFI_posy+= Text_Height;
//    tft->drawString(gateway , 0, ConnectWIFI_posy,FONT2);
//    ConnectWIFI_posy+= Text_Height;
//    tft->drawString(dns , 0, ConnectWIFI_posy,FONT2);
//    ConnectWIFI_posy+= Text_Height;
//    tft->drawString(_Localport , 0, ConnectWIFI_posy,FONT2);
//    ConnectWIFI_posy+= Text_Height;
//    tft->drawString(server_ipAdress , 0, ConnectWIFI_posy,FONT2);
//    ConnectWIFI_posy+= Text_Height;
//    tft->drawString(_Serverport , 0, ConnectWIFI_posy,FONT2);
//    ConnectWIFI_posy+= Text_Height;
//    tft->drawString(MacAdress , 0, ConnectWIFI_posy,FONT2);
//    ConnectWIFI_posy+= Text_Height;


   
}
void WiFiConfig::httpInit()
{
   _server.on("/", HTTP_GET, []() {
    _server.sendHeader("Connection", "close");
    _server.send(200, "text/html", serverIndex);
    });
    /*handling uploading firmware file */
    _server.on("/update", HTTP_POST, []() {
    _server.sendHeader("Connection", "close");
    _server.send(200, "text/plain", (Update.hasError()) ? "FAIL" : "OK");
    ESP.restart();
    }, []() {
    HTTPUpload& upload = _server.upload();
    if (upload.status == UPLOAD_FILE_START)
    {
      Serial.printf("Update: %s\n", upload.filename.c_str());
      if (!Update.begin(UPDATE_SIZE_UNKNOWN)) 
      { //start with max available size
        Update.printError(Serial);
      }
    }
    else if (upload.status == UPLOAD_FILE_WRITE)
    {
      /* flashing firmware to ESP*/
      if (Update.write(upload.buf, upload.currentSize) != upload.currentSize)
      {
        Update.printError(Serial);
      }
    }
    else if (upload.status == UPLOAD_FILE_END)
    {
      if (Update.end(true)) 
      { //true to set the size to the current progress
        Serial.printf("Update Success: %u\nRebooting...\n", upload.totalSize);
      } else
      {
        Update.printError(Serial);
      }
    }
    });
    _server.begin();
}

void WiFiConfig::HandleClient()
{
   _server.handleClient();
}
void WiFiConfig::WIFI_Connenct()
{
    WiFi.mode(WIFI_STA);
    WiFi.setSleep(false);
    WiFi.disconnect();
   
    byte* ipAdress_ptr = this -> Get_IPAdress();
    byte* gateway_ptr = this -> Get_Gateway();
    byte* subnet_ptr = this -> Get_Subnet();
    byte* dns_ptr = this -> Get_DNS();
    IPAddress ipAdress(*(ipAdress_ptr + 0 ),*(ipAdress_ptr + 1 ),*(ipAdress_ptr + 2 ),*(ipAdress_ptr + 3 ));
    IPAddress gateway(*(gateway_ptr + 0 ),*(gateway_ptr + 1 ),*(gateway_ptr + 2 ),*(gateway_ptr + 3 ));
    IPAddress subnet(*(subnet_ptr + 0 ),*(subnet_ptr + 1 ),*(subnet_ptr + 2 ),*(subnet_ptr + 3 ));
    IPAddress dns(*(dns_ptr + 0 ),*(dns_ptr + 1 ),*(dns_ptr + 2 ),*(dns_ptr + 3 ));
    WiFi.config(ipAdress ,gateway ,subnet ,dns ,dns );
    
    String _SSID =  this -> Get_SSID_Str();
    char _ssid[sizeof(_SSID)];
    _SSID.toCharArray(_ssid , sizeof(_SSID) );
    
    String _Password =  this -> Get_Password_Str();
    char _password[sizeof(_Password)];
    _Password.toCharArray(_password , sizeof(_Password));
    WiFi.begin(_ssid, _password);
    if(!this -> flag_Init)Serial.print("connecting.");
    int retry = 0;
    while (WiFi.status() != WL_CONNECTED) //等待网络连接成功
    {
      if( retry >= 20)
      {
         break;
      }
      esp_task_wdt_reset();
      delay(500);
      if(!this -> flag_Init)Serial.print(".");
      retry++;
    }
        if(!this -> flag_Init)Serial.print("\n\r");
    this -> IsConnected = (WiFi.status() == WL_CONNECTED);
    if( this -> IsConnected)
    {
      if(!this -> flag_Init)Serial.println("WiFi connected!");
      if(!this -> flag_Init)Serial.print("IP address: ");
      if(!this -> flag_Init)Serial.println(WiFi.localIP()); //打印模块IP   
    }
    else
    {
      if(!this -> flag_Init)Serial.println("WiFi connect failed!");      
    }
    this -> flag_Init = true;
    
}
void WiFiConfig::Set_IPAdress(byte IP0 ,byte IP1 ,byte IP2 ,byte IP3)
{
    EEPROM.write(this -> iP_Adress_ADDR[0] , IP0);
    EEPROM.write(this -> iP_Adress_ADDR[1] , IP1);
    EEPROM.write(this -> iP_Adress_ADDR[2] , IP2);
    EEPROM.write(this -> iP_Adress_ADDR[3] , IP3);
    EEPROM.commit();
}
void WiFiConfig::Set_Subnet(byte IP0 ,byte IP1 ,byte IP2 ,byte IP3)
{
    EEPROM.write(this -> subnet_ADDR[0] , IP0);
    EEPROM.write(this -> subnet_ADDR[1] , IP1);
    EEPROM.write(this -> subnet_ADDR[2] , IP2);
    EEPROM.write(this -> subnet_ADDR[3] , IP3);
    EEPROM.commit();
}
void WiFiConfig::Set_Gateway(byte IP0 ,byte IP1 ,byte IP2 ,byte IP3)
{
    EEPROM.write(this -> gateway_ADDR[0] , IP0);
    EEPROM.write(this -> gateway_ADDR[1] , IP1);
    EEPROM.write(this -> gateway_ADDR[2] , IP2);
    EEPROM.write(this -> gateway_ADDR[3] , IP3);
    EEPROM.commit();
}
void WiFiConfig::Set_DNS(byte IP0 ,byte IP1 ,byte IP2 ,byte IP3)
{
    EEPROM.write(this -> dns_ADDR[0] , IP0);
    EEPROM.write(this -> dns_ADDR[1] , IP1);
    EEPROM.write(this -> dns_ADDR[2] , IP2);
    EEPROM.write(this -> dns_ADDR[3] , IP3);
    EEPROM.commit();
}
void WiFiConfig::Set_Server_IPAdress(byte IP0 ,byte IP1 ,byte IP2 ,byte IP3)
{
    EEPROM.write(this -> server_iP_Adress_ADDR[0] , IP0);
    EEPROM.write(this -> server_iP_Adress_ADDR[1] , IP1);
    EEPROM.write(this -> server_iP_Adress_ADDR[2] , IP2);
    EEPROM.write(this -> server_iP_Adress_ADDR[3] , IP3);
    EEPROM.commit();
}
void WiFiConfig::Set_Localport(int port)
{
    byte L = port;
    byte H = (port >> 8);
    EEPROM.write(this -> localport_ADDR[0] , L);
    EEPROM.write(this -> localport_ADDR[1] , H);
    EEPROM.commit();
}
void WiFiConfig::Set_Serverport(int port)
{
    byte L = port;
    byte H = (port >> 8);
    EEPROM.write(this -> serverport_ADDR[0] , L);
    EEPROM.write(this -> serverport_ADDR[1] , H);
    EEPROM.commit();
}
void WiFiConfig::Set_SSID(String ssid)
{  
  byte buf[20];
  ssid.getBytes(buf , 20);
  for(int i = 0 ; i < this -> SSID_SIZE ; i++)
  {
     EEPROM.write((this -> SSID_ADDR) + i , buf[i]);
  }
  EEPROM.commit();
}
void WiFiConfig::Set_Password(String password)
{  
  byte buf[20];
  password.getBytes(buf , 20);
  for(int i = 0 ; i < this -> Password_SIZE ; i++)
  {
     EEPROM.write((this -> Password_ADDR) + i , buf[i]);
  }
  EEPROM.commit();
}
void WiFiConfig::Set_Station(int station)
{
    byte L = station;
    byte H = (station >> 8);
    EEPROM.write(this -> station_ADDR[0] , L);
    EEPROM.write(this -> station_ADDR[1] , H);
    EEPROM.commit();
}
void WiFiConfig::Set_PC_Restart(bool state)
{
    if(state)
    {
      EEPROM.write(this -> pc_restart_ADDR , 1);
    }
    else
    {
      EEPROM.write(this -> pc_restart_ADDR , 0);
    }    
    EEPROM.commit();
}
void WiFiConfig::Set_IsUpdate(bool state)
{
    if(state)
    {
      EEPROM.write(this -> IsUpdate_ADDR , 1);
    }
    else
    {
      EEPROM.write(this -> IsUpdate_ADDR , 0);
    }    
    EEPROM.commit();
}

byte* WiFiConfig::Get_IPAdress()
{
    this -> iP_Adress[0] = EEPROM.read(this -> iP_Adress_ADDR[0]);
    this -> iP_Adress[1] = EEPROM.read(this -> iP_Adress_ADDR[1]);
    this -> iP_Adress[2] = EEPROM.read(this -> iP_Adress_ADDR[2]);
    this -> iP_Adress[3] = EEPROM.read(this -> iP_Adress_ADDR[3]);
    return this -> iP_Adress;
}
byte* WiFiConfig::Get_Subnet()
{
    this -> subnet[0] = EEPROM.read(this -> subnet_ADDR[0]);
    this -> subnet[1] = EEPROM.read(this -> subnet_ADDR[1]);
    this -> subnet[2] = EEPROM.read(this -> subnet_ADDR[2]);
    this -> subnet[3] = EEPROM.read(this -> subnet_ADDR[3]);
    return this -> subnet;
}
byte* WiFiConfig::Get_Gateway()
{
    this -> gateway[0] = EEPROM.read(this -> gateway_ADDR[0]);
    this -> gateway[1] = EEPROM.read(this -> gateway_ADDR[1]);
    this -> gateway[2] = EEPROM.read(this -> gateway_ADDR[2]);
    this -> gateway[3] = EEPROM.read(this -> gateway_ADDR[3]);
    return this -> gateway;
}
byte* WiFiConfig::Get_DNS()
{
    this -> dns[0] = EEPROM.read(this -> dns_ADDR[0]);
    this -> dns[1] = EEPROM.read(this -> dns_ADDR[1]);
    this -> dns[2] = EEPROM.read(this -> dns_ADDR[2]);
    this -> dns[3] = EEPROM.read(this -> dns_ADDR[3]);
    return this -> dns;
}
byte* WiFiConfig::Get_Server_IPAdress()
{
    this -> server_iP_Adress[0] = EEPROM.read(this -> server_iP_Adress_ADDR[0]);
    this -> server_iP_Adress[1] = EEPROM.read(this -> server_iP_Adress_ADDR[1]);
    this -> server_iP_Adress[2] = EEPROM.read(this -> server_iP_Adress_ADDR[2]);
    this -> server_iP_Adress[3] = EEPROM.read(this -> server_iP_Adress_ADDR[3]);
    return this -> server_iP_Adress;
}
int WiFiConfig::Get_Localport()
{
    byte L = EEPROM.read(this -> localport_ADDR[0]);
    byte H = EEPROM.read(this -> localport_ADDR[1]);
    this -> localport = ( L | (H << 8));
    return this -> localport;
}
int WiFiConfig::Get_Serverport()
{
    byte L = EEPROM.read(this -> serverport_ADDR[0]);
    byte H = EEPROM.read(this -> serverport_ADDR[1]);
    this -> serverport = ( L | (H << 8));
    return this -> serverport;
}
int WiFiConfig::Get_Station()
{
    byte L = EEPROM.read(this -> station_ADDR[0]);
    byte H = EEPROM.read(this -> station_ADDR[1]);
    this -> station = ( L | (H << 8));
    return this -> station;
}
byte WiFiConfig::Get_PC_Restart()
{
    byte flag = EEPROM.read(this -> pc_restart_ADDR);    
    return flag;
}
byte WiFiConfig::Get_IsUpdate()
{
    byte flag = EEPROM.read(this -> IsUpdate_ADDR);    
    return flag;
}

String WiFiConfig::Get_IPAdress_Str()
{
    byte* ip; 
    ip = this -> Get_IPAdress();
    return String(*(ip + 0)) + '.' + String(*(ip + 1)) + '.' + String(*(ip + 2)) + '.' + String(*(ip + 3));
}
IPAddress WiFiConfig::Get_IPAdressClass()
{
    byte* ip; 
    ip = this -> Get_IPAdress();
    IPAddress ipAdress(*(ip + 0 ),*(ip + 1 ),*(ip + 2 ),*(ip + 3 ));
    return ipAdress;
}
String WiFiConfig::Get_Subnet_Str()
{
    byte* ip; 
    ip = this -> Get_Subnet();
    return String(*(ip + 0)) + '.' + String(*(ip + 1)) + '.' + String(*(ip + 2)) + '.' + String(*(ip + 3));
}
String WiFiConfig::Get_Gateway_Str()
{
    byte* ip; 
    ip = this -> Get_Gateway();
    return String(*(ip + 0)) + '.' + String(*(ip + 1)) + '.' + String(*(ip + 2)) + '.' + String(*(ip + 3));
}
String WiFiConfig::Get_DNS_Str()
{
    byte* ip; 
    ip = this -> Get_DNS();
    return String(*(ip + 0)) + '.' + String(*(ip + 1)) + '.' + String(*(ip + 2)) + '.' + String(*(ip + 3));
}
String WiFiConfig::Get_Server_IPAdress_Str()
{
    byte* ip; 
    ip = this -> Get_Server_IPAdress();
    return String(*(ip + 0)) + '.' + String(*(ip + 1)) + '.' + String(*(ip + 2)) + '.' + String(*(ip + 3));
}
IPAddress WiFiConfig::Get_Server_IPAdressClass()
{
    byte* ip; 
    ip = this -> Get_Server_IPAdress();
    IPAddress ipAdress(*(ip + 0 ),*(ip + 1 ),*(ip + 2 ),*(ip + 3 ));
    return ipAdress;
}
String WiFiConfig::Get_Localport_Str()
{
    return String(this -> Get_Localport());
}
String WiFiConfig::Get_Serverport_Str()
{
    return String(this -> Get_Serverport());
}
String WiFiConfig::Get_SSID_Str()
{
    String str_SSID = "";
    for(int i = 0 ; i < this -> SSID_SIZE ; i++)
    {
       char temp = EEPROM.read((this -> SSID_ADDR) + i);
       if(temp != 0)
       {
         str_SSID+= temp;
       }
       else
       {
         break;
       }
    }
    return str_SSID;
}
String WiFiConfig::Get_Password_Str()
{
    String str_Password = "";
    for(int i = 0 ; i < this -> Password_SIZE ; i++)
    {
       char temp = EEPROM.read((this -> Password_ADDR) + i);
       if(temp != 0)
       {
         str_Password+= temp;
       }
       else
       {
         break;
       }
    }
    return str_Password;
}
String WiFiConfig::Get_Station_Str()
{
    return String(this -> Get_Station());
}
