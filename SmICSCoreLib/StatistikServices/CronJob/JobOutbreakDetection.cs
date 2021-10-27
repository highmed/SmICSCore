using Quartz;
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
                string path = @"./Resources/RKIConfig/RKIConfig.json"; //TODO: Für publish Version anpassen
                //List<RKIConfigTemplate> configs = JSONReader<RKIConfigTemplate>.Read(path);
                List<RKIConfigTemplate> configs = new List<RKIConfigTemplate>()
                {
                    new RKIConfigTemplate
                    {
                        Station = "Coronastation",
                        Erreger = "Sars-Cov-2",
                        ErregerID = new List<LabDataKeimReceiveModel>
                        {
                            new LabDataKeimReceiveModel()
                            {
                                KeimID = "94500-6"
                            },
                            new LabDataKeimReceiveModel()
                            {
                                KeimID = "94558-4"
                            },
                            new LabDataKeimReceiveModel()
                            {
                                KeimID = "94745-7"
                            }
                        },
                        Retro = false,
                        Erregerstatus = "virologisch",
                        Zeitraum = "4"
                    }
                };
                

                foreach(RKIConfigTemplate config in configs)
                {
                    string savingFolder = GetSavingFolder(config);

                    OutbreakDetectionParameter outbreakParam = ConfigToParam(config, savingFolder);
                    SmICSVersion version = config.Erregerstatus == "virologisch" ? SmICSVersion.VIROLOGY : SmICSVersion.MICROBIOLOGY;

                    ProxyParameterModel parameter = new ProxyParameterModel()
                    {
                        EpochsObserved = _paramFac.Process(outbreakParam, version),
                        SavingFolder = savingFolder,
                        SavingDirectory = Directory.GetCurrentDirectory(),
                        FitRange = GetFitRange(outbreakParam, savingFolder, Convert.ToInt32(config.Zeitraum)),
                        LookbackWeeks = Convert.ToInt32(config.Zeitraum)
                    };

                    _proxy.Covid19Extension(parameter);
                }
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                //_logger.Warning("JobOutbreakDetection FAILED: " + e.Message);
            }
            
        }

        private string GetSavingFolder(RKIConfigTemplate config)
        {
            return config.Erreger + "_" + config.Station + "_" + config.Zeitraum + (config.Retro ? "_Retro" : "") + "/";
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
            outbreakParam.Starttime = DateTime.Now.AddDays(-((Convert.ToInt32(config.Zeitraum) * 7)+1));
            outbreakParam.Endtime = DateTime.Now;
            outbreakParam.PathogenIDs = config.ErregerID.Select(k => k.KeimID).ToList();
            outbreakParam.Ward = config.Station;
            
            return outbreakParam;
        }
    }
}
