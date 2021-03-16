using SmICSCoreLib.AQL.General;
using System.Collections.Generic;

namespace SmICSCoreLib.AQL.PatientInformation.Symptome
{
    public interface ISymptomFactory
    {
        List<SymptomModel> Process(PatientListParameter parameter);
    }
}