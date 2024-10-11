#define VERSION "Ver 1.5.17"
//#define HandSensor
//#define RowLED_Device
//#define EPD213
//#define EPD266
//#define EPD290
#define EPD420
//#define OLCD_114
//#define MCP23017

#define LASER_D 50


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

#else
#define EPD_WIDTH 0
#define EPD_HEIGHT 0
#define EPD_TYPE ""
#endif



#ifdef EPD_Device
#define Device "EPD"
#elif defined(OLCD_114)
#define Device "OLCD_114"
#endif
