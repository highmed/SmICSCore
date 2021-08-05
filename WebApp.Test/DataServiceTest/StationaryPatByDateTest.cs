using SmICSCoreLib.AQL.Patient_Stay.Stationary;
using System.Collections.Generic;
using Xunit;
using SmICSFactory.Tests;
using SmICSDataGenerator.Tests;
using System.Collections;
using SmICSCoreLib.REST;
using System;

namespace WebApp.Test.DataServiceTest
{
    public class StationaryPatByDateTest
    {

        [Theory]
        [ClassData(typeof(StationaryTestData))]
        public void ProcessorTest(DateTime dateTime, int expectedResultSet, int ehrNo)
        {
            RestDataAccess _data = TestConnection.Initialize();

            StationaryFactory factory = new StationaryFactory(_data);
            List<StationaryDataModel> actual = factory.ProcessFromDate(dateTime);
            List<StationaryDataModel> expected = GetExpectedStationaryDataModels(expectedResultSet, ehrNo);



            Assert.Equal(expected.Count, actual.Count);
            for (int i = 0; i < expected.Count; i++)
            {
                Assert.Equal(expected[i].PatientID, actual[i].PatientID);
                Assert.Equal(expected[i].FallID, actual[i].FallID);
                Assert.Equal(expected[i].Datum_Uhrzeit_der_Aufnahme.ToUniversalTime().ToString("s"), actual[i].Datum_Uhrzeit_der_Aufnahme.ToUniversalTime().ToString("s"));
                Assert.Equal(expected[i].Datum_Uhrzeit_der_Entlassung, actual[i].Datum_Uhrzeit_der_Entlassung);
                Assert.Equal(expected[i].Versorgungsfallgrund, actual[i].Versorgungsfallgrund);
                Assert.Equal(expected[i].Aufnahmeanlass, actual[i].Aufnahmeanlass);
                Assert.Equal(expected[i].Art_der_Entlassung, actual[i].Art_der_Entlassung);
            }
        }

        private class StationaryTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                List<PatientInfos> patientInfos = SmICSCoreLib.JSONFileStream.JSONReader<PatientInfos>.Read(@"../../../../WebApp.Test/Resources/EHRID_Stationary.json");              
                int i = 0;
                yield return new object[] { patientInfos[i].Datum_Uhrzeit_der_Aufnahme, i, i };
            }
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }


        //Get Expected Data
        private List<StationaryDataModel> GetExpectedStationaryDataModels(int ResultSetID, int ehrNo)
        {
            string testResultPath = "../../../../WebApp.Test/Resources/StationaryPatByDateTestResults.json";
            string parameterPath = "../../../../WebApp.Test/Resources/EHRID_Stationary.json";

            List<StationaryDataModel> result = ExpectedResultJsonReader.ReadResults<StationaryDataModel, PatientInfos>(testResultPath, parameterPath, ResultSetID, ehrNo, ExpectedType.STATIONARY);
            return result;
        }
    }
}
