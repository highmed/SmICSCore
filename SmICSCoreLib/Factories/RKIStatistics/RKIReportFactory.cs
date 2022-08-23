using Microsoft.Extensions.Logging;
using SmICSCoreLib.Factories.General;
using SmICSCoreLib.REST;
using System;
using System.Threading.Tasks;

namespace SmICSCoreLib.Factories.RKIStatistics
{
    public class RKIReportFactory : IRKIReportFactory
    {
        
        public IRestDataAccess RestDataAccess { get; set; }
        private readonly ILogger<RKIReportFactory> _logger;

        public RKIReportFactory(IRestDataAccess restDataAccess, ILogger<RKIReportFactory> logger)
        {
            RestDataAccess = restDataAccess;
            _logger = logger;
        }

        public void Process(string path)
        {
            try
            {
                
            }
            catch (Exception e)
            {
                _logger.LogWarning("Can not get aggregated Data " + e.Message);
            }
        }
    }
}
