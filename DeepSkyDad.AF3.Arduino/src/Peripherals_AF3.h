#include <OneWire.h>
#include <DallasTemperature.h>
#include "Motor_AF3.h"

#pragma once
#define HC_PIN A0

// BTNUP
#define HC_BTNUP_VAL 455
#define HC_BTNUP_MIN 405
#define HC_BTNUP_MAX 495
// BTNDOWN
#define HC_BTNDOWN_VAL 575
#define HC_BTNDOWN_MIN 525
#define HC_BTNDOWN_MAX 625
// BTNBOTH
#define HC_BTNBOTH_VAL 375
#define HC_BTNBOTH_MIN 325
#define HC_BTNBOTH_MAX 425
// CONNECTED
#define HC_CONNECTED_VAL 780
#define HC_CONNECTED_MIN 325
#define HC_CONNECTED_MAX 850

#define HC_BTN_UP 1
#define HC_BTN_DOWN 2
#define HC_BTN_BOTH 3

#define HC_CONTINUOUS_MOVE_TIMEOUT_MS 500
#define HC_STEP_CHANGE_RESET_TIMEOUT_MS 1000

class Peripherals_AF3
{
    private:
        EEPROM_AF3* _eeprom;
        Motor_AF3* _motor;
        OneWire _ds = OneWire(HC_PIN);
        DallasTemperature _sensors = DallasTemperature(&_ds);
        bool _hcConnected;
        bool _tsConnected;
        float _temperatureCelsius = -127;
        float _temperatureFahrenheit = -127;
        int _readHcPin();
        int _readHcButton();
        unsigned long lastRun = 0;
        float tempC;
        float tempF;
    public:
        void init(EEPROM_AF3 &eeprom, Motor_AF3 &motor);
        void autoDiscovery();
        void handleHC();
        void handleTS();
        float getTempCBy();
        int readHcPin();
        int readHcButton();
        void debug();
};