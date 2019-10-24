/*
    Deep Sky Dad AF3 firmware
    
    EEPROM IMPLEMENTATION
        Autofocuser state is stored in EEPROM long[] array, using EEPROMEx library.
        Each index contains different property, with last one containing checksum (sum of all previous values, so we can validate its contents).
        Additionally, values are saved to a different address every time. Writing to same address every time would wear EEPROM out faster.
        Autofocuseer state:
        {<position>, <maxPosition>, <maxMovement>, <stepMode>, <speedMode>, <settleBufferMs>, <idleEepromWriteMs>, <reverseDirection>, <motorIMoveMultiplier>, <motorIHoldMultiplier>, <checksum>}

    COMMAND SET
        Commands are executed via serial COM port communication. 
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

        version 1.0.0 - 13.9.2019: clone of AF1
*/

#include <Arduino.h>
#include <EEPROMEx.h>
#include <OneWire.h>
#include <DallasTemperature.h>
#include <TMCStepper.h>

/* EEPROM */
//{<position>, <maxPosition>, <maxMovement>, <stepMode>, <stepModeManual>, <speedMode>, <settleBufferMs>, <idleEepromWriteMs>, <reverseDirection>, <motorIMoveMultiplier>, <motorIHoldMultiplier>, <checksum>}
#define EEPROM_AF_STATE_POSITION 0
#define EEPROM_AF_STATE_MAX_POSITION 1
#define EEPROM_AF_STATE_MAX_MOVEMENT 2
#define EEPROM_AF_STATE_STEP_MODE 3
#define EEPROM_AF_STATE_STEP_MODE_MANUAL 4
#define EEPROM_AF_STATE_SPEED_MODE 5
#define EEPROM_AF_STATE_SETTLE_BUFFER_MS 6
#define EEPROM_AF_STATE_IDLE_EEPROM_WRITE_MS 7
#define EEPROM_AF_STATE_REVERSE_DIRECTION 8
#define EEPROM_AF_STATE_MOTOR_I_MOVE_MULTIPLIER 9
#define EEPROM_AF_STATE_MOTOR_I_HOLD_MULTIPLIER 10
#define EEPROM_AF_STATE_CHECKSUM 11

long _eepromAfState[] = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 9999};
long _eepromAfPrevState[] = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 9999};
long _eepromAfStateDefault[] = {500000, 1000000, 50000, 2, 2, 2, 0, 180000, 0, 90, 40, 0};
int _eepromAfStatePropertyCount = sizeof(_eepromAfState) / sizeof(long);
int _eepromAfStateAddressSize = sizeof(_eepromAfState);
int _eepromAfStateAdressesCount = EEPROMSizeATmega328 / _eepromAfStateAddressSize;
int _eepromAfStateCurrentAddress;
bool _eepromSaveAfState;

/* MOTOR */
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

bool _motorIsMoving;
bool _motorManualIsMoving;
bool _motorManualIsMovingContinuous;
bool _motorManualIsMovingContinuousDir;
int _motorManualSteps = 100;
unsigned long _motorTargetPosition;
unsigned long _motorSettleBufferPrevMs;
unsigned long _motorIsMovingLastRunMs;
unsigned long _motorLastMoveEepromMs;
long _motorMoveDelay;
long _motorI;
long _motorIMoveMultiplier;
long _motorIHoldMultiplier;
bool _motorUARTInitialized = false;

TMC2208Stepper _driver = TMC2208Stepper(TMC220X_PIN_UART_RX, TMC220X_PIN_UART_TX, 0.11, true);

/* HAND CONTROLLER/TEMPERATURE SENSOR */
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

OneWire _ds = OneWire(HC_PIN);
DallasTemperature _sensors = DallasTemperature(&_ds);

bool _hcConnected;
bool _tsConnected;

float _temperatureCelsius = -127;
float _temperatureFahrenheit = -127;

/* SERIAL */
char _serialCommandRaw[70];
int _serialCommandRawIdx;
int _serialCommandRawLength;
char _command[5];
char _commandParam[65];
int _commandParamLength;

const char firmwareName[] = "DeepSkyDad.AF3";
const char firmwareVersion[] = "1.0.0";

/* EEPROM - functions */
bool eepromValidateChecksum()
{
    long checksum = 0;
    for (int i = 0; i < _eepromAfStatePropertyCount - 1; i++)
    {
        checksum += _eepromAfState[i];
    }

    return checksum == _eepromAfState[_eepromAfStatePropertyCount - 1];
}

void eepromGetAddress()
{
    _eepromAfStateCurrentAddress = EEPROM.getAddress(sizeof(_eepromAfState));
    if (_eepromAfStateCurrentAddress + sizeof(_eepromAfState) > EEPROMSizeATmega328)
    {
        EEPROM.setMemPool(0, EEPROMSizeATmega328);
        _eepromAfStateCurrentAddress = EEPROM.getAddress(sizeof(_eepromAfState));
    }
}

