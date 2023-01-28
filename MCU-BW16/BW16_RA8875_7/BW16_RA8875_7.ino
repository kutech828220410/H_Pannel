#include <SoftwareSerial.h>
#include <SPI.h>
#define PIN_RST PA30
#define PIN_CS PA15

SoftwareSerial mySerial(PA8, PA7); // RX, TX
String Version = "Ver 1.0.0";
void setup()
{
    pinMode(PIN_RST, OUTPUT);
    pinMode(PIN_CS, OUTPUT);
    mySerial.begin(115200);   
    mySerial.println(Version);
    HardwareReset();
    SPI.begin();
    SPI_Begin();
}

void loop()
{

  uint8_t value = 0;
  SPI.transfer(0x01);
  mySerial.print(value);mySerial.println("");
  value = SPI.transfer(0x02);
  mySerial.print(value);mySerial.println(""); 
  value = SPI.transfer(0x04);
  mySerial.print(value);mySerial.println("");
  delay(100);

}

void SPI_Begin()
{
   mySerial.println("SPI.beginTransaction(SPISettings(2000000, MSBFIRST, SPI_MODE0));");
   SPI.beginTransaction(SPISettings(2000000, MSBFIRST, SPI_MODE0));
   mySerial.println("SPI.beginTransaction end!");
}
void SPI_End()
{
   SPI.endTransaction();
}
void HardwareReset()
{
   digitalWrite(PIN_RST, LOW);                //module reset    
   delay(1);
   digitalWrite(PIN_RST, HIGH);
   delay(10);   
}
