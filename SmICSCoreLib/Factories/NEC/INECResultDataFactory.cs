using SmICSCoreLib.Factories.General;
using System;
using System.Collections.Generic;

namespace SmICSCoreLib.Factories.NEC
{
    public interface INECResultDataFactory
    {
        List<NECResultDataModel> Process(DateTime date);
        List<NECResultDataModel> Process(TimespanParameter timespan);
    }
}