#ifndef UART0_HANDLER_H
#define UART0_HANDLER_H

#include <Arduino.h>
#include "Global.h"
#include "Timer.h"
#include "Config.h"
#include "IO.h"



// 緩衝區大小
#define UART0_RX_SIZE 256

// 外部變數宣告
extern byte UART0_RX[UART0_RX_SIZE];
extern int UART0_len;
extern MyTimer MyTimer_UART0;

// 外部函式
void serialEvent();
void Get_Checksum();

#endif
