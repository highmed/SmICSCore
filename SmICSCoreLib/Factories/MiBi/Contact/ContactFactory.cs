using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.PatientMovementNew;
using SmICSCoreLib.Factories.PatientMovementNew.PatientStays;
using SmICSCoreLib.REST;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmICSCoreLib.Factories.MiBi.Contact
{
    public class ContactFactory : IContactFactory
    {      
        public IRestDataAccess RestDataAccess { get; set; }
        private readonly IHospitalizationFactory _hospitalizationFac;
        private readonly IPatientStayFactory _patientStayFac;
        public ContactFactory(IRestDataAccess restDataAccess, IHospitalizationFactory hospitalizationFac, IPatientStayFactory patientStayFac)
        {
            RestDataAccess = restDataAccess;
            _hospitalizationFac = hospitalizationFac;
            _patientStayFac = patientStayFac;
        }

        public async Task<Dictionary<Hospitalization, List<PatientMovementNew.PatientStays.PatientStay>>> ProcessAsync(Patient parameter)
        {
            try
            {
                Dictionary<Hospitalization, List<PatientMovementNew.PatientStays.PatientStay>> contacts = new Dictionary<Hospitalization, List<PatientMovementNew.PatientStays.PatientStay>>();

                List<Hospitalization> Hospitalizations = await _hospitalizationFac.ProcessAsync(parameter);
                Hospitalizations.ForEach(h => contacts.Add(h, null));

                Hospitalization hospitalization = Hospitalizations.Last();

                List<PatientMovementNew.PatientStays.PatientStay> patientStays = await _patientStayFac.ProcessAsync(hospitalization);
                List<PatientMovementNew.PatientStays.PatientStay> contactCases = await DetermineContacts(Hospitalizations.Last());

                if (contacts[hospitalization] == null)
                {
                    contacts[hospitalization] = contactCases;
                }

                return contacts;
            }
            catch
            {
                throw;
            }
           
        }

        public async Task<List<PatientMovementNew.PatientStays.PatientStay>> ProcessAsync(Hospitalization hospitalization)
        {
            try
            {
                return await DetermineContacts(hospitalization);
            }
            catch
            {
                throw;
            }
        }

        private async Task<List<PatientMovementNew.PatientStays.PatientStay>> DetermineContacts(Hospitalization hospitalization)
        {
            List<PatientMovementNew.PatientStays.PatientStay> patientStays = await _patientStayFac.ProcessAsync(hospitalization);
            RemoveDoubleStays(patientStays);
            List<HospStay> possibleContactHosp = await _hospitalizationFac.ProcessAsync(hospitalization.Admission.Date, hospitalization.Discharge.Date);
            List<PatientMovementNew.PatientStays.PatientStay> cases = new List<PatientMovementNew.PatientStays.PatientStay>();
            if (possibleContactHosp is not null)
            {
                foreach (HospStay _case in possibleContactHosp)
                {
                    List<HospitalizationWard> wards = await RestDataAccess.AQLQueryAsync<HospitalizationWard>(GetsAllWardsFromHospitalization(_case));
                    if (wards is not null)
                    {
                        foreach (PatientMovementNew.PatientStays.PatientStay patientStay in patientStays)
                        {

                            if (((_case.Discharge.HasValue && _case.Discharge.Value >= patientStay.Admission) ||
                                (patientStay.Discharge.HasValue && _case.Admission <= patientStay.Discharge.Value)))
                            {
                                bool hasPotentialLocationContact = false;
                                if (string.IsNullOrEmpty(patientStay.Ward))
                                {
                                    hasPotentialLocationContact = wards.Where(w => string.IsNullOrEmpty(w.Ward) && w.DepartementID == patientStay.DepartementID).Count() > 0;
                                }
                                else
                                {
                                    hasPotentialLocationContact = wards.Where(w => w.Ward == patientStay.Ward).Count() > 0;
                                }
                                if (hasPotentialLocationContact)
                                {
                                    WardParameter wardParameter = new WardParameter
                                    {
                                        Ward = patientStay.Ward,
                                        DepartementID = patientStay.DepartementID,
                                        Start = patientStay.Admission,
                                        End = patientStay.Discharge.Value,
                                        PatientID = _case.PatientID,
                                        CaseID = _case.CaseID
                                    };


                                    List<PatientMovementNew.PatientStays.PatientStay> casesOnWard = await _patientStayFac.ProcessAsync(wardParameter);
                                    if (casesOnWard is not null)
                                    {
                                        cases.AddRange(casesOnWard);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            RemoveDoubleStays(cases);
            return cases;
        }

        private void RemoveDoubleStays(List<PatientMovementNew.PatientStays.PatientStay> patientStays)
        {

            List<int> indices = new List<int>();
            for(int i = 0; i < (patientStays.Count-2) ; i++)
            {
                for (int j = (i + 1); j < (patientStays.Count - 1); j++)
                {
                    if (patientStays[i].Equals(patientStays[j]) ||
                        (patientStays[i].MovementType == MovementType.PROCEDURE &&
                        patientStays[j].MovementType == MovementType.PROCEDURE &&
                        patientStays[i].DepartementID == patientStays[j].DepartementID &&
                        patientStays[i].Admission.Date == patientStays[j].Admission.Date))
                    {
                        if (!indices.Contains(j))
                        {
                            indices.Add(j);
                        }
                    }
                }
            }
            indices = indices.OrderBy(i => i).ToList();
            for(int i = (indices.Count - 1); i >= 0; i--)
            {
                patientStays.RemoveAt(indices[i]);
            }
        }

        private AQLQuery GetsAllWardsFromHospitalization(HospStay hospStay)
        {
            return new AQLQuery()
            {
                Name = "Wards For Hospitalization",
                Query = @$"SELECT DISTINCT a/items[at0027]/value/value as Ward, 
                        o/items[at0024]/value/defining_code/code_string as DepartementID
                        FROM EHR e
                        CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.event_summary.v0] 
                        CONTAINS (CLUSTER i[openEHR-EHR-CLUSTER.case_identification.v0] 
                        AND ADMIN_ENTRY u[openEHR-EHR-ADMIN_ENTRY.hospitalization.v0] 
                        CONTAINS (CLUSTER a[openEHR-EHR-CLUSTER.location.v1]
                        AND CLUSTER o[openEHR-EHR-CLUSTER.organization.v0]))
                        WHERE e/ehr_status/subject/external_ref/id/value = '{hospStay.PatientID}'
                        AND i/items[at0001]/value/value='{hospStay.CaseID}'"
            };
        }
    }
}
