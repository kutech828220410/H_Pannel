#line 1 "C:\\Users\\Evan\\source\\repos\\H_Pannel\\MCU-BW16\\BW16_EPD266\\libraries\\ArduinoJson\\extras\\tests\\Misc\\custom_string.hpp"
// ArduinoJson - https://arduinojson.org
// Copyright © 2014-2025, Benoit BLANCHON
// MIT License

#pragma once

#include <string>

struct custom_char_traits : std::char_traits<char> {};

using custom_string = std::basic_string<char, custom_char_traits>;
