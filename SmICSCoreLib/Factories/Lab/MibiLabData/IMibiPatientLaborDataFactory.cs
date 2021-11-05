using SmICSCoreLib.Factories.General;
using System.Collections.Generic;

namespace SmICSCoreLib.Factories.Lab.MibiLabData
{
    public interface IMibiPatientLaborDataFactory
    {
        List<MibiLabDataModel> Process(PatientListParameter parameter);
    }
}