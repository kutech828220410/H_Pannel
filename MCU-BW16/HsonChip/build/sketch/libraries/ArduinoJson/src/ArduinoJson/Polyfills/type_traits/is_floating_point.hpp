#line 1 "C:\\Users\\Evan\\source\\repos\\H_Pannel\\MCU-BW16\\BW16_EPD266\\libraries\\ArduinoJson\\src\\ArduinoJson\\Polyfills\\type_traits\\is_floating_point.hpp"
// ArduinoJson - https://arduinojson.org
// Copyright © 2014-2025, Benoit BLANCHON
// MIT License

#pragma once

#include "integral_constant.hpp"
#include "is_same.hpp"
#include "remove_cv.hpp"

ARDUINOJSON_BEGIN_PRIVATE_NAMESPACE

template <class T>
struct is_floating_point
    : integral_constant<bool,  //
                        is_same<float, remove_cv_t<T>>::value ||
                            is_same<double, remove_cv_t<T>>::value> {};

ARDUINOJSON_END_PRIVATE_NAMESPACE
