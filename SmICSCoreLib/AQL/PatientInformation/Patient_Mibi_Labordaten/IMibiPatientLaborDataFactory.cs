using SmICSCoreLib.AQL.General;
using SmICSCoreLib.AQL.PatientInformation.Patient_Mibi_Labordaten.ReceiveModel;
using System.Collections.Generic;

namespace SmICSCoreLib.AQL.PatientInformation.Patient_Mibi_Labordaten
{
    public interface IMibiPatientLaborDataFactory
    {
        List<MibiLabDataModel> Process(PatientListParameter parameter);
        List<MibiLabDataKeimReceiveModel> ProcessGetErreger(string name);
    }
}