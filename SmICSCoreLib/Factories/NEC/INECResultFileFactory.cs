using System.Collections.Generic;

namespace SmICSCoreLib.Factories.NEC
{
    public interface INECResultFileFactory
    {
        void Process(List<NECResultDataModel> data);
    }
}