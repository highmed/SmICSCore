using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmICSCoreLib.AQL.General;
using SmICSCoreLib.AQL.PatientInformation.Patient_Bewegung;
using SmICSCoreLib.AQL.PatientInformation.PatientMovement;
using SmICSCoreLib.REST;
using SmICSFactory.Tests;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Xunit;


namespace SmICSDataGenerator.Tests.PatientInformationTests
{
    public class PatientMovementProcessorTest
    {
        [Theory]
        [ClassData(typeof(PatientMovementTestData))]
        public void ProcessorTest(string ehrID, int ResultSetID) 
        {
            RestDataAccess _data = TestConnection.Initialize();

            PatientListParameter patientParams = new PatientListParameter()
            {
                patientList = new List<string>() { ehrID }
            };

            PatientMovementFactory factory = new PatientMovementFactory(_data, NullLogger<PatientMovementFactory>.Instance);
            List<PatientMovementModel> actual = factory.Process(patientParams);
            List<PatientMovementModel> expected = GetExpectedPatientMovementModels(ResultSetID);

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
                List<PatientIDs> patient = SmICSCoreLib.JSONFileStream.JSONReader<PatientIDs>.Read(@"../../../../TestData/GeneratedEHRIDs.json");
                for (int i = 0; i <= 34; i++)
                {
                    yield return new object[] { patient[i].EHR_ID, i };
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private List<PatientMovementModel> GetExpectedPatientMovementModels(int ResultSetID)
        {
            string path = "../../../../TestData/PatientMovementTestResults.json";
            List<PatientMovementModel> result = ExpectedResultJsonReader.ReadResults<PatientMovementModel>(path, ResultSetID, ExpectedType.PATIENT_MOVEMENT);
            return result;
        }
    }
}
