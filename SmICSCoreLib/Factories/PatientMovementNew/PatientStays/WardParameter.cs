using System;

namespace SmICSCoreLib.Factories.PatientMovementNew.PatientStays
{
    public class WardParameter
    {
        public string Ward { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Pathogen { get; set; }
    }
}