using Microsoft.Extensions.Logging.Abstractions;
using SmICSCoreLib.AQL.General;
using SmICSCoreLib.AQL.PatientInformation.Vaccination;
using SmICSCoreLib.REST;
using SmICSFactory.Tests;
using System.Collections;
using System.Collections.Generic;
using Xunit;


namespace SmICSDataGenerator.Tests.PatientInformationTests
{
    public class PatientVaccinationTest
    {
        [Theory]
        [ClassData(typeof(PatientVaccinationTestData))]
        public void ProcessorTest(string ehrID, int expectedResultSet) 
        {
            RestDataAccess _data = TestConnection.Initialize();

            PatientListParameter patientParams = new PatientListParameter()
            {
                patientList = new List<string>() { ehrID }
            };

            VaccinationFactory factory = new VaccinationFactory(_data, NullLogger<VaccinationFactory>.Instance);
            List<VaccinationModel> actual = factory.Process(patientParams);
            List<VaccinationModel> expected = GetExpectedVaccinationModels(expectedResultSet);

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

        private class PatientVaccinationTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                List<PatientIDs> patient = SmICSCoreLib.JSONFileStream.JSONReader<PatientIDs>.Read(@"../../../../TestData/GeneratedEHRIDs.json");
                
                yield return new object[] { patient[11].EHR_ID, 11 };
                yield return new object[] { patient[5].EHR_ID, 5 };
                yield return new object[] { patient[7].EHR_ID, 7 };
                yield return new object[] { patient[13].EHR_ID, 13 };
                yield return new object[] { patient[14].EHR_ID, 14 };
                yield return new object[] { patient[15].EHR_ID, 15 };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private List<VaccinationModel> GetExpectedVaccinationModels(int ResultSetID)
        {
            string path = "../../../../TestData/PatientVaccinationTestResults.json";
            string parameterPath = "../../../../TestData/GeneratedEHRIDs.json";

            List<VaccinationModel> result = ExpectedResultJsonReader.ReadResults<VaccinationModel, PatientIDs>(path, parameterPath, ResultSetID, ExpectedType.PATIENT_VACCINATION);
            return result;
        }

    }
}
