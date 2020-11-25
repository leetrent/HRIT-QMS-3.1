using System;
using System.Collections.Generic;

namespace QmsCore.Lib
{
    static class DateCalc
    {
        public static int DaysBetween(DateTime start, DateTime end)
        {
            TimeSpan ts = end.Subtract(start);
            return ts.Days;
        }

        public static int DaysOld(DateTime start)
        {
            return DaysBetween(start, DateTime.Now);
        }
    }
}