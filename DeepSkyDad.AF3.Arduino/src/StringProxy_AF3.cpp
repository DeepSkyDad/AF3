#include "StringProxy_AF3.h"

char const* StringProxy_AF3::_formatResponse(float value)
{
    dtostrf(value, 6, 2, _resultBuffer1);
    sprintf(_resultBuffer2,"(%s)", _resultBuffer1);
    return _resultBuffer2;
}

char const* StringProxy_AF3::_formatResponse(unsigned long value)
{
    sprintf(_resultBuffer1, "(%lu)", value);
    return _resultBuffer1;
}

char const* StringProxy_AF3::_formatResponse(unsigned short value)
{
    sprintf(_resultBuffer1, "(%hu)", value);
    return _resultBuffer1;
}

void StringProxy_AF3::init(EEPROM_AF3 &eeprom, Motor_AF3 &motor, Peripherals_AF3 &peri, Test_AF3 &test)
{
    _eeprom = &eeprom;
    _motor = &motor;
    _peri = &peri;
    _test = &test;
}

char const *StringProxy_AF3::processCommand(char *command, char *commandParam, int commandParamLength)
{
    if (strcmp("LEGT", command) == 0)
    {
        _motor->legacyTest();
        return RESPONSE_OK;
    }

    if(!_motor->isUartInitialized()) {
        return RESPONSE_CMD_UART_NOT_INITIALIZED;
    }

    if (strcmp("GFRM", command) == 0)
    {
        return "(Board=" FW_NAME ", Version=" FW_VERSION ")";
    }
    else if (strcmp("GPOS", command) == 0)
    {
        return _formatResponse(_eeprom->getPosition());
    }
    else if (strcmp("GTRG", command) == 0)
    {
        return _formatResponse(_eeprom->getTargetPosition());
    }
    else if (strcmp("STRG", command) == 0)
    {
        unsigned long pos = strtoul(commandParam, NULL, 10);
        if(!_eeprom->setTargetPosition(pos)) {
            return RESPONSE_CMD_MOVE_TOO_BIG;
        } else {
            return RESPONSE_OK;
        }
    }
    else if (strcmp("GMOV", command) == 0)
    {
        return _formatResponse((unsigned short)(_motor->isMovingWithSettle() ? 1 : 0));
    }
    else if (strcmp("SMOV", command) == 0)
    {
        _motor->applyStepMode();
        _motor->startMotor();
        return RESPONSE_OK;
    }
    else if (strcmp("STOP", command) == 0)
    {
        _motor->stopMotor();
        return RESPONSE_OK;
    }
    else if (strcmp("GMXP", command) == 0)
    {
        return _formatResponse(_eeprom->getMaxPosition());
    }
    else if (strcmp("SMXP", command) == 0)
    {
        _eeprom->setMaxPosition(strtoul(commandParam, NULL, 10));
        return RESPONSE_OK;
    }
    else if (strcmp("GMXM", command) == 0)
    {
        return _formatResponse(_eeprom->getMaxMovement());
    }
    else if (strcmp("SMXM", command) == 0)
    {
        _eeprom->setMaxMovement(strtoul(commandParam, NULL, 10));
        return RESPONSE_OK;
    }
        else if (strcmp("GMST", command) == 0)
    {
        return _formatResponse(_eeprom->getStepModeManual());
    }
    else if (strcmp("GSTP", command) == 0)
    {
        return _formatResponse(_eeprom->getStepMode());
    }
    else if (strcmp("SSTP", command) == 0)
    {
        long sm = (unsigned short) strtoul(commandParam, NULL, 10);
        if(_eeprom->setStepMode(sm)) {
                 return RESPONSE_OK;
        } else {
            return RESPONSE_CMD_INVALID_STEP_MODE; 
        }
    }
    else if (strcmp("GSPD", command) == 0)
    {
        return _formatResponse((unsigned short)_eeprom->getSpeedMode());
    }
    else if (strcmp("SSPD", command) == 0)
    {
        if(_eeprom->setSpeedMode((unsigned char) strtoul(commandParam, NULL, 10))) {
            return RESPONSE_OK;
        } else {
            return RESPONSE_CMD_INVALID_SPEED_MODE;
        }
    }
    else if (strcmp("RSET", command) == 0)
    {
        _eeprom->resetToDefaults();
        return RESPONSE_OK;
    }
    else if (strcmp("RBOT", command) == 0)
    {
        asm volatile("  jmp 0");
    }
    else if (strcmp("GBUF", command) == 0)
    {
        return _formatResponse(_eeprom->getSettleBufferMs());
    }
    else if (strcmp("SBUF", command) == 0)
    {
        _eeprom->setSettleBufferMs(strtoul(commandParam, NULL, 10));
        return RESPONSE_OK;
    }
    else if (strcmp("WEPR", command) == 0)
    {
        return RESPONSE_OK;
    }
    else if (strcmp("GREV", command) == 0)
    {
        return _formatResponse((unsigned short)(_eeprom->getReverseDirection() ? 1 : 0));
    }
    else if (strcmp("SREV", command) == 0)
    {
        _eeprom->setReverseDirection(atoi(commandParam));
        return RESPONSE_OK;
    }
    else if (strcmp("SPOS", command) == 0)
    {
        _eeprom->syncPosition(strtoul(commandParam, NULL, 10));
        return RESPONSE_OK;
    }
    else if (strcmp("GIDE", command) == 0)
    {
        return _formatResponse(_eeprom->getIdleEepromWriteMs());
    }
    else if (strcmp("SIDE", command) == 0)
    {
        _eeprom->setIdleEepromWriteMs(strtoul(commandParam, NULL, 10));
        return RESPONSE_OK;
    }
    else if (strcmp("GMMM", command) == 0)
    {
        return _formatResponse((unsigned short)_eeprom->getMotorIMoveMultiplier());
    }
     else if (strcmp("SMMM", command) == 0)
    {
        _eeprom->setMotorIMoveMultiplier((unsigned char)strtoul(commandParam, NULL, 10));
        _motor->applyMotorCurrent();
        return RESPONSE_OK;
    }
    else if (strcmp("GMHM", command) == 0)
    {
        return _formatResponse((unsigned short)_eeprom->getMotorIHoldMultiplier());
    }
     else if (strcmp("SMHM", command) == 0)
    {
        _eeprom->setMotorIHoldMultiplier((unsigned char)strtoul(commandParam, NULL, 10));
        _motor->applyMotorCurrent();
        return RESPONSE_OK;
    }
    else if (strcmp("GTMC", command) == 0)
    {
        return _formatResponse(_peri->getTempCBy());
    }
    else if (strcmp("DEBG", command) == 0)
    {
        Serial.println();
        Serial.println("---- EEPROM ----");
        _eeprom->debug();
        Serial.println("---- MOTOR ----");
        _motor->debug();
        Serial.println("---- PERIP ----");
        _peri->debug();
        Serial.println();
    }

    return RESPONSE_CMD_NOT_FOUND;
}