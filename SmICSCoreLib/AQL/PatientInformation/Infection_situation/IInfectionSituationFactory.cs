using System.Collections.Generic;
using SmICSCoreLib.StatistikDataModels;

namespace SmICSCoreLib.AQL.PatientInformation.Infection_situation
{
    public interface IInfectionSituationFactory
    {
        List<Patient> Process();
    }
}
