#line 1 "C:\\Users\\Evan\\source\\repos\\H_Pannel\\MCU-BW16\\BW16_EPD266\\libraries\\ArduinoJson\\extras\\fuzzing\\msgpack_fuzzer.cpp"
#include <ArduinoJson.h>

extern "C" int LLVMFuzzerTestOneInput(const uint8_t* data, size_t size) {
  JsonDocument doc;
  DeserializationError error = deserializeMsgPack(doc, data, size);
  if (!error) {
    std::string json;
    serializeMsgPack(doc, json);
  }
  return 0;
}
