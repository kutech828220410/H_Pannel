#ifndef GLOBAL_H
#define GLOBAL_H

#include <Arduino.h>

#include "./src/IO.h"
#include "./src/UART0_Handler.h"
#include "./src/UART1_Handler.h"
#include "./src/UDP_Receive.h"
#include "./src/UDP_Reporter.h"

#include "./Global.h"
#include "./Config.h"
#include "./Timer.h"
#include "./OLCD114.h"
#include "./WiFiConfig.h"
#include "./EPD.h"
#include "./MyWS2812.h"
#include "./Timer.h"
#include "./LED.h"
#include "./EPD730E.h"
#include "./EPD360E.h"

#ifdef MCP23017
#include "DFRobot_MCP23017.h"
extern DFRobot_MCP23017 mcp;//constructor, change the Level of A2, A1, A0 via DIP switch to revise the I2C address within 0x20~0x27.
#endif

#ifdef DHTSensor
extern DHT dht;
extern float dht_h;
extern float dht_t;
extern float dht_f;
extern float dht_hif;
extern float dht_hic;
#endif

extern bool flag_udp_232back;
extern bool flag_JsonSend;
extern bool flag_writeMode;

#if defined(EPD266)
    extern EPD epd;
#elif defined(EPD290)
    extern EPD epd;
#elif defined(EPD213)
    extern EPD epd;
#elif defined(EPD420)
    extern EPD epd;
#elif defined(EPD420_D)
    extern EPD epd;
#elif defined(EPD583)
    extern EPD epd;
#elif defined(EPD213_BRW_V0)
    extern EPD epd;
#elif defined(EPD579G)
    extern EPD epd;
#elif defined(EPD579B)
    extern EPD epd;
#elif defined(DEPG0579RYT158FxX)
    extern EPD epd;
#elif defined(EPD7IN3E)
    extern EPD730E epd;
#elif defined(EPD3IN6E)
    extern EPD360E epd;
#else
    extern EPD epd;
#endif

extern OLCD114 oLCD114;

extern WiFiConfig wiFiConfig;
extern int UDP_SemdTime;
extern int Localport;
extern IPAddress ServerIp;
extern int Serverport;
extern String GetwayStr;


extern bool flag_WS2812B_Refresh;
extern bool flag_WS2812B_breathing_ON_OFF;
extern bool flag_WS2812B_breathing_Ex_ON_OFF;
extern bool flag_WS2812B_breathing_Ex_lightOff;
extern bool flag_WS2812B_breathing_Ex_dTrigger;

extern int WS2812B_breathing_ON_OFF_cnt;
extern int WS2812B_breathing_Ex_ON_OFF_cnt;
extern int WS2812B_breathing_onAddVal;
extern int WS2812B_breathing_offSubVal;

extern float WS2812B_breathing_val;

extern byte WS2812B_breathing_R;
extern byte WS2812B_breathing_G;
extern byte WS2812B_breathing_B;


extern MyWS2812 myWS2812;
extern byte* framebuffer;

extern MyTimer MyTimer_BoardInit;
extern MyTimer MyTimer_OLCD_144_Init;
extern MyTimer MyTimer_CheckWS2812;
extern MyTimer MyTimer_CheckWIFI;

extern bool flag_boradInit;
extern bool flag_OLCD_144_boradInit;

extern MyLED MyLED_IS_Connented;

extern SemaphoreHandle_t xSpiMutex;

extern SoftwareSerial mySerial;
extern SoftwareSerial mySerial2;

#endif
