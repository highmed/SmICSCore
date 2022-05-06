using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.PatientMovementNew;
using SmICSCoreLib.REST;
using System.Collections.Generic;

namespace SmICSCoreLib.Factories.Helpers
{
    public interface IHelperFactory
    {
        IRestDataAccess RestDataAccess { get; set; }

        List<Case> GetPatientOnWardsFromFiltered(List<HospStay> cases, string ward);
    }
}