#include <DFRobot_MCP23017.h>

DFRobot_MCP23017 mcp(Wire, /*addr =*/0x20);//constructor, change the Level of A2, A1, A0 via DIP switch to revise the I2C address within 0x20~0x27.

void setup()
{
  Serial.begin(115200);
  while(mcp.begin() != 0){
    Serial.println("Initialization of the chip failed, please confirm that the chip connection is correct!");
    delay(1000);
  }
  mcp.pinMode(/*pin = */mcp.eGPA0, /*mode = */INPUT_PULLUP);
  mcp.pinMode(/*pin = */mcp.eGPA1, /*mode = */INPUT_PULLUP);
  mcp.pinMode(/*pin = */mcp.eGPA2, /*mode = */INPUT_PULLUP);
  mcp.pinMode(/*pin = */mcp.eGPA3, /*mode = */INPUT_PULLUP);
  mcp.pinMode(/*pin = */mcp.eGPA4, /*mode = */INPUT_PULLUP);
  mcp.pinMode(/*pin = */mcp.eGPA5, /*mode = */INPUT_PULLUP);
  mcp.pinMode(/*pin = */mcp.eGPA6, /*mode = */INPUT_PULLUP);
  mcp.pinMode(/*pin = */mcp.eGPA7, /*mode = */INPUT_PULLUP);

  mcp.pinMode(/*pin = */mcp.eGPB0, /*mode = */OUTPUT);

  pinMode(PA14, OUTPUT);
  pinMode(PA12, INPUT_PULLUP);
 
}
bool falg_G = false;
void loop() 
{

  uint8_t value = mcp.digitalRead(/*pin = */mcp.eGPA);
 
  printf("value : %d\n",value);
  // put your main code here, to run repeatedly:
  digitalWrite(PA14 , falg_G);
  mcp.digitalWrite(mcp.eGPB0 , falg_G);
  falg_G = !falg_G;
  digitalWrite(PA14 ,digitalRead(PA12));
  
  delay(500);
}
