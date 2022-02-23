using SmICSCoreLib.REST;

namespace SmICSCoreLib.Factories.Feasability
{
    public interface IFeasabilityFactory
    {
        IRestDataAccess RestDataAccess { get; set; }

        Feasability GetPersonMovementCount(SmICSCoreLib.Factories.General.Patient patient);
    }
}