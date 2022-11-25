
using System;
using System.IO;
using System.Linq;
using SmICSCoreLib.Factories.RKIStatistics.Models;
using Microsoft.Extensions.Logging;
using SmICSCoreLib.JSONFileStream;

namespace SmICSWebApp.Data.RKIData
{
    public class RKIReportService
    {      
        private readonly ILogger<RKIReportService> _logger;
        
        public RKIReportService(ILogger<RKIReportService> logger)
        {
            _logger = logger;
        }

        public string SetCaseColor(double day, double daybefore)
        {
            string color = "#b0bec5";
            try
            {
                if (day < daybefore)
                {
                    color = "#66C166";
                }else if (day == daybefore)
                {
                    color = "#FFC037";
                }else if (day > daybefore)
                {
                    color = "#F35C58";
                }else
                {
                    color = "#8CA2AE";
                }
                return color;
            }
            catch (Exception e)
            {
                _logger.LogWarning("SetCaseColor " + e.Message);
                return color;
            }
        }

        public RKIDailyReportModel GetBLReport(string path)
        {
            string file = path + "RKI_DailyReport_" + DateTime.Now.ToString("yyyy-MM-dd") + ".json";
            if (!File.Exists(file))
            {
                return null;
            }
            RKIDailyReportModel report = JSONReader<RKIDailyReportModel>.ReadObject(file);
            return report;
        }

        public RKIDailyReportModel GetOldBLReport(string path)
        {
            var directory = new DirectoryInfo(path);
            var file = (from f in directory.GetFiles()
                    orderby f.LastWriteTime descending
                    select f).FirstOrDefault();
            string filepath = path + file.Name;
            RKIDailyReportModel report = JSONReader<RKIDailyReportModel>.ReadObject(filepath);
            return report;
        }
    }
}
