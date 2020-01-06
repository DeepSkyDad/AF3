#include "Test_AF3.h"

#define ASK_YN if (!askYN()) fail++;
#define P(a) Serial.print(a);
#define PLN(a) Serial.println(a);
#define CHECK_PORT(a,b,c,d,e) if(!checkPort(a,b,c,d,e)) fail++;
#define CHECK_TEMP(a,b,c) if(!checkTemp(a,b,c)) fail++;

#define HC_MAX_DEV 10 // allow 10% deviation in HC readings

bool askYN();
bool checkPort(Peripherals_AF3 &_peri, int min, int max, int expected, float max_dev_percent);
bool checkTemp(Peripherals_AF3 &_peri, float min, float max);

int Test_AF3::execute(EEPROM_AF3 &_eeprom, Motor_AF3 &_motor, Peripherals_AF3 &_peri) {
    unsigned int motorPos;
    int fail = 0;
    
    // basic info
    PLN();
    P("FW version: ");
    P(FW_VERSION);
    
    // reset to defaults
    P("Reseting settings to defaults. Proceed?");
    if (!askYN()) {
        PLN("Test SKIPPED!");
        return 0;
    }
    
    _eeprom.resetToDefaults();
    _motor.applyStepMode();
    _motor.applyMotorCurrent();
    
    // print eeprom settings
    _eeprom.debug();

    // motor testing
    PLN("--- MOTOR TEST");
    motorPos = _eeprom.getPosition();
    P("Moving motor CW... ");
    _eeprom.setTargetPosition(motorPos-400);
    _motor.startMotor();
    delay(500);
    while (_motor.isMoving())
        delay(100);
    PLN("DONE");

    P("Moving motor CCW... ");
    _eeprom.setTargetPosition(motorPos);
    _motor.startMotor();
    delay(500);
    while (_motor.isMoving())
        delay(100);
    PLN("DONE");
    P("Did motor return to original position?"); ASK_YN;
    
    // aux port testing
    PLN("--- AUX PORT TEST");
    P("Checking: "); CHECK_PORT(_peri, 0, 1023, 1023, 0.1);
    PLN("- HC");
    P("Connect HC to PORT1: "); CHECK_PORT(_peri, HC_CONNECTED_MIN, HC_CONNECTED_MAX, HC_CONNECTED_VAL, HC_MAX_DEV);
    P("Press and hold + on HC on PORT1: "); CHECK_PORT(_peri, HC_BTNUP_MIN, HC_BTNUP_MAX, HC_BTNUP_VAL, HC_MAX_DEV);
    P("Press and hold - on HC on PORT1: "); CHECK_PORT(_peri, HC_BTNDOWN_MIN, HC_BTNDOWN_MAX, HC_BTNDOWN_VAL, HC_MAX_DEV);
    P("Press and hold BOTH BUTTONS on HC on PORT1: "); CHECK_PORT(_peri, HC_BTNBOTH_MIN, HC_BTNBOTH_MAX, HC_BTNBOTH_VAL, HC_MAX_DEV);
   
    PLN("TP");
    P("Connect TP: ");CHECK_TEMP(_peri, 15.0, 35.0);

    // reset setting again before ending
    P("Reseting settings to defaults... ");
    _eeprom.resetToDefaults();
    _motor.applyStepMode();
    _motor.applyMotorCurrent();
    PLN("DONE");
    
    if (fail > 0) {
        P("Test FAILED, with ");P(fail);PLN(" failures!");
        return 0;
    }

    PLN("Test SUCESSFULY COMPLETED!");
    return 1;
}

bool askYN() {
    P(" [y/n] ");

    while (Serial.available() == 0)
        delay(1);
    
    char res = Serial.read();

    if (res == 'y') {
        PLN("- OK");
        return true;
    }

    PLN("- FAIL");
    return false;
}

bool checkPort(Peripherals_AF3 &_peri, int min, int max, int expected, float max_dev_percent) {
    int val = -1;

    for (int i = 0; i < 10; i++) {
        val = _peri.readHcPin();
        if (val >= min && val <= max)
            break;
        delay(1000);
    }

    P("min: ");P(min);P(", max: ");P(max);P(", expected: ");P(expected);P(", max dev: ");P(max_dev_percent);P("% - ");

    if (!(val >= min && val <= max))
    {
        P("didn't get correct reading in 10 seconds, last value: "); P(val); PLN(" - FAIL");
        return false;
    }

    float dev_percent = ((val-expected)/(float)(expected))*100;

    P("val: ");P(val);P(", dev: ");P(dev_percent);P("% - ");

    if (dev_percent >= max_dev_percent) {
        PLN("FAIL");
        return false;
    }

    PLN("OK");
    return true;
}

bool checkTemp(Peripherals_AF3 &_peri, int pin, float min, float max) {
    float val = -127.0;

    for (int i = 0; i < 10; i++) {
        val = _peri.getTempCBy();
        if (val >= min && val <= max)
            break;
        delay(1000);
    }

    P("min: ");P(min);P(", max: ");P(max);P(" - ");

    if (!(val >= min && val <= max))
    {
        P("didn't get correct reading in 10 seconds, last value: "); P(val); PLN(" - FAIL");
        return false;
    }

    P("val: ");P(val);PLN("OK");
    return true;
}