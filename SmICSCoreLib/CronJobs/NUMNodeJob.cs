
using System.IO;
using System.Threading.Tasks;
using Quartz;
using SmICSCoreLib.Factories.NUMNode;

namespace SmICSCoreLib.CronJobs
{
    [DisallowConcurrentExecution]
    public class NUMNodeJob : IJob
    {
        private INUMNodeFactory _listFac;
        private readonly string path = @"../SmICSWebApp/Resources/NUMNode.json";

        public NUMNodeJob(INUMNodeFactory listFac)
        {
            _listFac = listFac;
        }
        public Task Execute(IJobExecutionContext context)
        {
            if (!File.Exists(path))
            {
                _listFac.FirstDataEntry();
            }else
            {
                _listFac.RegularDataEntry();
            }
            
            return Task.CompletedTask;
        }

    }
}
