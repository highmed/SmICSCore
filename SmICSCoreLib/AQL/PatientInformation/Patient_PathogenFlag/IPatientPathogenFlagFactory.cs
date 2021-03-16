using SmICSCoreLib.AQL.General;
using System.Collections.Generic;

namespace SmICSCoreLib.AQL.PatientInformation.Patient_PathogenFlag
{
    public interface IPatientPathogenFlagFactory
    {
        List<PathogenFlagModel> Process(PatientListParameter parameter);
    }
}