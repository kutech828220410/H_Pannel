#line 1 "C:\\Users\\Evan\\source\\repos\\H_Pannel\\MCU-BW16\\BW16_EPD266\\libraries\\ArduinoJson\\src\\ArduinoJson\\Deserialization\\Readers\\VariantReader.hpp"
// ArduinoJson - https://arduinojson.org
// Copyright © 2014-2025, Benoit BLANCHON
// MIT License

#pragma once

#include <ArduinoJson/Object/MemberProxy.hpp>
#include <ArduinoJson/Variant/JsonVariantConst.hpp>

ARDUINOJSON_BEGIN_PRIVATE_NAMESPACE

template <typename TVariant>
struct Reader<TVariant, enable_if_t<IsVariant<TVariant>::value>>
    : Reader<char*, void> {
  explicit Reader(const TVariant& x)
      : Reader<char*, void>(x.template as<const char*>()) {}
};

ARDUINOJSON_END_PRIVATE_NAMESPACE
