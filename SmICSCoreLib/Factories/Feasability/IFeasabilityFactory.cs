using SmICSCoreLib.REST;
using System.Threading.Tasks;

namespace SmICSCoreLib.Factories.Feasability
{
    public interface IFeasabilityFactory
    {
        IRestDataAccess RestDataAccess { get; set; }

        Task<Feasability> GetPersonMovementCountAsync(SmICSCoreLib.Factories.General.Patient patient);
    }
}