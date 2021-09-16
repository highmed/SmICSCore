using SmICSCoreLib.Factories.General;
using System.Collections.Generic;

namespace SmICSCoreLib.Factories.Lab.ViroLabData
{
    public interface IViroLabDataFactory
    {
        List<LabDataModel> Process(PatientListParameter parameter);
    }
}