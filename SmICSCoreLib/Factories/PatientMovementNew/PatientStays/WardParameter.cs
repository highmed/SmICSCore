using System;
using System.Collections.Generic;

namespace SmICSCoreLib.Factories.PatientMovementNew.PatientStays
{
    public class WardParameter
    {
        public string Ward { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public List<string> PathogenCode { get; set; }
        public string DepartementID { get; set; }
    }
}