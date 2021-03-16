using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SmICSCoreLib.Util
{
    public class PayloadControl
    {
        public static void checkStringParam(string parameter, string pattern)
        {
            if (parameter is null || parameter.Equals(""))
            {
                throw new ArgumentNullException("Parameter may not be empty", parameter);
            }
            else if (!Regex.IsMatch(parameter, pattern))
            {
                throw new ArgumentException("Valid paramameter needs to be a comma seperated list of IDs containing just numerical values", parameter);
            }
        }
        public void checkIntegerParam(int number)
        {
            if(number.GetType() != typeof(int))
            {
                throw new ArgumentException("Valid parameter needs to be an integer", number.ToString());
            }
        }

        public void checkDateStringParam(string parameter, string pattern)
        {
            if (parameter.Equals(""))
            {
                throw new ArgumentNullException("Parameter may not be empty", parameter);
            }
            else if (!Regex.IsMatch(parameter, pattern))
            {
                throw new ArgumentException("Valid paramameter needs to be formated as yyyy-MM-ddTHH:mm:sszzz", parameter);
            }
        }
        public class Pattern
        {
            public static string LIST = @"^(\s*\d{1,10}\s*){1}(,\s*\d{1,10}\s*)*$";
            public static string ID = @"^(\s*\d{1,10}\s*)+$";
            public static string DATETIME = @" ^ (\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2}):(\d{2})(/[+-]/)(\d{2}):(\d{2}){1}$";
        }
    }
}
