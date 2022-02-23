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

        public List<ComparedContact> Compare(List<SmICSCoreLib.Factories.General.Patient> patients)
        {
            List<KeyValuePair<SmICSCoreLib.Factories.General.Patient, List<PatientStay>>> patientStayDict = new List<KeyValuePair<SmICSCoreLib.Factories.General.Patient, List<PatientStay>>>();
            foreach(SmICSCoreLib.Factories.General.Patient pat in patients)
            {
                List<PatientStay> patientStays = new List<PatientStay>();
                List<Hospitalization> hospitalizations = _hospFac.Process(pat);
                foreach(Hospitalization hosp in hospitalizations)
                {
                    List<PatientStay> patStays = _patStayFac.Process(hosp);
                    patientStays.AddRange(patStays);
                }
                patientStayDict.Add(new KeyValuePair<SmICSCoreLib.Factories.General.Patient, List<PatientStay>>(pat, patientStays));
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

        public bool ExistsPatient(SmICSCoreLib.Factories.General.Patient Patient)
        {
            Feasability feas = _feasabilityFac.GetPersonMovementCount(Patient);
            if(feas.Count > 0)
            {
                return true;
            }
            return false;
        }

        private List<ContactPoint> FindContact(List<PatientStay> First, List<PatientStay> Second)
        {
            List<ContactPoint> contact = new List<ContactPoint>();
            for (int i = 0; i < First.Count; i++)
            {
                for (int j = 0; j < Second.Count; j++)
                {
                    if (IsContact(First[i], Second[j], ContactType.INDIRECT, ContactLevel.WARD))
                    {
                        ContactPoint cp = new ContactPoint
                        {
                            Ward = First[i].Ward,
                            Room = First[i].Room,
                            Departement = First[i].DepartementID == Second[j].DepartementID ? First[i].DepartementID : null,
                            Start = First[i].Admission > Second[j].Admission ? First[i].Admission : Second[j].Admission,
                            End = GetContactEnd(First[i].Discharge, Second[j].Discharge)
                        };
                        contact.Add(cp);
                    }
                }
            }
            return contact;
        }

        private bool IsContact(PatientStay First, PatientStay Second, ContactType type, ContactLevel level)
        {
            if (First.Ward == Second.Ward)
            {
                if (level == ContactLevel.ROOM && First.Room == Second.Room)
                {
                    if (type == ContactType.DIRECT)
                    {
                        return IsDirect(First, Second);
                    }
                    else if (type == ContactType.INDIRECT)
                    {
                        return IsIndirect(First, Second);
                    }
                }
                else
                {
                    if (type == ContactType.DIRECT)
                    {
                        return IsDirect(First, Second);
                    }
                    else if (type == ContactType.INDIRECT)
                    {
                        return IsIndirect(First, Second);
                    }
                }
            }
            else if (First.DepartementID == Second.DepartementID)
            {
                return IsIndirect(First, Second);
            }
            return false;
        }

        private bool IsDirect(PatientStay First, PatientStay Second)
        {
            if (First.Discharge.HasValue && First.Discharge.Value >= Second.Admission)
            {
                if (Second.Discharge.HasValue && First.Admission <= Second.Discharge.Value)
                {
                    return true;
                }
                else if (!Second.Discharge.HasValue)
                {
                    return true;
                }
            }
            else if (!First.Discharge.HasValue)
            {
                if (Second.Discharge.HasValue && First.Admission <= Second.Discharge.Value)
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsIndirect(PatientStay First, PatientStay Second)
        {
            {
                if (First.Discharge.HasValue && First.Discharge.Value.Date >= Second.Admission.Date)
                {
                    if (Second.Discharge.HasValue && First.Admission.Date <= Second.Discharge.Value.Date)
                    {
                        return true;
                    }
                    else if (!Second.Discharge.HasValue)
                    {
                        return true;
                    }
                }
                else if (!First.Discharge.HasValue)
                {
                    if (Second.Discharge.HasValue && First.Admission.Date <= Second.Discharge.Value.Date)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        private DateTime? GetContactEnd(DateTime? First, DateTime? Second)
        {
            if(First.HasValue && Second.HasValue)
            {
                return First < Second ? First : Second;
            }
            else if (!First.HasValue)
            {
                return Second;
            }
            else if (!Second.HasValue)
            {
                return First;
            }
            return null;
        }
    }
}
