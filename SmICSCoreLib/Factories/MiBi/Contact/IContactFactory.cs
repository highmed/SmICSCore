using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.PatientMovementNew;
using SmICSCoreLib.REST;
using System.Collections.Generic;

namespace SmICSCoreLib.Factories.MiBi.Contact
{
    public interface IContactFactory
    {
        IRestDataAccess RestDataAccess { get; set; }

        Dictionary<Hospitalization, List<Case>> Process(Case parameter);
    }
}