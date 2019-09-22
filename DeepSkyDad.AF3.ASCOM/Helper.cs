using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASCOM.DeepSkyDad.AF1
{
    public class Helper
    {
        public static string FormatCommand(string command)
        {
            return string.Format("[{0}]", command);
        }

        public static double RadPerMin2DegPerSec(double radianPerMin)
        {
            return Radian2Degree(radianPerMin) / 60;
        }

        public static double DegPerSec2RadPerMin(double degPerSec)
        {
            return Degree2Radian(degPerSec * 60);
        }

        public static double ArcsecPerSec2RadPerMin(double arcsecPerSec)
        {
            //return Degree2Radian(arcsecPerSec / 3600 * 60); //3600 to get degree, 60 to get minute
            //specified by Andras
            return arcsecPerSec * 0.0002908882086657;
        }

        public static double RadPerMin2ArcsecPerSec(double radPerMin)
        {
            //return Radian2Degree(radPerMin) * 3600 / 60; //3600 to get degree, 60 to get minute
            //specified by Andras
            return radPerMin / 0.0002908882086657;
        }

        public static double RadPerMin2SecPerSiderealSec(double radPerMin)
        {
            //return radPerMin / 0.00007272205216643039;
            //specified by Andras
            return radPerMin / 0.004363323129985;
        }

        public static double SecPerSiderealSec2RadPerMin(double secPerSiderealSec)
        {
            //return secPerSiderealSec * 0.00007272205216643039;
            //specified by Andras
            return secPerSiderealSec * 0.004363323129985;
        }

        private static double Degree2Radian(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        private static double Radian2Degree(double angle)
        {
            return angle * (180.0 / Math.PI);
        }

        public static double GetHourAngle(double siderealTime, double rightAscension)
        {
            var r = siderealTime - rightAscension;
            if (r > 12)
                r -= 24;
            else if (r < -12)
                r += 24;
            return r;
        }

        public static double GetHourAngle24(double siderealTime, double rightAscension)
        {
            var r = siderealTime - rightAscension;
            if (r < 0)
                r += 24;
            return r;
        }

        //ASCOM
        // Summary:
        //     Well-known telescope tracking rates.
        //public enum DriveRates
        //{
        //    // Summary:
        //    //     Sidereal tracking rate (15.0 arcseconds per sidereal second).
        //    driveSidereal = 0,
        //    //
        //    // Summary:
        //    //     Lunar tracking rate (14.685 arcseconds per second).
        //    driveLunar = 1,
        //    //
        //    // Summary:
        //    //     Solar tracking rate (15.0 arcseconds per second).
        //    driveSolar = 2,
        //    //
        //    // Summary:
        //    //     King tracking rate (15.0369 arcseconds per second).
        //    driveKing = 3,
        //}
        //PULSAR
        //STOP 0,0
        //SID 1,0
        //LUN 2,0
        //SOL 3,0
        //USER1 4,0
        //USER2 5,0
        //USER2 6,0
        //public static int GetPulsarRateFromASCOMRate(DriveRates ascomRate)
        //{
        //    if (ascomRate == DriveRates.driveSidereal)
        //        return 1;
        //    else if (ascomRate == DriveRates.driveLunar)
        //        return 2;
        //    else if (ascomRate == DriveRates.driveSolar)
        //        return 3;
        //    throw new DriverException("Invalid drive rate " + ascomRate);
        //}

        //public static DriveRates GetASCOMRateFromPulsarRate(int pulsarRate)
        //{
        //    if (pulsarRate == 1)
        //        return DriveRates.driveSidereal;
        //    else if (pulsarRate == 2)
        //        return DriveRates.driveLunar;
        //    else if (pulsarRate == 3)
        //        return DriveRates.driveSolar;
        //    throw new DriverException("Pulsar rate not supported by ASCOM " + pulsarRate);
        //}

        //public bool IsWholeNumber(double number)
        //{
        //    return number % 1 == 0;
        //}
    }
}
