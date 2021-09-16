using Microsoft.Extensions.Logging.Abstractions;
using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.Vaccination;
using SmICSCoreLib.REST;
using SmICSFactory.Tests;
using System.Collections;
using System.Collections.Generic;
using Xunit;
using System.Text.RegularExpressions;


namespace SmICSDataGenerator.Tests.PatientInformationTests
{
    public class PatientVaccinationTest
    {
        [Theory]
        [ClassData(typeof(PatientVaccinationTestData))]
        public void ProcessorTest(int ehrNo, int expectedResultSet)
        {
            RestDataAccess _data = TestConnection.Initialize();
            List<PatientIDs> patient = SmICSCoreLib.JSONFileStream.JSONReader<PatientIDs>.Read(@"../../../../TestData/GeneratedEHRIDs.json");

            PatientListParameter patientParams = new PatientListParameter()
            {
                patientList = new List<string>() { patient[ehrNo].EHR_ID }
            };

            VaccinationFactory factory = new VaccinationFactory(_data, NullLogger<VaccinationFactory>.Instance);
            List<VaccinationModel> actual = factory.Process(patientParams);
            List<VaccinationModel> expected = GetExpectedVaccinationModels(expectedResultSet, ehrNo);

            Assert.Equal(expected.Count, actual.Count);

            for (int i = 0; i < actual.Count; i++)
            {
                Assert.Equal(expected[i].PatientenID, actual[i].PatientenID);
                Assert.Equal(expected[i].DokumentationsID.ToString("s"), actual[i].DokumentationsID.ToUniversalTime().ToString("s"));
                Assert.Equal(Regex.Replace(expected[i].Impfstoff, @"\s", ""), Regex.Replace(actual[i].Impfstoff, @"\s", ""));
                Assert.Equal(expected[i].Dosierungsreihenfolge.ToString(), actual[i].Dosierungsreihenfolge);
                Assert.Equal(expected[i].Dosiermenge.ToString(), actual[i].Dosiermenge);
                Assert.Equal(Regex.Replace(expected[i].ImpfungGegen, @"\s", ""), Regex.Replace(actual[i].ImpfungGegen, @"\s", ""));
                Assert.Equal(expected[i].Abwesendheit == null ? null : Regex.Replace(expected[i].Abwesendheit, @"\s", ""), actual[i].Abwesendheit == null ? null : Regex.Replace(actual[i].Abwesendheit, @"\s", ""));
            }
        }

        private class PatientVaccinationTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { 7, 0 };
                yield return new object[] { 5, 1 };
                yield return new object[] { 14, 2 };
                yield return new object[] { 3, 3 };
                yield return new object[] { 13, 4 };
                yield return new object[] { 15, 5 };
                yield return new object[] { 37, 6 };
                yield return new object[] { 2, 7 };
                yield return new object[] { 4, 8 };
                yield return new object[] { 6, 9 };
                yield return new object[] { 9, 10 };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private List<VaccinationModel> GetExpectedVaccinationModels(int ResultSetID, int ehrNo)
        {
            string path = "../../../../TestData/PatientVaccinationTestResults.json";
            string parameterPath = "../../../../TestData/GeneratedEHRIDs.json";

            List<VaccinationModel> result = ExpectedResultJsonReader.ReadResults<VaccinationModel, PatientIDs>(path, parameterPath, ResultSetID, ehrNo, ExpectedType.PATIENT_VACCINATION);
            return result;
        }

    }
}
