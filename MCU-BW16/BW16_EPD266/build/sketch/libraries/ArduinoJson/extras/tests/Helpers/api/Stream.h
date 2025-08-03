#line 1 "C:\\Users\\Evan\\source\\repos\\H_Pannel\\MCU-BW16\\BW16_EPD266\\libraries\\ArduinoJson\\extras\\tests\\Helpers\\api\\Stream.h"
// ArduinoJson - https://arduinojson.org
// Copyright © 2014-2025, Benoit BLANCHON
// MIT License

#pragma once

// Reproduces Arduino's Stream class
class Stream  // : public Print
{
 public:
  virtual ~Stream() {}
  virtual int read() = 0;
  virtual size_t readBytes(char* buffer, size_t length) = 0;
};
