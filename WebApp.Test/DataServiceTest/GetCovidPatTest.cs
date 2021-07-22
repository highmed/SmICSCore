using System.Collections.Generic;
using Xunit;
using SmICSFactory.Tests;
using SmICSDataGenerator.Tests;
using System.Collections;
using SmICSCoreLib.REST;
using SmICSCoreLib.AQL.Patient_Stay.Count;

namespace WebApp.Test.DataServiceTest
{
    public class GetCovidPatTest
    {
        [Theory]
        [ClassData(typeof(CovidPatTestData))]
        public void ProcessorTest( string nachweis, int ResultSetID)
        {
            RestDataAccess _data = TestConnection.Initialize();

            CountFactory factory = new CountFactory(_data);
            List<CountDataModel> actual = factory.Process(nachweis);
            List<CountDataModel> expected = GetCovidPat(ResultSetID);

            Assert.Equal(expected.Count, actual.Count);

            for (int i = 0; i < 24; i++)
            {
                Assert.Equal(expected[i].PatientID, actual[i].PatientID);
                Assert.Equal(expected[i].Fallkennung, actual[i].Fallkennung);
                Assert.Equal(expected[i].Zeitpunkt_des_Probeneingangs.ToString("yyyy-MM-dd"), actual[i].Zeitpunkt_des_Probeneingangs.ToString("yyyy-MM-dd"));
            }

        }

        private class CovidPatTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                List<PatientInfos> patient = SmICSCoreLib.JSONFileStream.JSONReader<PatientInfos>.Read(@"../../../../WebApp.Test/Resources/EHRID_CovidPat.json");

                yield return new object[] { "260373001", 0 };

            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private List<CountDataModel> GetCovidPat(int ResultSetID)
        {
            string testResultPath = "../../../../WebApp.Test/Resources/CovidPatTestResults.json";
            string parameterPath = "../../../../WebApp.Test/Resources/EHRID_CovidPat.json";

            List<CountDataModel> result = ExpectedResultJsonReader.ReadResults<CountDataModel, PatientInfos>(testResultPath, parameterPath, ResultSetID, ExpectedType.COUNT_DATA_MODEL);
            return result;
        }
    }
}
