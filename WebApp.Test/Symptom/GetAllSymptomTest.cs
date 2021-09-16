
using SmICSCoreLib.Factories.Symptome;
using SmICSDataGenerator.Tests;
using System.Collections.Generic;
using SmICSFactory.Tests;
using System.Collections;
using SmICSCoreLib.REST;
using Xunit;
using Microsoft.Extensions.Logging.Abstractions;

namespace WebApp.Test.Symptom
{
    public class GetAllSymptomTest
    {
        [Fact]
        public void ProcessorTest()
        {
            RestDataAccess _data = TestConnection.Initialize();

            SymptomFactory factory = new SymptomFactory(_data, NullLogger<SymptomFactory>.Instance);
            List<SymptomModel> actual = factory.ProcessNoParam();
            List<SymptomModel> expected = GetSymptomList();

            Assert.Equal(expected.Count, actual.Count);
            for (int i = 0; i < expected.Count; i++)
            {
                Assert.Equal(expected[i].NameDesSymptoms, actual[i].NameDesSymptoms);
                Assert.Equal(expected[i].Anzahl_Patienten, actual[i].Anzahl_Patienten);
            }
        }

        private List<SymptomModel> GetSymptomList()
        {
            List<SymptomModel> symptomList = new();
            symptomList.Add(new SymptomModel("Cough (finding)", 8));
            symptomList.Add(new SymptomModel("Diarrhea (finding)", 3));
            symptomList.Add(new SymptomModel("Dyspnea (finding)", 1));
            symptomList.Add(new SymptomModel("Fatigue (finding)", 3));
            symptomList.Add(new SymptomModel("Feeling feverish (finding)", 2));
            symptomList.Add(new SymptomModel("Fever (finding)", 5));
            symptomList.Add(new SymptomModel("Fever greater than 100.4 Fahrenheit / 38° Celsius (finding)", 1));
            symptomList.Add(new SymptomModel("Nasal discharge (finding)", 2));
            symptomList.Add(new SymptomModel("Pain in throat (finding)", 1));
            symptomList.Add(new SymptomModel("Vomiting (disorder)", 4));
            return symptomList;
        }
    }
}
