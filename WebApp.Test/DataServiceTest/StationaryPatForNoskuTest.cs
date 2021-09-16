using SmICSCoreLib.Factories.PatientStay.Stationary;
using System.Collections.Generic;
using Xunit;
using SmICSFactory.Tests;
using SmICSDataGenerator.Tests;
using System.Collections;
using SmICSCoreLib.REST;
using System;

namespace WebApp.Test.DataServiceTest
{
    public class StationaryPatForNoskuTest
    {

        [Theory]
        [ClassData(typeof(StationaryTestData))]
        public void ProcessorTest(string ehrID, string fallkennung, DateTime dateTime, int expectedResultSet, int ehrNo)
        {
            RestDataAccess _data = TestConnection.Initialize();

            StationaryFactory factory = new StationaryFactory(_data);
            List<StationaryDataModel> actual = factory.Process(ehrID, fallkennung, dateTime);
            List<StationaryDataModel> expected = GetExpectedStationaryDataModels(expectedResultSet, ehrNo);

            Assert.Equal(expected.Count, actual.Count);
            for (int i = 0; i < expected.Count; i++)
            {
                Assert.Equal(expected[i].PatientID, actual[i].PatientID);
                Assert.Equal(expected[i].FallID, actual[i].FallID);
                Assert.Equal(expected[i].Datum_Uhrzeit_der_Aufnahme, actual[i].Datum_Uhrzeit_der_Aufnahme);
                Assert.Equal(expected[i].Datum_Uhrzeit_der_Entlassung, actual[i].Datum_Uhrzeit_der_Entlassung);
                Assert.Equal(expected[i].Aufnahmeanlass, actual[i].Aufnahmeanlass);
                Assert.Equal(expected[i].Art_der_Entlassung, actual[i].Art_der_Entlassung);
                Assert.Equal(expected[i].Versorgungsfallgrund, actual[i].Versorgungsfallgrund);
            }

        }

        private class StationaryTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                List<PatientIDs> patient = SmICSCoreLib.JSONFileStream.JSONReader<PatientIDs>.Read(@"../../../../TestData/GeneratedEHRIDs.json");

                string  fallkennung="00000020";
                DateTime datum = Convert.ToDateTime("2020-12-02T12:40:33Z");
                yield return new object[] { patient[1].EHR_ID, fallkennung, datum, 0, 1 }; 
            }
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }


        //Get Expected Data
        private List<StationaryDataModel> GetExpectedStationaryDataModels(int ResultSetID, int ehrNo)
        {
            string testResultPath = "../../../../WebApp.Test/Resources/StationaryPatForNosuResults.json";
            string parameterPath = "../../../../TestData/GeneratedEHRIDs.json";

            List<StationaryDataModel> result = ExpectedResultJsonReader.ReadResults<StationaryDataModel, PatientIDs>(testResultPath, parameterPath, ResultSetID, ehrNo, ExpectedType.STATIONARY);
            return result;
        }
    }
}
