using Microsoft.Extensions.Logging.Abstractions;
using SmICSCoreLib.AQL.General;
using SmICSCoreLib.AQL.PatientInformation.Patient_Labordaten;
using SmICSCoreLib.REST;
using SmICSFactory.Tests;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace SmICSDataGenerator.Tests.PatientInformationTests
{
    public class PatientLabordataTest
    {

        [Theory]
        [ClassData(typeof(LabTestData))]
        public void ProcessorTest(string ehrID, int expectedResultSet)
        {
            RestDataAccess _data = TestConnection.Initialize();

            PatientListParameter patientParams = new PatientListParameter()
            {
                patientList = new List<string>() { ehrID }
            };

            PatientLabordataFactory factory = new PatientLabordataFactory(_data, NullLogger<PatientLabordataFactory>.Instance);
            List<LabDataModel> actual = factory.Process(patientParams);
            List<LabDataModel> expected = GetExpectedLabDataModels(expectedResultSet);

            Assert.Equal(expected.Count, actual.Count);

            for (int i = 0; i < actual.Count; i++)
            {
                Assert.Equal(expected[i].PatientID, actual[i].PatientID);
                Assert.Equal(expected[i].FallID, actual[i].FallID);
                Assert.Equal(expected[i].Befund, actual[i].Befund);
                Assert.Equal(expected[i].Befunddatum.ToString("s"), actual[i].Befunddatum.ToUniversalTime().ToString("s"));
                Assert.Equal(expected[i].Befundkommentar, actual[i].Befundkommentar);
                Assert.Equal(expected[i].KeimID, actual[i].KeimID);
                Assert.Equal(expected[i].LabordatenID, actual[i].LabordatenID);
                Assert.Equal(expected[i].MaterialID, actual[i].MaterialID);
                Assert.Equal(expected[i].Material_l, actual[i].Material_l);
                Assert.Equal(expected[i].ProbeID, actual[i].ProbeID);
                Assert.Equal(expected[i].ZeitpunktProbeneingang.ToString("s"), actual[i].ZeitpunktProbeneingang.ToUniversalTime().ToString("s"));
                Assert.Equal(expected[i].ZeitpunktProbenentnahme.ToString("s"), actual[i].ZeitpunktProbenentnahme.ToUniversalTime().ToString("s"));
                //Assert.Equal(expected[i].Fachabteilung, actual[i].Fachabteilung); --> Exisitiert noch nicht, muss aber eingebunden werden
            }
        }

        private class LabTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                List<PatientIDs> patient = SmICSCoreLib.JSONFileStream.JSONReader<PatientIDs>.Read(@"../../../../SmICSDataGenerator.Test/Resources/GeneratedEHRIDs.json");
                for (int i = 0; i <= 17; i++)
                {
                    yield return new object[] { patient[i].EHR_ID, i };
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private List<LabDataModel> GetExpectedLabDataModels(int ResultSetID)
        {
            string path = "../../../../TestData/LabDataTestResults.json";
            List<LabDataModel> result = ExpectedResultJsonReader.ReadResults<LabDataModel>(path, ResultSetID, ExpectedType.LAB_DATA);
            return result;

        }
    }
}