void eepromWrite(bool forceWrite)
{
    _eepromSaveAfState = false;
    _motorLastMoveEepromMs = 0L;

    //prevent unneccessary saves
    if (!forceWrite)
    {
        bool stateChanged = false;
        for (int i = 0; i < _eepromAfStatePropertyCount - 1; i++)
        {
            if (_eepromAfState[i] != _eepromAfPrevState[i])
            {
                stateChanged = true;
                break;
            }
        }

        if (!stateChanged)
        {
            return;
        }
    }

    //invalidate previous state
    if (eepromValidateChecksum())
    {
        _eepromAfState[EEPROM_AF_STATE_CHECKSUM] = _eepromAfState[EEPROM_AF_STATE_CHECKSUM] - 1;
    }
    EEPROM.writeBlock<long>(_eepromAfStateCurrentAddress, _eepromAfState, _eepromAfStatePropertyCount);

    //write new state
    long checksum = 0;
    for (int i = 0; i < _eepromAfStatePropertyCount - 1; i++)
    {
        _eepromAfPrevState[i] = _eepromAfState[i];
        checksum += _eepromAfState[i];
    }
    _eepromAfState[EEPROM_AF_STATE_CHECKSUM] = checksum;
    _eepromAfPrevState[EEPROM_AF_STATE_CHECKSUM] = checksum;

    //write to new address
    eepromGetAddress();

    EEPROM.writeBlock<long>(_eepromAfStateCurrentAddress, _eepromAfState, _eepromAfStatePropertyCount);

    //Serial.print("EEPROM WRITE");
}

void eepromResetState()
{
    for (int i = 0; i < _eepromAfStatePropertyCount; i++)
    {
        _eepromAfState[i] = _eepromAfStateDefault[i];
        _eepromAfPrevState[i] = _eepromAfStateDefault[i];
    }
    eepromWrite(true);
}

bool eepromRead()
{
    eepromGetAddress();
    EEPROM.readBlock<long>(_eepromAfStateCurrentAddress, _eepromAfState, _eepromAfStatePropertyCount);
    EEPROM.readBlock<long>(_eepromAfStateCurrentAddress, _eepromAfPrevState, _eepromAfStatePropertyCount);
    return eepromValidateChecksum();
}

bool setTargetPosition(long pos) {
    if (abs(_eepromAfState[EEPROM_AF_STATE_POSITION] - pos) > _eepromAfState[EEPROM_AF_STATE_MAX_MOVEMENT])
    {
        return false;
    }
    else if (pos < 0)
    {
        pos = 0;
    }
    else if (pos > _eepromAfState[EEPROM_AF_STATE_MAX_POSITION])
    {
        pos = _eepromAfState[EEPROM_AF_STATE_MAX_POSITION];
    }
    else
    {
        _motorTargetPosition = pos;
    }
    return true;
}

/* MOTOR FUNCTIONS */

void startMotor() {
    _motorIsMoving = true;
    _motorIsMovingLastRunMs = millis();

    switch (_eepromAfState[EEPROM_AF_STATE_SPEED_MODE])
    {
        case 1:
            _motorMoveDelay = 96000;
            break;
        case 2:
            _motorMoveDelay = 48000;
            break;
        case 3:
            _motorMoveDelay = 24000;
            break;
        case 4:
            _motorMoveDelay = 8000;
            break;
        case 5:
            _motorMoveDelay = 4000;
            break;
    }

    _motorMoveDelay /= _eepromAfState[EEPROM_AF_STATE_STEP_MODE];
}

void stopMotor()
{
    if (_motorIsMoving && _eepromAfState[EEPROM_AF_STATE_SETTLE_BUFFER_MS] > 0)
        _motorSettleBufferPrevMs = millis();
    _motorLastMoveEepromMs = millis();

    _motorManualIsMoving = false;
    _motorManualIsMovingContinuous = false;
    _motorIsMoving = false;
    _motorTargetPosition = _eepromAfState[EEPROM_AF_STATE_POSITION];
}

bool writeStepMode(int sm)
{
    if (sm != 1 && sm != 2 && sm != 4 && sm != 8 && sm != 16 && sm != 32 && sm != 64 && sm != 128 && sm != 256)
    {
        return false;
    }

    //select microsteps (0,2,4,8,16,32,64,128,256)
    _driver.microsteps(sm == 1 ? 0 : sm); 
    _eepromAfState[EEPROM_AF_STATE_STEP_MODE] = sm;
    return true;
}

