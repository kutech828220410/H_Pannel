#line 1 "C:\\Users\\Evan\\source\\repos\\H_Pannel\\MCU-BW16\\BW16_EPD266\\libraries\\ArduinoJson\\src\\ArduinoJson\\Polyfills\\type_traits\\is_const.hpp"
// ArduinoJson - https://arduinojson.org
// Copyright © 2014-2025, Benoit BLANCHON
// MIT License

#pragma once

#include "integral_constant.hpp"

ARDUINOJSON_BEGIN_PRIVATE_NAMESPACE

// A meta-function that return the type T without the const modifier
template <typename T>
struct is_const : false_type {};

template <typename T>
struct is_const<const T> : true_type {};

ARDUINOJSON_END_PRIVATE_NAMESPACE
