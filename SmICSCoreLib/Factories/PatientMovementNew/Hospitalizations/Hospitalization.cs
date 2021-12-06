using SmICSCoreLib.Factories.General;

namespace SmICSCoreLib.Factories.PatientMovementNew
{
    public class Hospitalization : Case
    {
        public Admission Admission { get; set; }
        public Discharge? Discharge { get; set; }
    }
}