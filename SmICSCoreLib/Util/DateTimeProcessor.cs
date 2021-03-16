using System;
using System.Collections.Generic;
using System.Text;

namespace SmICSCoreLib.Util
{
    public class DateTimeProcessor
    {
        public static DateTime DateTimeFromString(string dateTimeString)
        {
            return Convert.ToDateTime(dateTimeString);
        }

        public static DateTime MaxDateTime(DateTime dateTime, DateTime dateTime2)
        {
            if(dateTime.CompareTo(dateTime2) >= 0)
            {
                return dateTime;
            }
            return dateTime2;
        }

        public static DateTime MinDateTime(DateTime dateTime, DateTime dateTime2)
        {
            if (dateTime.CompareTo(dateTime) <= 0)
            {
                return dateTime;
            }
            return dateTime2;
        }
    }
}
