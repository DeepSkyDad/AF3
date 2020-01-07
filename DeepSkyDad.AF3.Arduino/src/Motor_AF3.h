#include <TMCStepper.h>
#include "EEPROM_AF3.h"

#pragma once
#define TMC220X_PIN_ENABLE 9
#define TMC220X_PIN_DIR 2
#define TMC220X_PIN_STEP 3
#define TMC220X_PIN_MS2 7
#define TMC220X_PIN_MS1 8
#define TMC220X_PIN_UART_RX 11
#define TMC220X_PIN_UART_TX 12

#define MOTOR_SELECT_PIN_D5 5
#define MOTOR_SELECT_PIN_D6 6

#define MOTOR_I_14HS10_0404S_04A 240
#define MOTOR_I_14HS17_0504S_05A 400

class Peripherals_AF3; //forwared declaration
class Motor_AF3
{
    private:
                EEPROM_AF3* _eeprom;
                Peripherals_AF3* _peri;
                bool _pinsInitialized = false;
                bool _uartInitialized = false;
                long _motorI;
                bool _motorIsMoving;
                bool _motorManualIsMoving;
                bool _motorManualIsMovingContinuous;
                bool _motorManualIsMovingContinuousDir;
                unsigned long _settleBufferPrevMs;
                unsigned long _debouncingLastRunMs = 0L;
                unsigned long _lastMoveFinishedMs = 0L;
                long _motorMoveDelay;
                TMC2208Stepper _driver = TMC2208Stepper(TMC220X_PIN_UART_RX, TMC220X_PIN_UART_TX, 0.11, true);
                void _startMotor();
                void _stopMotor();
                void _applyStepMode();
                void _applyStepModeManual();
                void _applyMotorCurrent();
    public:
        bool init(EEPROM_AF3 &eeprom, Peripherals_AF3 &peri);
                bool isUartInitialized();
                bool handleMotor();
                void setMoveManual(bool motorManualIsMoving, bool motorManualIsMovingContinuous, bool motorManualIsMovingContinuousDir);
                bool getMotorManualIsMoving();
                bool getMotorManualIsMovingContinuous();
                bool getMotorManualIsMovingContinuousDir();
                void startMotor();
                void stopMotor();
                void applyStepMode();
                void applyStepModeManual();
                void applyMotorCurrent();
                long getLastMoveFinishedMs();
                bool isMoving();
                bool isMovingWithSettle();
                void debug();
                void legacyTest();
};