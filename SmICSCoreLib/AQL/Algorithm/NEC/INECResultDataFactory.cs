using SmICSCoreLib.AQL.General;
using System;
using System.Collections.Generic;

namespace SmICSCoreLib.AQL.Algorithm.NEC
{
    public interface INECResultDataFactory
    {
        List<NECResultDataModel> Process(DateTime date);
        List<NECResultDataModel> Process(TimespanParameter timespan);
    }
}