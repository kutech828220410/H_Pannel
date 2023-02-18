#include <Arduino.h>
#include "GT911.h"
#include "TAMC_GT911.h"

GT911 ts = GT911();

#define TOUCH_SDA  PA26
#define TOUCH_SCL  PA25
#define TOUCH_INT -1
#define TOUCH_RST -1
#define TOUCH_WIDTH  800
#define TOUCH_HEIGHT 480

TAMC_GT911 tp = TAMC_GT911(TOUCH_SDA, TOUCH_SCL, TOUCH_INT, TOUCH_RST, TOUCH_WIDTH, TOUCH_HEIGHT);
void setup() 
{
  Serial.begin(115200);
  Serial.println("GT911 Example: Ready");
  //tp.begin();
  tp.begin();
  Serial.println("GT911.begin()");

}

void loop()
{
//  uint8_t touches = ts.touched(GT911_MODE_POLLING);
//
//  if (touches) {
//    GTPoint* tp = ts.getPoints();
//    for (uint8_t  i = 0; i < touches; i++) {
//      Serial.print("12");
//    }
//  }
  tp.read();
  if (tp.isTouched){
    for (int i=0; i<tp.touches; i++){
      Serial.print("Touch ");Serial.print(i+1);Serial.print(": ");;
      Serial.print("  x: ");Serial.print(tp.points[i].x);
      Serial.print("  y: ");Serial.print(tp.points[i].y);
      Serial.print("  size: ");Serial.println(tp.points[i].size);
      Serial.println(' ');
    }
  }
}
