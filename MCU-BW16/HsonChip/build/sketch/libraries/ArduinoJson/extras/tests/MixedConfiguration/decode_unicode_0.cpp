#line 1 "C:\\Users\\Evan\\source\\repos\\H_Pannel\\MCU-BW16\\BW16_EPD266\\libraries\\ArduinoJson\\extras\\tests\\MixedConfiguration\\decode_unicode_0.cpp"
#define ARDUINOJSON_DECODE_UNICODE 0
#include <ArduinoJson.h>

#include <catch.hpp>

TEST_CASE("ARDUINOJSON_DECODE_UNICODE == 0") {
  JsonDocument doc;
  DeserializationError err = deserializeJson(doc, "\"\\uD834\\uDD1E\"");

  REQUIRE(err == DeserializationError::Ok);
  REQUIRE(doc.as<std::string>() == "\\uD834\\uDD1E");
}
