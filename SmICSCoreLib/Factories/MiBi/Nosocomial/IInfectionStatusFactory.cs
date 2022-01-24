using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.MiBi.PatientView.Parameter;
using SmICSCoreLib.Factories.PatientMovementNew;
using SmICSCoreLib.REST;
using System.Collections.Generic;

namespace SmICSCoreLib.Factories.MiBi.Nosocomial
{
    public interface IInfectionStatusFactory
    {
        IRestDataAccess RestDataAccess { get; set; }
        SortedList<Hospitalization, Dictionary<string, Dictionary<string, InfectionStatus>>> Process(Patient patient, string MedicalField);
        SortedList<Hospitalization, Dictionary<string, InfectionStatus>> Process(Patient patient, PathogenParameter pathogen);
        SortedList<Hospitalization, InfectionStatus> Process(Patient patient, PathogenParameter pathogen, string Resistence);
    }
}