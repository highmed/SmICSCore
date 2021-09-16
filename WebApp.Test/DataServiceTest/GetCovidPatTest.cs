using System.Collections.Generic;
using Xunit;
using SmICSFactory.Tests;
using SmICSDataGenerator.Tests;
using SmICSCoreLib.REST;
using SmICSCoreLib.Factories.PatientStay.Count;

namespace WebApp.Test.DataServiceTest
{
    public class GetCovidPatTest
    {
        [Fact]
        public void ProcessorTest()
        {
            RestDataAccess _data = TestConnection.Initialize();

            CountFactory factory = new CountFactory(_data);
            List<CountDataModel> actual = factory.Process("260373001");
            List<CountDataModel> expected = GetCovidPat(0,0);

            Assert.Equal(expected.Count, actual.Count);

            for (int i = 0; i < expected.Count; i++)
            {
                Assert.Equal(expected[i].PatientID, actual[i].PatientID);
                Assert.Equal(expected[i].Fallkennung, actual[i].Fallkennung);
                Assert.Equal(expected[i].Zeitpunkt_des_Probeneingangs.ToString("yyyy-MM-dd"), actual[i].Zeitpunkt_des_Probeneingangs.ToString("yyyy-MM-dd"));
            }
        }

        private List<CountDataModel> GetCovidPat(int ResultSetID, int ehrNo)
        {
            string testResultPath = "../../../../WebApp.Test/Resources/CovidPatTestResults.json";
            string parameterPath = "../../../../TestData/GeneratedEHRIDs.json";

            List<CountDataModel> result = ExpectedResultJsonReader.ReadResults<CountDataModel, PatientInfos>(testResultPath, parameterPath, ResultSetID, ehrNo, ExpectedType.COUNT_DATA_MODEL);
            return result;
        }
    }
}
