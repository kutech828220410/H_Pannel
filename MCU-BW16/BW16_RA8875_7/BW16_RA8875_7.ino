#include "RA8871.h"
#include <SoftwareSerial.h>
#include <SPI.h>
#include "Adafruit_GFX.h"
#include "Adafruit_RA8875.h"

#include "LT768.h"

#define RA8875_CS PA15
#define RA8875_RESET PA25
RA8875 ra8875;
SoftwareSerial mySerial(PA8, PA7); // RX, TX
String Version = "Ver 1.0.0";
void setup()
{
    SPI.begin();
    mySerial.begin(115200);   
    mySerial.println(Version);
    ra8875.mySerial = &mySerial;
    ra8875.Init();
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

//  uint8_t value = 0;
//  SPI.transfer(0x01);
//  mySerial.print(value);mySerial.println("");
//  value = SPI.transfer(0x02);
//  mySerial.print(value);mySerial.println(""); 
//  value = SPI.transfer(0x04);
//  mySerial.print(value);mySerial.println("");
//  delay(100);

}
