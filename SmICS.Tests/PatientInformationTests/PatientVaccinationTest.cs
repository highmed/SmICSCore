using Autofac.Extras.Moq;
using Microsoft.Extensions.Logging.Abstractions;
using SmICSCoreLib.AQL;
using SmICSCoreLib.AQL.General;
using SmICSCoreLib.AQL.PatientInformation.Vaccination;
using SmICSCoreLib.REST;
using SmICSFactory.Tests;
using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;


namespace SmICSDataGenerator.Tests.PatientInformationTests
{
    public class PatientVaccinationTest
    {
        [Theory]
        [ClassData(typeof(PatientVaccinationTestData))]
        public void ProcessorTest(string ehrID, int ResultSetID) 
        {
            RestDataAccess _data = TestConnection.Initialize();

            PatientListParameter patientParams = new PatientListParameter()
            {
                patientList = new List<string>() { ehrID }
            };

            VaccinationFactory factory = new VaccinationFactory(_data, NullLogger<VaccinationFactory>.Instance);
            List<VaccinationModel> actual = factory.Process(patientParams);
            List<VaccinationModel> expected = GetExpectedVaccinationModels(ResultSetID);

            Assert.Equal(expected.Count, actual.Count);

            for (int i = 0; i < actual.Count; i++)
            {
                Assert.Equal(expected[i].PatientenID, actual[i].PatientenID);
                Assert.Equal(expected[i].DokumentationsID.ToUniversalTime(), actual[i].DokumentationsID.ToUniversalTime());
                Assert.Equal(expected[i].Impfstoff, actual[i].Impfstoff);
                Assert.Equal(expected[i].Dosierungsreihenfolge, actual[i].Dosierungsreihenfolge);
                Assert.Equal(expected[i].Dosiermenge, actual[i].Dosiermenge);
                Assert.Equal(expected[i].Impfung_gegen, actual[i].Impfung_gegen);
                Assert.Equal(expected[i].Abwesendheit, actual[i].Abwesendheit);
            }
        }

        private class PatientVaccinationTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                List<PatientIDs> patient = SmICSCoreLib.JSONFileStream.JSONReader<PatientIDs>.Read(@"../../../../SmICSDataGenerator.Test/Resources/GeneratedEHRIDs.json");
                
                yield return new object[] { patient[3].EHR_ID, 3 };
                yield return new object[] { patient[6].EHR_ID, 6 };
                yield return new object[] { patient[8].EHR_ID, 8 };
                yield return new object[] { patient[14].EHR_ID, 14 };
                yield return new object[] { patient[15].EHR_ID, 15 };
                yield return new object[] { patient[16].EHR_ID, 16 };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private List<VaccinationModel> GetExpectedVaccinationModels(int ResultSetID)
        {
            string path = "../../../../TestData/PatientVaccinationTestResults.json";
            List<VaccinationModel> result = ExpectedResultJsonReader.ReadResults<VaccinationModel>(path, ResultSetID, ExpectedType.PATIENT_VACCINATION);
            return result;
        }

    }
}
