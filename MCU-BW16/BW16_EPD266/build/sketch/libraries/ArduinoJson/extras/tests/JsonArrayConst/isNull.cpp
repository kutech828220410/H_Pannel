#line 1 "C:\\Users\\Evan\\source\\repos\\H_Pannel\\MCU-BW16\\BW16_EPD266\\libraries\\ArduinoJson\\extras\\tests\\JsonArrayConst\\isNull.cpp"
// ArduinoJson - https://arduinojson.org
// Copyright © 2014-2025, Benoit BLANCHON
// MIT License

#include <ArduinoJson.h>
#include <catch.hpp>

TEST_CASE("JsonArrayConst::isNull()") {
  SECTION("returns true") {
    JsonArrayConst arr;
    REQUIRE(arr.isNull() == true);
  }

  SECTION("returns false") {
    JsonDocument doc;
    JsonArrayConst arr = doc.to<JsonArray>();
    REQUIRE(arr.isNull() == false);
  }
}

TEST_CASE("JsonArrayConst::operator bool()") {
  SECTION("returns false") {
    JsonArrayConst arr;
    REQUIRE(static_cast<bool>(arr) == false);
  }

  SECTION("returns true") {
    JsonDocument doc;
    JsonArrayConst arr = doc.to<JsonArray>();
    REQUIRE(static_cast<bool>(arr) == true);
  }
}
