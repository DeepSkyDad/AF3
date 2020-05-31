/*
    Deep Sky Dad AF3 firmware

        version 1.0.0 - 13.09.2019: clone of AF1
        version 1.0.1 - 04.01.2020: refactoring, nano-every compatibility, EEPROM optimizations
        version 1.0.2 - 11.02.2020: refactoring, nano-every compatibility & eeprom fix
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

int pm = 0;

void setup()
{
    pinMode(LED_BUILTIN, OUTPUT);
    while (!Serial) {
        digitalWrite(LED_BUILTIN, (pm++%2) == 0 ? HIGH : LOW);
        delay(10);
        // wait for serial port to connect. Needed for native USB port only
        //Nano Every NOTE: https://github.com/arduino/ArduinoCore-megaavr/issues/51
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
            digitalWrite(LED_BUILTIN, (pm++%2) == 0 ? HIGH : LOW);
            _serial.serialEvent();
            delay(500);
            return;
        } else {
            digitalWrite(LED_BUILTIN, LOW);
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
