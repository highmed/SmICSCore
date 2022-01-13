using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.MiBi.Contact;
using SmICSCoreLib.Factories.MiBi.Nosocomial;
using SmICSCoreLib.Factories.PatientMovementNew;
using SmICSCoreLib.Factories.PatientMovementNew.PatientStays;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmICSWebApp.Data.Contact
{
    public class ContactService
    {
        private readonly IContactFactory _contactFac;
        private readonly InfectionStatusFactory _infectionStatusFac;
        private readonly IHospitalizationFactory _hospitalizationFac;

        public ContactService(IContactFactory contactFac, InfectionStatusFactory infectionStatusFac, IHospitalizationFactory hospitalizationFac)
        {
            _contactFac = contactFac;
            _infectionStatusFac = infectionStatusFac;
            _hospitalizationFac = hospitalizationFac;
        }

        public ContactRoot LoadContactData(SmICSCoreLib.Factories.General.Patient patient)
        {
            List<Hospitalization> hospitalizations = _hospitalizationFac.Process(patient);
            Hospitalization latestHospitalization = hospitalizations.Last();
            SortedList<Hospitalization, Dictionary<string, Dictionary<string, InfectionStatus>>> infectionStatus =_infectionStatusFac.Process(patient);
            List<SmICSCoreLib.Factories.PatientMovementNew.PatientStays.PatientStay> contactLocations = _contactFac.Process(latestHospitalization);
            ContactRoot rootContact = new ContactRoot()
            {
                CurrentHospitalization = latestHospitalization,
                Hospitalizations = hospitalizations,
                InfectionStatus = new Dictionary<Hospitalization, Dictionary<string, InfectionStatus>> { { latestHospitalization, infectionStatus[latestHospitalization]["pathogen"] } },
                PatientStays = new Dictionary<Hospitalization, List<PatientStay>> { { latestHospitalization, null } },
                Contacts = new Dictionary<Hospitalization, List<Contact>> { { latestHospitalization, null } }
            }; 
            
            infectionStatus.Clear();

            MergeInfectionStatusAndContactCases(ref rootContact, latestHospitalization, contactLocations);
           
            return rootContact;
        }

        public void GetPreviousHospitalizationContacts(ref ContactRoot rootContact, Hospitalization hospitalization)
        {
            SortedList<Hospitalization, Dictionary<string, Dictionary<string, InfectionStatus>>> infectionStatus = _infectionStatusFac.Process(hospitalization);
            List<SmICSCoreLib.Factories.PatientMovementNew.PatientStays.PatientStay> contactLocations = _contactFac.Process(hospitalization);
            rootContact.AddHospitalization(hospitalization, infectionStatus[hospitalization]["pathogen"], null, null);
            MergeInfectionStatusAndContactCases(ref rootContact, hospitalization, contactLocations);
        }

        private void MergeInfectionStatusAndContactCases(ref ContactRoot rootContact, Hospitalization hospitalization, List<SmICSCoreLib.Factories.PatientMovementNew.PatientStays.PatientStay> contactLocations) 
        {
            List<Contact> contacts = new List<Contact>();
            foreach(SmICSCoreLib.Factories.PatientMovementNew.PatientStays.PatientStay patLoc in contactLocations)
            {
                SortedList<Hospitalization, Dictionary<string, Dictionary<string, InfectionStatus>>> infectionStatus = _infectionStatusFac.Process(patLoc);
                SmICSCoreLib.Factories.PatientMovementNew.PatientStays.PatientStay rootStay = rootContact.PatientStays[hospitalization].
                    Where(stay => 
                    stay.Admission <= patLoc.Discharge && 
                    stay.Discharge >= stay.Admission && 
                    patLoc.Ward == stay.Ward).
                    FirstOrDefault();
                Contact contact = new Contact
                {
                    InfectionStatus = infectionStatus.First().Value["pathogen"],
                    PatientLocation = patLoc,
                    ContactStart = rootStay.Admission >= patLoc.Admission ? rootStay.Admission : patLoc.Admission,
                    ContactEnd = rootStay.Discharge <= patLoc.Discharge ? rootStay.Discharge : patLoc.Discharge,
                    RoomContact = rootStay.Room != null && rootStay.Room == patLoc.Room ? true : false,
                    WardContact = rootStay.Ward != null && rootStay.Ward == patLoc.Ward ? true : false,
                    DepartementContact = rootStay.DepartementID != null && rootStay.DepartementID == patLoc.DepartementID ? true : false
                };
                contacts.Add(contact);
                
            }
            rootContact.Contacts[hospitalization] = contacts;
        }
    }
}
