using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.MiBi.PatientView.Parameter;
using SmICSCoreLib.REST;
using System.Collections.Generic;

namespace SmICSCoreLib.Factories.MiBi.PatientView
{
    public interface ILabResultFactory
    {
        IRestDataAccess RestDataAccess { get; set; }

        List<LabResult> Process(Patient patient, PathogenParameter pathogen = null);
        List<LabResult> Process(Case Case, PathogenParameter pathogen = null);
    }
}