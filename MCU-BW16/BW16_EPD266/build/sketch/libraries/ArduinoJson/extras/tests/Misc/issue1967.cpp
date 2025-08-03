#line 1 "C:\\Users\\Evan\\source\\repos\\H_Pannel\\MCU-BW16\\BW16_EPD266\\libraries\\ArduinoJson\\extras\\tests\\Misc\\issue1967.cpp"
// ArduinoJson - https://arduinojson.org
// Copyright © 2014-2025, Benoit BLANCHON
// MIT License

// we expect ArduinoJson.h to include <string>
#define ARDUINOJSON_ENABLE_STD_STRING 1

// but we don't want it to included accidentally
#undef ARDUINO
#define ARDUINOJSON_ENABLE_STD_STREAM 0
#define ARDUINOJSON_ENABLE_STRING_VIEW 0

#include <ArduinoJson.h>
