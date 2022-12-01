#ifndef __Button_h
#define __Button_h
#include <TFT_eSPI.h>
#include "Fonts.h"
#include "FT62XXTouchScreen.h"
#define BUTTON_ON 1
#define BUTTON_OFF 0
 typedef void (*ButtonHandle) (void);
 class Button
 {
   public:
    Button();
    Button(int location_X, int location_Y, int Size_X, int Size_Y, String Text_ON, String Text_OFF);
    void Init(int location_X, int location_Y, int Size_X, int Size_Y, String Text_ON, String Text_OFF);
    int Draw(TFT_eSPI tft , TouchPoint touchPos);
    int Draw(TFT_eSPI tft);
    void Draw_OFF_State(TFT_eSPI tft);
    void Draw_ON_State(TFT_eSPI tft);
    void Set_Font_SIZE(uint16_t ON_SIZE, uint16_t OFF_SIZE);
    void Set_State(uint16_t state);
    bool IsPress(TouchPoint touchPos);
    ButtonHandle ButtonPressDown = nullptr;
    ButtonHandle ButtonPressUp = nullptr;
    String Text_ON = "";
    String Text_OFF = "";
    uint16_t FONT_SIZE_ON = FONT4;
    uint16_t BGColor_ON = TFT_BLACK;
    uint16_t FontColor_ON = TFT_WHITE;
    uint16_t BGColor_OFF = TFT_DARKGREY;
    uint16_t FONT_SIZE_OFF = FONT4;
    uint16_t FontColor_OFF = TFT_WHITE;
   private:   
    bool flag_ON_OFF = false;
    int location_X = 0;
    int location_Y = 0;
    int Size_X = 0;
    int Size_Y = 0;
    int state = 0;
    int state_buf = -1;
    
 };

  #endif
