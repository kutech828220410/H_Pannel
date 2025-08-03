/*
  EEPROM.cpp - esp8266 EEPROM emulation

  Copyright (c) 2014 Ivan Grokhotkov. All rights reserved.
  This file is part of the esp8266 core for Arduino environment.

  This library is free software; you can redistribute it and/or
  modify it under the terms of the GNU Lesser General Public
  License as published by the Free Software Foundation; either
  version 2.1 of the License, or (at your option) any later version.

  This library is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
  Lesser General Public License for more details.

  You should have received a copy of the GNU Lesser General Public
  License along with this library; if not, write to the Free Software
  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
*/

#include "Arduino.h"
#include "EEPROM.h"

#ifdef __cplusplus
extern "C" {
#endif

#include "flash_api.h"

#ifdef __cplusplus
}
#endif

extern flash_t flash_obj;

EEPROMClass::EEPROMClass(uint32_t base_address)
: _base_address(base_address)
{
    pFlash = (void *)(&flash_obj);
}

EEPROMClass::EEPROMClass(void)
: _base_address(EEPROM_MEMORY_APP_BASE)
{
    pFlash = (void *)(&flash_obj);
}

void EEPROMClass::begin(size_t size) {
  if (size <= 0) {
    _rtl_printf("EEPROMClass::begin error, size <= 0\n");
    return;
  }
  if (size > FLASH_SECTOR_SIZE) {
    _rtl_printf("EEPROMClass::begin error, requested size %d > %d\n", size, FLASH_SECTOR_SIZE);
    size = FLASH_SECTOR_SIZE;
  }

  //In case begin() is called a 2nd+ time, don't reallocate if size is the same
  if(_data && (size != _size)) {
    delete[] _data;
    _data = new uint8_t[size];
  } else if(!_data) {
    _data = new uint8_t[size];
  }

  _size = size;

  if (!flash_stream_read((flash_t *)pFlash, _base_address, _size, _data)) {
    _rtl_printf("EEPROMClass::begin flash read failed\n");
  }

  _dirty = false; //make sure dirty is cleared in case begin() is called 2nd+ time
}

bool EEPROMClass::end() {
  bool retval;

  if(!_size) {
    return false;
  }

  retval = commit();
  if(_data) {
    delete[] _data;
  }
  _data = 0;
  _size = 0;
  _dirty = false;

  return retval;
}


uint8_t EEPROMClass::read(int const address) {
  if (address < 0 || (size_t)address >= _size) {
    _rtl_printf("EEPROMClass::read error, address %d > %d or %d < 0\n", address, _size, address);
    return 0;
  }
  if (!_data) {
    _rtl_printf("EEPROMClass::read without ::begin\n");
    return 0;
  }

  return _data[address];
}

void EEPROMClass::write(int const address, uint8_t const value) {
  if (address < 0 || (size_t)address >= _size) {
    _rtl_printf("EEPROMClass::write error, address %d > %d or %d < 0\n", address, _size, address);
    return;
  }
  if(!_data) {
    _rtl_printf("EEPROMClass::write without ::begin\n");
    return;
  }

  // Optimise _dirty. Only flagged if data written is different.
  uint8_t* pData = &_data[address];
  if (*pData != value)
  {
    *pData = value;
    _dirty = true;
  }
}

void EEPROMClass::update(int    const address, uint8_t const value) {
  write(address, value);
}

bool EEPROMClass::commit() {
  if (!_size)
    return false;
  if(!_dirty)
    return true;
  if(!_data)
    return false;

  flash_erase_sector((flash_t *)pFlash, _base_address);
    if (flash_stream_write((flash_t *)pFlash, _base_address, _size, _data)) {
      _dirty = false;
      return true;
    }

  _rtl_printf("EEPROMClass::commit failed\n");
  return false;
}

uint8_t * EEPROMClass::getDataPtr() {
  _dirty = true;
  return &_data[0];
}

uint8_t const * EEPROMClass::getConstDataPtr() const {
  return &_data[0];
}

EEPROMClass EEPROM;
