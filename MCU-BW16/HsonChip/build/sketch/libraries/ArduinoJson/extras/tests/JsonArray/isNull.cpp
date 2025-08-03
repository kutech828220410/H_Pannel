#line 1 "C:\\Users\\Evan\\source\\repos\\H_Pannel\\MCU-BW16\\BW16_EPD266\\libraries\\ArduinoJson\\extras\\tests\\JsonArray\\isNull.cpp"
// ArduinoJson - https://arduinojson.org
// Copyright © 2014-2025, Benoit BLANCHON
// MIT License

#include <ArduinoJson.h>
#include <catch.hpp>

TEST_CASE("JsonArray::isNull()") {
  SECTION("returns true") {
    JsonArray arr;
    REQUIRE(arr.isNull() == true);
  }

  SECTION("returns false") {
    JsonDocument doc;
    JsonArray arr = doc.to<JsonArray>();
    REQUIRE(arr.isNull() == false);
  }
}

TEST_CASE("JsonArray::operator bool()") {
  SECTION("returns false") {
    JsonArray arr;
    REQUIRE(static_cast<bool>(arr) == false);
  }

  SECTION("returns true") {
    JsonDocument doc;
    JsonArray arr = doc.to<JsonArray>();
    REQUIRE(static_cast<bool>(arr) == true);
  }
}
