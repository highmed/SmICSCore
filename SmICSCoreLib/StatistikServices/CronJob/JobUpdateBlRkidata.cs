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
    public class JobUpdateBlRkidata :IJob
    {
        private readonly ILogger<JobUpdateBlRkidata > _logger;

        public JobUpdateBlRkidata (ILogger<JobUpdateBlRkidata > logger)
        {
            _logger = logger;
        }

        public Task Execute(IJobExecutionContext context)
        {
            RkiRestApi rkiRestApi = new();
            bool status = rkiRestApi.UpdateBlRkidata();
            if (status == true)
            {
                _logger.LogInformation($"**BL RKI-Daten wurden am " + DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss") + " erfolgreich aktualisiert!");
            }
            else
            {
                _logger.LogWarning($"**BL RKI-Daten könnten am " + DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss") + " leider nicht aktualisiert werden!");
            }
            return Task.CompletedTask;
        }
    }
}
