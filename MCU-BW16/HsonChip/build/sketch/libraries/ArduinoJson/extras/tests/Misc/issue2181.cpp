#line 1 "C:\\Users\\Evan\\source\\repos\\H_Pannel\\MCU-BW16\\BW16_EPD266\\libraries\\ArduinoJson\\extras\\tests\\Misc\\issue2181.cpp"
// ArduinoJson - https://arduinojson.org
// Copyright © 2014-2025, Benoit BLANCHON
// MIT License

#define true 0x1
#define false 0x0

#include <ArduinoJson.h>
#include <catch.hpp>

TEST_CASE("Issue #2181") {
  JsonDocument doc;
  doc["hello"] = "world";
  REQUIRE(doc.as<std::string>() == "{\"hello\":\"world\"}");
}
