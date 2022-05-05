using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.PatientMovementNew;
using SmICSCoreLib.REST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmICSCoreLib.Factories.Helpers
{
    public class HelperFactory : IHelperFactory
    {
        public IRestDataAccess RestDataAccess { get; set; }

        public HelperFactory(IRestDataAccess restDataAccess)
        {
            RestDataAccess = restDataAccess;
        }

        public List<Case> GetPatientOnWardsFromFiltered(List<HospStay> cases, string ward)
        {
            List<Case> casesOnWard = RestDataAccess.AQLQuery<Case>(GetsAllWardsFromHospitalization(cases, ward));
            if (casesOnWard is not null)
            {
                return casesOnWard;
            }
            return null;
        }

        private AQLQuery GetsAllWardsFromHospitalization(List<HospStay> cases, string ward)
        {
            string matchString = "{'" + string.Join("','", cases.Select(c => c.PatientID)) + "'}";
            return new AQLQuery()
            {
                Name = "Wards For Hospitalization",
                Query = @$"SELECT DISTINCT e/ehr_status/subject/external_ref/id/value as PatientID,
                        i/items[at0001]/value/value as CaseID
                        FROM EHR e
                        CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.event_summary.v0] 
                        CONTAINS (CLUSTER i[openEHR-EHR-CLUSTER.case_identification.v0] 
                        AND ADMIN_ENTRY u[openEHR-EHR-ADMIN_ENTRY.hospitalization.v0] 
                        CONTAINS CLUSTER a[openEHR-EHR-CLUSTER.location.v1])
                        WHERE e/ehr_status/subject/external_ref/id/value MATCHES {matchString}
                        AND i/items[at0001]/name/value = 'Zugehöriger Versorgungsfall (Kennung)' 
                        AND a/items[at0027]/value/value='{ward}'"
            };
        }
    }
}
