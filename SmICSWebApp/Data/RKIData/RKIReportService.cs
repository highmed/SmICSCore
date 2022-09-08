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
        
        public RKIReportService(ILogger<RKIReportService> logger, IRKIReportFactory listFac)
        {
            _logger = logger;
            _listFac = listFac;
        }

    }
}
