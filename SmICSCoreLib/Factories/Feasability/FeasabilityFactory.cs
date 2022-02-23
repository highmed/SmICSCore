using SmICSCoreLib.REST;
using System.Linq;


namespace SmICSCoreLib.Factories.Feasability
{
    public class FeasabilityFactory : IFeasabilityFactory
    {
        public IRestDataAccess RestDataAccess { get; set; }

        public FeasabilityFactory(IRestDataAccess restDataAccess)
        {
            RestDataAccess = restDataAccess;
        }

        public Feasability GetPersonMovementCount(SmICSCoreLib.Factories.General.Patient patient)
        {
            return RestDataAccess.AQLQuery<Feasability>(PersonMovementCount(patient)).First();
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
