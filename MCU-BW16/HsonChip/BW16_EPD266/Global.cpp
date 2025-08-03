#include "Global.h"

#ifdef MCP23017
DFRobot_MCP23017 mcp(Wire, /*addr =*/0x20);//constructor, change the Level of A2, A1, A0 via DIP switch to revise the I2C address within 0x20~0x27.
#endif


#ifdef DHTSensor
DHT dht(DHTPIN, DHTTYPE);
float dht_h = 0;
float dht_t = 0;
float dht_f = 0;
float dht_hif = 0;
float dht_hic = 0;
#endif

bool flag_udp_232back = false;
bool flag_JsonSend = false;
bool flag_writeMode = false;

#if defined(EPD266)
    EPD epd;
#elif defined(EPD290)
    EPD epd;
#elif defined(EPD213)
    EPD epd;
#elif defined(EPD420)
    EPD epd;
#elif defined(EPD420_D)
    EPD epd;
#elif defined(EPD583)
    EPD epd;
#elif defined(EPD213_BRW_V0)
    EPD epd;
#elif defined(EPD579G)
    EPD epd;
#elif defined(EPD579B)
    EPD epd;
#elif defined(DEPG0579RYT158FxX)
    EPD epd;
#elif defined(EPD7IN3E)
    EPD730E epd;
#elif defined(EPD3IN6E)
    EPD360E epd;    
#else
    EPD epd;
#endif

OLCD114 oLCD114;

WiFiConfig wiFiConfig;
int UDP_SemdTime = 0;
int Localport = 0;
IPAddress ServerIp;
int Serverport;
String GetwayStr;


bool flag_WS2812B_Refresh = true;
bool flag_WS2812B_breathing_ON_OFF  = false;
bool flag_WS2812B_breathing_Ex_ON_OFF  = false;
bool flag_WS2812B_breathing_Ex_lightOff  = false;
bool flag_WS2812B_breathing_Ex_dTrigger = false;
int WS2812B_breathing_ON_OFF_cnt = 0;
int WS2812B_breathing_Ex_ON_OFF_cnt = 0;
int WS2812B_breathing_onAddVal  = 20;
int WS2812B_breathing_offSubVal  = 20;
float WS2812B_breathing_val = 0.1F;
byte WS2812B_breathing_R = 255;
byte WS2812B_breathing_G = 0;
byte WS2812B_breathing_B = 0;

MyWS2812 myWS2812;
byte* framebuffer;

MyTimer MyTimer_BoardInit;
MyTimer MyTimer_OLCD_144_Init;
MyTimer MyTimer_CheckWS2812;
MyTimer MyTimer_CheckWIFI;

bool flag_boradInit = false;
bool flag_OLCD_144_boradInit = false;

MyLED MyLED_IS_Connented;

SemaphoreHandle_t xSpiMutex;

SoftwareSerial mySerial(PA8, PA7); // RX, TX
SoftwareSerial mySerial2(PB2, PB1); // RX, TX
