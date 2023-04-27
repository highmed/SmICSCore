using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.PatientMovementNew;
using SmICSCoreLib.Factories.PatientMovementNew.PatientStays;
using SmICSCoreLib.Models.General;
using SmICSCoreLib.REST;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SmICSCoreLib.Factories.ContactNew
{    
    public class ContactFactory2 : IContactFactory2
    {
        public IRestDataAccess RestDataAccess { get; set; }
        private readonly IHospitalizationFactory _hospitalizationFac;
        private readonly IPatientStayFactory _patientStayFac;
        public ContactFactory2(IRestDataAccess restDataAccess, IHospitalizationFactory hospitalizationFac, IPatientStayFactory patientStayFac)
        {
            RestDataAccess = restDataAccess;
            _hospitalizationFac = hospitalizationFac;
            _patientStayFac = patientStayFac;
        }

        public async IAsyncEnumerable<ContactPatient> TestProcessAsync(Patient parameter)
        {
            List<Hospitalization> Hospitalizations = await _hospitalizationFac.ProcessAsync(parameter);
            Hospitalization hospitalization = Hospitalizations[0];
            LocationCategoryDict RootPatientStays = await GetSortedPatientStays(hospitalization);
            List<HospStay> PossibleContactPatients = await _hospitalizationFac.ProcessAsync(hospitalization.Admission.Date, hospitalization.Discharge.Date);
            IAsyncEnumerable<KeyValuePair<Patient, LocationCategoryDict>> patientStayDict = GetAllPossibleContactPatientStays(PossibleContactPatients);
            IAsyncEnumerable<ContactPatient> contacts = GetRealContactPatientStays(patientStayDict, RootPatientStays);
            await foreach(ContactPatient cp in contacts)
            {
                yield return cp;
            }
        }

        private async IAsyncEnumerable<KeyValuePair<Patient, LocationCategoryDict>> GetAllPossibleContactPatientStays(List<HospStay> PossibleContactPatients)
        {
            PossibleContactPatients = PossibleContactPatients.OrderBy(p => p.PatientID).ToList();
            for (int i = 0; i < PossibleContactPatients.Count; i++)
            {
                LocationCategoryDict locCatDict = await GetSortedPatientStays(PossibleContactPatients[i]);
                int next = i + 1;
                while (PossibleContactPatients[next].PatientID == PossibleContactPatients[i].PatientID)
                {
                    LocationCategoryDict locCatDict_next = await GetSortedPatientStays(PossibleContactPatients[i]);
                    locCatDict.Merge(locCatDict_next);
                    next++;
                }
                yield return new KeyValuePair<Patient, LocationCategoryDict>(PossibleContactPatients[i], locCatDict);
            }
        }
         
        private async IAsyncEnumerable<ContactInformation> GetRealContactPatientStays(IAsyncEnumerable<KeyValuePair<Patient, LocationCategoryDict>> patientStayDict, LocationCategoryDict rootPatientStays)
        {
            await foreach (KeyValuePair<Patient, LocationCategoryDict> patient in patientStayDict)
            {
                foreach (KeyValuePair<LocationCategory, LocationDict> location in patient.Value)
                {
                    if (rootPatientStays.ContainsKey(location.Key))
                    {
                        foreach (KeyValuePair<Location, List<PatientStay>> stayInformation in location.Value)
                        {
                            if (rootPatientStays[location.Key].ContainsKey(stayInformation.Key))
                            {

                                ContactPatient cPatient = CompareContactPoints(rootPatientStays[location.Key][stayInformation.Key], stayInformation.Value, location.Key, stayInformation.Key);
                                if (cPatient is not null)
                                {
                                    yield return cInfo;
                                }
                            }
                        }
                    }
                }
            }
        }

        private ContactPatient CompareContactPoints(List<PatientStay> rootStays, List<PatientStay> possibleContactStays, LocationCategory locationCategory, Location location)
        {
            ContactPatient contactPatient = null;
            foreach (PatientStay rootStay in rootStays)
            {
                foreach (PatientStay possibleContact in possibleContactStays)
                {
                    if (HasTimeOverlap(rootStay, possibleContact))
                    {
                        if (contactPatient is null)
                        {
                            contactPatient = new ContactPatient();
                        }
                        ContactInformation contactInformation = new ContactInformation { PatientStayMetaData = possibleContact};
                        contactInformation.ContactLocation.SetFromLocation(locationCategory, location);
                        contactInformation.ContactLocation.SetRoom(rootStay, possibleContact);
                        contactInformation.ContactTime.DetermineContactTime(rootStay, possibleContact);
                        contactPatient.Contacts.Add(contactInformation);
                    }
                }
            }
            return contactPatient;
        }

        private bool HasTimeOverlap(PatientStay rootStay, PatientStay possibleContact)
        {
            if (rootStay.Discharge.HasValue && possibleContact.Discharge.HasValue)
            {
                if (rootStay.Admission.Date <= possibleContact.Discharge.Value.Date && rootStay.Discharge.Value.Date >= possibleContact.Admission.Date)
                {
                    return true;
                }
                return false;
            }
            else if (rootStay.Discharge.HasValue && possibleContact.Admission.Date > rootStay.Discharge.Value.Date)
            {
                return false;
            }
            else if (possibleContact.Discharge.HasValue && rootStay.Admission.Date > possibleContact.Discharge.Value.Date)
            {
                return false;
            }
            return true;
        }

        private async Task<LocationCategoryDict> GetSortedPatientStays(Case c)
        {
            List<PatientStay> stays = await _patientStayFac.ProcessAsync(c);
            LocationCategoryDict locationCategoryDict = SortStaysToWards(stays);
            return locationCategoryDict;
        }

        private LocationCategoryDict SortStaysToWards(List<PatientStay> stays)
        {
            LocationCategoryDict SortedByLocationCategory = new LocationCategoryDict
            {
                { LocationCategory.WARD, new LocationDict() },
                { LocationCategory.DEPARTEMENT, new LocationDict() },
            };

            foreach (PatientStay stay in stays)
            {
                LocationCategory locationCategory = LocationCategory.WARD;
                Location loc = null;
                if (string.IsNullOrEmpty(stay.Ward))
                {
                    locationCategory = LocationCategory.DEPARTEMENT;
                    loc = new Departement { ID = stay.DepartementID, Name = stay.Departement };
                }
                else
                {
                    loc = new Ward { Name = stay.Ward };
                }
                AddLocationInformation(SortedByLocationCategory[locationCategory], loc, stay);


            }
            return SortedByLocationCategory;
        }

        private void AddLocationInformation(LocationDict locationDict, Location location, PatientStay stay)
        {
            if (locationDict.Keys.Contains(location))
            {
                locationDict[location].Add(stay);
            }
            else
            {
                locationDict.Add(location, new List<PatientStay> { stay });
            }
        }

        private class PatientStayDict : Dictionary<Patient, LocationCategoryDict> { }
        private class LocationDict : Dictionary<Location, List<PatientStay>>
        {
            internal void Merge(LocationDict locationDict)
            {
                foreach (KeyValuePair<Location, List<PatientStay>> location in locationDict)
                {
                    this[location.Key].AddRange(location.Value);
                }
            }
        }
        private class LocationCategoryDict : Dictionary<LocationCategory, LocationDict>
        {
            internal void Merge(LocationCategoryDict locationCategoryDict)
            {
                foreach(KeyValuePair<LocationCategory, LocationDict> locations in locationCategoryDict) 
                {
                    this[locations.Key].Merge(locations.Value);
                }
            }
        }
    }
}
