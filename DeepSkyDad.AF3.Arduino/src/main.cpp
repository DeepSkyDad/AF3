/*
    Deep Sky Dad AF3 firmware

        version 1.0.0 - 13.09.2019: clone of AF1
        version 1.0.1 - 04.01.2020: refactoring, nano-every compatibility, EEPROM optimizations
*/

#include <Arduino.h>
#include "EEPROM_AF3.h"
#include "Motor_AF3.h"
#include "Peripherals_AF3.h"
#include "StringProxy_AF3.h"
#include "Serial_AF3.h"
#include "Test_AF3.h"
#include "EEPROM.h"

EEPROM_AF3 _eeprom;
Motor_AF3 _motor;
Peripherals_AF3 _peri;
StringProxy_AF3 _stringProxy;
Serial_AF3 _serial;
Test_AF3 _test;

void setup()
{
    while (!Serial) {
        ; // wait for serial port to connect. Needed for native USB port only
        //NOTE: https://github.com/arduino/ArduinoCore-megaavr/issues/51
    }
    Serial.begin(115200);
    _eeprom.init();
    _motor.init(_eeprom, _peri);
    _peri.init(_eeprom, _motor);
    _stringProxy.init(_eeprom, _motor, _peri, _test);
    _serial.init(_stringProxy);
}

void loop()
{ 
    if(!_motor.isUartInitialized()) {
        if(!_motor.init(_eeprom, _peri)) {
            _serial.serialEvent();
            delay(500);
            return;
        } 
    }

    _motor.handleMotor();
    _serial.serialEvent();

    if(_motor.isMoving())
        return;

    _eeprom.handleEeprom();
    _peri.autoDiscovery();
    _peri.handleHC();
    _peri.handleTS();
}