bool writeManualStepMode(int sm)
{
    if (sm != 1 && sm != 2 && sm != 4 && sm != 8 && sm != 16 && sm != 32 && sm != 64 && sm != 128 && sm != 256)
    {
        return false;
    }

    //select microsteps (0,2,4,8,16,32,64,128,256)
    _driver.microsteps(sm == 1 ? 0 : sm); 
    _eepromAfState[EEPROM_AF_STATE_STEP_MODE_MANUAL] = sm;
    return true;
}

bool setSpeedMode(char param[])
{
    long speedMode = strtol(param, NULL, 10);
    if (speedMode != 1 && speedMode != 2 && speedMode != 3 && speedMode != 4 && speedMode != 5)
    {
        return false;
    }
    else
    {
        _eepromAfState[EEPROM_AF_STATE_SPEED_MODE] = speedMode;
        return true;
    }
}

void setReverseDir(char param[])
{
    long motorDir = strtol(param, NULL, 10);
    if (motorDir != 0 && motorDir != 1)
        return;

    _eepromAfState[EEPROM_AF_STATE_REVERSE_DIRECTION] = motorDir;
}

void setMaxPos(char param[])
{
    long maxPos = strtol(param, NULL, 10);
    if (maxPos < 10000)
        maxPos = 10000;

    if (_motorTargetPosition > (unsigned long)maxPos)
        _motorTargetPosition = maxPos;

    if (_eepromAfState[EEPROM_AF_STATE_POSITION] > maxPos)
        _eepromAfState[EEPROM_AF_STATE_POSITION] = maxPos;

    _eepromAfState[EEPROM_AF_STATE_MAX_POSITION] = maxPos;
}

void setMaxMovement(char param[])
{
    long maxMov = strtol(param, NULL, 10);
    if (maxMov < 1000)
        maxMov = 1000;

    _eepromAfState[EEPROM_AF_STATE_MAX_MOVEMENT] = maxMov;
}

void setSettleBuffer(char param[])
{
    long settleBufferMs = strtol(param, NULL, 10);
    if (settleBufferMs < 0)
    {
        settleBufferMs = 0;
    }

    _eepromAfState[EEPROM_AF_STATE_SETTLE_BUFFER_MS] = settleBufferMs;
}

void setIdleEepromWriteMs(char param[])
{
    long ms = strtol(param, NULL, 10);
    if (ms < 0)
    {
        ms = 0;
    }

    _eepromAfState[EEPROM_AF_STATE_IDLE_EEPROM_WRITE_MS] = ms;
}

void setMotorCurrent(int moveMultiplier, int holdMultiplier)
{
    if (moveMultiplier < 0)
    {
        moveMultiplier = 0;
    } else if(moveMultiplier > 100) {
        moveMultiplier = 100;
    }

    if (holdMultiplier < 0)
    {
        holdMultiplier = 0;
    } else if(holdMultiplier > 100) {
        holdMultiplier = 100;
    }

    _motorIMoveMultiplier = moveMultiplier;
    _motorIHoldMultiplier = holdMultiplier;
    
    int mA = _motorI*(((float)moveMultiplier)/100.0);
    float multiplier = ((float)holdMultiplier)/100.0;

    //Serial.print("Move current (mA): ");
    //Serial.print(mA);
    //Serial.print(", Hold multiplier: ");
    //Serial.println(multiplier);

    _driver.rms_current(mA, multiplier*0.8); // set hold multiplier to maximum of 80% of motor move current
    _eepromAfState[EEPROM_AF_STATE_MOTOR_I_MOVE_MULTIPLIER] = moveMultiplier;
    _eepromAfState[EEPROM_AF_STATE_MOTOR_I_HOLD_MULTIPLIER] = holdMultiplier;
}

bool initUART() {
    _driver.begin();

    if (_driver.test_connection() != 0) {
        return false;
    }

    _driver.pdn_disable(true); //enable UART
    setMotorCurrent((int)_motorIMoveMultiplier, (int)_motorIHoldMultiplier);
    _driver.mstep_reg_select(true); //enable microstep selection over UART
    _driver.I_scale_analog(false); //disable Vref scaling
    writeStepMode(_eepromAfState[EEPROM_AF_STATE_STEP_MODE]);
    _driver.blank_time(24); //Comparator blank time. This time needs to safely cover the switching event and the duration of the ringing on the sense resistor. Choose a setting of 24 or 32 for typical applications. For higher capacitive loads, 3 may be required. Lower settings allow stealthChop to regulate down to lower coil current values. 
    _driver.toff(5); //enable stepper driver (For operation with stealthChop, this parameter is not used, but >0 is required to enable the motor)
    _driver.intpol(true); //use interpolation
    _driver.TPOWERDOWN(255); //time until current reduction after the motor stops. Use maximum (5.6s)
    digitalWrite(TMC220X_PIN_ENABLE, LOW); //enable coils
       
    _motorUARTInitialized = true;
    return true;
}

