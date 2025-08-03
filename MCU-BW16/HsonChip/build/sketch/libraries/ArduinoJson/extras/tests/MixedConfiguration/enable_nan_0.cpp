#line 1 "C:\\Users\\Evan\\source\\repos\\H_Pannel\\MCU-BW16\\BW16_EPD266\\libraries\\ArduinoJson\\extras\\tests\\MixedConfiguration\\enable_nan_0.cpp"
#define ARDUINOJSON_ENABLE_NAN 0
#include <ArduinoJson.h>

#include <catch.hpp>
#include <limits>

TEST_CASE("ARDUINOJSON_ENABLE_NAN == 0") {
  JsonDocument doc;
  JsonObject root = doc.to<JsonObject>();

  SECTION("serializeJson()") {
    root["X"] = std::numeric_limits<double>::signaling_NaN();

    std::string json;
    serializeJson(doc, json);

    REQUIRE(json == "{\"X\":null}");
  }

  SECTION("deserializeJson()") {
    DeserializationError err = deserializeJson(doc, "{\"X\":NaN}");

    REQUIRE(err == DeserializationError::InvalidInput);
  }
}
