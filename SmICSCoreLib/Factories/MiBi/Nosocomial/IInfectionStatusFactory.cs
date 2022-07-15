using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.MiBi.PatientView.Parameter;
using SmICSCoreLib.Factories.PatientMovementNew;
using SmICSCoreLib.REST;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmICSCoreLib.Factories.MiBi.Nosocomial
{
    public interface IInfectionStatusFactory
    {
        IRestDataAccess RestDataAccess { get; set; }
        Task<SortedList<Hospitalization, Dictionary<string, Dictionary<string, InfectionStatus>>>> ProcessAsync(Patient patient, string MedicalField);
        Task<SortedList<Hospitalization, Dictionary<string, InfectionStatus>>> ProcessAsync(Patient patient, PathogenParameter pathogen);
        Task<SortedList<Hospitalization, InfectionStatus>> ProcessAsync(Patient patient, PathogenParameter pathogen, string Resistence);
    }
}