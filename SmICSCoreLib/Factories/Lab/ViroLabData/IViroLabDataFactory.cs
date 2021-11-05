using SmICSCoreLib.Factories.General;
using SmICSCoreLib.REST;
using System.Collections.Generic;

namespace SmICSCoreLib.Factories.Lab.ViroLabData
{
    public interface IViroLabDataFactory
    {
        IRestDataAccess RestDataAccess { get; }
        List<LabDataModel> Process(PatientListParameter parameter);
    }
}