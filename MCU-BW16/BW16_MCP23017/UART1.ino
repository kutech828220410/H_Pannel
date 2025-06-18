#include "Timer.h"
#define UART1_RX_SIZE 256
static byte UART1_RX[UART1_RX_SIZE];
static byte UART1_RX_BUF[UART1_RX_SIZE];
int UART1_len = 0;
MyTimer MyTimer_UART1;
byte UART1_read_start0 = 0;
byte UART1_read_start1 = 0;
bool flag_UART1_IsMsgGet = false;
void serialEvent1()
{
  
  if (mySerial_485.available())
  {
    UART1_RX[UART1_len] = mySerial_485.read();
//    if(UART1_RX[0] == 2)
//    {
////       mySerial.print("UART1_len : 0 ,Get strat tx byte!\n");
//       if(UART1_len >= 0 && UART1_len < 1)
//       {
//          UART1_len++;
//       }
//       else if(UART1_len >= 1 && UART1_len < 2)
//       {
//          if(UART1_RX[1] == (byte)station)
//          {
////             mySerial.print("UART1_len : 1 , station : ");
////             mySerial.print((byte)station);
////             mySerial.print("station check ok! \n");
//             UART1_len++;
//          }
//          else
//          {
//             UART1_len = 0;
//          }        
//       }
//       else if(UART1_len >= 2)
//       {          
//          UART1_len++;
//          if(UART1_len > UART1_RX_SIZE) UART1_len = 0;
//       }
//       
//    }
//    else
//    {
//       UART1_len = 0;
//    }
    UART1_len++;
    MyTimer_UART1.TickStop();
    MyTimer_UART1.StartTickTime(2);
    
  }
  if (MyTimer_UART1_IsConnected.IsTimeOut())
  {
    UART1_IsConnected = false;
  }
  if (MyTimer_UART1.IsTimeOut() && UART1_len > 0)
  {
    MyTimer_UART1.TickStop();
    MyTimer_UART1.StartTickTime(1000);
    
    if (UART1_RX[0] == 'T'&& ((byte)wiFiConfig.station == UART1_RX[1]))
    {
       Set_RS485_Tx_Enable();
       mySerial_485.print("Station:");
       mySerial_485.print(wiFiConfig.station);
       mySerial_485.println(" UART1 TEST OK!");
       Set_RS485_Rx_Enable();
    }
    if ( (UART1_RX[UART1_len - 3] == 3) && ((byte)wiFiConfig.station == UART1_RX[0]))
    {
       uint16_t CRC16 = 0;
       byte read_CRC16_L =  UART1_RX[UART1_len - 2];
       byte read_CRC16_H =  UART1_RX[UART1_len - 1];
       byte caculate_CRC16_L = 0;
       byte caculate_CRC16_H = 0;
       CRC16 = Get_CRC16(UART1_RX , UART1_len);
       caculate_CRC16_L = CRC16;
       caculate_CRC16_H = CRC16 >> 8;
       byte command = UART1_RX[2];
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
               Set_RS485_Tx_Enable();
               mySerial_485.write(tx , len);
               mySerial_485.flush();
               Set_RS485_Rx_Enable();
               UART1_IsConnected = true;
               MyTimer_UART1_IsConnected.TickStop();
               MyTimer_UART1_IsConnected.StartTickTime(1000);
           }
           else if(command == 'F')
           {
               int output_L = UART1_RX[3]; 
               int output_H = UART1_RX[4];
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
               Set_RS485_Tx_Enable();
               
               mySerial_485.write(tx , len);
               mySerial_485.flush();             
               Set_RS485_Rx_Enable();
           }
           else if(command == 'G')
           {
               int PIN = UART1_RX[3]; 
               int state = UART1_RX[4];
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
               Set_RS485_Tx_Enable();
               mySerial_485.write(tx , len);
               mySerial_485.flush();
               Set_RS485_Rx_Enable();
           }
           else if(command == 'M')
           {
               int input_dir_L = UART1_RX[3]; 
               int input_dir_H = UART1_RX[4];
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
               Set_RS485_Tx_Enable();
               mySerial_485.write(tx , len);
               mySerial_485.flush();
               Set_RS485_Rx_Enable();
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
               Set_RS485_Tx_Enable();
               mySerial_485.write(tx , len);
               mySerial_485.flush();
               Set_RS485_Rx_Enable();
           }
           else if(command == 'H')
           {
               int output_dir_L = UART1_RX[3]; 
               int output_dir_H = UART1_RX[4];
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
               Set_RS485_Tx_Enable();
               mySerial_485.write(tx , len);
               mySerial_485.flush();
               Set_RS485_Rx_Enable();
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
               Set_RS485_Tx_Enable();
               mySerial_485.write(tx , len);
               mySerial_485.flush();
               Set_RS485_Rx_Enable();
           }
       }
       else
       {
//           Set_RS485_Tx_Enable();
//           mySerial_485.print("CRC check error!");
//           Set_RS485_Rx_Enable();
       }
       
    }
    UART1_len = 0;
    flag_UART1_IsMsgGet = false;
    for (int i = 0 ; i < UART1_RX_SIZE ; i++)
    {
      UART1_RX[i] = 0;
    }
  }
}
