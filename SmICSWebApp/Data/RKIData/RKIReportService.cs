using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SmICSCoreLib.Factories.RKIStatistics.Models;
using Microsoft.Extensions.Logging;
using SmICSCoreLib.DownloadFile;
using SmICSCoreLib.Factories.RKIStatistics;
using System.Threading.Tasks;
using SmICSCoreLib.Factories.RKIStatistics.ReceiveModels;

namespace SmICSWebApp.Data.RKIData
{
    public class RKIReportService
    {      
        private readonly ILogger<RKIReportService> _logger;
        private readonly IRKIReportFactory _listFac;
        private readonly string path = "";
        
        public RKIReportService(ILogger<RKIReportService> logger, IRKIReportFactory listFac)
        {
            _logger = logger;
            _listFac = listFac;
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

        public void GetBLReport()
        {

        }
    }
}
