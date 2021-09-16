using SmICSCoreLib.Factories.General;
using System.Collections.Generic;

namespace SmICSCoreLib.Factories.Vaccination
{
    public interface IVaccinationFactory
    {
        List<VaccinationModel> Process(PatientListParameter parameter);
        List<VaccinationModel> ProcessSpecificVaccination(PatientListParameter parameter, string vaccination);
    }
}