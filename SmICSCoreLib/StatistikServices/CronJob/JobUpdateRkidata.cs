using Microsoft.Extensions.Logging;
using Quartz;
using SmICSCoreLib.StatistikDataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            RkiService rkiRestApi = new();
            string dailyReportPath = @"../Resources/statistik/json/" + DateTime.Now.ToString("yyyy-MM-dd") + ".json";
            string blReportPath = @"../Resources/Rkidata/BLReport.json";
            string lkReportPath = @"../Resources/Rkidata/LKReport.json";
            string targetPath = @"../Resources/Rkidata";
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
