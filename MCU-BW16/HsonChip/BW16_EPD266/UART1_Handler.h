#ifndef UART1_HANDLER_H
#define UART1_HANDLER_H

#include <Arduino.h>
#include "Timer.h"
#include "Global.h"
#include "Config.h"
#include "IO.h"


// 設定 UART1 緩衝區大小
#define UART1_RX_SIZE 256

// 外部變數宣告
extern byte UART1_RX[UART1_RX_SIZE];
extern int UART1_len;
extern MyTimer MyTimer_UART1;

extern String str_distance;
extern String str_TOF10120;
extern int LaserDistance;
extern int LASER_ON_cnt;
extern bool LASER_ON;
extern bool LASER_ON_buf;
extern bool flag_UART1_Init;
extern int LASER_ON_num;

// 外部函式宣告
void serial2Event();

#endif
