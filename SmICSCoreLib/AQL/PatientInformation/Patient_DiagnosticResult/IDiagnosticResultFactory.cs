using SmICSCoreLib.AQL.General;
using System.Collections.Generic;

namespace SmICSCoreLib.AQL.PatientInformation.Patient_DiagnosticResult
{
    public interface IDiagnosticResultFactory
    {
        List<DiagnosticResultModel> Process(PatientListParameter parameter);
    }
}