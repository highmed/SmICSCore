using SmICSCoreLib.JSONFileStream;
using SmICSCoreLib.OutbreakDetection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SmICSWebApp.Data.OutbreakDetection
{
    public class OutbreakDetectionService
    {
        private readonly string dir = "@./Resources/OutbreakDetection";
        //DB
        public OutbreakDetectionConfigs GetConfigurations()
        {
            return new OutbreakDetectionConfigs()
            {
                ConfigNames = Directory.GetDirectories(dir).ToList()
            };
        }
        //DB
        public OutbreakDetectionResultModel GetLatestResult(OutbreakSaving outbreak)
        {
            DirectoryInfo directory = new DirectoryInfo(dir + outbreak.ConfigName);
            FileInfo file = directory.GetFiles().OrderByDescending(f => f.Name).FirstOrDefault();
            return JSONReader<OutbreakDetectionResultModel>.ReadObject(file.FullName);
        }
        //DB
        public List<OutbreakDetectionResultModel> GetsResultsInTimespan(OutbreakSavingInTimespan outbreak)
        {
            List<OutbreakDetectionResultModel> OutbreakDetectionResults = new List<OutbreakDetectionResultModel>();
            for (DateTime date = outbreak.Start; date <= outbreak.End; date.AddDays(1.0))
            {
                OutbreakDetectionResults.Add(JSONReader<OutbreakDetectionResultModel>.ReadObject(dir + outbreak.ConfigName + "/" + date.ToString("yyyy-MM-dd")));
            }
            return OutbreakDetectionResults;
        }
    }
}
