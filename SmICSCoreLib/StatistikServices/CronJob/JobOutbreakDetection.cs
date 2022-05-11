using Quartz;
using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.OutbreakDetection;
using SmICSCoreLib.Factories.RKIConfig;
using SmICSCoreLib.JSONFileStream;
using SmICSCoreLib.OutbreakDetection;
using System;
using System.Collections.Generic;
using System.IO;
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
                await Task.Run(InitiateOutbreakDetection);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                //_logger.Warning("JobOutbreakDetection FAILED: " + e.Message);
            }
            
        }

        private void InitiateOutbreakDetection()
        {
            string path = @"./Resources/OutbreakDetection/RKIConfig.json"; //TODO: Für publish Version anpassen
            List<RKIConfigTemplate> configs = JSONReader<RKIConfigTemplate>.Read(path);

            foreach (RKIConfigTemplate config in configs)
            {
                string savingFolder = GetSavingFolder(config);
                savingFolder = "";
                OutbreakDetectionParameter outbreakParam = ConfigToParam(config, savingFolder);
                outbreakParam.MedicalField = config.Pathogenstatus == "virologisch" ? MedicalField.VIROLOGY : MedicalField.MICROBIOLOGY;
                ProxyParameterModel parameter = new ProxyParameterModel()
                {
                    EpochsObserved = _paramFac.Process(outbreakParam),
                    SavingFolder = savingFolder,
                    SavingDirectory = Directory.GetCurrentDirectory(),
                    FitRange = GetFitRange(outbreakParam, savingFolder, Convert.ToInt32(config.Timespan)),
                    LookbackWeeks = Convert.ToInt32(config.Timespan),
                    MedicalField = outbreakParam.MedicalField
                };

                _proxy.Covid19Extension(parameter);
            }
        }

        private string GetSavingFolder(RKIConfigTemplate config)
        {
            return config.Pathogen + "_" + config.Ward + "_" + config.Timespan + (config.Retro ? "_Retro" : "") + "/";
        }

        private int[] GetFitRange(OutbreakDetectionParameter outbreakParam, string savingFolder, int lookback)
        {
            int[] fitrange = new int[2];
            int dayCount = (int)(outbreakParam.Endtime.Date - outbreakParam.Starttime.Date).TotalDays;
            int minDayCount = (lookback * 7) + 1;
            if (outbreakParam.Retro && !File.Exists(@"./Resources/OutbreakDetection/" + savingFolder + DateTime.Now.AddDays(-1.0).ToString("yyyy-MM-dd")))
            {
                fitrange = new int[] { minDayCount, dayCount };
            }
            else
            {
                fitrange = new int[] { dayCount, dayCount };
            }

            return fitrange;
        }

        private OutbreakDetectionParameter ConfigToParam(RKIConfigTemplate config, string savingFolder)
        {
            OutbreakDetectionParameter outbreakParam = new OutbreakDetectionParameter();
            if (config.Retro && !File.Exists(@"./Resources/OutbreakDetection/" + savingFolder + DateTime.Now.AddDays(-1.0).ToString("yyyy-MM-dd")))
            {
                outbreakParam.Retro = true; 
            }
            else
            {
                outbreakParam.Retro = false;
            }
            outbreakParam.Starttime = DateTime.Now.AddDays(-((Convert.ToInt32(config.Timespan) * 7)+1));
            outbreakParam.Endtime = DateTime.Now;
            outbreakParam.PathogenIDs = config.PathogenCodes.ToList();
            outbreakParam.Ward = config.Ward;
            return outbreakParam;
        }
    }
}
