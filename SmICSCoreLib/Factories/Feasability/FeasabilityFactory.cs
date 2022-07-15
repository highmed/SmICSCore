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

        private AQLQuery PersonMovementCount(SmICSCoreLib.Factories.General.Patient patient)
        {
            return new AQLQuery()
            {
                Name = "Anzahl Personenbewegungen",
                Query = $"SELECT COUNT(c/uid/value) as Count FROM EHR e CONTAINS COMPOSITION c WHERE c/name/value='Patientenaufenthalt' AND e/ehr_status/subject/external_ref/id/value = '{patient.PatientID}'"
            };
        }
    }
}
