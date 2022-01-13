using System;

namespace SmICSCoreLib.Factories.PatientMovementNew
{
    public class Discharge
    {
        public DateTime? Date { get; set; }
        public int MovementTypeID { get; set; } = 2;
        public bool Equals(Discharge other)
        {
            if (other != null)
            {
                return Date.HasValue && other.Date.HasValue && Date.Equals(other.Date);
            }
            return false;
        }
    }
}