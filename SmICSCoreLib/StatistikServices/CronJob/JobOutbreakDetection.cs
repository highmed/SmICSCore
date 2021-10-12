using Quartz;
using SmICSCoreLib.Factories.OutbreakDetection;
using System;
using System.Threading.Tasks;

namespace SmICSCoreLib.StatistikServices.CronJob
{
    [DisallowConcurrentExecution]
    public class JobOutbreakDetection : IJob
    {
        private readonly OutbreakDetectionParameterFactory _paramFac;

        public JobOutbreakDetection(OutbreakDetectionParameterFactory paramFac)
        {
            _paramFac = paramFac;
        }

        public Task Execute(IJobExecutionContext context)
        {
            try
            {
                OutbreakDetectionParameter outbreakParam = new OutbreakDetectionParameter();
                //generate OutbreakParam from Config
                int[][] epochs_and_outbreakCount = _paramFac.Process(outbreakParam, SmICSVersion.VIROLOGY);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                //_logger.Warning("JobOutbreakDetection FAILED: " + e.Message);
            }
        }
    }
}
