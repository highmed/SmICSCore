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
        private readonly string dir = @"./Resources/OutbreakDetection";
        public List<OutbreakDetectionConfig> GetConfigurations()
        {
            List<OutbreakDetectionConfig> configs = new List<OutbreakDetectionConfig>();
            foreach(string folder in Directory.GetDirectories(dir))
            {
                string name = folder.Substring(dir.Length + 1);
                string ward = name.Split("_")[1];
                configs.Add(new OutbreakDetectionConfig { Name = name, Ward = ward});
            }
            return configs;
        }

        public OutbreakDetectionStoringModel GetLatestResult(OutbreakSaving outbreak)
        {
            DirectoryInfo directory = new DirectoryInfo(dir + outbreak.ConfigName);
            FileInfo file = directory.GetFiles().OrderByDescending(f => f.Name).FirstOrDefault();
            return JSONReader<OutbreakDetectionStoringModel>.NewtonsoftReadSingle(file.FullName);
        }

        public List<OutbreakDetectionStoringModel> GetsResultsInTimespan(OutbreakSavingInTimespan outbreak)
        {
            List<OutbreakDetectionStoringModel> OutbreakDetectionResults = new List<OutbreakDetectionStoringModel>();
            for (DateTime date = outbreak.Starttime; date <= outbreak.Endtime; date = date.AddDays(1.0))
            {
                OutbreakDetectionResults.Add(JSONReader<OutbreakDetectionStoringModel>.NewtonsoftReadSingle(dir + "/" + outbreak.ConfigName + "/" + date.ToString("yyyy-MM-dd") + ".json"));
            }
            return OutbreakDetectionResults;
        }
    }
}
