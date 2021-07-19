using SmICSCoreLib.AQL.Patient_Stay.Stationary;
using System.Collections.Generic;
using Xunit;
using SmICSFactory.Tests;
using SmICSDataGenerator.Tests;
using System.Collections;
using SmICSCoreLib.REST;
using System;
using SmICSCoreLib.AQL.General;
using SmICSCoreLib.AQL.PatientInformation.Patient_Bewegung;
using SmICSCoreLib.AQL.PatientInformation.PatientMovement;
using Microsoft.Extensions.Logging.Abstractions;

namespace WebApp.Test.DataServiceTest
{
    public class GetPatMovementFromStationTest
    {
        [Theory]
        [ClassData(typeof(PatientMovementTestData))]
        public void ProcessorTest(string ehrID, string station, DateTime starttime, DateTime endtime, int ResultSetID)
        {
            RestDataAccess _data = TestConnection.Initialize();

            PatientListParameter patientParams = new PatientListParameter()
            {
                patientList = new List<string>() { ehrID }
            };

            PatientMovementFactory factory = new PatientMovementFactory(_data, NullLogger<PatientMovementFactory>.Instance);
            List<PatientMovementModel> actual = factory.ProcessFromStation(patientParams, station, starttime, endtime);
            List<PatientMovementModel> expected = GetPatMovementFromStation(ResultSetID);

            Assert.Equal(expected.Count, actual.Count);

            int i = 0;
            Assert.Equal(expected[i].PatientID, actual[i].PatientID);
            Assert.Equal(expected[i].FallID, actual[i].FallID);
            Assert.Equal(expected[i].Bewegungsart_l, actual[i].Bewegungsart_l);
            Assert.Equal(expected[i].Beginn.ToString("s"), actual[i].Beginn.ToString("s"));
            //Assert.Equal(expected[i].Ende.ToString("g"), actual[i].Ende.ToString("g"));
            Assert.Equal(expected[i].StationID, actual[i].StationID);
            Assert.Equal(expected[i].Raum, actual[i].Raum);

        }


        private class PatientMovementTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                List<PatientInfos> patient = SmICSCoreLib.JSONFileStream.JSONReader<PatientInfos>.Read(@"../../../../WebApp.Test/Resources/EHRID_PatMovement.json");

                int i = 0;
                yield return new object[] { patient[i].EHR_ID, patient[i].StationID, patient[i].Beginn, patient[i].Ende , i };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private List<PatientMovementModel> GetPatMovementFromStation(int ResultSetID)
        {
            string testResultPath = "../../../../WebApp.Test/Resources/PatMovementTestResults.json";
            string parameterPath = "../../../../WebApp.Test/Resources/EHRID_PatMovement.json";

            List<PatientMovementModel> result = ExpectedResultJsonReader.ReadResults<PatientMovementModel, PatientInfos>(testResultPath, parameterPath, ResultSetID, ExpectedType.PATIENT_MOVEMENT_FROM_STATION);
            return result;
        }
    }
}
