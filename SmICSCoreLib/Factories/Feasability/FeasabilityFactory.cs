using SmICSCoreLib.REST;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmICSCoreLib.Factories.Feasability
{
    public class FeasabilityFactory : IFeasabilityFactory
    {
        public IRestDataAccess RestDataAccess { get; set; }

        public FeasabilityFactory(IRestDataAccess restDataAccess)
        {
            RestDataAccess = restDataAccess;
        }

        public async Task<Feasability> GetPersonMovementCountAsync(SmICSCoreLib.Factories.General.Patient patient)
        {
            try
            {
                List<Feasability> feasability = await RestDataAccess.AQLQueryAsync<Feasability>(PersonMovementCount(patient));
                return feasability.First();
            }
            catch 
            {
                throw;
            }
        }

        //public Feasability GetPersonWardCount(SmICSCoreLib.Factories.General.Patient patient)
        //{
        //    return RestDataAccess.AQLQuery<Feasability>(PersonWardCount(patient)).First();
        //}

        private AQLQuery PersonMovementCount(SmICSCoreLib.Factories.General.Patient patient)
        {
            return new AQLQuery()
            {
                Name = "Anzahl Personenbewegungen",
                Query = $"SELECT COUNT(c/uid/value) as Count FROM EHR e CONTAINS COMPOSITION c WHERE c/name/value='Patientenaufenthalt' AND e/ehr_status/subject/external_ref/id/value = '{patient.PatientID}'"
            };
        }

        //private AQLQuery PersonWardCount(SmICSCoreLib.Factories.General.Patient patient)
        //{
        //    return new AQLQuery()
        //    {
        //        Name = "Anzahl Stationsbewegungen",
        //        Query = $"SELECT DISTINCT COUNT(z/items[at0027]/value) as Ward, " +
        //        $"COUNT(n / items[at0024, 'Fachabteilungsschlüssel'] / value) as Department " +
        //        $"FROM EHR e CONTAINS COMPOSITION c " +
        //        $"CONTAINS ADMIN_ENTRY g[openEHR - EHR - ADMIN_ENTRY.hospitalization.v0] " +
        //        $"CONTAINS(CLUSTER z[openEHR - EHR - CLUSTER.location.v1] and CLUSTER n[openEHR - EHR - CLUSTER.organization.v0]) " +
        //        $"WHERE c / name / value = 'Patientenaufenthalt' AND e / ehr_status / subject / external_ref / id / value = '{patient.PatientID}'"
        //    };
        //}
    }
}
