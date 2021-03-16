using SmICSCoreLib.AQL.General;
using System.Collections.Generic;

namespace SmICSCoreLib.AQL.PatientInformation.Patient_Mibi_Labordaten
{
    public interface IMibiPatientLaborDataFactory
    {
        List<MibiLabDataModel> Process(PatientListParameter parameter);
    }
}