using SmICSCoreLib.Factories.MiBi.Contact;
using SmICSCoreLib.Factories.MiBi.Nosocomial;
using SmICSCoreLib.Factories.MiBi.PatientView.Parameter;
using SmICSCoreLib.Factories.PatientMovementNew;
using SmICSCoreLib.Factories.PatientMovementNew.PatientStays;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<ContactRoot> LoadContactData(ContactParameter parameter)
        {
            try
            {
                List<Hospitalization> hospitalizations =await _hospitalizationFac.ProcessAsync(parameter);
                if (hospitalizations is not null)
                {
                    Hospitalization latestHospitalization = hospitalizations.Last();
                    PathogenParameter pathogenParameter = new PathogenParameter() { PathogenCodes = parameter.PathogenCodes };
                    SortedList<Hospitalization, InfectionStatus> infectionStatus = await _infectionStatusFac.ProcessAsync(parameter, pathogenParameter, parameter.Resistence);
                    List<SmICSCoreLib.Factories.PatientMovementNew.PatientStays.PatientStay> contactLocations = await _patientStayFac.ProcessAsync(latestHospitalization);
                    RemoveDoubleStays(contactLocations);
                    ContactRoot rootContact = new ContactRoot()
                    {
                        CurrentHospitalization = latestHospitalization,
                        Hospitalizations = hospitalizations,
                        InfectionStatus = infectionStatus,
                        PatientStays = new Dictionary<Hospitalization, List<PatientStay>> { { latestHospitalization, contactLocations } },
                        Contacts = new Dictionary<Hospitalization, List<Contact>> { { latestHospitalization, null } }
                    };

                    MergeInfectionStatusAndContactCases(rootContact, latestHospitalization, pathogenParameter, parameter.Resistence);

                    return rootContact;
                }
                return null;
            }
            catch
            {
                throw;
            }
        }
        private void RemoveDoubleStays(List<PatientStay> patientStays)
        {
            List<int> indices = new List<int>();
            for (int i = 0; i < (patientStays.Count - 1); i++)
            {
                if (patientStays[i].DepartementID == patientStays[i + 1].Departement &&
                    patientStays[i].Ward == patientStays[i + 1].Ward &&
                    patientStays[i].Admission.Date == patientStays[i + 1].Admission.Date &&
                    ((patientStays[i].Discharge.HasValue && patientStays[i + 1].Discharge.HasValue &&
                    patientStays[i].Discharge.Value.Date == patientStays[i + 1].Discharge.Value.Date) ||
                    (!patientStays[i].Discharge.HasValue && !patientStays[i + 1].Discharge.HasValue)))
                {
                    indices.Add(i);
                }
            }
            foreach (int index in indices)
            {
                patientStays.RemoveAt(index);
            }
        }
        public async Task GetPreviousHospitalizationContacts(ContactRoot rootContact, ContactParameter parameter)
        {
            try
            {
                int index = rootContact.Hospitalizations.IndexOf(rootContact.CurrentHospitalization);
                Hospitalization prevHospitalization = rootContact.Hospitalizations[index - 1];
                if (!rootContact.Contacts.ContainsKey(prevHospitalization))
                {
                    PathogenParameter pathogenParameter = new PathogenParameter() { PathogenCodes = parameter.PathogenCodes };
                    SortedList<Hospitalization, InfectionStatus> infectionStatus = await _infectionStatusFac.ProcessAsync(prevHospitalization, pathogenParameter, parameter.Resistence);
                    List<SmICSCoreLib.Factories.PatientMovementNew.PatientStays.PatientStay> contactLocations = await _patientStayFac.ProcessAsync(prevHospitalization);
                    rootContact.AddHospitalization(prevHospitalization, infectionStatus[prevHospitalization], null, null);
                    await MergeInfectionStatusAndContactCases(rootContact, prevHospitalization, pathogenParameter, parameter.Resistence);
                }
                rootContact.CurrentHospitalization = prevHospitalization;
            }
            catch
            {
                throw;
            }
        }

        public List<string> GetFilter(List<string> pathogenCodes)
        {
            try
            {
                List<string> filter = Rules.GetPossibleMREClasses(pathogenCodes);
                return filter;
            }
            catch
            {
                throw;
            }
        }

        private async Task MergeInfectionStatusAndContactCases(ContactRoot rootContact, Hospitalization hospitalization, PathogenParameter pathogenParameter, string resistence)
        {
            try
            {
                List<Contact> contacts = new List<Contact>();
                List<SmICSCoreLib.Factories.PatientMovementNew.PatientStays.PatientStay> contactLocations = await _contactFac.ProcessAsync(hospitalization);
                List<string> distinctPatientIDs = contactLocations.Select(cl => cl.PatientID).Distinct().ToList();
                Dictionary<string, SortedList<Hospitalization, InfectionStatus>> infectionStati = new Dictionary<string, SortedList<Hospitalization, InfectionStatus>>();
                distinctPatientIDs.ForEach(async patID => infectionStati.Add(patID, await _infectionStatusFac.ProcessAsync(new SmICSCoreLib.Factories.General.Patient() { PatientID = patID }, pathogenParameter, resistence)));
                foreach (SmICSCoreLib.Factories.PatientMovementNew.PatientStays.PatientStay locationContact in contactLocations)
                {
                    if (locationContact.PatientID != rootContact.Hospitalizations.Last().PatientID)
                    {
                        SortedList<Hospitalization, InfectionStatus> infectionStatus = infectionStati[locationContact.PatientID];
                        List<PatientStay> rootStays = GetRootStay(rootContact.PatientStays[hospitalization], locationContact);
                        if (rootStays is not null)
                        {
                            Hospitalization hosp = await _hospitalizationFac.ProcessAsync(locationContact);
                            foreach (PatientStay stay in rootStays)
                            {
                                Contact contact = new Contact
                                {
                                    PatientID = locationContact.PatientID,
                                    CaseID = locationContact.CaseID,
                                    PatientLocation = locationContact,
                                    ContactStart = stay.Admission >= locationContact.Admission ? stay.Admission : locationContact.Admission,
                                    ContactEnd = GetContactEnd(stay.Discharge, locationContact.Discharge),
                                    RoomContact = stay.Room != null && stay.Room == locationContact.Room ? true : false,
                                    WardContact = stay.Ward != null && stay.Ward == locationContact.Ward ? true : false,
                                    DepartementContact = stay.DepartementID != null && stay.DepartementID == locationContact.DepartementID ? true : false
                                };
                                contact.Hospitalization = hosp;
                                contact.InfectionStatus = infectionStatus.Count > 0 ? infectionStatus.First().Value : null;
                                contact.StatusDate = contact.InfectionStatus is not null && contact.InfectionStatus.Known ? contact.Hospitalization.Admission.Date : null;
                                contacts.Add(contact);
                            }
                        }
                    }
                }
                contacts = SortContacts(contacts);
                rootContact.Contacts[hospitalization] = contacts;
            }
            catch
            {
                throw;
            }
        }

        public List<string> GetFilter(ContactParameter parameter)
        {
            try
            {
                List<string> filter = Rules.GetPossibleMREClasses(parameter.PathogenCodes);
                return filter;
            }
            catch
            {
                throw;
            }
        }

        private List<PatientStay> GetRootStay(List<SmICSCoreLib.Factories.PatientMovementNew.PatientStays.PatientStay> patientStays, SmICSCoreLib.Factories.PatientMovementNew.PatientStays.PatientStay patientLocation)
        {
            List<PatientStay> contactsPoint = new List<PatientStay>();
            foreach(PatientStay stay in patientStays)
            {
                if(patientLocation.MovementType == MovementType.PROCEDURE)
                {
                    if (stay.MovementType == MovementType.PROCEDURE)
                    {
                        if (stay.Admission.Date == patientLocation.Admission.Date && stay.DepartementID == patientLocation.DepartementID)
                        {
                            contactsPoint.Add(stay);
                        }
                    }
                }
                else
                {
                    if (patientLocation.Discharge.HasValue && stay.Discharge.HasValue)
                    {
                        if (stay.Admission <= patientLocation.Discharge && stay.Discharge >= patientLocation.Admission && patientLocation.Ward == stay.Ward)
                        {
                            contactsPoint.Add(stay);
                        }
                    }
                    else if (!patientLocation.Discharge.HasValue && stay.Discharge.HasValue)
                    {
                        if (stay.Discharge >= patientLocation.Admission && patientLocation.Ward == stay.Ward)
                        {
                            contactsPoint.Add(stay);
                        }
                    }
                    else if (patientLocation.Discharge.HasValue && !stay.Discharge.HasValue)
                    {
                        if (stay.Admission <= patientLocation.Discharge && patientLocation.Ward == stay.Ward)
                        {
                            contactsPoint.Add(stay);
                        }
                    }
                    else 
                    {
                        contactsPoint.Add(stay);
                    }
                }
               
            }
            if (contactsPoint.Count > 0)
            {
                return contactsPoint;
            }
            return null;
        }

        private DateTime? GetContactEnd(DateTime? one, DateTime? other)
        {
            if(one.HasValue && other.HasValue)
            {
                return one.Value <= other.Value ? one.Value : other.Value;
            }
            else if (!one.HasValue && other.HasValue)
            {
                return other.Value;
            }
            else if (one.HasValue && !other.HasValue)
            {
                return one.Value;
            }
            return null;
        }

        private List<Contact> SortContacts(List<Contact> contacts)
        {
            List<Contact> roomContacts = contacts.Where(c => c.RoomContact).OrderByDescending(c => c.ContactStart).ThenBy(c => c.ContactEnd).ToList();
            List<Contact> wardContacts = contacts.Where(c => !c.RoomContact && c.WardContact).OrderByDescending(c => c.ContactStart).ThenBy(c => c.ContactEnd).ToList();
            List<Contact> departementContacts = contacts.Where(c => !c.RoomContact && !c.WardContact).OrderByDescending(c => c.ContactStart).ThenBy(c => c.ContactEnd).ToList();

            List<Contact> sortedList = new List<Contact>();
            sortedList.AddRange(roomContacts);
            sortedList.AddRange(wardContacts);
            sortedList.AddRange(departementContacts);

            return sortedList;
        }
    }

}

