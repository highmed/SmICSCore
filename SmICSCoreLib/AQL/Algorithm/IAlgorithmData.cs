using Newtonsoft.Json.Linq;
using SmICSCoreLib.AQL.Algorithm.NEC;
using SmICSCoreLib.AQL.General;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SmICSCoreLib.AQL.Algorithm
{
    public interface IAlgorithmData
    {
        JArray RKI_Dataset(IDictionary parameter);
        NECCombinedDataModel NEC_Dataset(DateTime date);
        List<NECCombinedDataModel> NEC_DatasetOverTimeSpan(TimespanParameter timespan);
        List<NECResultDataModel> NECAlgorithmResult(TimespanParameter timespan);
        void NECResultFile(List<NECResultDataModel> data);
    }
}