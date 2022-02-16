using System;

namespace SmICSCoreLib.Factories.PatientMovementNew
{
    public class Discharge
    {
        public DateTime? Date { get; set; }
        public MovementType MovementTypeID { get; } = MovementType.DISCHARGE;
        public string MovementTypeName { get; } = "Entlassung";
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