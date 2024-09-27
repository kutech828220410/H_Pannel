#define VERSION "Ver 1.5.11"

//#define EPD213
//#define EPD266
//#define EPD290
#define EPD420

#ifdef EPD266
#define EPD_WIDTH 19
#define EPD_HEIGHT 296
#define EPD_TYPE "EPD266"

#elif defined(EPD290)
#define EPD_WIDTH 16
#define EPD_HEIGHT 296
#define EPD_TYPE "EPD290"

#elif defined(EPD213)
#define EPD_WIDTH 16
#define EPD_HEIGHT 250
#define EPD_TYPE "EPD213"

#elif defined(EPD420)
#define EPD_WIDTH 50
#define EPD_HEIGHT 300
#define EPD_TYPE "EPD420"

#endif


//#define EPD_Device
//#define RowLED_Device
#define OLED114_HandSensor


#ifdef EPD_Device
#define Device "EPD"
#elif defined(OLED114_HandSensor)
#define Device "OLED114_HandSensor"
#endif
