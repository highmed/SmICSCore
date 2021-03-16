using System;
using System.Collections.Generic;
using System.Text;

namespace SmICSCoreLib.Util
{
    public class ExceptionHandler
    {
        public static Exception CastException(Exception e)
        {
            if (e is ArgumentNullException)
            {
                return new ArgumentNullException();
            }
            else if (e is ArgumentException)
            {
                return new ArgumentException();
            }
            else
            {
                return new Exception();
            }
        }
    }
}
