using SmICSCoreLib.Factories.Feasability;
using SmICSCoreLib.Factories.PatientMovementNew;
using SmICSCoreLib.Factories.PatientMovementNew.PatientStays;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmICSWebApp.Data.ContactComparison
{
    public class ComparisonService
    {
        private readonly IFeasabilityFactory _feasabilityFac;
        private readonly IPatientStayFactory _patStayFac;
        private readonly IHospitalizationFactory _hospFac;

        public ComparisonService(IFeasabilityFactory feasabilityFac, IPatientStayFactory patStayFac, IHospitalizationFactory hospFac)
        {
            _feasabilityFac = feasabilityFac;
            _patStayFac = patStayFac;
            _hospFac = hospFac;
        }

        public async Task<List<ComparedContact>> Compare(List<SmICSCoreLib.Factories.General.Patient> patients)
        {
            try
            {
                List<KeyValuePair<SmICSCoreLib.Factories.General.Patient, List<PatientStay>>> patientStayDict = new List<KeyValuePair<SmICSCoreLib.Factories.General.Patient, List<PatientStay>>>();
                foreach (SmICSCoreLib.Factories.General.Patient pat in patients)
                {
                    List<PatientStay> patientStays = new List<PatientStay>();
                    List<Hospitalization> hospitalizations = await _hospFac.ProcessAsync(pat);
                    if (hospitalizations is not null)
                    {
                        foreach (Hospitalization hosp in hospitalizations)
                        {
                            List<PatientStay> patStays = await _patStayFac.ProcessAsync(hosp);
                            patientStays.AddRange(patStays);
                        }
                        patientStayDict.Add(new KeyValuePair<SmICSCoreLib.Factories.General.Patient, List<PatientStay>>(pat, patientStays));
                    }
                }
                List<ComparedContact> contacts = new List<ComparedContact>();
                for (int i = 0; i < patientStayDict.Count - 1; i++)
                {
                    List<PatientStay> first = patientStayDict[i].Value;
                    for (int j = (i + 1); j < patientStayDict.Count; j++)
                    {
                        List<PatientStay> second = patientStayDict[j].Value;
                        List<ContactPoint> contactLocations = FindContact(first, second);
                        if (contactLocations.Count > 0)
                        {
                            ComparedContact contact = new ComparedContact
                            {
                                PatientA = first[0].PatientID,
                                PatientB = second[0].PatientID,
                                ContactLocations = contactLocations
                            };
                            contacts.Add(contact);
                        }
                    }
                }
                return contacts;
            }
            catch
            {
                throw;
            }
            
        }

        public async Task<bool> ExistsPatient(SmICSCoreLib.Factories.General.Patient Patient)
        {
            try
            {
                Feasability feas = await _feasabilityFac.GetPersonMovementCountAsync(Patient);
                if (feas.Count > 0)
                {
                    return true;
                }
                return false;
            }
            catch
            {
                throw;
            }
            
        }

        private List<ContactPoint> FindContact(List<PatientStay> First, List<PatientStay> Second)
        {
            List<ContactPoint> contact = new List<ContactPoint>();
            for (int i = 0; i < First.Count; i++)
            {
                for (int j = 0; j < Second.Count; j++)
                {
                    ContactPoint cp = GetContact(First[i], Second[j]);
                    if (cp is not null)
                    {
                        contact.Add(cp);
                    }
                }
            }
            return contact;
        }

        private ContactPoint GetContact(PatientStay First, PatientStay Second)
        {
                  
            ContactLevel lvl = GetContactLevel(First, Second);
            DateTime contactStart = First.Admission >= Second.Admission ? First.Admission : Second.Admission;
            DateTime? contactEnd = null;
            if (First.Discharge.HasValue && Second.Discharge.HasValue)
            {
                if (First.Admission <= Second.Discharge.Value && Second.Admission <= First.Discharge.Value)
                {
                    contactEnd = First.Discharge.Value <= Second.Discharge.Value ? First.Discharge.Value : Second.Discharge.Value;
                    return GetContactPoint(First, contactStart, contactEnd, lvl);
                }
                return null;
            }
            else if (!First.Discharge.HasValue && Second.Discharge.HasValue)
            {
                if (First.Admission <= Second.Discharge.Value)
                {
                    contactEnd = Second.Discharge.Value;
                    return GetContactPoint(First, contactStart, contactEnd, lvl);
                }
                return null;
            }
            else if (First.Discharge.HasValue && !Second.Discharge.HasValue)
            {
                if (Second.Admission <= First.Discharge.Value)
                {
                    contactEnd = First.Discharge.Value;
                    return GetContactPoint(First, contactStart, contactEnd, lvl);
                }
                return null;
            }
            return GetContactPoint(First, contactStart, contactEnd, lvl);
        }

        private ContactPoint GetContactPoint(PatientStay stay, DateTime contactStart, DateTime? contactEnd, ContactLevel level)
        {
            if (level == ContactLevel.DEPARTEMENT || level == ContactLevel.WARD || level == ContactLevel.ROOM)
            {
                ContactPoint cp = new ContactPoint
                {
               
                    Departement = stay.Departement,
                    Start = contactStart,
                    End = contactEnd
                };
                if(level == ContactLevel.WARD || level == ContactLevel.ROOM)
                {
                    cp.Ward = stay.Ward;
                    if (level == ContactLevel.ROOM)
                    {
                        cp.Room = stay.Room;
                    }
                }
                return cp;
            }
            return null;
        }
        private ContactLevel GetContactLevel(PatientStay first, PatientStay second)
        {
            if (first.MovementType == second.MovementType)
            {
                if (first.DepartementID == second.DepartementID)
                {
                    if (first.Ward == second.Ward)
                    {
                        if (first.Room == second.Room)
                        {
                            return ContactLevel.ROOM;
                        }
                        return ContactLevel.WARD;
                    }
                    return ContactLevel.DEPARTEMENT;

                }
            }
            return ContactLevel.NON;
        }
    }
}
