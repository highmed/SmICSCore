
using System.Threading.Tasks;
using Quartz;
using SmICSCoreLib.Factories.NUMNode;

namespace SmICSCoreLib.CronJobs
{
    [DisallowConcurrentExecution]
    public class NUMNodeJob : IJob
    {
        private INUMNodeFactory _listFac;

        public NUMNodeJob(INUMNodeFactory listFac)
        {
            _listFac = listFac;
        }
        public Task Execute(IJobExecutionContext context)
        {
            _listFac.FirstDataEntry();
            return Task.CompletedTask;
        }

    }
}
