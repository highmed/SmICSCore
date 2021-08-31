using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Quartz;
using System;
using System.Threading.Tasks;

namespace SmICSCoreLib.StatistikServices.CronJob
{
    [DisallowConcurrentExecution]
    public class JobUpdateRkidata :IJob
    {
        private readonly ILogger<JobUpdateRkidata > _logger;

        public JobUpdateRkidata (ILogger<JobUpdateRkidata > logger)
        {
            _logger = logger;
        }

        public Task Execute(IJobExecutionContext context)
        {
            RkiService rkiRestApi = new(NullLogger<RkiService>.Instance);
            string dailyReportPath = @"../SmICSWebApp/Resources/statistik/json/" + DateTime.Now.ToString("yyyy-MM-dd") + ".json";
            string blReportPath = @"../SmICSWebApp/Resources/Rkidata/BLReport.json";
            string lkReportPath = @"../SmICSWebApp/Resources/Rkidata/LKReport.json";
            string targetPath = @"../SmICSWebApp/Resources/Rkidata";
            string blFilename = ("BLReport");
            string lkFilename = ("LKReport");

            bool blStatus = rkiRestApi.UpdateBlRkidata(dailyReportPath, blReportPath, targetPath, blFilename);
            if (blStatus == true)
            {
                _logger.LogInformation($"**BL RKI-Daten wurden am " + DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss") + " erfolgreich aktualisiert!");
            }
            else
            {
                _logger.LogWarning($"**BL RKI-Daten könnten am " + DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss") + " leider nicht aktualisiert werden!");
            } 
         
            bool lkStatus = rkiRestApi.UpdateLklRkidata(dailyReportPath, lkReportPath, targetPath, lkFilename);
            if (lkStatus == true)
            {
                _logger.LogInformation($"**LK RKI-Daten wurden am " + DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss") + " erfolgreich aktualisiert!");
            }
            else
            {
                _logger.LogWarning($"**LK RKI-Daten könnten am " + DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss") + " leider nicht aktualisiert werden!");
            }

            return Task.CompletedTask;
        }
    }
}
