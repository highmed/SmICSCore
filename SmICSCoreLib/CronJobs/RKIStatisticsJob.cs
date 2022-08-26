using System.IO;
using System.Linq;
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
            bool items = IsDirectoryEmpty(path);
            if(items == true)
            {
                _listFac.FirstDataEntry();
            }
            else
            {
                _listFac.RegularDataEntry();
            }
        
            return Task.CompletedTask;
        }
        private bool IsDirectoryEmpty(string path)
        {
            return !Directory.EnumerateFileSystemEntries(path).Any();
        }
    }
}
