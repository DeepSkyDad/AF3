#include <Arduino.h>
#include "General_AF3.h"
#include <EEPROM.h>
#include "EEPROM_AF3.h"

unsigned long EEPROM_AF3::_calculateChecksum(EEPROMState state)
{
    return state.position + state.maxPosition + state.maxMovement + (unsigned char)state.stepMode + (unsigned char)state.stepModeManual + (unsigned char)state.speedMode + state.settleBufferMs + state.idleEepromWriteMs + state.reverseDirection + state.motorIMoveMultiplier + state.motorIHoldMultiplier;
}

void EEPROM_AF3::_readEeprom()
{
    int address = 0;
    eeprom_read_block((void *)&_state.maxPosition, (const void *)address, sizeof(_state.maxPosition)); address+=sizeof(_state.maxPosition);
    eeprom_read_block((void *)&_state.maxMovement, (const void *)address, sizeof(_state.maxMovement)); address+=sizeof(_state.maxMovement);
    eeprom_read_block((void *)&_state.stepMode, (const void *)address, sizeof(_state.stepMode)); address+=sizeof(_state.stepMode);
    eeprom_read_block((void *)&_state.stepModeManual, (const void *)address, sizeof(_state.stepModeManual)); address+=sizeof(_state.stepModeManual);
    eeprom_read_block((void *)&_state.speedMode, (const void *)address, sizeof(_state.speedMode)); address+=sizeof(_state.speedMode);
    eeprom_read_block((void *)&_state.settleBufferMs, (const void *)address, sizeof(_state.settleBufferMs)); address+=sizeof(_state.settleBufferMs);
    eeprom_read_block((void *)&_state.idleEepromWriteMs, (const void *)address, sizeof(_state.idleEepromWriteMs)); address+=sizeof(_state.idleEepromWriteMs);
    eeprom_read_block((void *)&_state.reverseDirection, (const void *)address, sizeof(_state.reverseDirection)); address+=sizeof(_state.reverseDirection);
    eeprom_read_block((void *)&_state.motorIMoveMultiplier, (const void *)address, sizeof(_state.motorIMoveMultiplier)); address+=sizeof(_state.motorIMoveMultiplier);
    eeprom_read_block((void *)&_state.motorIHoldMultiplier, (const void *)address, sizeof(_state.motorIHoldMultiplier)); address+=sizeof(_state.motorIHoldMultiplier);

    bool found = false;
    for (int i = 0; i < _slidingAddressCount; i++)
    {
        eeprom_read_block((void *)&_state.position, (const void *)address, sizeof(_state.position)); address+=sizeof(_state.position);
        eeprom_read_block((void *)&_state.targetPosition, (const void *)address, sizeof(_state.targetPosition)); address+=sizeof(_state.targetPosition);
        eeprom_read_block((void *)&_state.checksum, (const void *)address, sizeof(_state.checksum)); address+=sizeof(_state.checksum);

        if (_state.checksum == _calculateChecksum(_state))
        {
            _slidingCurrentAddress = address - _slidingSize;
            found = true;
            break;
        }
    }

    if (!found)
    {
        _resetEeprom();
    }
}

