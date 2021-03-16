using SmICSCoreLib.AQL.General;
using System;

namespace SmICSCoreLib.AQL.Algorithm.NEC
{
    public interface INECCombinedFactory
    {
        NECCombinedDataModel Process(DateTime date);
    }
}