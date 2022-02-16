using SmICSCoreLib.Factories.General;
using SmICSCoreLib.REST;
using System.Collections.Generic;

namespace SmICSCoreLib.Factories.PatientMovementNew.PatientStays
{
    public class PatientStayFactory : IPatientStayFactory
    {
        public IRestDataAccess RestDataAccess { get; set; }

        public PatientStayFactory(IRestDataAccess restDataAccess)
        {
            RestDataAccess = restDataAccess;
        }

        public List<PatientStay> Process(Case Case)
        {
            List<PatientStay> patientStays = RestDataAccess.AQLQuery<PatientStay>(PatientStay(Case));
            foreach (PatientStay patientStay in patientStays)
            {
                patientStay.MovementType = patientStay.Admission == patientStay.Discharge ? MovementType.PROCEDURE : MovementType.TRANSFER;
            }
            return patientStays;
        }

        public List<PatientStay> Process(WardParameter wardParameter)
        {
            List<PatientStay> patStays = RestDataAccess.AQLQuery<PatientStay>(PatientStayByWard(wardParameter));
            return patStays;
        }

        private AQLQuery PatientStayByWard(WardParameter parameter)
        {
            return new AQLQuery()
            {
                Name = "PatientsOnWard",
                Query = $@"SELECT e/ehr_status/subject/external_ref/id/value as PatientID,
                        i/items[at0001]/value/value as CaseID,
                        u/data[at0001]/items[openEHR-EHR-CLUSTER.location.v1]/items[at0027]/value/value as Ward, 
                        u/data[at0001]/items[openEHR-EHR-CLUSTER.location.v1]/items[at0029]/value/value as Room,
                        u/data[at0001]/items[at0004]/value/value as Admission,
                        u/data[at0001]/items[at0005]/value/value as Discharge
                        FROM EHR e 
                        CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.event_summary.v0] 
                        CONTAINS (CLUSTER i[openEHR-EHR-CLUSTER.case_identification.v0] 
                        AND ADMIN_ENTRY u[openEHR-EHR-ADMIN_ENTRY.hospitalization.v0] 
                        CONTAINS (CLUSTER a[openEHR-EHR-CLUSTER.location.v1]
                        AND CLUSTER o[openEHR-EHR-CLUSTER.organization.v0]))
                        WHERE c/name/value = 'Patientenaufenthalt'
                        AND i/items[at0001]/name/value='Zugehöriger Versorgungsfall (Kennung)'
                        AND a/items[at0027]/value/value = '{parameter.Ward}' 
                        AND u/data[at0001]/items[at0005]/value/value > '{ parameter.Start.ToString("yyyy-MM-dd") }'
                        AND (u/data[at0001]/items[at0004]/value/value < '{ parameter.End.AddDays(1.0).ToString("yyyy-MM-dd") }'
                        OR NOT EXISTS u/data[at0001]/items[at0005]) 
                        ORDER BY u/data[at0001]/items[at0004]/value/value ASC"
            };
        }

        private AQLQuery PatientStay(Case Case)
        {
            return new AQLQuery
            {
                Name = "",
                Query = @$"SELECT e/ehr_status/subject/external_ref/id/value as PatientID,
                                i/items[at0001]/value/value as CaseID,
                                h/data[at0001]/items[at0004]/value/value as Admission,
                                h/data[at0001]/items[at0005]/value/value as Discharge,
                                h/data[at0001]/items[at0006]/value/value as StayingReason,
                                s/items[at0027]/value/value as Ward,
                                s/items[at0029]/value/value as Room,
                                f/items[at0024]/value/value as Departement,
                                f/items[at0024]/value/defining_code/code_string as DepartementID
                                FROM EHR e 
                                CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.event_summary.v0] 
                                CONTAINS (CLUSTER i[openEHR-EHR-CLUSTER.case_identification.v0]
                                AND ADMIN_ENTRY h[openEHR-EHR-ADMIN_ENTRY.hospitalization.v0]
                                CONTAINS (CLUSTER s[openEHR-EHR-CLUSTER.location.v1]
                                AND CLUSTER f[openEHR-EHR-CLUSTER.organization.v0]))
                                WHERE c/name/value = 'Patientenaufenthalt'
                                AND i/items[at0001]/name/value = 'Zugehöriger Versorgungsfall (Kennung)'
                                AND e/ehr_status/subject/external_ref/id/value = '{Case.PatientID}'
                                AND  i/items[at0001]/value/value = '{Case.CaseID}'
                                ORDER BY h/data[at0001]/items[at0004]/value/value ASC"
            };
        }
    }
}
