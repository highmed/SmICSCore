using SmICSCoreLib.Factories.General;
using SmICSCoreLib.REST;
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


        public Hospitalization Process(Case Case)
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
    }
}
