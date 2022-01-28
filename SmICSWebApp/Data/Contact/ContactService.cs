using SmICSCoreLib.Factories.MiBi.Contact;
using SmICSCoreLib.Factories.MiBi.Nosocomial;
using SmICSCoreLib.Factories.MiBi.PatientView.Parameter;
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
        private readonly IPatientStayFactory _patientStayFac;
        public ContactService(IContactFactory contactFac, InfectionStatusFactory infectionStatusFac, IHospitalizationFactory hospitalizationFac, IPatientStayFactory patientStayFac)
        {
            _contactFac = contactFac;
            _infectionStatusFac = infectionStatusFac;
            _hospitalizationFac = hospitalizationFac;
            _patientStayFac = patientStayFac;
        }

        public ContactRoot LoadContactData(ContactParameter parameter)
        {
            List<Hospitalization> hospitalizations = _hospitalizationFac.Process(parameter);
            Hospitalization latestHospitalization = hospitalizations.Last();
            PathogenParameter pathogenParameter = new PathogenParameter() { Name = parameter.Pathogen };
            SortedList<Hospitalization, InfectionStatus> infectionStatus =_infectionStatusFac.Process(parameter, pathogenParameter, parameter.Resistence);
            List<SmICSCoreLib.Factories.PatientMovementNew.PatientStays.PatientStay> contactLocations = _patientStayFac.Process(latestHospitalization);
            ContactRoot rootContact = new ContactRoot()
            {
                CurrentHospitalization = latestHospitalization,
                Hospitalizations = hospitalizations,
                InfectionStatus = infectionStatus,
                PatientStays = new Dictionary<Hospitalization, List<PatientStay>> { { latestHospitalization, contactLocations } },
                Contacts = new Dictionary<Hospitalization, List<Contact>> { { latestHospitalization, null } }
            }; 
            
            infectionStatus.Clear();

            MergeInfectionStatusAndContactCases(ref rootContact, latestHospitalization, pathogenParameter, parameter.Resistence);
           
            return rootContact;
        }

        public void GetPreviousHospitalizationContacts(ref ContactRoot rootContact, ContactParameter parameter)
        {
            int index = rootContact.Hospitalizations.IndexOf(rootContact.CurrentHospitalization);
            Hospitalization prevHospitalization = rootContact.Hospitalizations[index - 1];
            if (!rootContact.Contacts.ContainsKey(prevHospitalization))
            {
                PathogenParameter pathogenParameter = new PathogenParameter() { Name = parameter.Pathogen };
                SortedList<Hospitalization, InfectionStatus> infectionStatus = _infectionStatusFac.Process(prevHospitalization, pathogenParameter, parameter.Resistence);
                List<SmICSCoreLib.Factories.PatientMovementNew.PatientStays.PatientStay> contactLocations = _patientStayFac.Process(prevHospitalization);
                rootContact.AddHospitalization(prevHospitalization, infectionStatus[prevHospitalization], null, null);
                MergeInfectionStatusAndContactCases(ref rootContact, prevHospitalization, pathogenParameter, parameter.Resistence);
            }
            rootContact.CurrentHospitalization = prevHospitalization;
        }

        public List<string> GetFilter(string pathogen)
        {
            List<string> filter = Rules.GetPossibleMREClasses(pathogen);
            return filter;
        }

        private void MergeInfectionStatusAndContactCases(ref ContactRoot rootContact, Hospitalization hospitalization, PathogenParameter pathogenParameter, string resistence)
        {
            List<Contact> contacts = new List<Contact>();
            List< SmICSCoreLib.Factories.PatientMovementNew.PatientStays.PatientStay> contactLocations = _contactFac.Process(hospitalization);
            foreach(SmICSCoreLib.Factories.PatientMovementNew.PatientStays.PatientStay patLoc in contactLocations)
            {
                if (patLoc.PatientID != rootContact.Hospitalizations.Last().PatientID)
                {
                    SortedList<Hospitalization, InfectionStatus> infectionStatus = _infectionStatusFac.Process(patLoc, pathogenParameter, resistence);
                    SmICSCoreLib.Factories.PatientMovementNew.PatientStays.PatientStay rootStay = rootContact.PatientStays[hospitalization].
                        Where(stay =>
                        stay.Admission <= patLoc.Discharge &&
                        stay.Discharge >= stay.Admission &&
                        patLoc.Ward == stay.Ward).
                        FirstOrDefault();
                    Contact contact = new Contact
                    {
                        PatientID = patLoc.PatientID,
                        CaseID = patLoc.CaseID,
                        PatientLocation = patLoc,
                        ContactStart = rootStay.Admission >= patLoc.Admission ? rootStay.Admission : patLoc.Admission,
                        ContactEnd = rootStay.Discharge <= patLoc.Discharge ? rootStay.Discharge : patLoc.Discharge,
                        RoomContact = rootStay.Room != null && rootStay.Room == patLoc.Room ? true : false,
                        WardContact = rootStay.Ward != null && rootStay.Ward == patLoc.Ward ? true : false,
                        DepartementContact = rootStay.DepartementID != null && rootStay.DepartementID == patLoc.DepartementID ? true : false
                    };
                    contact.InfectionStatus = infectionStatus.Count > 0 ? infectionStatus.First().Value : null;
                    contact.StatusDate = contact.InfectionStatus is not null && contact.InfectionStatus.Known ? _hospitalizationFac.Process(patLoc).Admission.Date : null;
                    contacts.Add(contact);
                }
            }
            contacts = contacts.OrderByDescending(c => c.RoomContact).ThenBy(c => c.ContactStart).ThenBy(c => c.ContactEnd).ToList();
            rootContact.Contacts[hospitalization] = contacts;
        }

        public List<string> GetFilter(ContactParameter parameter)
        {
            List<string> filter = Rules.GetPossibleMREClasses(parameter.Pathogen);
            return filter;
        }
    }
}
