using Microsoft.Extensions.Logging.Abstractions;
using SmICSCoreLib.AQL.General;
using SmICSCoreLib.AQL.PatientInformation.Symptome;
using SmICSCoreLib.REST;
using SmICSFactory.Tests;
using System.Collections;
using System.Collections.Generic;
using Xunit;


namespace SmICSDataGenerator.Tests.PatientInformationTests
{
    public class PatientSymptomTest
    {
        [Theory]
        [ClassData(typeof(PatientSymptomTestData))]
        public void ProcessorTest(int ehrNo, int expectedResultSet)
        {
            RestDataAccess _data = TestConnection.Initialize();
            List<PatientIDs> patient = SmICSCoreLib.JSONFileStream.JSONReader<PatientIDs>.Read(@"../../../../TestData/GeneratedEHRIDs.json");

            PatientListParameter patientParams = new PatientListParameter()
            {
                patientList = new List<string>() { patient[ehrNo].EHR_ID }
            };

            SymptomFactory factory = new SymptomFactory(_data, NullLogger<SymptomFactory>.Instance);
            List<SymptomModel> actual = factory.Process(patientParams);
            List<SymptomModel> expected = GetExpectedSymptomModels(expectedResultSet, ehrNo);

            Assert.Equal(expected.Count, actual.Count);

            for (int i = 0; i < actual.Count; i++)
            {
                Assert.Equal(expected[i].PatientenID, actual[i].PatientenID);
                Assert.Equal(expected[i].BefundDatum.ToString("s"), actual[i].BefundDatum.ToUniversalTime().ToString("s"));
                Assert.Equal(expected[i].NameDesSymptoms, actual[i].NameDesSymptoms);
                Assert.Equal(expected[i].Lokalisation, actual[i].Lokalisation);
                Assert.Equal(expected[i].Beginn.ToString("s"), actual[i].Beginn.ToUniversalTime().ToString("s"));
                Assert.Equal(expected[i].Schweregrad, actual[i].Schweregrad);
                Assert.Equal(expected[i].Rueckgang.ToString("s"), actual[i].Rueckgang.ToUniversalTime().ToString("s"));
                Assert.Equal(expected[i].AusschlussAussage, actual[i].AusschlussAussage);
                Assert.Equal(expected[i].Diagnose, actual[i].Diagnose);
                Assert.Equal(expected[i].UnbekanntesSymptom, actual[i].UnbekanntesSymptom);
                Assert.Equal(expected[i].AussageFehlendeInfo, actual[i].AussageFehlendeInfo);
            }
        }

        private class PatientSymptomTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {

                for (int i = 0; i <= 15; i++)
                {
                    yield return new object[] { i, i };
                }

            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private List<SymptomModel> GetExpectedSymptomModels(int ResultSetID, int ehrNo)
        {
            string path = "../../../../TestData/PatientSymptomTestResults.json";
            string parameterPath = "../../../../TestData/GeneratedEHRIDs.json";

            List<SymptomModel> result = ExpectedResultJsonReader.ReadResults<SymptomModel, PatientIDs>(path, parameterPath, ResultSetID, ehrNo, ExpectedType.PATIENT_SYMPTOM);
            return result;
        }

    }
}
