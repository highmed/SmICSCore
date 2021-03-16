using Newtonsoft.Json.Linq;
using SmICSCoreLib.AQL.Algorithm.NEC;
using SmICSCoreLib.AQL.General;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SmICSCoreLib.AQL.Algorithm
{
    public class AlgortihmData : IAlgorithmData
    {
        
        private INECCombinedFactory _necDataFac;
        private INECResultDataFactory _necResultDataFac;
        private INECResultFileFactory _necResultFileFac;

        public AlgortihmData(INECCombinedFactory necDataFac, INECResultDataFactory nECResultDataFac, INECResultFileFactory necResultFileFac)
        {
            _necDataFac = necDataFac;
            _necResultDataFac = nECResultDataFac;
            _necResultFileFac = necResultFileFac;
        }

        public JArray RKI_Dataset(IDictionary parameter)
        {
            throw new NotImplementedException();
        }


        public NECCombinedDataModel NEC_Dataset(DateTime date) 
        {
            return _necDataFac.Process(date);
        }

        public List<NECCombinedDataModel> NEC_DatasetOverTimeSpan(TimespanParameter timespan)
        {
            List<NECCombinedDataModel> data = new List<NECCombinedDataModel>();
            for(DateTime date = timespan.Starttime.Date; date <= timespan.Endtime.Date; date.AddDays(1))
            {
                data.Add(_necDataFac.Process(date));
            }
            return data;
        }

        public List<NECResultDataModel> NECAlgorithmResult(TimespanParameter timespan) 
        { 
            if(timespan.Starttime == timespan.Endtime)
            {
                return _necResultDataFac.Process(timespan.Starttime);
            }
            else
            {
                return _necResultDataFac.Process(timespan);
            }
        }

        public void NECResultFile(List<NECResultDataModel> data)
        {
            _necResultFileFac.Process(data);
        }
    }
}
