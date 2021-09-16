using Microsoft.Extensions.Logging.Abstractions;
using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.Vaccination;
using SmICSCoreLib.REST;
using SmICSDataGenerator.Tests;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace SmICSFactory.Tests.PatientInformationTests
{
    public class SpecificVaccinationTest
    {
        [Theory]
        [ClassData(typeof(SpecificVaccinationTestData))]
        public void ProcessorTest(int ehrNo, int expectedResultSet)
        {
            RestDataAccess _data = TestConnection.Initialize();
            List<PatientIDs> patient = SmICSCoreLib.JSONFileStream.JSONReader<PatientIDs>.Read(@"../../../../TestData/GeneratedEHRIDs.json");

            PatientListParameter patientParams = new PatientListParameter()
            {
                patientList = new List<string>() { patient[ehrNo].EHR_ID }
            };

            VaccinationFactory factory = new VaccinationFactory(_data, NullLogger<VaccinationFactory>.Instance);
            List<VaccinationModel> actual = factory.ProcessSpecificVaccination(patientParams,"Infectious disease (disorder)" );
            List<VaccinationModel> expected = GetExpectedVaccinationModels(expectedResultSet, ehrNo);

            Assert.Equal(expected.Count, actual.Count);

            for (int i = 0; i < actual.Count; i++)
            {
                Assert.Equal(expected[i].PatientenID, actual[i].PatientenID);
                Assert.Equal(expected[i].DokumentationsID.ToString("s"), actual[i].DokumentationsID.ToUniversalTime().ToString("s"));
                Assert.Equal(expected[i].Impfstoff, actual[i].Impfstoff);
                Assert.Equal(expected[i].Dosierungsreihenfolge, actual[i].Dosierungsreihenfolge);
                Assert.Equal(expected[i].Dosiermenge, actual[i].Dosiermenge);
                Assert.Equal(expected[i].ImpfungGegen, actual[i].ImpfungGegen);
                Assert.Equal(expected[i].Abwesendheit, actual[i].Abwesendheit);
            }
        }

        private class SpecificVaccinationTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { 4 , 0 };
                yield return new object[] { 37, 1 };
                yield return new object[] { 6 , 2 };
                yield return new object[] { 9 , 3 };
                yield return new object[] { 2 , 4 };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private List<VaccinationModel> GetExpectedVaccinationModels(int ResultSetID, int ehrNo)
        {
            string path = "../../../../TestData/SpecificVaccinationTestResults.json";
            string parameterPath = "../../../../TestData/GeneratedEHRIDs.json";

            List<VaccinationModel> result = ExpectedResultJsonReader.ReadResults<VaccinationModel, PatientIDs>(path, parameterPath, ResultSetID, ehrNo, ExpectedType.PATIENT_VACCINATION);
            return result;
        }
    }
}