void EEPROM_AF3::_writeEeprom(bool isReset)
{

    _lastEepromCheckMs = millis();
    _isConfigDirty = false;
    _lastPositionChangeMs = 0L;
    _state.checksum = _calculateChecksum(_state);

    //write new state

    int address = 0;

    //write configuration
    eeprom_update_block((void *)&_state.maxPosition, (void *)address, sizeof(_state.maxPosition)); address+=sizeof(_state.maxPosition);
    eeprom_update_block((void *)&_state.maxMovement, (void *)address, sizeof(_state.maxMovement)); address+=sizeof(_state.maxMovement);
    eeprom_update_block((void *)&_state.stepMode, (void *)address, sizeof(_state.stepMode)); address+=sizeof(_state.stepMode);
    eeprom_update_block((void *)&_state.stepModeManual, (void *)address, sizeof(_state.stepModeManual)); address+=sizeof(_state.stepModeManual);
    eeprom_update_block((void *)&_state.speedMode, (void *)address, sizeof(_state.speedMode)); address+=sizeof(_state.speedMode);
    eeprom_update_block((void *)&_state.settleBufferMs, (void *)address, sizeof(_state.settleBufferMs)); address+=sizeof(_state.settleBufferMs);
    eeprom_update_block((void *)&_state.idleEepromWriteMs, (void *)address, sizeof(_state.idleEepromWriteMs)); address+=sizeof(_state.idleEepromWriteMs);
    eeprom_update_block((void *)&_state.reverseDirection, (void *)address, sizeof(_state.reverseDirection)); address+=sizeof(_state.reverseDirection);
    eeprom_update_block((void *)&_state.motorIMoveMultiplier, (void *)address, sizeof(_state.motorIMoveMultiplier)); address+=sizeof(_state.motorIMoveMultiplier);
    eeprom_update_block((void *)&_state.motorIHoldMultiplier, (void *)address, sizeof(_state.motorIHoldMultiplier)); address+=sizeof(_state.motorIHoldMultiplier);

    if (isReset)
    {
        _slidingCurrentAddress = _configurationSize;
    }
    else
    {
        unsigned long storedPosition, storedTargetPosition, storedChecksum;
        address = _slidingCurrentAddress;
        eeprom_read_block((void *)&storedPosition, (const void *)address, sizeof(storedPosition)); address+=sizeof(storedPosition);
        eeprom_read_block((void *)&storedTargetPosition, (const void *)address, sizeof(storedTargetPosition)); address+=sizeof(storedTargetPosition);
        eeprom_read_block((void *)&storedChecksum, (const void *)address, sizeof(storedChecksum)); address+=sizeof(storedChecksum);

        if (storedPosition != _state.position || storedTargetPosition != _state.targetPosition || storedChecksum != _state.checksum)
        {
            //invalidate previous sliding state checksum
            address = _slidingCurrentAddress + _slidingSize - sizeof(_state.checksum);
            unsigned long invalidChecksum = 0;
            eeprom_update_block((void *)&invalidChecksum, (void *)address, sizeof(invalidChecksum)); address+=sizeof(invalidChecksum);


            //slide address for position, target position, checksum
            _slidingCurrentAddress += _slidingSize;
            if (_slidingCurrentAddress + _slidingSize > EEPROM_SIZE)
            {
                _slidingCurrentAddress = _configurationSize;
            }
        }
    }

    address = _slidingCurrentAddress;
    eeprom_update_block((void *)&_state.position, (void *)address, sizeof(_state.position)); address+=sizeof(_state.position);
    eeprom_update_block((void *)&_state.targetPosition, (void *)address, sizeof(_state.targetPosition)); address+=sizeof(_state.targetPosition);
    eeprom_update_block((void *)&_state.checksum, (void *)address, sizeof(_state.checksum)); address+=sizeof(_state.checksum);
}

void EEPROM_AF3::_resetEeprom()
{
    _state = _stateDefaults;
    _writeEeprom(true);
}

void EEPROM_AF3::init()
{
    _readEeprom();
    _lastEepromCheckMs = millis();
}

void EEPROM_AF3::handleEeprom()
{
    if ((_lastEepromCheckMs + EEPROM_CHECK_PERIOD_MS) < millis())
    {
        if (_isConfigDirty)
        {
            _writeEeprom(false);
        }
        _lastEepromCheckMs = millis();
    }
    else if (_lastPositionChangeMs != 0L)
    {
        if ((_lastPositionChangeMs + _state.idleEepromWriteMs) < millis())
        {
            _writeEeprom(false);
        }
    }
}

void EEPROM_AF3::resetToDefaults()
{
    _resetEeprom();
}

void EEPROM_AF3::debug()
{
    Serial.print("current sliding address: ");
    Serial.println(_slidingCurrentAddress);
    Serial.print("sliding slots count: ");
    Serial.println(_slidingAddressCount);
    Serial.print("maxPosition: ");
    Serial.println(_state.maxPosition);
    Serial.print("maxMovement: ");
    Serial.println(_state.maxMovement);
    Serial.print("stepMode: ");
    Serial.println((unsigned char)_state.stepMode);
    Serial.print("stepModeManual: ");
    Serial.println((unsigned char)_state.stepModeManual);
    Serial.print("speedMode: ");
    Serial.println((unsigned char)_state.speedMode);
    Serial.print("settleBufferMs: ");
    Serial.println(_state.settleBufferMs);
    Serial.print("idleEepromWriteMs: ");
    Serial.println(_state.idleEepromWriteMs);
    Serial.print("reverseDirection: ");
    Serial.println(_state.reverseDirection);
    Serial.print("motorIMoveMultiplier: ");
    Serial.println(_state.motorIMoveMultiplier);
    Serial.print("motorIHoldMultiplier: ");
    Serial.println(_state.motorIHoldMultiplier);
    Serial.print("position: ");
    Serial.println(_state.position);
    Serial.print("targetPosition: ");
    Serial.println(_state.targetPosition);
    Serial.print("checksum: ");
    Serial.println(_state.checksum);
}

