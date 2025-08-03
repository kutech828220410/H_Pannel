#line 1 "C:\\Users\\Evan\\source\\repos\\H_Pannel\\MCU-BW16\\BW16_EPD266\\libraries\\ArduinoJson\\extras\\tests\\ResourceManager\\clear.cpp"
// ArduinoJson - https://arduinojson.org
// Copyright © 2014-2025, Benoit BLANCHON
// MIT License

#include <ArduinoJson/Memory/ResourceManager.hpp>
#include <ArduinoJson/Memory/ResourceManagerImpl.hpp>
#include <ArduinoJson/Strings/StringAdapters.hpp>
#include <catch.hpp>

using namespace ArduinoJson::detail;

TEST_CASE("ResourceManager::clear()") {
  ResourceManager resources;

  SECTION("Discards allocated variants") {
    resources.allocVariant();

    resources.clear();
    REQUIRE(resources.size() == 0);
  }

  SECTION("Discards allocated strings") {
    resources.saveString(adaptString("123456789"));
    REQUIRE(resources.size() == sizeofString(9));

    resources.clear();

    REQUIRE(resources.size() == 0);
  }
}
