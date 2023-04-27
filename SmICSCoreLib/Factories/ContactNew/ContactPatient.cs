using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.PatientMovementNew.PatientStays;
using SmICSCoreLib.Models.General;
using System;
using System.Collections.Generic;

namespace SmICSCoreLib.Factories.ContactNew
{
    public class ContactPatient : Patient 
    {
        public ContactPatient()
        {
            Contacts = new List<ContactInformation>();
        }
        public List<ContactInformation> Contacts { get; set; }
    }
    
    public class ContactInformation
    {
        public ContactInformation()
        {
            ContactTime = new ContactTime();
            ContactLocation = new ContactLocation();
        }
        public ContactTime ContactTime { get; set; }

        public ContactLocation ContactLocation { get; set; }

        public PatientStay PatientStayMetaData { get; init; }
    }
    public class ContactTime
    {
        public DateOnly ContactStart { get; set; }
        public DateOnly? ContactEnd { get; set; }

        internal void DetermineContactTime(PatientStay rootStay, PatientStay possibleContact)
        {
            ContactStart = GetContactStart(rootStay, possibleContact);
            ContactEnd = GetContactEnd(rootStay, possibleContact);
        }

        private DateOnly GetContactStart(PatientStay rootStay, PatientStay possibleContact)
        {
            return rootStay.Admission.Date >= possibleContact.Admission.Date ? DateOnly.FromDateTime(rootStay.Admission) : DateOnly.FromDateTime(possibleContact.Admission);
        }

        private DateOnly? GetContactEnd(PatientStay rootStay, PatientStay possibleContact)
        {
            if (rootStay.Discharge.HasValue && possibleContact.Discharge.HasValue)
            {
                return rootStay.Discharge.Value <= possibleContact.Discharge.Value ? DateOnly.FromDateTime(rootStay.Discharge.Value) : DateOnly.FromDateTime(possibleContact.Discharge.Value.Date);
            }
            else if (rootStay.Discharge.HasValue)
            {
                return DateOnly.FromDateTime(rootStay.Discharge.Value);
            }
            else if (possibleContact.Discharge.HasValue)
            {
                return DateOnly.FromDateTime(possibleContact.Discharge.Value);
            }
            return null;
        }
    } 
    public class ContactLocation
    {
        public Ward? Ward { get; set; }
        public Departement? Departement { get; set; }
        public string? Room { get; set; }

        internal void SetFromLocation(LocationCategory category, Location location)
        {
            if (category.Equals(LocationCategory.WARD) && location is Ward)
            {
                Ward = location as Ward;
            }
            else if (category.Equals(LocationCategory.DEPARTEMENT) && location is Departement)
            {
                Departement = location as Departement;
            }
            else
            {
                throw new ArgumentException("LocationCategory doesn't fit to the given Location");
            }
        }

        internal void SetRoom(PatientStay rootStay, PatientStay possibleContact)
        {
            Room = rootStay.Room == possibleContact.Room ? rootStay.Room : null;
        }
    }
}
