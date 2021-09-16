using Microsoft.Extensions.Logging.Abstractions;
using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.Lab.ViroLabData;
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
        public void ProcessorTest(int ehrNo, int expectedResultSet)
        {
            RestDataAccess _data = TestConnection.Initialize();
            List<PatientIDs> patient = SmICSCoreLib.JSONFileStream.JSONReader<PatientIDs>.Read(@"../../../../TestData/GeneratedEHRIDs.json");

            PatientListParameter patientParams = new PatientListParameter()
            {
                patientList = new List<string>() { patient[ehrNo].EHR_ID }
            };

            ViroLabDataFactory factory = new ViroLabDataFactory(_data, NullLogger<ViroLabDataFactory>.Instance);
            List<LabDataModel> actual = factory.Process(patientParams);
            List<LabDataModel> expected = GetExpectedLabDataModels(expectedResultSet, ehrNo);

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
                Assert.Equal(expected[i].ZeitpunktProbeneingang.Value.ToString("s"), actual[i].ZeitpunktProbeneingang.Value.ToUniversalTime().ToString("s"));
                Assert.Equal(expected[i].ZeitpunktProbenentnahme.ToString("s"), actual[i].ZeitpunktProbenentnahme.ToUniversalTime().ToString("s"));
                //Assert.Equal(expected[i].Fachabteilung, actual[i].Fachabteilung); --> Exisitiert noch nicht, muss aber eingebunden werden
            }
        }

        private class LabTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                for (int i = 0; i <= 10; i++)
                {
                    yield return new object[] { i, i };
                }
                yield return new object[] { 13, 11 };
                yield return new object[] { 14, 12 };
                yield return new object[] { 15, 13 };
                for (int i = 16; i <= 37; i++)
                {
                    yield return new object[] { i, i };
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private List<LabDataModel> GetExpectedLabDataModels(int ResultSetID, int ehrNo)
        {
            string path = "../../../../TestData/LabDataTestResults.json";
            string parameterPath = "../../../../TestData/GeneratedEHRIDs.json";

            List<LabDataModel> result = ExpectedResultJsonReader.ReadResults<LabDataModel, PatientIDs>(path, parameterPath, ResultSetID, ehrNo, ExpectedType.LAB_DATA);
            return result;

        }
    }
}