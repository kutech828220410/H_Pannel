#define VERSION "Ver 1.5.501"
//#define MQTT
//#define DHTSensor
//#define HandSensor
//#define RowLED_Device
//#define EPD213
//#define EPD266
#define EPD290
//#define EPD420
//#define EPD420_D
//#define EPD583
//#define EPD579G
//#define EPD579B
//#define DEPG0579RYT158FxX
//#define EPD213_BRW_V0
//#define EPD7IN3E

//#define OLCD_114
//#define MCP23017

#ifdef DHTSensor
#define DHTPIN PA27
#define DHTTYPE DHT22   // DHT 22  (AM2302), AM2321
#endif


#define LASER_D_MIN 20
#define LASER_D_MAX 115


#ifdef EPD266
#define EPD_WIDTH 19
#define EPD_HEIGHT 296
#define EPD_TYPE "EPD266"
#define EPD_Device

#elif defined(EPD290)
#define EPD_WIDTH 16
#define EPD_HEIGHT 296
#define EPD_TYPE "EPD290"
#define EPD_Device

#elif defined(EPD213)
#define EPD_WIDTH 16
#define EPD_HEIGHT 250
#define EPD_TYPE "EPD213"
#define EPD_Device

#elif defined(EPD420)
#define EPD_WIDTH 50
#define EPD_HEIGHT 300
#define EPD_TYPE "EPD420"
#define EPD_Device

#elif defined(EPD420_D)
#define EPD_WIDTH 50
#define EPD_HEIGHT 300
#define EPD_TYPE "EPD420_D"
#define EPD_Device  
                                                                                                                                                               
#elif defined(EPD583)
#define EPD_WIDTH 81
#define EPD_HEIGHT 480
#define EPD_TYPE "EPD583"
#define EPD_Device

#elif defined(EPD213_BRW_V0)
#define EPD_WIDTH 31
#define EPD_HEIGHT 250
#define EPD_TYPE "EPD213_BRW_V0"
#define EPD_Device

#elif defined(EPD579G)
#define EPD_WIDTH 198
#define EPD_HEIGHT 272
#define EPD_TYPE "EPD579G"
#define EPD_Device

#elif defined(EPD579B)
#define EPD_WIDTH 100
#define EPD_HEIGHT 272
#define EPD_TYPE "EPD579B"
#define EPD_Device

#elif defined(DEPG0579RYT158FxX)
#define EPD_WIDTH 198
#define EPD_HEIGHT 272
#define EPD_TYPE "DEPG0579RYT158FxX"
#define EPD_Device

#elif defined(EPD7IN3E)
#define EPD_WIDTH 198
#define EPD_HEIGHT 272
#define EPD_TYPE "EPD7IN3E"
#define EPD_Device

#else
#define EPD_WIDTH 0
#define EPD_HEIGHT 0
#define EPD_TYPE ""
#endif



#ifdef EPD_Device
#define Device "EPD"
#elif defined(OLCD_114)
#define Device "OLCD_114"
#else
#define Device ""
#endif
