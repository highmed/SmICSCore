using SmICSCoreLib.Factories.General;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SmICSCoreLib.Factories.EpiCurve
{
    public class ExtendedEpiCurveParameter : EpiCurveParameter
    {
        [JsonIgnore]
        public List<string> PathogenCodes { get; set; }

        public ExtendedEpiCurveParameter()
        {

        }
        public ExtendedEpiCurveParameter(TimespanParameter timespanParameter, List<string> pathogenCodes, string pathogen) : base(timespanParameter, pathogen)
        {
            PathogenCodes = pathogenCodes;
        }

        public string PathogenCodesToAqlMatchString()
        {
            string convertedList = string.Join("','", PathogenCodes);
            return "{'" + convertedList + "'}";
        }

    }
}
