using Microsoft.Extensions.Logging.Abstractions;
using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.PatientMovement;
using SmICSCoreLib.REST;
using SmICSFactory.Tests;
using System.Collections;
using System.Collections.Generic;
using Xunit;


namespace SmICSDataGenerator.Tests.PatientInformationTests
{
    public class PatientMovementProcessorTest
    {
        [Theory]
        [ClassData(typeof(PatientMovementTestData))]
        public void ProcessorTest(int ehrNo, int ResultSetID)
        {
            RestDataAccess _data = TestConnection.Initialize();
            List<PatientIDs> patient = SmICSCoreLib.JSONFileStream.JSONReader<PatientIDs>.Read(@"../../../../TestData/GeneratedEHRIDs.json");

            PatientListParameter patientParams = new PatientListParameter()
            {
                patientList = new List<string>() { patient[ehrNo].EHR_ID }
            };

            PatientMovementFactory factory = new PatientMovementFactory(_data, NullLogger<PatientMovementFactory>.Instance);
            List<PatientMovementModel> actual = factory.Process(patientParams);
            List<PatientMovementModel> expected = GetExpectedPatientMovementModels(ResultSetID, ehrNo);

            Assert.Equal(expected.Count, actual.Count);

            for (int i = 0; i < actual.Count; i++)
            {
                Assert.Equal(expected[i].PatientID, actual[i].PatientID);
                Assert.Equal(expected[i].FallID, actual[i].FallID);
                Assert.Equal(expected[i].Bewegungsart_l, actual[i].Bewegungsart_l);
                Assert.Equal(expected[i].Bewegungstyp, actual[i].Bewegungstyp);
                Assert.Equal(expected[i].BewegungstypID, actual[i].BewegungstypID);
                Assert.Equal(expected[i].Beginn.ToString("s"), actual[i].Beginn.ToString("s"));
                Assert.Equal(expected[i].Ende.ToString("g"), actual[i].Ende.ToString("g"));
                Assert.Equal(expected[i].StationID, actual[i].StationID);
                Assert.Equal(expected[i].Raum, actual[i].Raum);
                //Assert.Equal(expected[i].Fachabteilung, actual[i].Fachabteilung); 
            }
        }

        private class PatientMovementTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                for (int i = 0; i <= 37; i++)
                {
                    yield return new object[] { i, i };
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private List<PatientMovementModel> GetExpectedPatientMovementModels(int ResultSetID, int ehrNo)
        {
            string path = "../../../../TestData/PatientMovementTestResults.json";
            string parameterPath = "../../../../TestData/GeneratedEHRIDs.json";

            List<PatientMovementModel> result = ExpectedResultJsonReader.ReadResults<PatientMovementModel, PatientIDs>(path, parameterPath, ResultSetID, ehrNo, ExpectedType.PATIENT_MOVEMENT);
            return result;
        }
    }
}