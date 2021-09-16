using System.Text.RegularExpressions;
using System.Collections.Generic;
using Xunit;
using SmICSFactory.Tests;
using SmICSDataGenerator.Tests;
using System.Collections;
using SmICSCoreLib.REST;
using System;
using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.PatientMovement;
using Microsoft.Extensions.Logging.Abstractions;

namespace WebApp.Test.DataServiceTest
{
    public class GetPatMovementFromStationTest
    {
        [Theory]
        [ClassData(typeof(PatientMovementTestData))]
        public void ProcessorTest(string ehrID, string station, DateTime starttime, DateTime endtime, int ResultSetID, int ehrNo)
        {
            RestDataAccess _data = TestConnection.Initialize();

            PatientListParameter patientParams = new PatientListParameter()
            {
                patientList = new List<string>() { ehrID }
            };

            PatientMovementFactory factory = new PatientMovementFactory(_data, NullLogger<PatientMovementFactory>.Instance);
            List<PatientMovementModel> actual = factory.ProcessFromStation(patientParams, station, starttime, endtime);
            List<PatientMovementModel> expected = GetPatMovementFromStation(ResultSetID,  ehrNo);

            Assert.Equal(expected.Count, actual.Count);

            for (int i = 0; i < expected.Count; i++)
            {
                Assert.Equal(expected[i].PatientID, actual[i].PatientID);
                Assert.Equal(expected[i].FallID, actual[i].FallID);
                Assert.Equal(expected[i].Bewegungsart_l, actual[i].Bewegungsart_l);
                Assert.Equal(expected[i].Beginn.ToString("yyyy-MM-dd"), actual[i].Beginn.ToString("yyyy-MM-dd"));
                Assert.Equal(expected[i].Ende.ToString("yyyy-MM-dd"), actual[i].Ende.ToString("yyyy-MM-dd"));
                Assert.Equal(expected[i].StationID, actual[i].StationID);
                Assert.Equal(expected[i].Raum, actual[i].Raum);
                Assert.Equal(Regex.Replace(expected[i].Fachabteilung, @"\s", ""), Regex.Replace(actual[i].Fachabteilung, @"\s", ""));
                Assert.Equal(expected[i].FachabteilungsID, actual[i].FachabteilungsID);
            }       
        }


        private class PatientMovementTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                List<PatientIDs> patient = SmICSCoreLib.JSONFileStream.JSONReader<PatientIDs>.Read(@"../../../../TestData/GeneratedEHRIDs.json");

                string stationID = "Coronastation";
                DateTime beginn = Convert.ToDateTime("2021-01-01T09:00:00");
                DateTime ende = Convert.ToDateTime("2021-01-05T15:00:00");
                yield return new object[] { patient[16].EHR_ID, stationID, beginn, ende, 0, 16 };
            }
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private List<PatientMovementModel> GetPatMovementFromStation(int ResultSetID, int ehrNo)
        {
            string testResultPath = "../../../../WebApp.Test/Resources/PatMovementTestResults.json";
            string parameterPath = "../../../../TestData/GeneratedEHRIDs.json";

            List<PatientMovementModel> result = ExpectedResultJsonReader.ReadResults<PatientMovementModel, PatientIDs>(testResultPath, parameterPath, ResultSetID, ehrNo, ExpectedType.PATIENT_MOVEMENT_FROM_STATION);
            return result;
        }
    }
}