/* HAND CONTROLLER / TEMPERATURE SENSOR */
int readHcPin() {
    //smooth out annomalies
    int x=0, i=0, xTmp;
    while (++i < 10) {
        xTmp = analogRead(HC_PIN);
        //Serial.println(xTmp);
        if(xTmp<HC_CONNECTED_MIN)
            continue;
        x+=xTmp;
        delayMicroseconds(5);
    }

    return x / 9;
}

int readHcButton()
{
    int x;
    if (_hcConnected)
    {
        x = readHcPin();
            
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
            if(_motorManualIsMovingContinuous)
                stopMotor();
            return -1;
        }
        else
        { // all buttons released
            return 0;
        }
    }
    return -1;
}

void autoDiscovery()
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
            x = readHcPin();
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

void handleHC()
{
    static unsigned long btnUpPressed = 0;
    static unsigned long btnDownPressed = 0;
    static unsigned long btnBothPressed = 0;
    static unsigned long lastRun = 0;
    static long stepChange = 0;

    //Serial.println("Handle HC");

    if (_hcConnected && !(_motorIsMoving && !_motorManualIsMovingContinuous))
    {
        if (millis() - lastRun < 20) // don't check every loop iteration - debouncing
            return;

        lastRun = millis();

        int btn_val = readHcButton();

        if(stepChange == 0) {
            if (btn_val <= 0)
            {
                if (_motorManualIsMovingContinuous)
                {
                    stopMotor();
                }
                else if (btnUpPressed != 0 && lastRun - btnUpPressed <= HC_CONTINUOUS_MOVE_TIMEOUT_MS)
                {
                    _motorManualIsMoving = true;
                    setTargetPosition(_eepromAfState[EEPROM_AF_STATE_POSITION] + _motorManualSteps);
                    writeManualStepMode(_eepromAfState[EEPROM_AF_STATE_STEP_MODE_MANUAL]);
                    startMotor();
                }
                else if (btnDownPressed != 0 && lastRun - btnDownPressed <= HC_CONTINUOUS_MOVE_TIMEOUT_MS)
                {
                    _motorManualIsMoving = true;
                    setTargetPosition(_eepromAfState[EEPROM_AF_STATE_POSITION] - _motorManualSteps);
                    writeManualStepMode(_eepromAfState[EEPROM_AF_STATE_STEP_MODE_MANUAL]);
                    startMotor();
                }

                stepChange = false;
                btnUpPressed = 0;
                btnDownPressed = 0;
                btnBothPressed = 0;
            }
            else if (btn_val == HC_BTN_UP)
            {
                if (_motorManualIsMoving && !_motorManualIsMovingContinuous)
                {
                    stopMotor();
                }
                else if (btnUpPressed == 0)
                {
                    stopMotor();
                    btnUpPressed = millis();
                    btnDownPressed = 0;
                    btnBothPressed = 0;
                }
                else if (!_motorManualIsMovingContinuous && (lastRun - btnUpPressed) > HC_CONTINUOUS_MOVE_TIMEOUT_MS)
                {
                    _motorManualIsMoving = true;
                    _motorManualIsMovingContinuousDir = true;
                    _motorManualIsMovingContinuous = true;
                    writeManualStepMode(_eepromAfState[EEPROM_AF_STATE_STEP_MODE_MANUAL]);
                    startMotor();
                }
            }
            else if (btn_val == HC_BTN_DOWN)
            {
                if (_motorManualIsMoving && !_motorManualIsMovingContinuous)
                {
                    stopMotor();
                }
                else if (btnDownPressed == 0)
                {
                    stopMotor();
                    btnUpPressed = 0;
                    btnDownPressed = millis();
                    btnBothPressed = 0;
                }
                else if (!_motorManualIsMovingContinuous && (lastRun - btnDownPressed) > HC_CONTINUOUS_MOVE_TIMEOUT_MS)
                {
                    _motorManualIsMoving = true;
                    _motorManualIsMovingContinuousDir = false;
                    _motorManualIsMovingContinuous = true;
                    writeManualStepMode(_eepromAfState[EEPROM_AF_STATE_STEP_MODE_MANUAL]);
                    startMotor();
                }
            }
            else if (btn_val == HC_BTN_BOTH)
            {
                stopMotor();
                btnUpPressed = 0;
                btnDownPressed = 0;
                btnBothPressed = millis();
                stepChange = 1;
            }
        } else if(stepChange == 1 && btn_val <= 0) {
            int newSm = _eepromAfState[EEPROM_AF_STATE_STEP_MODE_MANUAL];
            if (lastRun - btnBothPressed > HC_STEP_CHANGE_RESET_TIMEOUT_MS) {
                newSm=1;
                writeManualStepMode(newSm);
                _eepromSaveAfState = true;
                //Serial.println("MANUAL STEP MODE RESET");
            } else if(newSm < 256) {
                newSm*=2;
                writeManualStepMode(newSm);
                _eepromSaveAfState = true;
                //Serial.print("MANUAL STEP MODE ");
                //Serial.println(newSm);
            }
            stepChange = 0;
        }
    }
}

