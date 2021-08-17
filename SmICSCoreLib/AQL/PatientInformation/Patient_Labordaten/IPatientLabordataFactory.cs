using SmICSCoreLib.AQL.General;
using SmICSCoreLib.AQL.PatientInformation.Patient_Labordaten.ReceiveModel;
using System.Collections.Generic;

namespace SmICSCoreLib.AQL.PatientInformation.Patient_Labordaten
{
    public interface IPatientLabordataFactory
    {
        List<LabDataModel> Process(PatientListParameter parameter);
        List<LabDataKeimReceiveModel> ProcessGetErreger(string name);
    }
}