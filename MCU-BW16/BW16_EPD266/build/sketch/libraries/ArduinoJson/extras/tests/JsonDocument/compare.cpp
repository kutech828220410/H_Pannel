#line 1 "C:\\Users\\Evan\\source\\repos\\H_Pannel\\MCU-BW16\\BW16_EPD266\\libraries\\ArduinoJson\\extras\\tests\\JsonDocument\\compare.cpp"
// ArduinoJson - https://arduinojson.org
// Copyright © 2014-2025, Benoit BLANCHON
// MIT License

#include <ArduinoJson.h>
#include <catch.hpp>

TEST_CASE("JsonDocument::operator==(const JsonDocument&)") {
  JsonDocument doc1;
  JsonDocument doc2;

  SECTION("Empty") {
    REQUIRE(doc1 == doc2);
    REQUIRE_FALSE(doc1 != doc2);
  }

  SECTION("With same object") {
    doc1["hello"] = "world";
    doc2["hello"] = "world";
    REQUIRE(doc1 == doc2);
    REQUIRE_FALSE(doc1 != doc2);
  }
  SECTION("With different object") {
    doc1["hello"] = "world";
    doc2["world"] = "hello";
    REQUIRE_FALSE(doc1 == doc2);
    REQUIRE(doc1 != doc2);
  }
}
