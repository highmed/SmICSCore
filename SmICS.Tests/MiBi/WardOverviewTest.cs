using Microsoft.Extensions.Logging.Abstractions;
using SmICSCoreLib.Factories.MiBi;
using SmICSCoreLib.Factories.MiBi.WardOverview;
using SmICSCoreLib.Factories.Lab.MibiLabData;
using SmICSCoreLib.REST;
using SmICSDataGenerator.Tests;
using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace SmICSFactory.Tests.MiBi
{
    public class WardOverviewTest
    {
        [Theory]
        [ClassData(typeof(WardOverviewTestData))]
        public void ProcessTest(WardOverviewParameters parameter)
        {
            RestDataAccess _data = TestConnection.Initialize();

            IAntibiogramFactory _antibiogram = new AntibiogramFactory(_data);
            IMibiPatientLaborDataFactory _mibi = new MibiPatientLaborDataFactory(_data, _antibiogram);
            IWardOverviewFactory factory = new WardOverviewFactory(_data, NullLogger<WardOverviewFactory>.Instance, _mibi);
            List<WardOverviewModel> actual = factory.Process(parameter);
            List<WardOverviewModel> expected = GetExpectedWardOverviewData();
            Assert.NotNull(actual);
            Assert.Equal(expected.Count, actual.Count);
            for (int i = 0; i < actual.Count; i++)
            {
                Assert.Equal(expected[i].PatientID , actual[i].PatientID);
                Assert.Equal(expected[i].TestDate , actual[i].TestDate);
                Assert.Equal(expected[i].NewCase , actual[i].NewCase);
                Assert.Equal(expected[i].Nosokomial , actual[i].Nosokomial);
                Assert.Equal(expected[i].OnWard , actual[i].OnWard);
            }
        }

        private class WardOverviewTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                //List<PatientIDs> patient = SmICSCoreLib.JSONFileStream.JSONReader<PatientIDs>.Read(@"../../../../TestData/GeneratedEHRIDs.json");
                yield return new object[] { 
                    new WardOverviewParameters 
                    {
                        Ward = "47",
                        MRE = "MRSA",
                        Start = new DateTime(2021,5,26),
                        End = new DateTime(2021,6,5)               
                    } 
                };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private List<WardOverviewModel> GetExpectedWardOverviewData()
        {
            return new List<WardOverviewModel>() {
                new WardOverviewModel
                {
                    NewCase = true,
                    OnWard = true,
                    Nosokomial = false,
                    PatientID = "Patient101",
                    TestDate = new DateTime(2021, 6, 1, 9, 0, 0).ToLocalTime()
                },
                new WardOverviewModel
                {
                    NewCase = false,
                    OnWard = true,
                    Nosokomial = false,
                    PatientID = "Patient102",
                    TestDate = new DateTime(2021, 6, 2, 9, 0, 0).ToLocalTime()
                },
                new WardOverviewModel
                {
                    NewCase = true,
                    OnWard = true,
                    Nosokomial = true,
                    PatientID = "Patient100",
                    TestDate = new DateTime(2021, 6, 1, 9, 0, 0).ToLocalTime()
                }
            };
        }

    }
}
