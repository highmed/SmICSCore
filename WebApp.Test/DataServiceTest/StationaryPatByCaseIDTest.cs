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
    public class StationaryPatByCaseIDTest
    {
        [Theory]
        [ClassData(typeof(StationaryTestData))]
        public void ProcessorTest(string ehrID, string fallkennung, int expectedResultSet, int ehrNo)
        {
            RestDataAccess _data = TestConnection.Initialize();
            StationaryFactory factory = new StationaryFactory(_data);
            List<StationaryDataModel> actual = factory.ProcessFromCase(ehrID, fallkennung);
            List<StationaryDataModel> expected = GetExpectedStayFromCase(expectedResultSet, ehrNo);

            Assert.Equal(expected.Count, actual.Count);
            for (int i = 0; i < expected.Count; i++)
            {
                Assert.Equal(expected[i].PatientID, actual[i].PatientID);
                Assert.Equal(expected[i].FallID, actual[i].FallID);
                //Assert.Equal(expected[i].Datum_Uhrzeit_der_Aufnahme, actual[i].Datum_Uhrzeit_der_Aufnahme);
                Assert.Equal(expected[i].Datum_Uhrzeit_der_Entlassung, actual[i].Datum_Uhrzeit_der_Entlassung);
                Assert.Equal(expected[i].Aufnahmeanlass, actual[i].Aufnahmeanlass);
                Assert.Equal(expected[i].Versorgungsfallgrund, actual[i].Versorgungsfallgrund);
            }
        }

        private class StationaryTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                List<PatientInfos> patientInfos = SmICSCoreLib.JSONFileStream.JSONReader<PatientInfos>.Read(@"../../../../WebApp.Test/Resources/EHRID_StayFromCase.json");
              
                yield return new object[] { patientInfos[0].EHR_ID, patientInfos[0].FallID, 0, 0 };
            }
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }


        //Get Expected Data
        private List<StationaryDataModel> GetExpectedStayFromCase(int ResultSetID, int ehrNo)
        {
            string testResultPath = "../../../../WebApp.Test/Resources/StationaryPatTestResults.json";
            string parameterPath = "../../../../WebApp.Test/Resources/EHRID_StayFromCase.json";

            List<StationaryDataModel> result = ExpectedResultJsonReader.ReadResults<StationaryDataModel, PatientInfos>(testResultPath, parameterPath, ResultSetID, ehrNo, ExpectedType.STATIONARY);
            return result;
        }

    }
}
