using SmICSCoreLib.Factories.General;
using SmICSCoreLib.REST;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmICSCoreLib.Factories.PatientMovementNew.PatientStays
{
    public class PatientStayFactory : IPatientStayFactory
    {
        public IRestDataAccess RestDataAccess { get; set; }

        public PatientStayFactory(IRestDataAccess restDataAccess)
        {
            RestDataAccess = restDataAccess;
        }

        public async Task<List<PatientStay>> ProcessAsync(Case Case)
        {
            List<PatientStay> patientStays = await RestDataAccess.AQLQueryAsync<PatientStay>(PatientStay(Case));
            patientStays = MergeConsectiveStaysAndSetMovementType(patientStays);

            return patientStays;
        }

        public async Task<List<PatientStay>> ProcessAsync(WardParameter wardParameter)
        {
            List<PatientStay> patientStays = await RestDataAccess.AQLQueryAsync<PatientStay>(PatientStayByWard(wardParameter));

            if (patientStays is not null)
            {
                patientStays = patientStays.OrderBy(stay => stay.Admission).ToList();
                patientStays = MergeConsectiveStaysAndSetMovementType(patientStays);
                return patientStays;
            }
            return null;
        }

        private List<PatientStay> MergeConsectiveStaysAndSetMovementType(List<PatientStay> patientStays)
        {
            List<int> mergedIndices = new List<int>();
            for (int i = 0; i < patientStays.Count; i++)
            {
                if (!mergedIndices.Contains(i))
                {
                    patientStays[i].MovementType = SetMovementType(patientStays[i]);
                
                    for (int j = (i+1); j < patientStays.Count; j++)
                    {
                        if (patientStays[i].DepartementID == patientStays[j].DepartementID)
                        {
                            if (patientStays[i].Ward == patientStays[j].Ward)
                            {
                                if (patientStays[i].Room == patientStays[j].Room)
                                {
                                    if (patientStays[i].Discharge.HasValue)
                                    {
                                        if (patientStays[i].Discharge.Value == patientStays[j].Admission)
                                        {
                                            patientStays[i].Discharge = patientStays[j].Discharge;
                                            mergedIndices.Add(j);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            for (int i = (mergedIndices.Count - 1); i >= 0; i--)
            {
                patientStays.RemoveAt(mergedIndices[i]);
            }
            return patientStays;
        }

        private MovementType SetMovementType(PatientStay patientStay)
        {
            return patientStay.Discharge.HasValue && patientStay.Admission == patientStay.Discharge.Value ? MovementType.PROCEDURE : MovementType.TRANSFER;
        }

        private AQLQuery PatientStayByWard(WardParameter parameter)
        {
            string aqlAddition = $"AND a/items[at0027]/value/value = '{parameter.Ward}'";
            if (string.IsNullOrEmpty(parameter.Ward))
            {
                aqlAddition = $"AND o/items[at0024]/value/defining_code/code_string = '{parameter.DepartementID}'";
            }
 

            AQLQuery aql = new AQLQuery()
            {
                Name = "PatientsOnWard",
                Query = $@"SELECT e/ehr_status/subject/external_ref/id/value as PatientID,
                        i/items[at0001]/value/value as CaseID,
                        o/items[at0024]/value/value as Departement,
                        o/items[at0024]/value/defining_code/code_string as DepartementID,
                        a/items[at0027]/value/value as Ward, 
                        a/items[at0029]/value/value as Room,
                        u/data[at0001]/items[at0004]/value/value as Admission,
                        u/data[at0001]/items[at0005]/value/value as Discharge
                        FROM EHR e 
                        CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.event_summary.v0] 
                        CONTAINS (CLUSTER i[openEHR-EHR-CLUSTER.case_identification.v0] 
                        AND ADMIN_ENTRY u[openEHR-EHR-ADMIN_ENTRY.hospitalization.v0] 
                        CONTAINS (CLUSTER a[openEHR-EHR-CLUSTER.location.v1]
                        AND CLUSTER o[openEHR-EHR-CLUSTER.organization.v0]))
                        WHERE c/name/value = 'Patientenaufenthalt'
                        AND e/ehr_status/subject/external_ref/id/value='{parameter.PatientID}'
                        AND i/items[at0001]/value/value='{parameter.CaseID}'
                        AND i/items[at0001]/name/value='Zugehöriger Versorgungsfall (Kennung)'
                        {aqlAddition} 
                        AND u/data[at0001]/items[at0004]/value/value <= '{ parameter.End.AddDays(1.0).ToString("yyyy-MM-dd") }'
                        AND (u/data[at0001]/items[at0005]/value/value > '{ parameter.Start.ToString("yyyy-MM-dd") }'
                        OR NOT EXISTS u/data[at0001]/items[at0005])"
            };
            return aql;
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
                                AND i/items[at0001]/value/value = '{Case.CaseID}'
                                ORDER BY h/data[at0001]/items[at0004]/value/value ASC"
            };
        }
    }
}
