using Quartz;
using SmICSCoreLib.Factories.OutbreakDetection;
using SmICSCoreLib.Factories.RKIConfig;
using SmICSCoreLib.JSONFileStream;
using SmICSCoreLib.OutbreakDetection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmICSCoreLib.StatistikServices.CronJob
{
    [DisallowConcurrentExecution]
    public class JobOutbreakDetection : IJob
    {
        private readonly IOutbreakDetectionParameterFactory _paramFac;
        private readonly OutbreakDetectionProxy _proxy;
        public JobOutbreakDetection(IOutbreakDetectionParameterFactory paramFac, OutbreakDetectionProxy proxy)
        {
            _paramFac = paramFac;
            _proxy = proxy;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                string path = @"../SmICSWebApp/Resources/RKIConfig/RKIConfig.json"; //TODO: Für publish Version anpassen
                List<RKIConfigTemplate> configs = JSONReader<RKIConfigTemplate>.Read(path);
                
                foreach(RKIConfigTemplate config in configs)
                {
                    OutbreakDetectionParameter outbreakParam = ConfigToParam(config);
                    SmICSVersion version = config.Erregerstatus == "virologisch" ? SmICSVersion.VIROLOGY : SmICSVersion.MICROBIOLOGY;


                    int[][] epochs_and_outbreakCount = _paramFac.Process(outbreakParam, version);

                    _proxy.Covid19Extension(epochs_and_outbreakCount);
                }
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                //_logger.Warning("JobOutbreakDetection FAILED: " + e.Message);
            }
            
        }

        private OutbreakDetectionParameter ConfigToParam(RKIConfigTemplate config)
        {
            OutbreakDetectionParameter outbreakParam = new OutbreakDetectionParameter();
            outbreakParam.Starttime = DateTime.Now.AddDays(-(Convert.ToInt32(config.Zeitraum) * 7));
            outbreakParam.Endtime = DateTime.Now; //oder DateTime.Now.AddDays(-1);
            outbreakParam.PathogenIDs = config.ErregerID.Select(k => k.KeimID).ToList();
            outbreakParam.Ward = config.Station;

            return outbreakParam;
        }
    }
}
