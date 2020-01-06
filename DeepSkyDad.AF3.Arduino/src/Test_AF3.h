#include "EEPROM_AF3.h"
#include "Motor_AF3.h"
#include "Peripherals_AF3.h"
#include "General_AF3.h"

#pragma once
class Test_AF3
{
    public:
        int execute(EEPROM_AF3 &_eeprom, Motor_AF3 &_motor, Peripherals_AF3 &_peri);
};