using SmICSCoreLib.Factories.General;
using System.Collections.Generic;
using System;

namespace SmICSCoreLib.Factories.OutbreakDetection
{
    public class OutbreakDetectionParameter : TimespanParameter
    {
        public List<string> PathogenIDs { get; set; }
        public string Ward { get; set; }
        public bool Retro { get; set; }
        public string ToAQLMatchString()
        {
            string convertedPathogenIDs = String.Join("','", PathogenIDs);
            return "{'" + convertedPathogenIDs + "'}";
        }
    }
}
