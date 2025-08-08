#ifndef UDP_REPORTER_H
#define UDP_REPORTER_H

#include <Arduino.h>
#include <ArduinoJson.h>
#include "Timer.h"
#include "Config.h"
#include "Global.h"
#include "IO.h"
#include "UART0_Handler.h"
#include "UART1_Handler.h"


// 外部變數
extern int cnt_UDP_Send;
extern MyTimer UDP_Send_Timer;
extern DynamicJsonDocument doc;
extern String JsonOutput;
extern int RSSI;

// 外部函式
void sub_UDP_Send();

#endif
