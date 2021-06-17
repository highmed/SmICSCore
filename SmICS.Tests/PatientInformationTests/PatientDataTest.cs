using SmICSCoreLib.AQL.PatientInformation.PatientData;
using System.Collections;
using System.Collections.Generic;
using Xunit;
namespace SmICSFactory.Tests.PatientInformationTests
{
    public class PatientDataTest
    {
        [Theory]
        [ClassData(typeof(PatientTestData))]
        public void ProcessTest(string PatientID)
        {
            PatientDataFactory factory = new PatientDataFactory();
            PatientData actual = factory.Process(PatientID);

            PatientData expected = GetExpectedPatientData();
            Assert.NotNull(actual);
            Assert.Equal(expected.PatientID, actual.PatientID);
            Assert.Equal(expected.Name, actual.Name);
            Assert.Equal(expected.Birthdate, actual.Birthdate);
            Assert.Equal(expected.Sex, actual.Sex);
        }

        private class PatientTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                //List<PatientIDs> patient = SmICSCoreLib.JSONFileStream.JSONReader<PatientIDs>.Read(@"../../../../TestData/GeneratedEHRIDs.json");
                yield return new object[] { "4100017667" };
                
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private PatientData GetExpectedPatientData()
        {
            return new PatientData
            {
                Birthdate = new System.DateTime(2000, 3, 18),
                Name = "IHE-Achzehnda Achzehnda da Freifrauda aus demda \0da da ",
                PatientID = "4100017667",
                Sex = "F"
            };
        }
    }
}
