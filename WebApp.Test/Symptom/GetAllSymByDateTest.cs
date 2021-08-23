﻿using SmICSFactory.Tests;
using System;
using System.Collections;
using System.Collections.Generic;
using SmICSCoreLib.AQL.PatientInformation.Symptome;
using Xunit;
using SmICSCoreLib.REST;
using SmICSDataGenerator.Tests;
using Microsoft.Extensions.Logging.Abstractions;

namespace WebApp.Test.Symptom
{
    public class GetAllSymByDateTest
    {
        [Theory]
        [ClassData(typeof(SymptomTestData))]
        public void ProcessorTest(string ehrID, DateTime beginn, int ResultSetID, int ehrNo)
        {
            RestDataAccess _data = TestConnection.Initialize();

            SymptomFactory factory = new SymptomFactory(_data, NullLogger<SymptomFactory>.Instance);
            List<SymptomModel> actual = factory.SymptomByPatient(ehrID, beginn);
            List<SymptomModel> expected = GetSymptom(ResultSetID, ehrNo);

            Assert.Equal(expected.Count, actual.Count);

            for (int i = 0; i < 3; i++)
            {
                Assert.Equal(expected[i].PatientenID, actual[i].PatientenID);
                Assert.Equal(expected[i].Beginn == null ? null : expected[i].Beginn.Value.ToString("yyyy.MM.dd"), actual[i].Beginn == null ? null : actual[i].Beginn.Value.ToUniversalTime().ToString("yyyy.MM.dd"));
                Assert.Equal(expected[i].Rueckgang == null ? null : expected[i].Rueckgang.Value.ToString("yyyy.MM.dd"), actual[i].Rueckgang == null ? null : actual[i].Rueckgang.Value.ToUniversalTime().ToString("yyyy.MM.dd"));
                Assert.Equal(expected[i].NameDesSymptoms, actual[i].NameDesSymptoms);
            }

        }

        private class SymptomTestData : IEnumerable<object[]>
        {
            List<PatientInfos> patient = SmICSCoreLib.JSONFileStream.JSONReader<PatientInfos>.Read(@"../../../../WebApp.Test/Resources/EHRID_Symptome.json");
            public IEnumerator<object[]> GetEnumerator()
            {
                 yield return new object[] { patient[0].EHR_ID, patient[0].Beginn, 0, 0 };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private List<SymptomModel> GetSymptom(int ResultSetID, int ehrNo)
        {
            string testResultPath = "../../../../WebApp.Test/Resources/SymptomTestResults.json";
            string parameterPath = "../../../../WebApp.Test/Resources/EHRID_Symptome.json";

            List<SymptomModel> result = ExpectedResultJsonReader.ReadResults<SymptomModel, PatientInfos>(testResultPath, parameterPath, ResultSetID, ehrNo, ExpectedType.SYMPTOM_MODEL);
            return result;
        }
    }
}
