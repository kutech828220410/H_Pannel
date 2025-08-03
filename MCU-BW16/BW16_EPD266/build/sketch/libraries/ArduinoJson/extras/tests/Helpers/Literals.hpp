#line 1 "C:\\Users\\Evan\\source\\repos\\H_Pannel\\MCU-BW16\\BW16_EPD266\\libraries\\ArduinoJson\\extras\\tests\\Helpers\\Literals.hpp"
// ArduinoJson - https://arduinojson.org
// Copyright © 2014-2025, Benoit BLANCHON
// MIT License

#pragma once

#include <string>

// the space before _s is required by GCC 4.8
inline std::string operator"" _s(const char* str, size_t len) {
  return std::string(str, len);
}
