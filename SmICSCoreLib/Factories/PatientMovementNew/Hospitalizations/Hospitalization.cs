using SmICSCoreLib.Factories.General;

namespace SmICSCoreLib.Factories.PatientMovementNew
{
    public class Hospitalization : Case
    {
        public Admission Admission { get; set; }
        public Discharge Discharge { get; set; }

        public bool Equals(Hospitalization other)
        {
            if(base.Equals(other))
            {
                if (Admission.Equals(other.Admission))
                {
                    return Discharge != null && Discharge.Equals(other.Discharge);
                }
            }
            return false;
        }
    }
}