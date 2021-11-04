using System.Collections.Generic;

namespace SmICSCoreLib.Factories.RKIConfig
{
    public interface IRKILabDataFactory
    {
        List<LabDataKeimReceiveModel> ProcessGetErreger(string name);
    }
}