#line 1 "C:\\Users\\Evan\\source\\repos\\H_Pannel\\MCU-BW16\\BW16_EPD266\\libraries\\ArduinoJson\\extras\\tests\\Deprecated\\shallowCopy.cpp"
// ArduinoJson - https://arduinojson.org
// Copyright © 2014-2025, Benoit BLANCHON
// MIT License

#include <ArduinoJson.h>
#include <catch.hpp>

TEST_CASE("shallowCopy()") {
  JsonDocument doc1, doc2;
  doc1["b"] = "c";
  doc2["a"].shallowCopy(doc1);

  REQUIRE(doc2.as<std::string>() == "{\"a\":{\"b\":\"c\"}}");
}
