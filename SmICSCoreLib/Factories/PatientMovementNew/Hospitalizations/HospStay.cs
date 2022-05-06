using SmICSCoreLib.Factories.General;
using System;

namespace SmICSCoreLib.Factories.PatientMovementNew
{
    public class HospStay : Case
    {
        public DateTime Admission { get; set; }
        public DateTime? Discharge { get; set; }
    }
}