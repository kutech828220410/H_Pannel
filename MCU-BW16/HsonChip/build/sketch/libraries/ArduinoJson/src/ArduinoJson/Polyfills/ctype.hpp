#line 1 "C:\\Users\\Evan\\source\\repos\\H_Pannel\\MCU-BW16\\BW16_EPD266\\libraries\\ArduinoJson\\src\\ArduinoJson\\Polyfills\\ctype.hpp"
// ArduinoJson - https://arduinojson.org
// Copyright © 2014-2025, Benoit BLANCHON
// MIT License

#pragma once

#include <ArduinoJson/Namespace.hpp>

ARDUINOJSON_BEGIN_PRIVATE_NAMESPACE

#ifndef isdigit
inline bool isdigit(char c) {
  return '0' <= c && c <= '9';
}
#endif

inline bool issign(char c) {
  return '-' == c || c == '+';
}

ARDUINOJSON_END_PRIVATE_NAMESPACE
