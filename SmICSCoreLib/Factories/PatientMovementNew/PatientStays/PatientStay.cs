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
    }
}
