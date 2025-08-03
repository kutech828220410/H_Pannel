#line 1 "C:\\Users\\Evan\\source\\repos\\H_Pannel\\MCU-BW16\\BW16_EPD266\\libraries\\ArduinoJson\\src\\ArduinoJson\\Polyfills\\type_traits\\is_enum.hpp"
// ArduinoJson - https://arduinojson.org
// Copyright © 2014-2025, Benoit BLANCHON
// MIT License

#pragma once

#include "is_class.hpp"
#include "is_convertible.hpp"
#include "is_floating_point.hpp"
#include "is_integral.hpp"
#include "is_same.hpp"

ARDUINOJSON_BEGIN_PRIVATE_NAMESPACE

template <typename T>
struct is_enum {
  static const bool value = is_convertible<T, long long>::value &&
                            !is_class<T>::value && !is_integral<T>::value &&
                            !is_floating_point<T>::value;
};

ARDUINOJSON_END_PRIVATE_NAMESPACE
