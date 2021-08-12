using Microsoft.Extensions.Logging.Abstractions;
using SmICSCoreLib.AQL.General;
using SmICSCoreLib.AQL.Patient_Stay.Count;
using SmICSCoreLib.AQL.Patient_Stay.Stationary;
using SmICSCoreLib.AQL.PatientInformation.Infection_situation;
using SmICSCoreLib.AQL.PatientInformation.Patient_Bewegung;
using SmICSCoreLib.AQL.PatientInformation.Symptome;
using SmICSCoreLib.AQL.PatientInformation.Vaccination;
using SmICSCoreLib.REST;
using SmICSCoreLib.StatistikDataModels;
using SmICSDataGenerator.Tests;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace SmICSFactory.Tests.PatientInformationTests
{   
    public class InfectionSituationTest
    {
        [Theory]
        [ClassData(typeof(InfectionSituationTestData))]
        public void ProcessorTest(int ehrNo, int expectedResultSet)
        {
            RestDataAccess _data = TestConnection.Initialize();

            List<PatientIDs> patient = SmICSCoreLib.JSONFileStream.JSONReader<PatientIDs>.Read(@"../../../../TestData/GeneratedEHRIDs.json");
            PatientListParameter patientParams = new PatientListParameter()
            {
                patientList = new List<string>() { patient[ehrNo].EHR_ID }
            };

            IPatientMovementFactory patMoveFac = new PatientMovementFactory(_data, NullLogger<PatientMovementFactory>.Instance);
            ISymptomFactory symptomFac = new SymptomFactory(_data, NullLogger<SymptomFactory>.Instance);
            IVaccinationFactory vaccFac = new VaccinationFactory(_data, NullLogger<VaccinationFactory>.Instance);
            ICountFactory countFactory = new CountFactory(_data);
            IStationaryFactory stationaryFactory = new StationaryFactory(_data);
            InfectionSituationFactory infecFac = new InfectionSituationFactory(countFactory, stationaryFactory, symptomFac, patMoveFac, vaccFac, NullLogger<InfectionSituationFactory>.Instance);

            List<Patient> actual = infecFac.Process(patientParams);
            List<Patient> expected = GetExpectedPatientModels(ehrNo, expectedResultSet);

            Assert.Equal(expected.Count, actual.Count);
            for (int i = 0; i < actual.Count; i++)
            {
                Assert.Equal(expected[i].PatientID, actual[i].PatientID);
                Assert.Equal(expected[i].Probenentnahme, actual[i].Probenentnahme);
                Assert.Equal(expected[i].Aufnahme, actual[i].Aufnahme);
                Assert.Equal(expected[i].Entlastung, actual[i].Entlastung);
                Assert.Equal(expected[i].Impfung, actual[i].Impfung);
                Assert.Equal(expected[i].Impfstoff, actual[i].Impfstoff);
                Assert.Equal(expected[i].Anzahl_Impfungen, actual[i].Anzahl_Impfungen);
            }
        }


        private class InfectionSituationTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { 1,  0 };
                yield return new object[] { 8,  1 };
                yield return new object[] { 10, 2 };
                yield return new object[] { 10, 3 };
                yield return new object[] { 9,  4 };
                yield return new object[] { 2,  5 };
                yield return new object[] { 4,  6 };
                yield return new object[] { 0,  7 };
                yield return new object[] { 7,  8 };
            }
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }


        private List<Patient> GetExpectedPatientModels(int ehrNo, int ResultSetID)
        {
            string path = "../../../../TestData/InfectionSituationTestResults.json";
            string parameterPath = "../../../../TestData/GeneratedEHRIDs.json";

            List<Patient> result = ExpectedResultJsonReader.ReadResults<Patient, PatientIDs>(path, parameterPath, ResultSetID, ehrNo, ExpectedType.PATIENT);
            return result;
        }
    }
}
