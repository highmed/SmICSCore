using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.MiBi.PatientView.Parameter;
using SmICSCoreLib.REST;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmICSCoreLib.Factories.MiBi.PatientView
{
    public interface ILabResultFactory
    {
        IRestDataAccess RestDataAccess { get; set; }

        Task<List<LabResult>> ProcessAsync(Case Case, string MedicalField);
        Task<List<LabResult>> ProcessAsync(Case Case, PathogenParameter pathogen);
        Task<List<LabResult>> ProcessAsync(Patient patient, string MedicalField);
        Task<List<LabResult>> ProcessAsync(Patient patient, PathogenParameter pathogen);
    }
}