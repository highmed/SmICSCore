using SmICSCoreLib.Factories.General;
using System.Collections.Generic;

namespace SmICSCoreLib.Factories.OutbreakDetection
{
    public class OutbreakDetectionParameter : TimespanParameter
    {
        public List<string> PathogenIDs { get; set; }
        public string Ward { get; set; }
    }
}