void handleTS()
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

float getTempCByPin() 
{
    _sensors.requestTemperatures();
    return _sensors.getTempCByIndex(0);
}


/* SERIAL - functions */

void printResponse(int response)
{
    Serial.print("(");
    Serial.print(response);
    Serial.print(")");
}

void printResponse(long response)
{
    Serial.print("(");
    Serial.print(response);
    Serial.print(")");
}

void printResponse(unsigned long response)
{
    Serial.print("(");
    Serial.print(response);
    Serial.print(")");
}

void printResponse(char response[])
{
    Serial.print("(");
    Serial.print(response);
    Serial.print(")");
}

void printResponse(float response)
{
    Serial.print("(");
    Serial.print(response);
    Serial.print(")");
}

void printSuccess()
{
    Serial.print("(OK)");
}

void printResponseErrorCode(int code)
{
    Serial.print("!");
    Serial.print(code);
    Serial.print(")");
}


void executeCommand()
{
    if (strcmp("LEGT", _command) == 0)
    {
        Serial.println("LEGACY MODE - START");
        digitalWrite(TMC220X_PIN_ENABLE, LOW);
        Serial.println("ENABLE LOW");
       
        Serial.println("MOVING STARTED");
        for(int i=0;i<500;i++) {
            digitalWrite(TMC220X_PIN_DIR, LOW);
            digitalWrite(TMC220X_PIN_STEP, 1);
            delayMicroseconds(1);
            digitalWrite(TMC220X_PIN_STEP, 0);
            delayMicroseconds(8000);
        }
         Serial.println("MOVING FINISHED");

        digitalWrite(TMC220X_PIN_ENABLE, HIGH);
        Serial.println("ENABLE HIGH");

        Serial.println("LEGACY MODE - END");

        printSuccess();
        return;
    }

    if(!_motorUARTInitialized) {
        printResponseErrorCode(999);
        return;
    }

    if (strcmp("GFRM", _command) == 0)
    {
        Serial.print("(");
        Serial.print("Board=");
        Serial.print(firmwareName);
        Serial.print(", Version=");
        Serial.print(firmwareVersion);
        Serial.print(")");
    }
    else if (strcmp("GPOS", _command) == 0)
    {
        printResponse(_eepromAfState[EEPROM_AF_STATE_POSITION]);
    }
    else if (strcmp("GTRG", _command) == 0)
    {
        printResponse(_motorTargetPosition);
    }
    else if (strcmp("STRG", _command) == 0)
    {
        long pos = strtol(_commandParam, NULL, 10);
        if(!setTargetPosition(pos)) {
            printResponseErrorCode(101);
        } else {
            printSuccess();
        }
    }
    else if (strcmp("GMOV", _command) == 0)
    {
        if (_motorIsMoving)
        {
            printResponse(1);
        }
        else
        {
            /*
                if your focuser has any play, this can affect the autofocuser performance. SGP for example goes aways from motorDir position and
                than starts traversing back. When it changes focus direction (traverse back), focuser play can cause FOV to wiggle just a bit,
                which causes enlongated stars on the next exposure. Settle buffer option returns IsMoving as TRUE after focuser reaches target position,
                letting it to settle a bit. Advices by Jared Wellman of SGP.
            */
            if (_eepromAfState[EEPROM_AF_STATE_SETTLE_BUFFER_MS] > 0L && _motorSettleBufferPrevMs != 0L)
            {
                long settleBufferCurrenMs = millis();
                if ((settleBufferCurrenMs - _motorSettleBufferPrevMs) < (unsigned long)_eepromAfState[EEPROM_AF_STATE_SETTLE_BUFFER_MS])
                {
                    printResponse(1);
                }
                else
                {
                    printResponse(0);
                    _motorSettleBufferPrevMs = 0L;
                }
            }
            else
            {
                printResponse(0);
            }
        }
    }
    else if (strcmp("SMOV", _command) == 0)
    {
        writeStepMode(_eepromAfState[EEPROM_AF_STATE_STEP_MODE]);
        startMotor();
        printSuccess();
    }
    else if (strcmp("STOP", _command) == 0)
    {
        stopMotor();
        printSuccess();
    }
    else if (strcmp("GMXP", _command) == 0)
    {
        printResponse((long)_eepromAfState[EEPROM_AF_STATE_MAX_POSITION]);
    }
    else if (strcmp("SMXP", _command) == 0)
    {
        setMaxPos(_commandParam);
        _eepromSaveAfState = true;
        printSuccess();
    }
    else if (strcmp("GMXM", _command) == 0)
    {
        printResponse((long)_eepromAfState[EEPROM_AF_STATE_MAX_MOVEMENT]);
    }
    else if (strcmp("SMXM", _command) == 0)
    {
        setMaxMovement(_commandParam);
        _eepromSaveAfState = true;
        printSuccess();
    }
        else if (strcmp("GMST", _command) == 0)
    {
        printResponse((int)_eepromAfState[EEPROM_AF_STATE_STEP_MODE_MANUAL]);
    }
    else if (strcmp("GSTP", _command) == 0)
    {
        printResponse((int)_eepromAfState[EEPROM_AF_STATE_STEP_MODE]);
    }
    else if (strcmp("SSTP", _command) == 0)
    {
        long sm = strtol(_commandParam, NULL, 10);
        if(writeStepMode(sm)) {
             _eepromSaveAfState = true;
            printSuccess();
        } else {
            printResponseErrorCode(102); 
        }
    }
    else if (strcmp("GSPD", _command) == 0)
    {
        printResponse((int)_eepromAfState[EEPROM_AF_STATE_SPEED_MODE]);
    }
    else if (strcmp("SSPD", _command) == 0)
    {
        if(setSpeedMode(_commandParam)) {
            _eepromSaveAfState = true;
            printSuccess();
        } else {
            printResponseErrorCode(103);
        }
    }
    else if (strcmp("RSET", _command) == 0)
    {
        eepromResetState();
        printSuccess();
    }
    else if (strcmp("RBOT", _command) == 0)
    {
        asm volatile("  jmp 0");
    }
    else if (strcmp("GBUF", _command) == 0)
    {
        printResponse((int)_eepromAfState[EEPROM_AF_STATE_SETTLE_BUFFER_MS]);
    }
    else if (strcmp("SBUF", _command) == 0)
    {
        setSettleBuffer(_commandParam);
        _eepromSaveAfState = true;
        printSuccess();
    }
    else if (strcmp("WEPR", _command) == 0)
    {
        _eepromSaveAfState = true;
        printSuccess();
    }
        else if (strcmp("GREV", _command) == 0)
    {
        printResponse((int)_eepromAfState[EEPROM_AF_STATE_REVERSE_DIRECTION]);
    }
    else if (strcmp("SREV", _command) == 0)
    {
        setReverseDir(_commandParam);
        _eepromSaveAfState = true;
        printSuccess();
    }
    else if (strcmp("SPOS", _command) == 0)
    {
        long newPosition = strtol(_commandParam, NULL, 10);
        if (newPosition < 0)
            newPosition = 0;
        else if (newPosition > _eepromAfState[EEPROM_AF_STATE_MAX_POSITION])
        {
            newPosition = _eepromAfState[EEPROM_AF_STATE_MAX_POSITION];
        }

        if (_eepromAfState[EEPROM_AF_STATE_POSITION] == newPosition)
        {
            printSuccess();
            return;
        }

        _eepromAfState[EEPROM_AF_STATE_POSITION] = newPosition;
        _motorTargetPosition = newPosition;
        _eepromSaveAfState = true;
        printSuccess();
    }
    else if (strcmp("GIDE", _command) == 0)
    {
        printResponse((long)_eepromAfState[EEPROM_AF_STATE_IDLE_EEPROM_WRITE_MS]);
    }
    else if (strcmp("SIDE", _command) == 0)
    {
        setIdleEepromWriteMs(_commandParam);
        _eepromSaveAfState = true;
        printSuccess();
    }
     else if (strcmp("GMHM", _command) == 0)
    {
        printResponse((long)_eepromAfState[EEPROM_AF_STATE_MOTOR_I_HOLD_MULTIPLIER]);
    }
     else if (strcmp("SMHM", _command) == 0)
    {
        long multiplier = strtol(_commandParam, NULL, 10);
        setMotorCurrent(_motorIMoveMultiplier, multiplier);
        _eepromSaveAfState = true;
        printSuccess();
    }
    else if (strcmp("GMMM", _command) == 0)
    {
        printResponse((long)_eepromAfState[EEPROM_AF_STATE_MOTOR_I_MOVE_MULTIPLIER]);
    }
     else if (strcmp("SMMM", _command) == 0)
    {
        long multiplier = strtol(_commandParam, NULL, 10);
        setMotorCurrent(multiplier, _motorIHoldMultiplier);
        _eepromSaveAfState = true;
        printSuccess();
    }
    else if (strcmp("SENA", _command) == 0)
    {
        long ena = strtol(_commandParam, NULL, 10);
        digitalWrite(TMC220X_PIN_ENABLE, ena == 1 ? LOW : HIGH);
        printSuccess();
    }
    else if (strcmp("SMHM", _command) == 0)
    {
        long multiplier = strtol(_commandParam, NULL, 10);
        setMotorCurrent(_motorIMoveMultiplier, multiplier);
        _eepromSaveAfState = true;
        printSuccess();
    }
    else if (strcmp("SENA", _command) == 0)
    {
        long ena = strtol(_commandParam, NULL, 10);
        digitalWrite(TMC220X_PIN_ENABLE, ena == 1 ? LOW : HIGH);
        printSuccess();
    }
    else if (strcmp("GTMC", _command) == 0)
    {
        printResponse(_temperatureCelsius);
    }
    else if (strcmp("SHST", _command) == 0)
    {
         long start = strtol(_commandParam, NULL, 10);
        _driver.hysteresis_start(start);
        printSuccess();
    }
    else if (strcmp("SHEN", _command) == 0)
    {
        long end = strtol(_commandParam, NULL, 10);
        _driver.hysteresis_end(end);
        printSuccess();
    }
    else if (strcmp("DEBG", _command) == 0)
    {
        Serial.print("Memory address: ");
        Serial.println(_eepromAfStateCurrentAddress);
        Serial.print("Motor full current (mA): ");
        Serial.println(_motorI);
        Serial.print("Motor current move multiplier (%): ");
        Serial.println(_eepromAfState[EEPROM_AF_STATE_MOTOR_I_MOVE_MULTIPLIER]);
        Serial.print("Motor current hold multiplier (%): ");
        Serial.println(_eepromAfState[EEPROM_AF_STATE_MOTOR_I_HOLD_MULTIPLIER]);
        Serial.print("HC/TP pin voltage: ");
        Serial.println(readHcPin());
        Serial.print("Position: ");
        Serial.println(_eepromAfState[EEPROM_AF_STATE_POSITION]);
        Serial.print("Temperature: ");
        Serial.println(_temperatureCelsius);
        Serial.print("HC connected: ");
        Serial.println(_hcConnected);
        Serial.print("Max position: ");
        Serial.println(_eepromAfState[EEPROM_AF_STATE_MAX_POSITION]);
        Serial.print("Max movement: ");
        Serial.println(_eepromAfState[EEPROM_AF_STATE_MAX_MOVEMENT]);
        Serial.print("Step mode: ");
        Serial.println(_eepromAfState[EEPROM_AF_STATE_STEP_MODE]);
        Serial.print("Speed mode: ");
        Serial.println(_eepromAfState[EEPROM_AF_STATE_SPEED_MODE]);
        Serial.print("Settle buffer ms: ");
        Serial.println(_eepromAfState[EEPROM_AF_STATE_SETTLE_BUFFER_MS]);
        Serial.print("Idle eeprom write (ms): ");
        Serial.println(_eepromAfState[EEPROM_AF_STATE_IDLE_EEPROM_WRITE_MS]);
        Serial.print("Reverse direction: ");
        Serial.println(_eepromAfState[EEPROM_AF_STATE_REVERSE_DIRECTION]);
    }
    else
    {
        printResponseErrorCode(100);
    }
}

