using Microsoft.Extensions.Logging.Abstractions;
using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.Symptome;
using SmICSCoreLib.REST;
using SmICSFactory.Tests;
using System.Collections;
using System.Collections.Generic;
using Xunit;
using System.Text.RegularExpressions;


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
                Assert.Equal(expected[i].NameDesSymptoms == null ? null : Regex.Replace(expected[i].NameDesSymptoms, @"\s", ""), actual[i].NameDesSymptoms == null ? null : Regex.Replace(actual[i].NameDesSymptoms, @"\s", ""));
                Assert.Equal(expected[i].Lokalisation == null ? null : Regex.Replace(expected[i].Lokalisation, @"\s", ""), actual[i].Lokalisation == null ? null : Regex.Replace(actual[i].Lokalisation, @"\s", ""));
                Assert.Equal(expected[i].Beginn == null ? null : expected[i].Beginn.Value.ToString("s"), actual[i].Beginn == null ? null : actual[i].Beginn.Value.ToUniversalTime().ToString("s"));
                Assert.Equal(expected[i].Schweregrad == null ? null : Regex.Replace(expected[i].Schweregrad, @"\s", ""), actual[i].Schweregrad == null ? null : Regex.Replace(actual[i].Schweregrad, @"\s", ""));
                Assert.Equal(expected[i].Rueckgang == null ? null : expected[i].Rueckgang.Value.ToString("s"), actual[i].Rueckgang == null ? null : actual[i].Rueckgang.Value.ToUniversalTime().ToString("s"));
                Assert.Equal(expected[i].AusschlussAussage == null ? null : Regex.Replace(expected[i].AusschlussAussage, @"\s", ""), actual[i].AusschlussAussage == null ? null : Regex.Replace(actual[i].AusschlussAussage, @"\s", ""));
                Assert.Equal(expected[i].Diagnose == null ? null : Regex.Replace(expected[i].Diagnose, @"\s", ""), actual[i].Diagnose == null ? null : Regex.Replace(actual[i].Diagnose, @"\s", ""));
                Assert.Equal(expected[i].UnbekanntesSymptom == null ? null : Regex.Replace(expected[i].UnbekanntesSymptom, @"\s", ""), actual[i].UnbekanntesSymptom == null ? null : Regex.Replace(actual[i].UnbekanntesSymptom, @"\s", ""));
                Assert.Equal(expected[i].AussageFehlendeInfo == null ? null : Regex.Replace(expected[i].AussageFehlendeInfo, @"\s", ""), actual[i].AussageFehlendeInfo == null ? null : Regex.Replace(actual[i].AussageFehlendeInfo, @"\s", ""));
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