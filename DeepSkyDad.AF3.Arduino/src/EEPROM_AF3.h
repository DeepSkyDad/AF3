/*
    Deep Sky Dad AF3 - EEPROM IMPLEMENTATION
        
		1. configuration properties (all except for position and checksum) do not constantly change, so we will keep them at beginning of eeprom
        2. position and checksum are sliding through different addresses to prevent EEPROM wear
*/

#define EEPROM_SIZE 1024 //Nano
//#define EEPROM_SIZE 256 //Nano-Every
#define EEPROM_CHECK_PERIOD_MS 5000

#pragma once

//use strong enum ("enum class") instead of leagy, which has issues
enum class STEP_MODE : unsigned short
{
	STP1 = 1,
    STP2 = 2,
    STP4 = 4,
	STP8 = 8,
	STP16 = 16,
	STP32 = 32,
	STP64 = 64,
	STP128 = 128,
	STP256 = 256
};

//use strong enum ("enum class") instead of leagy, which has issues
enum class SPEED_MODE : unsigned char
{
	VERYSLOW = 1,
    SLOW = 2,
    MEDIUM = 3,
	FAST = 4,
	VERYFAST = 5
};

class EEPROMState
{
	public:
		unsigned long maxPosition;
		unsigned long maxMovement;
		STEP_MODE stepMode;
		STEP_MODE stepModeManual;
		SPEED_MODE speedMode;
		unsigned long settleBufferMs;
		unsigned long idleEepromWriteMs;
		bool reverseDirection;
		unsigned char motorIMoveMultiplier;
		unsigned char motorIHoldMultiplier;
		unsigned long position;
		unsigned long targetPosition;
		unsigned long checksum;
};

class EEPROM_AF3
{
	private:
		EEPROMState _state = {0, 0, STEP_MODE::STP1, STEP_MODE::STP1, SPEED_MODE::SLOW, 0, 0, 0, 0, 0, 0, 0, 9999};
		EEPROMState _stateDefaults = {1000000, 50000, STEP_MODE::STP2, STEP_MODE::STP2, SPEED_MODE::SLOW, 0, 180000, 0, 90, 40, 500000, 500000, 0};
		int _slidingSize = sizeof(_state.position) + sizeof(_state.targetPosition) + sizeof(_state.checksum);
		int _configurationSize = sizeof(EEPROMState) - _slidingSize;
		int _slidingAddressCount = EEPROM_SIZE - _configurationSize;
		int _slidingCurrentAddress = _configurationSize;
		bool _isConfigDirty;
		unsigned long _lastEepromCheckMs;
		unsigned long _lastPositionChangeMs = 0L;
		long _propertyWritesSinceBoot = 0L;

		template <typename T> bool _tryWriteProperty(int &address, T &property);
		template <typename T> void _readProperty(int &address, T &property);
		unsigned long _calculateChecksum(EEPROMState state);
		void _readEeprom();
		void _writeEeprom(bool isReset);
		void _resetEeprom();
	public:
		void init();
		void handleEeprom();
		void resetToDefaults();
		void debug();
		
		unsigned long getPosition();
		void setPosition(unsigned long value);
		void syncPosition(unsigned long value);
		unsigned long getTargetPosition();
		bool setTargetPosition(unsigned long value);
		unsigned long getMaxPosition();
		void setMaxPosition(unsigned long value);
		unsigned long getMaxMovement();
		void setMaxMovement(unsigned long value);
		unsigned short getStepMode();
		bool setStepMode(unsigned short value);
		unsigned short getStepModeManual();
		bool setStepModeManual(unsigned short value);
		unsigned char getSpeedMode();
		bool setSpeedMode(unsigned char value);
		unsigned long getSettleBufferMs();
		void setSettleBufferMs(unsigned long value); 
		bool getReverseDirection();
		void setReverseDirection(bool value);
		unsigned long getIdleEepromWriteMs();
		void setIdleEepromWriteMs(unsigned long value);
		unsigned char getMotorIMoveMultiplier();
		void setMotorIMoveMultiplier(unsigned char value);
		unsigned char getMotorIHoldMultiplier();
		void setMotorIHoldMultiplier(unsigned char value);
		unsigned long getChecksum();
		void setChecksum(unsigned long value);
};