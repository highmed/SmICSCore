using SmICSCoreLib.Factories.General;
using SmICSCoreLib.REST;
using System.Collections.Generic;

namespace SmICSCoreLib.Factories.Vaccination
{
    public interface IVaccinationFactory
    {
        IRestDataAccess RestDataAccess { get; }
        List<VaccinationModel> Process(PatientListParameter parameter);
        List<VaccinationModel> ProcessSpecificVaccination(PatientListParameter parameter, string vaccination);
    }
}