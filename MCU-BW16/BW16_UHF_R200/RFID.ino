#define RFID_RX_SIZE 512
static byte RFID_RX[RFID_RX_SIZE];
int RFID_len;
String Card_00 = "";
String Card_01 = "";
String Card_02 = "";
String Card_03 = "";
String Card_04 = "";

String Card_00_buf = "";
String Card_01_buf = "";
String Card_02_buf = "";
String Card_03_buf = "";
String Card_04_buf = "";

long RSSI_00 = 0;
int RSSI_01 = 0;
int RSSI_02 = 0;
int RSSI_03 = 0;
int RSSI_04 = 0;
MyTimer MyTimer_RFID;

void Set_RF_Power()
{
  //BB 00 B6 00 02 05 DC 99 7E  15 dbm
  //BB 00 B6 00 02 03 E8 A3 7E  10 dbm
   byte tx[9];
   tx[0] = 0xBB;
   tx[1] = 0x00;
   tx[2] = 0xB6;
   tx[3] = 0x00;
   tx[4] = 0x02;
   tx[5] = 0x03;
   tx[6] = 0xE8;
   tx[7] = 0xA3;
   tx[8] = 0x7E;
   mySerial_R200.write(tx , 9);
   mySerial_R200.flush();
}
void Get_CardID()
{
   int retry = 0;
   bool flag_rx_Start = false;
   int Card_index = 0;
   byte tx[7];
   tx[0] = 0xBB;
   tx[1] = 0x00;
   tx[2] = 0x22;
   tx[3] = 0x00;
   tx[4] = 0x00;
   tx[5] = 0x22;
   tx[6] = 0x7E;

   RFID_len = 0;
   for (int i = 0 ; i < RFID_RX_SIZE ; i++)
   {
       RFID_RX[i] = 0;         
   }
   MyTimer_RFID.TickStop();
   MyTimer_RFID.StartTickTime(100);
   mySerial_R200.write(tx , 7);
   mySerial_R200.flush();
   if(flag_udp_232back)mySerial.println("Write R200 reading command...");
   while(true)
   {
      if( RFID_len == 0 )
      {
         if(MyTimer_RFID.IsTimeOut())
         {
             return;
         }
      }
      if (mySerial_R200.available())
      {
        RFID_RX[RFID_len] = mySerial_R200.read();
        RFID_len++;
        MyTimer_RFID.TickStop();
        MyTimer_RFID.StartTickTime(10);
      }
      if(MyTimer_RFID.IsTimeOut())
      {
        if(flag_udp_232back)mySerial.print("Read R200 lenghth : ");
        if(flag_udp_232back)mySerial.println(RFID_len);
        if(RFID_len >= 8)
        {
           Card_00 = "";
           Card_01 = "";
           Card_02 = "";
           Card_03 = "";
           Card_04 = "";
           RSSI_00 = 0;
           RSSI_01 = 0;
           RSSI_02 = 0;
           RSSI_03 = 0;
           RSSI_04 = 0;           
        }
        for(int i = 0 ; i < RFID_len ; i++)
        {
           if(RFID_RX[i] == 0xBB && RFID_RX[i + 1] == 0x02)
           {
              if((i + 19) >= RFID_len ) i = RFID_len;
              else
              {
                 byte _RSSI = RFID_RX[i + 5]; 
                 String HEX_0 =  String(RFID_RX[i + 8], HEX); 
                 String HEX_1 =  String(RFID_RX[i + 9], HEX); 
                 String HEX_2 =  String(RFID_RX[i + 10], HEX); 
                 String HEX_3 =  String(RFID_RX[i + 11], HEX); 
                 String HEX_4 =  String(RFID_RX[i + 12], HEX); 
                 String HEX_5 =  String(RFID_RX[i + 13], HEX); 
                 String HEX_6 =  String(RFID_RX[i + 14], HEX); 
                 String HEX_7 =  String(RFID_RX[i + 15], HEX); 
                 String HEX_8 =  String(RFID_RX[i + 16], HEX); 
                 String HEX_9 =  String(RFID_RX[i + 17], HEX); 
                 String HEX_10 =  String(RFID_RX[i + 18], HEX); 
                 String HEX_11 =  String(RFID_RX[i + 19], HEX); 

                 HEX_0.toUpperCase();
                 HEX_1.toUpperCase();
                 HEX_2.toUpperCase();
                 HEX_3.toUpperCase();
                 HEX_4.toUpperCase();
                 HEX_5.toUpperCase();
                 HEX_6.toUpperCase();
                 HEX_7.toUpperCase();
                 HEX_8.toUpperCase();
                 HEX_9.toUpperCase();
                 HEX_10.toUpperCase();
                 
                 String Card = HEX_0 + HEX_1 + HEX_2 + HEX_3 + HEX_4 + HEX_5 + HEX_6 + HEX_7 + HEX_8 + HEX_9 + HEX_10 ;
                 if(Card_index == 0)
                 {
                    Card_00 = Card;
                    RSSI_00 = 0xFFFFFFFFFFFFFF00 | _RSSI;
                    if(flag_udp_232back)mySerial.print("Read R200 Card 00 : ");
                    if(flag_udp_232back)mySerial.print(Card_00);
                    if(flag_udp_232back)mySerial.print(" ,RSSI : ");
                    if(flag_udp_232back)mySerial.println(RSSI_00);
                    flag_JsonSend = true;
                 }
                 else if(Card_index == 1)
                 {
                    Card_01 = Card;
                    RSSI_01 = 0xFFFFFFFFFFFFFF00 | _RSSI;
                    if(flag_udp_232back)mySerial.print("Read R200 Card 01 : ");
                    if(flag_udp_232back)mySerial.print(Card_01);
                    if(flag_udp_232back)mySerial.print(" ,RSSI : ");
                    if(flag_udp_232back)mySerial.println(RSSI_01);
                    flag_JsonSend = true;
                 }
                 else if(Card_index == 2)
                 {
                    Card_02 = Card;
                    RSSI_02 = 0xFFFFFFFFFFFFFF00 | _RSSI;
                    if(flag_udp_232back)mySerial.print("Read R200 Card 02 : ");
                    if(flag_udp_232back)mySerial.print(Card_02);
                    if(flag_udp_232back)mySerial.print(" ,RSSI : ");
                    if(flag_udp_232back)mySerial.println(RSSI_02);
                    flag_JsonSend = true;
                 }
                 else if(Card_index == 3)
                 {
                    Card_03 = Card;
                    RSSI_03 = 0xFFFFFFFFFFFFFF00 | _RSSI;
                    if(flag_udp_232back)mySerial.print("Read R200 Card 03 : ");
                    if(flag_udp_232back)mySerial.print(Card_03);
                    if(flag_udp_232back)mySerial.print(" ,RSSI : ");
                    if(flag_udp_232back)mySerial.println(RSSI_03);
                    flag_JsonSend = true;
                 }
                 else if(Card_index == 4)
                 {
                    Card_04 = Card;
                    RSSI_04 = 0xFFFFFFFFFFFFFF00 | _RSSI;
                    if(flag_udp_232back)mySerial.print("Read R200 Card 04 : ");
                    if(flag_udp_232back)mySerial.print(Card_04);
                    if(flag_udp_232back)mySerial.print(" ,RSSI : ");
                    if(flag_udp_232back)mySerial.println(RSSI_04);
                    flag_JsonSend = true;
                 }
                 Card_index++;
                 i += 23;
              }                        
           }
        } 
        break;
      }
      
   }
     
}
