#include <SoftwareSerial.h>
#include <SPI.h>
#include "LT768X.h"
#include "MyJPEGDecoder.h"
#include "image.h"

#include "TAMC_GT911.h"


#define TOUCH_SDA  PA26
#define TOUCH_SCL  PA25
#define TOUCH_INT -1
#define TOUCH_RST -1
#define TOUCH_WIDTH  800
#define TOUCH_HEIGHT 480
TAMC_GT911 tp = TAMC_GT911(TOUCH_SDA, TOUCH_SCL, TOUCH_INT, TOUCH_RST, TOUCH_WIDTH, TOUCH_HEIGHT);

SoftwareSerial mySerial(PA8, PA7); // RX, TX
String Version = "Ver 1.0.0";

LT768X lT768X;
void setup()
{
    SPI.begin();
    mySerial.begin(115200);   
    mySerial.println(Version);
    lT768X.Init(&mySerial);
    lT768X.Display_ON();
    lT768X.Set_Backlight(100);

    mySerial.println("GT911 Example: Ready");
    tp.begin();
    mySerial.println("GT911.begin()");
//    LT768_Init(&mySerial);
//    Display_ON();
//    Color_Bar();
//    
//    LT768_
//
//    Set_PWM_Prescaler_1_to_256(0x02);
//    LT768_DrawSquare_Fill(0,799,0,479, color65k_yellow);
  
//    mySerial.println("RA8875 Init done!");
//    if (!tft.begin(RA8875_800x480))
//    {
//      mySerial.println("RA8875 Not Found!");
//
//    }
//    else
//    {
//      mySerial.println("RA8875 Founded!");
//    }

}

void loop()
{
//  tp.read();
//  if (tp.isTouched){
//    for (int i=0; i<tp.touches; i++){
//      Serial.print("Touch ");Serial.print(i+1);Serial.print(": ");;
//      Serial.print("  x: ");Serial.print(tp.points[i].x);
//      Serial.print("  y: ");Serial.print(tp.points[i].y);
//      Serial.print("  size: ");Serial.println(tp.points[i].size);
//      Serial.println(' ');
//    }
//  }
//   lT768X.fillScreen(color65k_red);
//   delay(1000);   
//   lT768X.fillScreen(color65k_blue);
//   delay(1000);
   lT768X.Draw_Picture(0,0,800 ,480 , 768000,gImage_test);
//   delay(1000);
//   lT768X.fillScreen(color65k_white);
//   delay(1000);
//   lT768X.fillScreen(color65k_purple);
//   delay(1000);
//  uint8_t value = 0;
//  SPI.transfer(0x01);
//  mySerial.print(value);mySerial.println("");
//  value = SPI.transfer(0x02);
//  mySerial.print(value);mySerial.println(""); 
//  value = SPI.transfer(0x04);
//  mySerial.print(value);mySerial.println("");
//  delay(100);

}
