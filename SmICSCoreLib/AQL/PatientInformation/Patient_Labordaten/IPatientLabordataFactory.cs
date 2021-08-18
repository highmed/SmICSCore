using SmICSCoreLib.AQL.General;
using System.Collections.Generic;

namespace SmICSCoreLib.AQL.PatientInformation.Patient_Labordaten
{
    public interface IPatientLabordataFactory
    {
        List<LabDataModel> Process(PatientListParameter parameter);
    }
}