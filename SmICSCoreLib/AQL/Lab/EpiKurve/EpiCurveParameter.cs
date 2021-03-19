using SmICSCoreLib.AQL.General;
using System;
using System.Collections.Generic;

namespace SmICSCoreLib.AQL.Lab.EpiKurve
{
    public class EpiCurveParameter : TimespanParameter
    {
        public List<string> PathogenCodes { get; set; }

        public EpiCurveParameter() { }

        public EpiCurveParameter(TimespanParameter timespanParameter, List<string> pathogenCodes)
        {
            Starttime = timespanParameter.Starttime;
            Endtime = timespanParameter.Endtime;
            PathogenCodes = pathogenCodes;
        }

        public string PathogenCodesToAqlMatchString()
        {
            string convertedList = String.Join("','", PathogenCodes);
            return "{'" + convertedList + "'}";
        }
    }
}