#include "Arduino.h"
#include "UART0_Handler.h"


byte UART0_RX[UART0_RX_SIZE];
int UART0_len = 0;
MyTimer MyTimer_UART0;



void serialEvent()
{

  if (mySerial.available())
  {
    UART0_RX[UART0_len] = mySerial.read();
    UART0_len++;
    MyTimer_UART0.TickStop();
    MyTimer_UART0.StartTickTime(10);
  }
  if (MyTimer_UART0.IsTimeOut())
  {
    MyTimer_UART0.TickStop();
    MyTimer_UART0.StartTickTime(1000);
    if (UART0_RX[0] == 'o')
    {
      String serverIP = wiFiConfig.server_IPAdress_str;
      int str_len = serverIP.length() + 1; 
      char serverIP_char_array[str_len];
      serverIP.toCharArray(serverIP_char_array , str_len );
      mySerial.print("Host :");
      mySerial.print(serverIP_char_array);
      mySerial.print(", port :");
      mySerial.println(8080);
      
      int ret = http_update_ota(serverIP_char_array, 8080, RESOURCE);
      printf("[%s] Update task exit\n\r", __FUNCTION__);
      if(!ret)
      {
        printf("[%s] Ready to reboot\n\r", __FUNCTION__); 
        ota_platform_reset();
      }
    }
    if (UART0_RX[0] == 'd' &&UART0_RX[1] == 'e' &&UART0_RX[2] == 'b' &&UART0_RX[3] == 'u' &&UART0_RX[4] == 'g' && UART0_len >= 5)
    {
       flag_udp_232back = true;
       mySerial.print("debug mode enable! \n");
    }
    if (UART0_RX[0] == 'm' && UART0_RX[1] == 't' && UART0_len >= 2)
    {
       flag_motor_debug = !flag_motor_debug;
       #if defined(DC_MOTOR)  
       mcp.digitalWrite(DC_MOTOR_OUTPUT , flag_motor_debug);
       if(!flag_motor_debug) mySerial.print("motor on...\n");
       if(flag_motor_debug) mySerial.print("motor off...\n");
       #else
       mySerial.print("DC_MOTOR not be define...\n");
       #endif
       
    }
    if (UART0_RX[0] == 'r' && UART0_len == 3)
    {
      String str = "";
      str += (char)UART0_RX[0];
      str += (char)UART0_RX[1];
      str += (char)UART0_RX[2];
      mySerial.print("TOF10120 read command : ");
      mySerial.println(str);
      mySerial2.print(str);
    }
    if (UART0_RX[0] == 's'&& UART0_len == 3)
    {
      String str = "";
      for (int i = 0 ; i < UART0_len ; i++)
      {
         str += (char)UART0_RX[i];
      }
      mySerial.print("TOF10120 write command : ");
      mySerial.println(str);
      mySerial2.print(str);
    }
    if (UART0_RX[0] == 'c'&& UART0_len == 3)
    {
      #if defined(EPD3IN6E)
      mySerial.print("EPD3IN6E Show7Block..\n ");
      epd.Show7Block();
      #elif defined(EPD4IN20G)
      mySerial.print("EPD4IN20G Clear..\n ");
      epd.Clear();
      #else
      mySerial.print("EPD Clear..\n ");
      epd.Clear();
      #endif
      
    }
    if (UART0_RX[0] == 'w'&& UART0_len == 3)
    {
      mySerial.println("flag_WS2812B_breathing_ON_OFF");
      flag_WS2812B_breathing_ON_OFF = !flag_WS2812B_breathing_ON_OFF;
    }
    if (UART0_RX[0] == 'D'&& UART0_len == 3)
    {
      #ifdef DHTSensor
      mySerial.print(F("Humidity: "));
      mySerial.print(dht_h);
      mySerial.print(F("%  Temperature: "));
      mySerial.print(dht_t);
      mySerial.print(F("°C "));
      mySerial.print(dht_f);
      mySerial.print(F("°F  Heat index: "));
      mySerial.print(dht_hic);
      mySerial.print(F("°C "));
      mySerial.print(dht_hif);
      mySerial.println(F("°F"));
      #endif
    }
    if (UART0_RX[0] == 'b'&& UART0_len == 3)
    {
      String str_int = "";
      str_int += (char)UART0_RX[1];
      str_int += (char)UART0_RX[2];
      int val = str_int.toInt();
      myWS2812.brightness = val;
      mySerial.print("Set WS2812 Brightness : int>>");
      mySerial.print(val);
      mySerial.print(" string>>");
      mySerial.println(str_int);
      flag_WS2812B_Refresh = true;
      flag_WS2812B_breathing_ON_OFF = false;
    }
    if (UART0_RX[0] == 'P'&& UART0_len == 3)
    {
      mySerial.print("SetOutputPINTrigger(1 , (1 == 1));\n");
      SetOutputPINTrigger(1 , true);
    }
    if(UART0_RX[0] == 'M'&& UART0_len == 3)
    {
      uint8_t mac[6];
      WiFi.macAddress(mac);
      String HEX_0 =  String(mac[0], HEX); 
      String HEX_1 =  String(mac[1], HEX); 
      String HEX_2 =  String(mac[2], HEX); 
      String HEX_3 =  String(mac[3], HEX); 
      String HEX_4 =  String(mac[4], HEX); 
      String HEX_5 =  String(mac[5], HEX);   
      
      
      String MacAdress = "MacAdress :" + HEX_0 + ":"+ HEX_1 + ":"+ HEX_2 + ":"+ HEX_3 + ":"+ HEX_4 + ":"+ HEX_5;   
      mySerial.println(MacAdress);
    }
    if (UART0_RX[0] == 2 && UART0_RX[UART0_len - 1] == 3)
    {
      if (UART0_RX[1] == '0' && UART0_len == 3)
      {
        flag_writeMode = false;
        String str = "";
        str += (char)2;
        str += wiFiConfig.Get_IPAdress_Str();
        str += ",";
        str += wiFiConfig.Get_Subnet_Str();
        str += ",";
        str += wiFiConfig.Get_Gateway_Str();
        str += ",";
        str += wiFiConfig.Get_DNS_Str();
        str += ",";
        str += wiFiConfig.Get_Server_IPAdress_Str();
        str += ",";
        str += wiFiConfig.Get_Localport_Str();
        str += ",";
        str += wiFiConfig.Get_Serverport_Str();
        str += ",";
        str += wiFiConfig.Get_SSID_Str();
        str += ",";
        str += wiFiConfig.Get_Password_Str();
        str += ",";
        str += wiFiConfig.Get_Station_Str();
        str += ",";
        str += wiFiConfig.Get_UDP_SemdTime_Str();
        str += (char)3;
        mySerial.print(str);
        mySerial.flush();
      }
      else if (UART0_RX[1] == 'v')
      {
        String str = VERSION;
        mySerial.print(str);
        mySerial.flush();
      }
      else if (UART0_RX[1] == 'L')
      {
        String str = "Set Loker sucess :";
        if(UART0_RX[2] >= 1)
        {
           str += "1"; 
           wiFiConfig.Set_IsLocker(true);
        }
        else
        {
           str += "0"; 
           wiFiConfig.Set_IsLocker(false);
        }
        mySerial.print(str);
        mySerial.flush();
      }
      else if (UART0_RX[1] == 'C')
      {
        int len = UART0_len - 5;
        int startpo =  UART0_RX[2] | (UART0_RX[3] << 8);
        int numofLED = len / 3 ;
        int startLED = startpo / 3;         
          
        for(int i = 0 ; i < numofLED ; i++)
        {             
           myWS2812.rgbBuffer[i * 3 + startLED + 0] = *(UART0_RX + 4 + i * 3 + 0);     // 将光带上第1个LED灯珠的RGB数值中R数值设置为255
           myWS2812.rgbBuffer[i * 3 + startLED + 1] = *(UART0_RX + 4 + i * 3 + 1);   // 将光带上第1个LED灯珠的RGB数值中G数值设置为255
           myWS2812.rgbBuffer[i * 3 + startLED + 2] = *(UART0_RX + 4 + i * 3 + 2);      // 将光带上第1个LED灯珠的RGB数值中B数值设置为0      
        }     
        flag_WS2812B_Refresh = true;
        flag_WS2812B_breathing_ON_OFF = false;
        Get_Checksum();
      }
      else if (UART0_RX[1] == 'D')
      {
        epd.Wakeup();
        epd.Clear();
        Get_Checksum();
      }
      else if (UART0_RX[1] == 'b')
      {
        epd.Wakeup();
        Get_Checksum();
      }
      else if (UART0_RX[1] == 'c')
      {
        epd.DrawFrame_RW();
        delay(10);
        Get_Checksum();
      }
      else if (UART0_RX[1] == 'd')
      {
        epd.DrawFrame_BW();
        delay(10);
        Get_Checksum();
      }
      else if (UART0_RX[1] == 'f')
      {
        epd.RefreshCanvas();
        Get_Checksum();
      }
      else if (UART0_RX[1] == 'e')
      {
        int len = UART0_len - 7;
        int startpo_L = (*(UART0_RX + 2)) | (*(UART0_RX + 3) << 8);
        int startpo_H = (*(UART0_RX + 4)) | (*(UART0_RX + 5) << 8);
        long startpo = startpo_L | (startpo_H << 16);
        for(int i = 0 ; i < len ; i ++)
        {
           *(epd.framebuffer +startpo + i) = *(UART0_RX + 6 + i);
        }
        Get_Checksum();
      }
      else if (UART0_RX[1] == 'g')
      {
        oLCD114.LCD_Clear(RED);
        delay(100);
        oLCD114.LCD_Clear(GREEN);
        delay(100);
        oLCD114.LCD_Clear(BLUE);
        delay(100);
        oLCD114.LCD_Clear(YELLOW);
        Get_Checksum();
      }
      else if (UART0_RX[1] == 'h')
      {
        oLCD114.LCD_ShowPicture();         
      }
      else if (UART0_RX[1] == '1' && UART0_len == 5)
      {
        flag_writeMode = true;
        int station = UART0_RX[2] | (UART0_RX[3] << 8);
        wiFiConfig.Set_Station(station);
        Get_Checksum();
      }
      else if (UART0_RX[1] == '2' && UART0_len == 7)
      {
        flag_writeMode = true;
        wiFiConfig.Set_IPAdress(UART0_RX[2], UART0_RX[3], UART0_RX[4], UART0_RX[5]);
        Get_Checksum();
      }
      else if (UART0_RX[1] == '3' && UART0_len == 7)
      {
        flag_writeMode = true;
        wiFiConfig.Set_Subnet(UART0_RX[2], UART0_RX[3], UART0_RX[4], UART0_RX[5]);
        Get_Checksum();
      }
      else if (UART0_RX[1] == '4' && UART0_len == 7)
      {
        flag_writeMode = true;
        wiFiConfig.Set_Gateway(UART0_RX[2], UART0_RX[3], UART0_RX[4], UART0_RX[5]);
        Get_Checksum();
      }
      else if (UART0_RX[1] == '5' && UART0_len == 7)
      {
        flag_writeMode = true;
        wiFiConfig.Set_DNS(UART0_RX[2], UART0_RX[3], UART0_RX[4], UART0_RX[5]);
        Get_Checksum();
      }
      else if (UART0_RX[1] == '6' && UART0_len == 7)
      {
        flag_writeMode = true;
        wiFiConfig.Set_Server_IPAdress(UART0_RX[2], UART0_RX[3], UART0_RX[4], UART0_RX[5]);
        Get_Checksum();
      }
      else if (UART0_RX[1] == '7' && UART0_len == 5)
      {
        flag_writeMode = true;
        int port = UART0_RX[2] | (UART0_RX[3] << 8);
        wiFiConfig.Set_Localport(port);
        Get_Checksum();
      }
      else if (UART0_RX[1] == '8' && UART0_len == 5)
      {
        flag_writeMode = true;
        int port = UART0_RX[2] | (UART0_RX[3] << 8);
        wiFiConfig.Set_Serverport(port);
        Get_Checksum();
      }
      else if (UART0_RX[1] == '9')
      {
        flag_writeMode = true;
        String str = "";
        for (int i = 2 ; i < UART0_len - 1 ; i++)
        {
          str += (char)UART0_RX[i];
        }
        wiFiConfig.Set_SSID(str);
        Get_Checksum();
      }
      else if (UART0_RX[1] == 'A')
      {
        flag_writeMode = true;
        String str = "";
        for (int i = 2 ; i < UART0_len - 1 ; i++)
        {
          str += (char)UART0_RX[i];
        }
        wiFiConfig.Set_Password(str);
        Get_Checksum();
      }
      else if (UART0_RX[1] == 'B')
      {
        flag_writeMode = true;
        int ms = UART0_RX[2] | (UART0_RX[3] << 8);
        wiFiConfig.Set_UDP_SemdTime(ms);
        Get_Checksum();
      }
    }
    
    UART0_len = 0;
    for (int i = 0 ; i < UART0_RX_SIZE ; i++)
    {
      UART0_RX[i] = 0;
    }
  }

}

void Get_Checksum()
{
  byte checksum = 0;
  for (int i = 0 ; i < UART0_len; i ++)
  {
    checksum += UART0_RX[i];
  }
  int checksum_2 = checksum / 100;
  int checksum_1 = (checksum - checksum_2 * 100) / 10 ;
  int checksum_0 = (checksum - checksum_2 * 100 - checksum_1 * 10) ;
  byte str_checksum[5] = {2 ,(checksum_2 + 48), (checksum_1 + 48), (checksum_0 + 48) , 3};
  mySerial.write(str_checksum , 5);
  mySerial.flush();
}
