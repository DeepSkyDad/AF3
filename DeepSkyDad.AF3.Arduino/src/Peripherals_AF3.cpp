#include <Arduino.h>
#include "Peripherals_AF3.h"

void Peripherals_AF3::init(EEPROM_AF3 &eeprom, Motor_AF3 &motor)
{
	_eeprom = &eeprom;
	_motor = &motor;

	_sensors.begin();
}

int Peripherals_AF3::_readHcPin() {
    //smooth out annomalies
    int x=0, i=0, xTmp;
    while (++i < 5) {
        xTmp = analogRead(HC_PIN);
        //Serial.println(xTmp);
        if(xTmp<HC_CONNECTED_MIN)
            continue;
        x+=xTmp;
        delayMicroseconds(5);
    }

    return x / 4;
}

int Peripherals_AF3::_readHcButton()
{
    int x;
    if (_hcConnected)
    {
        x = _readHcPin();
            
        //Serial.println(x);
        if (x < HC_BTNUP_MAX && x > HC_BTNUP_MIN)
        { // + button pressed
            return HC_BTN_UP;
        }
        else if (x < HC_BTNDOWN_MAX && x > HC_BTNDOWN_MIN)
        { // - button pressed
            return HC_BTN_DOWN;
        }
        else if (x < HC_BTNBOTH_MAX && x > HC_BTNBOTH_MIN)
        { // - button pressed
            return HC_BTN_BOTH;
        }
        else if (x > HC_CONNECTED_MAX || x < HC_CONNECTED_MIN)
        { // HC disconnected
            //Serial.println("HC disconnected!");
            _hcConnected = false;
            if(_motor->getMotorManualIsMovingContinuous())
                _motor->stopMotor();
            return -1;
        }
        else
        { // all buttons released
            return 0;
        }
    }
    return -1;
}

void Peripherals_AF3::autoDiscovery()
{
    static unsigned long lastHCAutoDiscovery = 0;
    static unsigned long lastTSAutoDiscovery = 0;
    int x;

    long ms = millis();

    if (ms - lastHCAutoDiscovery > 300 || lastHCAutoDiscovery == 0)
    {
        lastHCAutoDiscovery = ms;

        // if currently no HC connected, first check if HC is connected here
        if (!_hcConnected)
        {
            x = _readHcPin();
            //Serial.println(x);
            if (x < HC_CONNECTED_MAX && x > HC_CONNECTED_MIN)
            {
                _hcConnected = true;
                //Serial.println("HC detected!");
            }
        }
    }

    if (ms - lastTSAutoDiscovery > 5000 || lastTSAutoDiscovery == 0)
    {
        lastTSAutoDiscovery = ms;

        // we might have found HC, so recheck - if not found, continue with tempC sensor discovery if we don't have it already
        if (!_tsConnected)
        {
            _sensors.requestTemperatures();
            float tempC = _sensors.getTempCByIndex(0);
            if (tempC != -127)
            {
                //Serial.print("TS Detected: ");
                //Serial.println(tempC);
                _temperatureCelsius = tempC;
                _tsConnected = true;
            }
        }
    }
}

