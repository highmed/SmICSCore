using System.Threading.Tasks;
using Quartz;
using SmICSCoreLib.Factories.RKIStatistics;

namespace SmICSCoreLib.CronJobs
{
    [DisallowConcurrentExecution]
    public class RKIStatisticsJob : IJob
    {
        private readonly IRKIReportFactory _listFac;
        private readonly string path = @"../SmICSWebApp/Resources/RKIReportData";

        public RKIStatisticsJob(IRKIReportFactory listFac)
        {
            _listFac = listFac;
        }
        public Task Execute(IJobExecutionContext context)
        {
            _listFac.Process(path);           
            return Task.CompletedTask;
        }

    }
}
