#line 1 "C:\\Users\\Evan\\source\\repos\\H_Pannel\\MCU-BW16\\BW16_EPD266\\libraries\\ArduinoJson\\extras\\tests\\JsonVariant\\clear.cpp"
// ArduinoJson - https://arduinojson.org
// Copyright © 2014-2025, Benoit BLANCHON
// MIT License

#include <ArduinoJson.h>
#include <stdint.h>
#include <catch.hpp>

#include "Allocators.hpp"
#include "Literals.hpp"

TEST_CASE("JsonVariant::clear()") {
  SpyingAllocator spy;
  JsonDocument doc(&spy);
  JsonVariant var = doc.to<JsonVariant>();

  SECTION("size goes back to zero") {
    var.add(42);
    var.clear();

    REQUIRE(var.size() == 0);
  }

  SECTION("isNull() return true") {
    var.add("hello");
    var.clear();

    REQUIRE(var.isNull() == true);
  }

  SECTION("releases owned string") {
    var.set("hello"_s);
    var.clear();

    REQUIRE(spy.log() == AllocatorLog{
                             Allocate(sizeofString("hello")),
                             Deallocate(sizeofString("hello")),
                         });
  }
}
