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
            RkiRestApi rkiRestApi = new();
            bool blStatus = rkiRestApi.UpdateBlRkidata();
            if (blStatus == true)
            {
                _logger.LogInformation($"**BL RKI-Daten wurden am " + DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss") + " erfolgreich aktualisiert!");
            }
            else
            {
                _logger.LogWarning($"**BL RKI-Daten könnten am " + DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss") + " leider nicht aktualisiert werden!");
            }

            bool lkStatus = rkiRestApi.UpdateLklRkidata();
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
