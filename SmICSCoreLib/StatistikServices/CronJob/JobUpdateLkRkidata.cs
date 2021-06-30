using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Threading.Tasks;

namespace SmICSCoreLib.StatistikServices.CronJob
{
    [DisallowConcurrentExecution]
    public class JobUpdateLkRkidata : IJob
    {
        private readonly ILogger<JobUpdateLkRkidata> _logger;

        public JobUpdateLkRkidata(ILogger<JobUpdateLkRkidata> logger)
        {
            _logger = logger;
        }
        public Task Execute(IJobExecutionContext context)
        {
            RkiRestApi rkiRestApi = new();
            bool status = rkiRestApi.UpdateLklRkidata();
            if (status == true)
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
