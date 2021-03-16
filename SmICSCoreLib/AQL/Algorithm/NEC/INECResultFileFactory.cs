using System.Collections.Generic;

namespace SmICSCoreLib.AQL.Algorithm.NEC
{
    public interface INECResultFileFactory
    {
        void Process(List<NECResultDataModel> data);
    }
}