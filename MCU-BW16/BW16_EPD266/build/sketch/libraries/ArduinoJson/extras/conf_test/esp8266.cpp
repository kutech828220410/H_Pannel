#line 1 "C:\\Users\\Evan\\source\\repos\\H_Pannel\\MCU-BW16\\BW16_EPD266\\libraries\\ArduinoJson\\extras\\conf_test\\esp8266.cpp"
#include <ArduinoJson.h>

static_assert(ARDUINOJSON_USE_LONG_LONG == 1, "ARDUINOJSON_USE_LONG_LONG");

static_assert(ARDUINOJSON_SLOT_ID_SIZE == 2, "ARDUINOJSON_SLOT_ID_SIZE");

static_assert(ARDUINOJSON_POOL_CAPACITY == 128, "ARDUINOJSON_POOL_CAPACITY");

static_assert(ARDUINOJSON_LITTLE_ENDIAN == 1, "ARDUINOJSON_LITTLE_ENDIAN");

static_assert(ARDUINOJSON_USE_DOUBLE == 1, "ARDUINOJSON_USE_DOUBLE");

static_assert(ArduinoJson::detail::ResourceManager::slotSize == 8, "slot size");

void setup() {}
void loop() {}
