using SmICSCoreLib.Factories.General;
using System;

namespace SmICSCoreLib.Factories.NEC
{
    public interface INECCombinedFactory
    {
        NECCombinedDataModel Process(DateTime date);
    }
}