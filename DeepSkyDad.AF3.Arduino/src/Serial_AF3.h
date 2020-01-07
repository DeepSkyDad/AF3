#include "StringProxy_AF3.h"

#pragma once
class Serial_AF3
{
    private:
        StringProxy_AF3* _stringProxy;
        char _serialCommandRaw[70];
        int _serialCommandRawIdx;
        int _serialCommandRawLength;
        char _command[5];
        char _commandParam[65];
        int _commandParamLength;
    public:
        void init(StringProxy_AF3 &stringProxy);
        void serialEvent();
};