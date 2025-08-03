#line 1 "C:\\Users\\Evan\\source\\repos\\H_Pannel\\MCU-BW16\\BW16_EPD266\\libraries\\ArduinoJson\\extras\\tests\\MixedConfiguration\\issue1707.cpp"
// ArduinoJson - https://arduinojson.org
// Copyright © 2014-2025, Benoit BLANCHON
// MIT License

#define ARDUINO
#define memcpy_P(dest, src, n) memcpy((dest), (src), (n))

#include <ArduinoJson.h>

#include <catch.hpp>

TEST_CASE("Issue1707") {
  JsonDocument doc;

  DeserializationError err = deserializeJson(doc, F("{\"hello\":12}"));
  REQUIRE(err == DeserializationError::Ok);
}
