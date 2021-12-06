using SmICSCoreLib.Factories.General;
using SmICSCoreLib.REST;
using System.Collections.Generic;

namespace SmICSCoreLib.Factories.PatientMovementNew
{
    public interface IHospitalizationFactory
    {
        IRestDataAccess RestDataAccess { get; set; }

        Hospitalization Process(Case Case);
        List<Hospitalization> Process(Patient patient);
    }
}