
using System.Threading.Tasks;
using Quartz;
using System;
using SmICSCoreLib.Factories.NUMNode;
using System.Collections.Generic;
using System.Linq;

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
            //Task allData = Task.Run(_listFac.);
            //await Task.WhenAll(allData);
            _listFac.FirstDataEntry();
            return Task.CompletedTask;
        }
    }
}
