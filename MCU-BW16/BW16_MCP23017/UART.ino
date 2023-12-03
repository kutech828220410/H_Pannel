#include "Timer.h"
#define UART0_RX_SIZE 256
static byte UART0_RX[UART0_RX_SIZE];
int UART0_len = 0;
MyTimer MyTimer_UART0;

#define RESOURCE "OTA_All.bin"    

void serialEvent()
{

  if (mySerial.available())
  {
    UART0_RX[UART0_len] = mySerial.read();
    UART0_len++;
    MyTimer_UART0.TickStop();
    MyTimer_UART0.StartTickTime(2);
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
    if (UART0_RX[0] == 'T')
    {
       mySerial.println("UART0 TEST OK!");
    }
    if ((UART0_RX[0] == 2) && (UART0_RX[UART0_len - 3] == 3) && ((byte)wiFiConfig.station == UART0_RX[1]))
    {
       uint16_t CRC16 = 0;
       byte read_CRC16_L =  UART0_RX[UART0_len - 2];
       byte read_CRC16_H =  UART0_RX[UART0_len - 1];
       byte caculate_CRC16_L = 0;
       byte caculate_CRC16_H = 0;
       CRC16 = Get_CRC16(UART0_RX , UART0_len);
       caculate_CRC16_L = CRC16;
       caculate_CRC16_H = CRC16 >> 8;
       byte command = UART0_RX[2];
       if(CRC16 == 0)
       {
           if(command == 'E')
           {
               int input = GetInput();
               int output = GetOutput();
               byte input_L = input; 
               byte input_H = input >> 8; 
               byte output_L = output; 
               byte output_H = output >> 8;
               byte len = 10;
               byte tx[len];
               tx[0] = 2;
               tx[1] = (byte)wiFiConfig.station;
               tx[2] = command;
               tx[3] = input_L;
               tx[4] = input_H;
               tx[5] = output_L;
               tx[6] = output_H;
               tx[7] = 3;
               CRC16 = Get_CRC16(tx , len - 2);
               caculate_CRC16_L = CRC16;
               caculate_CRC16_H = CRC16 >> 8;
               tx[len - 2] = caculate_CRC16_L;
               tx[len - 1] = caculate_CRC16_H;
               mySerial.write(tx , len);
               mySerial.flush();
           }
           else if(command == 'F')
           {
               int output_L = UART0_RX[3]; 
               int output_H = UART0_RX[4];
               int output = output_L | (output_H << 8);
               output = ~output;
               byte len = 8;
               byte tx[len];
               tx[0] = 2;
               tx[1] = (byte)wiFiConfig.station;
               tx[2] = command;
               tx[3] = output_L;
               tx[4] = output_H;
               tx[5] = 3;
               CRC16 = Get_CRC16(tx , len - 2);
               caculate_CRC16_L = CRC16;
               caculate_CRC16_H = CRC16 >> 8;
               tx[len - 2] = caculate_CRC16_L;
               tx[len - 1] = caculate_CRC16_H;
               SetOutputEx(output);
               mySerial.write(tx , len);
               mySerial.flush();
           }
           else if(command == 'G')
           {
               int PIN = UART0_RX[3]; 
               int state = UART0_RX[4];
               byte len = 8;
               byte tx[len];
               tx[0] = 2;
               tx[1] = (byte)wiFiConfig.station;
               tx[2] = command;
               tx[3] = PIN;
               tx[4] = state;
               tx[5] = 3;
               CRC16 = Get_CRC16(tx , len - 2);
               caculate_CRC16_L = CRC16;
               caculate_CRC16_H = CRC16 >> 8;
               tx[len - 2] = caculate_CRC16_L;
               tx[len - 1] = caculate_CRC16_H;
               SetOutputPIN_Ex(PIN ,(state == 1));
               mySerial.write(tx , len);
               mySerial.flush();
           }
           else if(command == 'M')
           {
               int input_dir_L = UART0_RX[3]; 
               int input_dir_H = UART0_RX[4];
               int input_dir = input_dir_L | (input_dir_H << 8);
               byte len = 8;
               byte tx[len];
               tx[0] = 2;
               tx[1] = (byte)wiFiConfig.station;
               tx[2] = command;
               tx[3] = input_dir_L;
               tx[4] = input_dir_H;
               tx[5] = 3;
               CRC16 = Get_CRC16(tx , len - 2);
               caculate_CRC16_L = CRC16;
               caculate_CRC16_H = CRC16 >> 8;
               tx[len - 2] = caculate_CRC16_L;
               tx[len - 1] = caculate_CRC16_H;
               
               mySerial.write(tx , len);
               mySerial.flush();
               Set_Input_dir(input_dir);
               wiFiConfig.Set_Input_dir(input_dir);
           }
           else if(command == 'I')
           {              
               int input_dir = Get_Input_dir();
               byte input_dir_L = (byte)(input_dir >> 0); 
               byte input_dir_H = (byte)(input_dir >> 8); 
               byte len = 8;
               byte tx[len];
               tx[0] = 2;
               tx[1] = (byte)wiFiConfig.station;
               tx[2] = command;
               tx[3] = input_dir_L;
               tx[4] = input_dir_H;
               tx[5] = 3;
               CRC16 = Get_CRC16(tx , len - 2);
               caculate_CRC16_L = CRC16;
               caculate_CRC16_H = CRC16 >> 8;
               tx[len - 2] = caculate_CRC16_L;
               tx[len - 1] = caculate_CRC16_H;
               mySerial.write(tx , len);
               mySerial.flush();
           }
           else if(command == 'H')
           {
               int output_dir_L = UART0_RX[3]; 
               int output_dir_H = UART0_RX[4];
               int output_dir = output_dir_L | (output_dir_H << 8);
               byte len = 8;
               byte tx[len];
               tx[0] = 2;
               tx[1] = (byte)wiFiConfig.station;
               tx[2] = command;
               tx[3] = output_dir_L;
               tx[4] = output_dir_H;
               tx[5] = 3;
               CRC16 = Get_CRC16(tx , len - 2);
               caculate_CRC16_L = CRC16;
               caculate_CRC16_H = CRC16 >> 8;
               tx[len - 2] = caculate_CRC16_L;
               tx[len - 1] = caculate_CRC16_H;
               Set_Output_dir(output_dir);
               wiFiConfig.Set_Output_dir(output_dir);
               mySerial.write(tx , len);
               mySerial.flush();
           }
           else if(command == 'J')
           {              
               int output_dir = Get_Output_dir();
               byte output_dir_L = (byte)(output_dir >> 0); 
               byte output_dir_H = (byte)(output_dir >> 8); 
               byte len = 8;
               byte tx[len];
               tx[0] = 2;
               tx[1] = (byte)wiFiConfig.station;
               tx[2] = command;
               tx[3] = output_dir_L;
               tx[4] = output_dir_H;
               tx[5] = 3;
               CRC16 = Get_CRC16(tx , len - 2);
               caculate_CRC16_L = CRC16;
               caculate_CRC16_H = CRC16 >> 8;
               tx[len - 2] = caculate_CRC16_L;
               tx[len - 1] = caculate_CRC16_H;
               mySerial.write(tx , len);
               mySerial.flush();
           }
       }
       else
       {
           mySerial.print("CRC check error!");
       }
    }
    else if (UART0_RX[0] == 2 && UART0_RX[UART0_len - 1] == 3)
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
        str += ",";
        str += wiFiConfig.Get_RFID_Enable_Str();
        str += (char)3;
        mySerial.print(str);
        mySerial.flush();
      }
      else if (UART0_RX[1] == 'v')
      {
        String str = Version;
        mySerial.print(str);
        mySerial.flush();
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
      else if (UART0_RX[1] == 'Z')
      {
        byte RFID_Enable = UART0_RX[2];
        wiFiConfig.Set_RFID_Enable(RFID_Enable);
        wiFiConfig.Get_RFID_Enable();
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
  byte str_checksum[3] = {(checksum_2 + 48), (checksum_1 + 48), (checksum_0 + 48)};
  mySerial.write(str_checksum , 3);
  mySerial.flush();

}

uint16_t Get_CRC16(byte* pDataBytes , int len)
{
    uint16_t crc = 0xffff;
    uint16_t polynom = 0xA001;
    for (int i = 0; i < len; i++)
    {
        crc ^= *(pDataBytes + i);
        for (int j = 0; j < 8; j++)
        {
            if ((crc & 0x01) == 0x01)
            {
                crc >>= 1;
                crc ^= polynom;
            }
            else
            {
                crc >>= 1;
            }
        }
    }
    return crc;
}
