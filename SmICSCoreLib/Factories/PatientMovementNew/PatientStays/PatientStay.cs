using SmICSCoreLib.Factories.General;
using System;

namespace SmICSCoreLib.Factories.PatientMovementNew.PatientStays
{
    public class PatientStay : Case
    {
        private const string TRANSFER = "Wechsel";
        private const string PROCEDURE = "Behandlung";

        public DateTime Admission { get; set; }
        public DateTime? Discharge { get; set; }
        public MovementType MovementType { get; set; }
        public string MovementTypeName
        {
            get
            {
                switch (MovementType)
                {
                    case MovementType.ADMISSION:
                        throw new ArgumentException("At this point the definition of 'ADMISSION' may not acure");
                    case MovementType.DISCHARGE:
                        throw new ArgumentException("At this point the definition of 'DISCHARGE' may not acure");
                    case MovementType.TRANSFER:
                        return TRANSFER;
                    case MovementType.PROCEDURE:
                        return PROCEDURE;
                    default:
                        throw new ArgumentException("Found no definition for MovementType");
                }          
            }
        }
        public string Ward { get; set; }
        public string Departement { get; set; }
        public string DepartementID { get; set; }
        public string Room { get; set; }
        public string StayingReason { get; set; }

        
        public bool Equals(PatientStay other)
        { 
            if(ReferenceEquals(this, other))
            {
                return true;
            }
            if (other is not null)
            {
                if (Admission == other.Admission)
                {
                    if (Discharge.HasValue)
                    {
                        if (other.Discharge.HasValue)
                        {
                            if (Discharge.Value == other.Discharge)
                            {
                                if (MovementType == other.MovementType)
                                {
                                    if (DepartementID == other.DepartementID)
                                    {
                                        if (Ward == other.Ward)
                                        {
                                            if (Room == other.Room)
                                            {
                                                return true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

    }
}
