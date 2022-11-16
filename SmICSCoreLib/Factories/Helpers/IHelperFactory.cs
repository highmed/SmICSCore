using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.PatientMovementNew;
using SmICSCoreLib.REST;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmICSCoreLib.Factories.Helpers
{
    public interface IHelperFactory
    {
        IRestDataAccess RestDataAccess { get; set; }

        Task<List<Case>> GetPatientOnWardsFromFilteredAsync(List<HospStay> cases, string ward);
    }
}