void setup()
{
    Serial.begin(115200);

    EEPROM.setMemPool(0, EEPROMSizeATmega328);

    bool afStateFound = false;
    for (int i = 0; i < _eepromAfStateAdressesCount; i++)
    {
        if (eepromRead())
        {
            afStateFound = true;
            break;
        }
    }

    if (!afStateFound)
    {
        eepromResetState();
    }

    pinMode(TMC220X_PIN_DIR, OUTPUT);
    pinMode(TMC220X_PIN_STEP, OUTPUT);
    pinMode(TMC220X_PIN_MS1, OUTPUT);
    pinMode(TMC220X_PIN_MS2, OUTPUT);
    pinMode(TMC220X_PIN_ENABLE, OUTPUT);
    pinMode(MOTOR_SELECT_PIN_D5, INPUT_PULLUP);
    pinMode(MOTOR_SELECT_PIN_D6, INPUT_PULLUP);
    pinMode(A0, INPUT);

    digitalWrite(TMC220X_PIN_DIR, LOW);
    digitalWrite(TMC220X_PIN_STEP, LOW);
    digitalWrite(TMC220X_PIN_ENABLE, HIGH);
    
    _motorTargetPosition = _eepromAfState[EEPROM_AF_STATE_POSITION];
    _motorIMoveMultiplier = _eepromAfState[EEPROM_AF_STATE_MOTOR_I_MOVE_MULTIPLIER];
    _motorIHoldMultiplier = _eepromAfState[EEPROM_AF_STATE_MOTOR_I_HOLD_MULTIPLIER];
    _motorSettleBufferPrevMs = 0L;
    _motorLastMoveEepromMs = 0L;

    //TEMPERATURE SENSOR
    _sensors.begin();

    //MOTOR SELECTION HEADERS
    _motorI = MOTOR_I_14HS10_0404S_04A;
    int d5 = digitalRead(MOTOR_SELECT_PIN_D5);
    int d6 = digitalRead(MOTOR_SELECT_PIN_D6);
    
    if(d5 == HIGH && d6 == HIGH) {
        _motorI = MOTOR_I_14HS10_0404S_04A;
    } else if(d5 == LOW && d6 == HIGH) {
        _motorI = MOTOR_I_14HS17_0504S_05A;
    } else if(d5 == HIGH && d6 == LOW) {
        //TODO
    } else if(d5 == LOW && d6 == LOW) {
        //TODO
    }

    initUART();
}

