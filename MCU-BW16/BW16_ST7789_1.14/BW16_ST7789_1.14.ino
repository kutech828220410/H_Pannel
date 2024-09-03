#include <SPI.h>

#define TFT_CS  PB3    // 屏幕1的 CS 引脚
#define TFT_CS2  PA15   // 屏幕2的 CS 引脚
#define TFT_DC   PA27   // DC 引脚
#define TFT_RST  -1     // 如果不使用 RST 引脚，设为 -1
// 定义 SPI 设置，假设目标频率为 20 MHz
// 定义 SPI 设置，假设目标频率为 20 MHz
SPISettings spiSettings(20000000, MSBFIRST, SPI_MODE0);

void setup() {
  // 初始化引脚
  pinMode(TFT_CS, OUTPUT);
  pinMode(TFT_DC, OUTPUT);

  // 初始化 SPI
  SPI.begin();

  // 初始化显示器
  initST7789();
}

void loop() {
  // 示例：填充整个屏幕为蓝色
  fillScreen(0x001F);  // 0x001F 是 16 位 RGB565 格式的蓝色
  delay(1000);
  
  // 示例：填充整个屏幕为红色
  fillScreen(0xF800);  // 0xF800 是 16 位 RGB565 格式的红色
  delay(1000);
}

// 初始化 ST7789
void initST7789() {
  sendCommand(0x01); // 软件重置
  delay(150);
  
  sendCommand(0x11); // 退出休眠模式
  delay(500);
  
  // 设置颜色格式为 RGB565
  sendCommand(0x3A);
  sendData(0x55);
  
  // 开启显示器
  sendCommand(0x29);
  delay(100);
}

// 发送命令到 ST7789
void sendCommand(uint8_t command) {
  SPI.beginTransaction(spiSettings);  // 开始 SPI 事务
  digitalWrite(TFT_DC, LOW);  // 命令模式
  digitalWrite(TFT_CS, LOW);
  SPI.transfer(command);
  digitalWrite(TFT_CS, HIGH);
  SPI.endTransaction();  // 结束 SPI 事务
}

// 发送数据到 ST7789
void sendData(uint8_t data) {
  SPI.beginTransaction(spiSettings);  // 开始 SPI 事务
  digitalWrite(TFT_DC, HIGH);  // 数据模式
  digitalWrite(TFT_CS, LOW);
  SPI.transfer(data);
  digitalWrite(TFT_CS, HIGH);
  SPI.endTransaction();  // 结束 SPI 事务
}

// 填充整个屏幕
void fillScreen(uint16_t color) {
  setAddrWindow(0, 0, 134, 239);  // 修正窗口范围
  uint32_t numPixels = 135 * 240;
  SPI.beginTransaction(spiSettings);
  digitalWrite(TFT_CS, LOW);
  for (uint32_t i = 0; i < numPixels; i++) {
    SPI.transfer16(color);  // 16位传输
  }
  digitalWrite(TFT_CS, HIGH);
  SPI.endTransaction();
}

// 设置地址窗口（用于绘制矩形或填充区域）
void setAddrWindow(uint16_t x0, uint16_t y0, uint16_t x1, uint16_t y1) {
  sendCommand(0x2A); // 列地址设置
  sendData(x0 >> 8);
  sendData(x0 & 0xFF);
  sendData(x1 >> 8);
  sendData(x1 & 0xFF);

  sendCommand(0x2B); // 行地址设置
  sendData(y0 >> 8);
  sendData(y0 & 0xFF);
  sendData(y1 >> 8);
  sendData(y1 & 0xFF);

  sendCommand(0x2C); // 内存写入
}
