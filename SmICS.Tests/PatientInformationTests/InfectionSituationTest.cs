using Microsoft.Extensions.Logging.Abstractions;
using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.PatientStay.Count;
using SmICSCoreLib.Factories.PatientStay.Stationary;
using SmICSCoreLib.Factories.InfectionSituation;
using SmICSCoreLib.Factories.PatientMovement;
using SmICSCoreLib.Factories.Symptome;
using SmICSCoreLib.Factories.Vaccination;
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
                patientList = new List<string>() { patient[ehrNo].Patient }
            };

            IPatientMovementFactory patMoveFac = new PatientMovementFactory(_data, NullLogger<PatientMovementFactory>.Instance);
            ISymptomFactory symptomFac = new SymptomFactory(_data, NullLogger<SymptomFactory>.Instance);
            IVaccinationFactory vaccFac = new VaccinationFactory(_data, NullLogger<VaccinationFactory>.Instance);
            ICountFactory countFactory = new CountFactory(_data);
            IStationaryFactory stationaryFactory = new StationaryFactory(_data);
            InfectionSituationFactory infecFac = new InfectionSituationFactory(countFactory, stationaryFactory, symptomFac, patMoveFac, vaccFac, NullLogger<InfectionSituationFactory>.Instance);

            List<PatientModel> actual = infecFac.Process(patientParams);
            List<PatientModel> expected = GetExpectedPatientModels(ehrNo, expectedResultSet);

            Assert.Equal(expected.Count, actual.Count);
            for (int i = 0; i < actual.Count; i++)
            {
                Assert.Equal(expected[i].PatientID, actual[i].PatientID);
                Assert.Equal(expected[i].Probenentnahme.ToUniversalTime().ToString("s"), actual[i].Probenentnahme.ToUniversalTime().ToString("s"));
                Assert.Equal(expected[i].Aufnahme.ToUniversalTime().ToString("s"), actual[i].Aufnahme.ToUniversalTime().ToString("s"));
                Assert.Equal(expected[i].Entlastung.ToUniversalTime().ToString("s"), actual[i].Entlastung.ToUniversalTime().ToString("s"));
                if ( actual[i].VaccinationModel != null)
                {
                    for (int j = 0; j < actual[i].VaccinationModel.Count; j++)
                    {
                        Assert.Equal(expected[i].VaccinationModel[j].PatientenID, actual[i].VaccinationModel[j].PatientenID);
                        Assert.Equal(expected[i].VaccinationModel[j].DokumentationsID.ToUniversalTime().ToString("s"), actual[i].VaccinationModel[j].DokumentationsID.ToUniversalTime().ToString("s"));
                        Assert.Equal(expected[i].VaccinationModel[j].Impfstoff, actual[i].VaccinationModel[j].Impfstoff);
                        Assert.Equal(expected[i].VaccinationModel[j].Dosierungsreihenfolge, actual[i].VaccinationModel[j].Dosierungsreihenfolge);
                        Assert.Equal(expected[i].VaccinationModel[j].Dosiermenge, actual[i].VaccinationModel[j].Dosiermenge);
                        Assert.Equal(expected[i].VaccinationModel[j].ImpfungGegen, actual[i].VaccinationModel[j].ImpfungGegen);
                        Assert.Equal(expected[i].VaccinationModel[j].Abwesendheit, actual[i].VaccinationModel[j].Abwesendheit);
                    }
                }
            }
        }


        private class InfectionSituationTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { 1,  0 };
                yield return new object[] { 8,  1 };
                yield return new object[] { 10, 2 };
                yield return new object[] { 9,  3 };
                yield return new object[] { 2,  4 };
                yield return new object[] { 4,  5 };
                yield return new object[] { 0,  6 };
                yield return new object[] { 7,  7 };
            }
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }


        private List<PatientModel> GetExpectedPatientModels(int ehrNo, int ResultSetID)
        {
            string path = "../../../../TestData/InfectionSituationTestResults.json";
            string parameterPath = "../../../../TestData/GeneratedEHRIDs.json";

            List<PatientModel> result = ExpectedResultJsonReader.ReadResults<PatientModel, PatientIDs>(path, parameterPath, ResultSetID, ehrNo, ExpectedType.PATIENT);
            return result;
        }
    }
}
