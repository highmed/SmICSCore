﻿using SmICSCoreLib.AQL.Patient_Stay.Stationary;
using System.Collections.Generic;
using Xunit;
using SmICSFactory.Tests;
using SmICSDataGenerator.Tests;
using System.Collections;
using SmICSCoreLib.REST;
using System;

namespace WebApp.Test.DataServiceTest
{
    public class GetStationaryPatTest
    {

        [Theory]
        [ClassData(typeof(StationaryTestData))]
        public void ProcessorTest(string ehrID, string fallkennung, DateTime dateTime, int expectedResultSet)
        {
            RestDataAccess _data = TestConnection.Initialize();

            StationaryFactory factory = new StationaryFactory(_data);
            List<StationaryDataModel> actual = factory.Process(ehrID, fallkennung, dateTime);
            List<StationaryDataModel> expected = GetExpectedStationaryDataModels(expectedResultSet);

            int i = 0;
            Assert.Equal(expected.Count, actual.Count);
            Assert.Equal(expected[i].PatientID, actual[i].PatientID);
            Assert.Equal(expected[i].FallID, actual[i].FallID);
            Assert.Equal(expected[i].Station, actual[i].Station);
            Assert.Equal(expected[i].Datum_Uhrzeit_der_Entlassung, actual[i].Datum_Uhrzeit_der_Entlassung);
            Assert.Equal(expected[i].Aufnahmeanlass, actual[i].Aufnahmeanlass);

            //Assert.Equal(expected[i].Datum_Uhrzeit_der_Aufnahme, actual[i].Datum_Uhrzeit_der_Aufnahme);
            //Assert.Equal(expected[i].Versorgungsfallgrund, actual[i].Versorgungsfallgrund);
            //Assert.Equal(expected[i].Versorgungsfallgrund, actual[i].Versorgungsfallgrund);


        }

        private class StationaryTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                List<PatientInfos> patientInfos = SmICSCoreLib.JSONFileStream.JSONReader<PatientInfos>.Read(@"../../../../WebApp.Test/Resources/EHRID_StayFromCase.json");
                for (int i = 0; i <= 1; i++)
                {
                    yield return new object[] { patientInfos[i].EHR_ID, patientInfos[i].FallID, patientInfos[i].Datum_Uhrzeit_der_Aufnahme, i };
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }


        //Get Expected Data
        private List<StationaryDataModel> GetExpectedStationaryDataModels(int ResultSetID)
        {
            string testResultPath = "../../../../WebApp.Test/Resources/StationaryPatTestResults.json";
            string parameterPath = "../../../../WebApp.Test/Resources/EHRID_StayFromCase.json";

            List<StationaryDataModel> result = ExpectedResultJsonReader.ReadResults<StationaryDataModel, PatientInfos>(testResultPath, parameterPath, ResultSetID, ExpectedType.STATIONARY);
            return result;
        }
    }
}