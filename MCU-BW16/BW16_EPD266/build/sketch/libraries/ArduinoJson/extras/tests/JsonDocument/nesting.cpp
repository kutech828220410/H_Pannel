#line 1 "C:\\Users\\Evan\\source\\repos\\H_Pannel\\MCU-BW16\\BW16_EPD266\\libraries\\ArduinoJson\\extras\\tests\\JsonDocument\\nesting.cpp"
// ArduinoJson - https://arduinojson.org
// Copyright © 2014-2025, Benoit BLANCHON
// MIT License

#include <ArduinoJson.h>
#include <catch.hpp>

TEST_CASE("JsonDocument::nesting()") {
  JsonDocument doc;

  SECTION("return 0 if uninitialized") {
    REQUIRE(doc.nesting() == 0);
  }

  SECTION("returns 0 for string") {
    JsonVariant var = doc.to<JsonVariant>();
    var.set("hello");
    REQUIRE(doc.nesting() == 0);
  }

  SECTION("returns 1 for empty object") {
    doc.to<JsonObject>();
    REQUIRE(doc.nesting() == 1);
  }

  SECTION("returns 1 for empty array") {
    doc.to<JsonArray>();
    REQUIRE(doc.nesting() == 1);
  }
}
