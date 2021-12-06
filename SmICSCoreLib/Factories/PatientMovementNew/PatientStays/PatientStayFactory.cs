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

        private AQLQuery PatientStay(Case Case)
        {
            return new AQLQuery
            {
                Name = "",
                Query = @$"SELECT e/ehr_status/subject/external_ref/id/value as PatientID,
                                i/items[at0001]/value/value as CaseID,
                                h/data[at0001]/items[at0004]/value/value as Admission,
                                h/data[at0001]/items[at0005]/value/value as Dischage,
                                h/data[at0001]/items[at0006]/value/value as Bewegungsart_l,
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
