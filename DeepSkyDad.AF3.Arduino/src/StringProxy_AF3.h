/*
        Deep Sky Dad AF3 - COMMAND SET

                Each command starts with [ and ends with ]
                Sample of parameterless command: [MOVE]
                Sample of command with parameters: [STRG5000]
                Each response starts with ( and ends with )
                If command results in error, response starts with ! and ends with ), containing error code. List of error codes:
                100 - command not found
                101 - relative movement bigger from max. movement
                102 - invalid step mode
                103 - invalid speed mode
                999 - UART not initalized (check motor power)
                The actual set of required commands is based on ASCOM IFocuserV3 interface, for more check:
                https://ascom-standards.org/Help/Platform/html/T_ASCOM_DeviceInterface_IFocuserV3.htm

*/
#include "EEPROM_AF3.h"
#include "Motor_AF3.h"
#include "Peripherals_AF3.h"
#include "Test_AF3.h"
#include "General_AF3.h"

#pragma once

#define RESPONSE_OK "(OK)"
#define RESPONSE_CMD_NOT_FOUND "!100)"
#define RESPONSE_CMD_MOVE_TOO_BIG "!101)"
#define RESPONSE_CMD_INVALID_STEP_MODE "!102)"
#define RESPONSE_CMD_INVALID_SPEED_MODE "!103)"
#define RESPONSE_CMD_UART_NOT_INITIALIZED "!999)"

class StringProxy_AF3
{
	private:
                EEPROM_AF3* _eeprom;
                Motor_AF3* _motor;
                Peripherals_AF3* _peri;
                Test_AF3* _test;
                char _resultBuffer1[50];
                char _resultBuffer2[50];
                char* _uintToChar(unsigned int value);
                char* _floatToChar(float value);
                bool _commandEndsWith(char c, char commandParam[], int commandParamLength);
                char const* _formatResponse(float value);
                char const* _formatResponse(unsigned long value);
                char const* _formatResponse(unsigned short value);
	public:
		void init(EEPROM_AF3 &eeprom, Motor_AF3 &motor, Peripherals_AF3 &peri, Test_AF3 &test);
                char const* processCommand(char *command, char *commandParam, int commandParamLength);
};