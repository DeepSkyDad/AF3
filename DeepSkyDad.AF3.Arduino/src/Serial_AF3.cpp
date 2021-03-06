#include "Serial_AF3.h"

void Serial_AF3::init(StringProxy_AF3 &stringProxy)
{
    _stringProxy = &stringProxy;
}

void Serial_AF3::serialEvent()
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

            _serialCommandRawLength = _serialCommandRawIdx;
            _commandParamLength = 0;
            memset(_command, 0, 5);
            memset(_commandParam, 0, 65);

            if (_serialCommandRawLength >= 4)
            {
                strncpy(_command, _serialCommandRaw, 4);
                _command[4] = 0; 
            }
            if (_serialCommandRawLength > 4)
            {
                _commandParamLength = _serialCommandRawLength - 4;
                strncpy(_commandParam, _serialCommandRaw + 4, _commandParamLength);
                 _commandParam[_commandParamLength] = 0; 
            }

            //Serial.print('<');
            //Serial.print(_serialCommandRawLength);
            //Serial.print('_');
            //Serial.print(_serialCommandRaw);
            //Serial.print('_');
            //Serial.print(_command);
            //Serial.print('>');
            Serial.print(_stringProxy->processCommand(_command, _commandParam, _commandParamLength));

            break;
        }
        else
        {
            if(_serialCommandRawIdx == 69) {
                _serialCommandRawIdx = 0;
                break;
            }
               
            _serialCommandRaw[_serialCommandRawIdx] = c;
            _serialCommandRawIdx++;
        }
    }
}