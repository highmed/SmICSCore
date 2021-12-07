using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.MiBi.PatientView.Parameter;
using SmICSCoreLib.REST;
using System.Collections.Generic;

namespace SmICSCoreLib.Factories.MiBi.PatientView
{
    public interface IMibiResultFactory
    {
        IRestDataAccess RestDataAccess { get; set; }

        List<MiBiResult> Process(Patient patient, PathogenParameter pathogen = null);
        List<MiBiResult> Process(Case Case, PathogenParameter pathogen = null);
    }
}