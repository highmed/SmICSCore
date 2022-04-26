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
            if (hospitalizations is not null)
            {
                Hospitalization latestHospitalization = hospitalizations.Last();
                PathogenParameter pathogenParameter = new PathogenParameter() { PathogenCodes = parameter.PathogenCodes };
                SortedList<Hospitalization, InfectionStatus> infectionStatus = _infectionStatusFac.Process(parameter, pathogenParameter, parameter.Resistence);
                List<SmICSCoreLib.Factories.PatientMovementNew.PatientStays.PatientStay> contactLocations = _patientStayFac.Process(latestHospitalization);
                RemoveDoubleStays(contactLocations);
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
            return null;
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
        public void GetPreviousHospitalizationContacts(ref ContactRoot rootContact, ContactParameter parameter)
        {
            int index = rootContact.Hospitalizations.IndexOf(rootContact.CurrentHospitalization);
            Hospitalization prevHospitalization = rootContact.Hospitalizations[index - 1];
            if (!rootContact.Contacts.ContainsKey(prevHospitalization))
            {
                PathogenParameter pathogenParameter = new PathogenParameter() { PathogenCodes = parameter.PathogenCodes };
                SortedList<Hospitalization, InfectionStatus> infectionStatus = _infectionStatusFac.Process(prevHospitalization, pathogenParameter, parameter.Resistence);
                List<SmICSCoreLib.Factories.PatientMovementNew.PatientStays.PatientStay> contactLocations = _patientStayFac.Process(prevHospitalization);
                rootContact.AddHospitalization(prevHospitalization, infectionStatus[prevHospitalization], null, null);
                MergeInfectionStatusAndContactCases(ref rootContact, prevHospitalization, pathogenParameter, parameter.Resistence);
            }
            rootContact.CurrentHospitalization = prevHospitalization;
        }

        public List<string> GetFilter(List<string> pathogenCodes)
        {
            List<string> filter = Rules.GetPossibleMREClasses(pathogenCodes);
            return filter;
        }

        private void MergeInfectionStatusAndContactCases(ref ContactRoot rootContact, Hospitalization hospitalization, PathogenParameter pathogenParameter, string resistence)
        {
            List<Contact> contacts = new List<Contact>();
            List< SmICSCoreLib.Factories.PatientMovementNew.PatientStays.PatientStay> contactLocations = _contactFac.Process(hospitalization);
            List<string> distinctPatientIDs = contactLocations.Select(cl => cl.PatientID).Distinct().ToList();
            Dictionary<string, SortedList<Hospitalization, InfectionStatus>> infectionStati = new Dictionary<string, SortedList<Hospitalization, InfectionStatus>>();
            distinctPatientIDs.ForEach(patID => infectionStati.Add(patID, _infectionStatusFac.Process(new SmICSCoreLib.Factories.General.Patient() { PatientID = patID }, pathogenParameter, resistence)));
            foreach (SmICSCoreLib.Factories.PatientMovementNew.PatientStays.PatientStay patLoc in contactLocations)
            {
                if (patLoc.PatientID != rootContact.Hospitalizations.Last().PatientID)
                {
                    SortedList<Hospitalization, InfectionStatus> infectionStatus = infectionStati[patLoc.PatientID];
                    SmICSCoreLib.Factories.PatientMovementNew.PatientStays.PatientStay rootStay = GetRootStay(rootContact.PatientStays[hospitalization], patLoc);
                    if(rootStay is not null)
                    {
                        Contact contact = new Contact
                        {
                            PatientID = patLoc.PatientID,
                            CaseID = patLoc.CaseID,
                            PatientLocation = patLoc,
                            ContactStart = rootStay.Admission >= patLoc.Admission ? rootStay.Admission : patLoc.Admission,
                            ContactEnd = GetContactEnd(rootStay.Discharge, patLoc.Discharge),
                            RoomContact = rootStay.Room != null && rootStay.Room == patLoc.Room ? true : false,
                            WardContact = rootStay.Ward != null && rootStay.Ward == patLoc.Ward ? true : false,
                            DepartementContact = rootStay.DepartementID != null && rootStay.DepartementID == patLoc.DepartementID ? true : false
                        };
                        contact.InfectionStatus = infectionStatus.Count > 0 ? infectionStatus.First().Value : null;
                        contact.StatusDate = contact.InfectionStatus is not null && contact.InfectionStatus.Known ? _hospitalizationFac.Process(patLoc).Admission.Date : null;
                        contacts.Add(contact);
                    }
                }
            }
            contacts = SortContacts(contacts);
            rootContact.Contacts[hospitalization] = contacts;
        }

        public List<string> GetFilter(ContactParameter parameter)
        {
            List<string> filter = Rules.GetPossibleMREClasses(parameter.PathogenCodes);
            return filter;
        }

        private SmICSCoreLib.Factories.PatientMovementNew.PatientStays.PatientStay GetRootStay(List<SmICSCoreLib.Factories.PatientMovementNew.PatientStays.PatientStay> patientStays, SmICSCoreLib.Factories.PatientMovementNew.PatientStays.PatientStay patientLocation)
        {
            foreach(PatientStay stay in patientStays)
            {
                if(patientLocation.MovementType == MovementType.PROCEDURE)
                {
                    if (stay.MovementType == MovementType.PROCEDURE)
                    {
                        if (stay.Admission.Date == patientLocation.Admission.Date && stay.DepartementID == patientLocation.DepartementID)
                        {
                            return stay;
                        }
                    }
                }
                else
                {
                    if (patientLocation.Discharge.HasValue && stay.Discharge.HasValue)
                    {
                        if (stay.Admission <= patientLocation.Discharge && stay.Discharge >= patientLocation.Admission && patientLocation.Ward == stay.Ward)
                        {
                            return stay;
                        }
                    }
                    else if (!patientLocation.Discharge.HasValue && stay.Discharge.HasValue)
                    {
                        if (stay.Discharge >= patientLocation.Admission && patientLocation.Ward == stay.Ward)
                        {
                            return stay;
                        }
                    }
                    else if (patientLocation.Discharge.HasValue && !stay.Discharge.HasValue)
                    {
                        if (stay.Admission <= patientLocation.Discharge && patientLocation.Ward == stay.Ward)
                        {
                            return stay;
                        }
                    }
                    else 
                    {
                        return stay;
                    }
                }
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

