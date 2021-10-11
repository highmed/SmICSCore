using System.Collections.Generic;
using SmICSCoreLib.Factories.General;
using SmICSCoreLib.StatistikDataModels;

namespace SmICSCoreLib.Factories.InfectionSituation
{
    public interface IInfectionSituationFactory
    {
        List<Patient> Process(PatientListParameter parameter);
    }
}
