using System;

namespace SmICSCoreLib.Factories.PatientMovementNew
{
    public class Admission
    {
        public DateTime Date { get; set; }
        public MovementType MovementTypeID { get; } = MovementType.ADMISSION;
        public string MovementTypeName { get; } = "Aufnahme";

        public bool Equals(Admission other)
        {
            if(other != null)
            {
                return Date.Equals(other.Date);
            }
            return false;
        }
    }
}