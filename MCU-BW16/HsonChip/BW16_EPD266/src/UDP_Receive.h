#ifndef UDP_RECEIVE_H
#define UDP_RECEIVE_H
//UDP_Receive
#include <Arduino.h>
#include "..\Global.h"

#define UDP_BUFFER_SIZE 19000
#define UDP_RX_BUFFER_SIZE 1500

extern char* UdpRead;
extern char* UdpRead_buf;
extern int remotePort;
extern IPAddress remoteIP;
extern bool UDP_ISConnented;
extern int UdpRead_len;
extern int UdpRead_len_buf;
extern long UDPcheck_len;
extern WiFiUDP Udp;
extern WiFiUDP Udp1;
extern bool flag_UDP_RX_BUFFER_Init;
extern bool flag_UDP_RX_OK;
extern bool flag_UDP_header;
extern bool flag_UDP0_packet;
extern bool flag_UDP1_packet;
extern MyTimer MyTimer_UDP;
extern MyTimer MyTimer_UDP_RX_TimeOut;
extern int ForeColor;
extern int BackColor;

void onPacketCallBack();
void Clear_char_buf();
void Get_Checksum_UDP();
void Connect_UDP(int localport);
void Send_Bytes(uint8_t *value ,int Size ,IPAddress RemoteIP ,int RemotePort);
void Send_String(String str ,int remoteUdpPort);
void Send_StringTo(String str ,IPAddress RemoteIP ,int RemotePort);
String IpAddress2String(const IPAddress& ipAddress);

#endif // UDP_RECEIVE_H
