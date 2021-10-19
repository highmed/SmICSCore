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
                string path = @"../SmICSWebApp/Resources/RKIConfig/RKIConfig.json"; //TODO: Für publish Version anpassen
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
                    OutbreakDetectionParameter outbreakParam = ConfigToParam(config);
                    outbreakParam.Starttime = new DateTime(2020, 1, 1);
                    outbreakParam.Endtime = new DateTime(2021, 5, 1);
                    SmICSVersion version = config.Erregerstatus == "virologisch" ? SmICSVersion.VIROLOGY : SmICSVersion.MICROBIOLOGY;

                    string savingFolder = GetSavingFolder(config);

                    ProxyParameterModel parameter = new ProxyParameterModel()
                    {
                        EpochsObserved = _paramFac.Process(outbreakParam, version),
                        SavingFolder = savingFolder,
                        FitRange = GetFitRange(config, savingFolder),
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

        private int[] GetFitRange(RKIConfigTemplate config, string savingFolder)
        {
            int[] fitrange = new int[2];
            int dayCount = (Convert.ToInt32(config.Zeitraum) * 7) + 1;
            if (config.Retro && !File.Exists(@"../SmICSWebApp/Resources/OutbreakDetection/" + savingFolder + DateTime.Now.AddDays(-1.0).ToString("yyyy-MM-dd")))
            {
                fitrange = new int[] { 1, dayCount };
            }
            else
            {
                fitrange = new int[] { dayCount, dayCount };
            }

            return fitrange;
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
