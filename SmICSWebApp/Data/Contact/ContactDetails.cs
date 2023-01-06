using System;

namespace SmICSWebApp.Data.Contact
{
    public class ContactDetails : IEquatable<ContactDetails>
    {
        public string PatientID { get; set; }
        public string StatusDate { get; set; }
        public string Room { get; set; }
        public string Ward { get; set; }
        public string Departement { get; set; }
        public string ContactStart { get; set; }
        public string ContactEnd { get; set; }
        public bool IsPresent { get; set; }
        public bool IsNosocomial { get; set; }
        public bool IsKnown { get; set; }

        public bool Equals(ContactDetails other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            if (other is not null)
            {
                if (PatientID == other.PatientID)
                {
                    if (Room == other.Room)
                    {
                        if (Ward == other.Ward)
                        {
                            if (Departement == other.Departement)
                            {
                                if (ContactStart == other.ContactStart)
                                {
                                    if (ContactEnd == other.ContactEnd)
                                    {
                                        return true;
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