void loop()
{ 
    if(!_motorUARTInitialized) {
        if(!initUART()) {
            delay(500);
            return;
        }
    }

    if (_motorIsMoving)
    {
        //give priority to motor with dedicated 50ms loops (effectivly pausing main loop, including serial event processing)
        while (millis() - _motorIsMovingLastRunMs < 50)
        {
            if(_motorManualIsMovingContinuous) {
                if(_motorManualIsMovingContinuousDir) {
                    _motorTargetPosition = _eepromAfState[EEPROM_AF_STATE_POSITION] + 1;
                } else {
                    _motorTargetPosition = _eepromAfState[EEPROM_AF_STATE_POSITION] - 1;
                }
            }

            if (_motorTargetPosition < (unsigned long)_eepromAfState[EEPROM_AF_STATE_POSITION])
            {
                digitalWrite(TMC220X_PIN_DIR, _eepromAfState[EEPROM_AF_STATE_REVERSE_DIRECTION] == 0 ? LOW : HIGH);
                digitalWrite(TMC220X_PIN_STEP, 1);
                delayMicroseconds(1);
                digitalWrite(TMC220X_PIN_STEP, 0);
                _eepromAfState[EEPROM_AF_STATE_POSITION]--;
                delayMicroseconds(_motorMoveDelay);
            }
            else if (_motorTargetPosition > (unsigned long)_eepromAfState[EEPROM_AF_STATE_POSITION])
            {
                digitalWrite(TMC220X_PIN_DIR, _eepromAfState[EEPROM_AF_STATE_REVERSE_DIRECTION] == 0 ? HIGH : LOW);
                digitalWrite(TMC220X_PIN_STEP, 1);
                delayMicroseconds(1);
                digitalWrite(TMC220X_PIN_STEP, 0);
                _eepromAfState[EEPROM_AF_STATE_POSITION]++;
                delayMicroseconds(_motorMoveDelay);
            }
            else
            {
                stopMotor();
            }

            if(_motorManualIsMovingContinuous && readHcButton() <= 0) {
                handleHC();
            }
        }

        _motorIsMovingLastRunMs = millis();
    }
    else
    {
        //save eeprom only after period of time from last movement (prevent EEPROM wear with write after each move)
        if (_motorLastMoveEepromMs != 0L && (millis() - _motorLastMoveEepromMs) > (unsigned long)_eepromAfState[EEPROM_AF_STATE_IDLE_EEPROM_WRITE_MS])
        {
            _eepromSaveAfState = true;
        }

        if (_eepromSaveAfState)
        {
            eepromWrite(false);
        }

        autoDiscovery();
        handleHC();
        handleTS();
    }
}

void serialEvent()
{
    while (Serial.available())
    {
        char c = Serial.read();

        if (c == '[')
        {
            _serialCommandRawIdx = 0;
            memset(_serialCommandRaw, 0, 70);
        }
        else if (c == ']')
        {

            _serialCommandRawLength = strlen(_serialCommandRaw);
            _commandParamLength = 0;
            memset(_command, 0, 5);
            memset(_commandParam, 0, 65);

            if (_serialCommandRawLength >= 4)
            {
                strncpy(_command, _serialCommandRaw, 4);
            }
            if (_serialCommandRawLength > 4)
            {
                _commandParamLength = _serialCommandRawLength - 4;
                strncpy(_commandParam, _serialCommandRaw + 4, _commandParamLength);
            }

            executeCommand();
            break;
        }
        else
        {
            _serialCommandRaw[_serialCommandRawIdx] = c;
            _serialCommandRawIdx++;
        }
    }
}
