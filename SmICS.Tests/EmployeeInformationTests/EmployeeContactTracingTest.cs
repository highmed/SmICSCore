using Autofac.Extras.Moq;
using Microsoft.Extensions.Logging.Abstractions;
using SmICSCoreLib.AQL;
using SmICSCoreLib.AQL.General;
using SmICSCoreLib.AQL.Employees.ContactTracing;
using SmICSCoreLib.REST;
using SmICSFactory.Tests;
using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;


namespace SmICSDataGenerator.Tests.EmployeeInformationTests
{
    public class EmployeeContactTracingTest
    {
        [Theory]
        [ClassData(typeof(EmployeeContactTracingTestData))]
        public void ProcessorTest(string ehrID, int ResultSetID, int ehrNo) 
        {
            //RestDataAccess _data = TestConnection.Initialize();

            //PatientListParameter patientParams = new PatientListParameter()
            //{
            //    patientList = new List<string>() { ehrID }
            //};

            //ContactTracingFactory factory = new ContactTracingFactory(_data);
            //List<ContactTracingModel> actual = factory.Process(patientParams);
            //List<ContactTracingModel> expected = GetExpectedContactTracingModels(ResultSetID, ehrNo);

            //Assert.Equal(expected.Count, actual.Count);

            //for (int i = 0; i < actual.Count; i++)
            //{
            //    Assert.Equal(expected[i].bericht_id, actual[i].bericht_id);
            //    Assert.Equal(expected[i].dokumentations_id, actual[i].dokumentations_id);
            //    Assert.Equal(expected[i].event_kennung, actual[i].event_kennung);
            //    Assert.Equal(expected[i].event_art, actual[i].event_art);
            //    Assert.Equal(expected[i].art_der_person_1, actual[i].art_der_person_1);
            //    Assert.Equal(expected[i].art_der_person_1_ID, actual[i].art_der_person_1_ID);
            //    Assert.Equal(expected[i].art_der_person_2, actual[i].art_der_person_2);
            //    Assert.Equal(expected[i].art_der_person_2_ID, actual[i].art_der_person_2_ID);
            //    Assert.Equal(expected[i].event_kategorie, actual[i].event_kategorie);
            //    Assert.Equal(expected[i].kontakt_kommentar, actual[i].kontakt_kommentar);
            //    Assert.Equal(expected[i].beschreibung, actual[i].beschreibung);
            //    Assert.Equal(expected[i].beginn, actual[i].beginn);
            //    Assert.Equal(expected[i].ende, actual[i].ende);
            //    Assert.Equal(expected[i].ort, actual[i].ort);
            //    Assert.Equal(expected[i].gesamtdauer, actual[i].gesamtdauer);
            //    Assert.Equal(expected[i].abstand, actual[i].abstand);
            //    Assert.Equal(expected[i].schutzkleidung, actual[i].schutzkleidung);
            //    Assert.Equal(expected[i].person, actual[i].person);
            //    Assert.Equal(expected[i].kommentar, actual[i].kommentar);
            //}
        }

        private class EmployeeContactTracingTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                List<PatientIDs> patient = SmICSCoreLib.JSONFileStream.JSONReader<PatientIDs>.Read(@"../../../../TestData/GeneratedEHRIDs.json");
                for (int i = 0; i <= 2; i++)
                {
                    yield return new object[] { patient[i].EHR_ID, i,  i };
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private List<ContactTracingModel> GetExpectedContactTracingModels(int ResultSetID, int ehrNo)
        {
            string path = "../../../../TestData/EmployeeContactTracinTestResults.json";
            string parameterPath = "../../../../TestData/GeneratedEHRIDs.json";

            List<ContactTracingModel> result = ExpectedResultJsonReader.ReadResults<ContactTracingModel, PatientIDs>(path, parameterPath, ResultSetID, ehrNo, ExpectedType.PATIENT_SYMPTOM);
            return result;
        }

    }
}
