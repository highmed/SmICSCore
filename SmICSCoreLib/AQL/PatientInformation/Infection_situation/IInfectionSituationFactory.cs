using System.Collections.Generic;
using SmICSCoreLib.AQL.General;
using SmICSCoreLib.StatistikDataModels;

namespace SmICSCoreLib.AQL.PatientInformation.Infection_situation
{
    public interface IInfectionSituationFactory
    {
        List<Patient> Process(PatientListParameter parameter);
    }
}
