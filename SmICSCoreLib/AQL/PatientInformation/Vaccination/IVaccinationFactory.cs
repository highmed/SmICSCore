using SmICSCoreLib.AQL.General;
using System.Collections.Generic;

namespace SmICSCoreLib.AQL.PatientInformation.Vaccination
{
    public interface IVaccinationFactory
    {
        List<VaccinationModel> Process(PatientListParameter parameter);
    }
}