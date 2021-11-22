using SmICSCoreLib.Factories.General;
using SmICSCoreLib.JSONFileStream;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SmICSCoreLib.Factories.NEC
{
    public class NECResultDataFactory : INECResultDataFactory
    {
        public List<NECResultDataModel> Process(DateTime date)
        {
            return JSONReader<NECResultDataModel>.Read(Directory.GetFiles($"../SmICSWebApp/Resources/nec/json", date.ToString("yyyy-MM-dd")+".json")[0]);
        }

        public List<NECResultDataModel> Process(TimespanParameter timespan)
        {
            List<NECResultDataModel> concatList = new List<NECResultDataModel>();
            for (DateTime date = timespan.Starttime.Date; date <= timespan.Endtime.Date; date.AddDays(1))
            {
                List<NECResultDataModel> lst = Process(date);
                concatList.AddRange(lst);
            }
            return concatList;
        }

    }
}
