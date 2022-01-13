using System;

namespace SmICSCoreLib.Factories.PatientMovementNew
{
    public class Admission
    {
        public DateTime Date { get; set; }
        public int MovementTypeID { get; set; } = 1;

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