void Peripherals_AF3::handleHC()
{
    static unsigned long btnUpPressed = 0;
    static unsigned long btnDownPressed = 0;
    static unsigned long btnBothPressed = 0;
    static unsigned long lastRun = 0;
    static long stepChange = 0;

    //Serial.println("Handle HC");
    if (_hcConnected && !(_motor->isMoving() && !_motor->getMotorManualIsMovingContinuous()))
    {
        if (millis() - lastRun < 20) // don't check every loop iteration - debouncing
            return;

        lastRun = millis();

        int btn_val = _readHcButton();
        if(stepChange == 0) {
            if (btn_val <= 0)
            {
                if (_motor->getMotorManualIsMovingContinuous())
                {
                    _motor->stopMotor();
                }
                else if (btnUpPressed != 0 && lastRun - btnUpPressed <= HC_CONTINUOUS_MOVE_TIMEOUT_MS)
                {
                    _motor->setMoveManual(true, false, false);
                    _eeprom->setTargetPosition(_eeprom->getPosition() + 100);
                    _motor->applyStepModeManual();
                    _motor->startMotor();
                }
                else if (btnDownPressed != 0 && lastRun - btnDownPressed <= HC_CONTINUOUS_MOVE_TIMEOUT_MS)
                {
                    _motor->setMoveManual(true, false, false);
                    _eeprom->setTargetPosition(_eeprom->getPosition() - 100);
                    _motor->applyStepModeManual();
                    _motor->startMotor();
                }

                stepChange = false;
                btnUpPressed = 0;
                btnDownPressed = 0;
                btnBothPressed = 0;
            }
            else if (btn_val == HC_BTN_UP)
            {
                if (_motor->getMotorManualIsMoving() && !_motor->getMotorManualIsMovingContinuous())
                {
                    _motor->stopMotor();
                }
                else if (btnUpPressed == 0)
                {
                    _motor->stopMotor();
                    btnUpPressed = millis();
                    btnDownPressed = 0;
                    btnBothPressed = 0;
                }
                else if (!_motor->getMotorManualIsMovingContinuous() && (lastRun - btnUpPressed) > HC_CONTINUOUS_MOVE_TIMEOUT_MS)
                {
                    _motor->setMoveManual(true, true, true);
                    _motor->applyStepModeManual();
                    _motor->startMotor();
                }
            }
            else if (btn_val == HC_BTN_DOWN)
            {
                if (_motor->getMotorManualIsMoving() && !_motor->getMotorManualIsMovingContinuous())
                {
                    _motor->stopMotor();
                }
                else if (btnDownPressed == 0)
                {
                    _motor->stopMotor();
                    btnUpPressed = 0;
                    btnDownPressed = millis();
                    btnBothPressed = 0;
                }
                else if (!_motor->getMotorManualIsMovingContinuous() && (lastRun - btnDownPressed) > HC_CONTINUOUS_MOVE_TIMEOUT_MS)
                {
                    _motor->setMoveManual(true, true, false);
                    _motor->applyStepModeManual();
                    _motor->startMotor();
                }
            }
            else if (btn_val == HC_BTN_BOTH)
            {
                _motor->stopMotor();
                btnUpPressed = 0;
                btnDownPressed = 0;
                btnBothPressed = millis();
                stepChange = 1;
            }
        } else if(stepChange == 1 && btn_val <= 0) {
            unsigned short newSm = _eeprom->getStepModeManual();
            if (lastRun - btnBothPressed > HC_STEP_CHANGE_RESET_TIMEOUT_MS) {
                newSm=1;
				_eeprom->setStepModeManual(newSm);
                _motor->applyStepModeManual();
            } else if(newSm <= 256) {
                newSm*=2;
                _eeprom->setStepModeManual(newSm);
                _motor->applyStepModeManual();
            }
            stepChange = 0;
        }
    }
}

void Peripherals_AF3::handleTS()
{
    static unsigned long lastRun = 0;
    static float tempC;
    static float tempF;

    if (_tsConnected)
    {
        if (millis() - lastRun < 5000)
            return;

        lastRun = millis();
        _sensors.requestTemperatures();
        tempC = _sensors.getTempCByIndex(0);
        tempF = _sensors.getTempFByIndex(0);
        //Serial.print("TEMP PIN 1: ");
        //Serial.println(tempC);

        if (tempC != -127 && tempC < 60 && tempC > -60)
        {
            _temperatureCelsius = tempC;
            _temperatureFahrenheit = tempF;
        }
        else
        {
            _tsConnected = false;
            _temperatureCelsius = -127;
            _temperatureFahrenheit = -127;
        }
    }
}

float Peripherals_AF3::getTempCBy() 
{
    _sensors.requestTemperatures();
    return _sensors.getTempCByIndex(0);
}

int Peripherals_AF3::readHcPin() {
	return _readHcPin();
}

int Peripherals_AF3::readHcButton() {
	return _readHcButton();
}

void Peripherals_AF3::debug() {
	Serial.print("HC/TP pin voltage: ");
	Serial.println(_readHcPin());
	Serial.print("Temperature: ");
	Serial.println(_temperatureCelsius);
	Serial.print("HC connected: ");
	Serial.println(_hcConnected);
}