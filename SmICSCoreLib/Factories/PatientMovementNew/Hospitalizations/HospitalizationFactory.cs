﻿using SmICSCoreLib.Factories.General;
using SmICSCoreLib.REST;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmICSCoreLib.Factories.PatientMovementNew
{
    public class HospitalizationFactory : IHospitalizationFactory
    {
        public IRestDataAccess RestDataAccess { get; set; }
        public HospitalizationFactory(IRestDataAccess restDataAccess)
        {
            RestDataAccess = restDataAccess;
        }

        public List<Hospitalization> Process(Patient patient)
        {
            try
            {
                List<Hospitalization> hospitalizations = new List<Hospitalization>();
                List<Case> cases = RestDataAccess.AQLQuery<Case>(HospitalizationCasesQuery(patient));
                if (cases is not null)
                {
                    foreach (Case Case in cases)
                    {
                        Hospitalization hospitalization = Process(Case);
                        hospitalizations.Add(hospitalization);
                    }
                    return hospitalizations;
                }
                return null;
            }
            catch
            {
                throw;
            }
        }


        public Hospitalization Process(Case Case)
        {
            try
            {
                Admission admission = RestDataAccess.AQLQuery<Admission>(AdmissionQuery(Case)).FirstOrDefault();
                var dischargeResponse = RestDataAccess.AQLQuery<Discharge>(DischargeQuery(Case));
                Discharge discharge = dischargeResponse == null ? new Discharge() { Date = null } : dischargeResponse.FirstOrDefault();

                return new Hospitalization
                {
                    CaseID = Case.CaseID,
                    PatientID = Case.PatientID,
                    Admission = admission,
                    Discharge = discharge
                };
            }
            catch
            {
                throw;
            }
        }

        public List<HospStay> Process(DateTime admission, DateTime? discharge)
        {
            try
            {
                List<HospStay> cases = new List<HospStay>();
                List<HospStay> casesWithDischarge = RestDataAccess.AQLQuery<HospStay>(GetCasesForTimespanWithDischarge(admission, (discharge.HasValue ? discharge.Value : DateTime.Now)));
                List<HospStay> casesWithoutDischarge = RestDataAccess.AQLQuery<HospStay>(GetCasesForTimespanWithoutDischarge((discharge.HasValue ? discharge.Value : DateTime.Now)));
                if (casesWithDischarge is not null)
                {
                    cases.AddRange(casesWithDischarge);
                }
                if (casesWithoutDischarge is not null)
                {
                    cases.AddRange(casesWithoutDischarge);
                }
                if (cases.Count > 0)
                {
                    return cases;
                }
                return null;
            }
            catch
            {
                throw;
            }
            
        }

        private AQLQuery HospitalizationCasesQuery(Patient patient)
        {
            return new AQLQuery
            {
                Name = "Aufnahme - Stationärer Versorgungsfall",
                Query = $@"SELECT e/ehr_status/subject/external_ref/id/value as PatientID, 
                        c/context/other_context[at0001]/items[at0003]/value/value as CaseID 
                        FROM EHR e 
                        CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.fall.v1] 
                        CONTAINS ADMIN_ENTRY p[openEHR-EHR-ADMIN_ENTRY.admission.v0] 
                        WHERE c/name/value = 'Stationärer Versorgungsfall' 
                        AND e/ehr_status/subject/external_ref/id/value = '{ patient.PatientID }' 
                        ORDER BY p/data[at0001]/items[at0071]/value/value ASC"
            };
        }
        private AQLQuery AdmissionQuery(Case Case)
        {
            return new AQLQuery
            {
                Name = "Aufnahme - Stationärer Versorgungsfall",
                Query = $@"SELECT p/data[at0001]/items[at0071]/value/value as Date
                        FROM EHR e 
                        CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.fall.v1] 
                        CONTAINS ADMIN_ENTRY p[openEHR-EHR-ADMIN_ENTRY.admission.v0] 
                        WHERE c/name/value = 'Stationärer Versorgungsfall' 
                        AND e/ehr_status/subject/external_ref/id/value = '{ Case.PatientID }' 
                        AND c/context/other_context[at0001]/items[at0003]/value/value = '{ Case.CaseID }'"
            };
        }

        private AQLQuery DischargeQuery(Case Case)
        {
            return new AQLQuery
            {
                Name = "Entlassung - Stationärer Versorgungsfall",
                Query = $@"SELECT b/data[at0001]/items[at0011,'Datum/Uhrzeit der Entlassung']/value/value as Date 
                        FROM EHR e 
                        CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.fall.v1] 
                        CONTAINS ADMIN_ENTRY b[openEHR-EHR-ADMIN_ENTRY.discharge_summary.v0] 
                        WHERE c/name/value = 'Stationärer Versorgungsfall' 
                        AND e/ehr_status/subject/external_ref/id/value = '{ Case.PatientID }' 
                        AND c/context/other_context[at0001]/items[at0003]/value/value = '{ Case.CaseID }'"
            };
        }

        private AQLQuery GetCasesForTimespanWithDischarge(DateTime admission, DateTime discharge)
        {
            return new AQLQuery
            {
                Name = "Cases in Timespan - With Discharge",
                Query = $@"SELECT c/context/other_context[at0001]/items[at0003]/value/value as CaseID,
                        e/ehr_status/subject/external_ref/id/value as PatientID,
                        d/data[at0001]/items[at0071]/value/value as Admission,
                        w/data[at0001]/items[at0011]/value/value as Discharge
                        FROM EHR e 
                        CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.fall.v1] 
                        CONTAINS (ADMIN_ENTRY d[openEHR-EHR-ADMIN_ENTRY.admission.v0] 
                        AND ADMIN_ENTRY w[openEHR-EHR-ADMIN_ENTRY.discharge_summary.v0]) 
                        WHERE d/data[at0001]/items[at0071]/value/value <= '{ discharge.ToString("yyyy-MM-dd") }'
                        AND w/data[at0001]/items[at0011]/value/value >= '{ admission.ToString("yyyy-MM-dd") }'"
            };
        }

        private AQLQuery GetCasesForTimespanWithoutDischarge(DateTime discharge)
        {
            return new AQLQuery
            {
                Name = "Cases in Timespan - With Discharge",
                Query = $@"SELECT c/context/other_context[at0001]/items[at0003]/value/value as CaseID,
                        e/ehr_status/subject/external_ref/id/value as PatientID,
                        d/data[at0001]/items[at0071]/value/value as Admission
                        FROM EHR e 
                        CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.fall.v1] 
                        CONTAINS ADMIN_ENTRY d[openEHR-EHR-ADMIN_ENTRY.admission.v0] 
                        WHERE NOT EXISTS c/content[openEHR-EHR-ADMIN_ENTRY.discharge_summary.v0] 
                        AND d/data[at0001]/items[at0071]/value/value <= '{ discharge.ToString("yyyy-MM-dd") }'"
            };
        }
    }
}
