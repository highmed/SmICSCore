using SmICSCoreLib.Factories.OutbreakDetection;
using SmICSCoreLib.Factories.RKIConfig;
using SmICSCoreLib.JSONFileStream;
using SmICSCoreLib.OutbreakDetection;
using SmICSCoreLib.REST;
using SmICSDataGenerator.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;



namespace SmICSFactory.Tests
{
    public class OutbreakDetectionTest
    {
        [Fact]
        public void ProcessorTest()
        {
            RestDataAccess _data = TestConnection.Initialize();
            IOutbreakDetectionParameterFactory _paramFac = new OutbreakDetectionParameterFactory(_data);
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

            OutbreakDetectionStoringModel expected = GetExpectedOutbreakDetectionResult();

            foreach (RKIConfigTemplate config in configs)
            {
                OutbreakDetectionParameter outbreakParam = new OutbreakDetectionParameter();
                outbreakParam.Retro = false;
                outbreakParam.Starttime = new DateTime(2021, 1, 30).AddDays(-((Convert.ToInt32(config.Zeitraum) * 7)+1));
                outbreakParam.Endtime = new DateTime(2021,1,30); //oder DateTime.Now.AddDays(-1);
                outbreakParam.PathogenIDs = config.ErregerID.Select(k => k.KeimID).ToList();
                outbreakParam.Ward = config.Station;
                SmICSVersion version = config.Erregerstatus == "virologisch" ? SmICSVersion.VIROLOGY : SmICSVersion.MICROBIOLOGY;

                ProxyParameterModel parameter = new ProxyParameterModel()
                {
                    EpochsObserved = _paramFac.Process(outbreakParam, version),
                    SavingFolder = "",
                    SavingDirectory = "C:\\Users\\biermapa\\source\\repos\\highmed\\SmICSCore\\SmICS.Tests",
                    FitRange = new int[] { 29, 29 },
                    LookbackWeeks = Convert.ToInt32(config.Zeitraum)
                };

                OutbreakDetectionProxy _proxy = new OutbreakDetectionProxy();
                _proxy.Covid19Extension(parameter);
            }
            OutbreakDetectionStoringModel actual = JSONReader<OutbreakDetectionStoringModel>.NewtonsoftReadSingle("@../../../../../Resources/OutbreakDetection/2021-01-29.json");

            Assert.Equal(expected.AlarmClassification, actual.AlarmClassification);
            Assert.Equal(expected.CasesAboveUpperBounds, actual.CasesAboveUpperBounds);
            Assert.Equal(expected.CasesBelowUpperBounds, actual.CasesBelowUpperBounds);
            Assert.Equal(expected.Date, actual.Date);
            Assert.Equal(expected.EndemicNiveau, actual.EndemicNiveau);
            Assert.Equal(expected.EpidemicNiveau, actual.EpidemicNiveau);
            Assert.Equal(expected.HasNoNullValues, actual.HasNoNullValues);
            Assert.Equal(expected.HasWarnings, actual.HasWarnings);
            Assert.Equal(expected.PathogenCount, actual.PathogenCount);
            Assert.Equal(expected.Probability, actual.Probability);
            Assert.Equal(expected.pValue, actual.pValue);
            Assert.Equal(expected.UpperBounds, actual.UpperBounds);
           
        }
        private OutbreakDetectionStoringModel GetExpectedOutbreakDetectionResult()
        {
            return new OutbreakDetectionStoringModel
            {
                AlarmClassification ="0 cases",
                CasesAboveUpperBounds = 0,
                CasesBelowUpperBounds = 0,
                PathogenCount=0,
                pValue= 1.0,
                Probability= 0.0003701269,
                Date=new DateTime(2021,1,29),
                EndemicNiveau= 1.374023,
                EpidemicNiveau= 4.108086,
                HasNoNullValues=true,
                HasWarnings=false,
                UpperBounds=6,
            };
        }
    }
}