unsigned long EEPROM_AF3::getPosition()
{
    return _state.position;
}
void EEPROM_AF3::setPosition(unsigned long value)
{
    if (value > _state.maxPosition)
    {
        value = _state.maxPosition;
    }
    else if (value < 0)
    {
        value = 0;
    }

    _lastPositionChangeMs = millis();
    _state.position = value;
}
void EEPROM_AF3::syncPosition(unsigned long value)
{
    if (value > _state.maxPosition)
    {
        value = _state.maxPosition;
    }
    else if (value < 0)
    {
        value = 0;
    }

    _isConfigDirty = true;
    _state.targetPosition = value;
    _state.position = value;
}
unsigned long EEPROM_AF3::getTargetPosition()
{
    return _state.targetPosition;
}
bool EEPROM_AF3::setTargetPosition(unsigned long value)
{
    unsigned long diff = (_state.position > value) ? _state.position - value : value - _state.position;
    if (diff > _state.maxMovement)
    {
        return false;
    }

    if (value > _state.maxPosition)
    {
        value = _state.maxPosition;
    }

    _lastPositionChangeMs = millis();
    _state.targetPosition = value;
    return true;
}
unsigned long EEPROM_AF3::getMaxPosition()
{
    return _state.maxPosition;
}
void EEPROM_AF3::setMaxPosition(unsigned long value)
{
    if (value < 10000)
        value = 10000;

    if (_state.targetPosition > value)
        _state.targetPosition = value;

    if (_state.position > value)
        _state.position = value;

    _isConfigDirty = true;
    _state.maxPosition = value;
}
unsigned long EEPROM_AF3::getMaxMovement()
{
    return _state.maxMovement;
}
void EEPROM_AF3::setMaxMovement(unsigned long value)
{
    if (value < 1000)
        value = 1000;

    _isConfigDirty = true;
    _state.maxMovement = value;
}

unsigned short EEPROM_AF3::getStepMode()
{
    return (unsigned short)_state.stepMode;
}
bool EEPROM_AF3::setStepMode(unsigned short value)
{
    if (value != 1 && value != 2 && value != 4 && value != 8 && value != 16 && value != 32 && value != 64 && value != 128 && value != 256)
    {
        return false;
    }

    _isConfigDirty = true;
    _state.stepMode = value;
    return true;
}
unsigned short EEPROM_AF3::getStepModeManual()
{
    return (unsigned short)_state.stepModeManual;
}
bool EEPROM_AF3::setStepModeManual(unsigned short value)
{
    if (value != 1 && value != 2 && value != 4 && value != 8 && value != 16 && value != 32 && value != 64 && value != 128 && value != 256)
    {
        return false;
    }

    _isConfigDirty = true;
    _state.stepModeManual = value;
    return true;
}
unsigned char EEPROM_AF3::getSpeedMode()
{
    return (unsigned char)_state.speedMode;
}
bool EEPROM_AF3::setSpeedMode(unsigned char value)
{
    if (value != 1 && value != 2 && value != 3 && value != 4 && value != 5)
    {
        return false;
    }

    _isConfigDirty = true;
    _state.speedMode = value;
    return true;
}
unsigned long EEPROM_AF3::getSettleBufferMs()
{
    return _state.settleBufferMs;
}
void EEPROM_AF3::setSettleBufferMs(unsigned long value)
{
    _isConfigDirty = true;
    _state.settleBufferMs = value;
}
bool EEPROM_AF3::getReverseDirection()
{
    return _state.reverseDirection;
}
void EEPROM_AF3::setReverseDirection(bool value)
{
    _isConfigDirty = true;
    _state.reverseDirection = value;
}
unsigned long EEPROM_AF3::getIdleEepromWriteMs()
{
    return _state.idleEepromWriteMs;
}
void EEPROM_AF3::setIdleEepromWriteMs(unsigned long value)
{
    _isConfigDirty = true;
    _state.idleEepromWriteMs = value;
}
unsigned char EEPROM_AF3::getMotorIMoveMultiplier()
{
    return _state.motorIMoveMultiplier;
}
void EEPROM_AF3::setMotorIMoveMultiplier(unsigned char value)
{
    if (value < 1)
    {
        value = 1;
    }
    else if (value > 100)
    {
        value = 100;
    }

    _isConfigDirty = true;
    _state.motorIMoveMultiplier = value;
}
unsigned char EEPROM_AF3::getMotorIHoldMultiplier()
{
    return _state.motorIHoldMultiplier;
}
void EEPROM_AF3::setMotorIHoldMultiplier(unsigned char value)
{
    if (value < 1)
    {
        value = 1;
    }
    else if (value > 100)
    {
        value = 100;
    }

    _isConfigDirty = true;
    _state.motorIHoldMultiplier = value;
}