using SmICSCoreLib.Factories.Symptome;
using SmICSDataGenerator.Tests;
using System.Collections.Generic;
using SmICSFactory.Tests;
using System.Collections;
using SmICSCoreLib.REST;
using SmICSCoreLib.StatistikServices;
using Xunit;
using SmICSCoreLib.Factories;
using SmICSCoreLib.Factories.PatientStay;
using SmICSCoreLib.Factories.PatientMovement;
using SmICSCoreLib.Factories.Lab.ViroLabData;
using Microsoft.Extensions.Logging.Abstractions;
using SmICSCoreLib.Factories.Lab.MibiLabData;
using SmICSCoreLib.Factories.PatientStay.Stationary;
using SmICSCoreLib.Factories.PatientStay.Count;
using System;
using SmICSCoreLib.Factories.Vaccination;
using SmICSCoreLib.Factories.InfectionSituation;

namespace WebApp.Test.Symptom
{
    public class GetAllSymGroupTest
    {
        [Theory]
        [ClassData(typeof(SymptomTestData))]
        public void ProcessorTest(DateTime datum, int min)
        {
            RestDataAccess _data = TestConnection.Initialize();

            IPatientMovementFactory patientMoveFac = new PatientMovementFactory(_data, NullLogger<PatientMovementFactory>.Instance);
            ISymptomFactory symptomFac = new SymptomFactory(_data, NullLogger<SymptomFactory>.Instance);
            IPatientStay patientStay = CreatePatientStay(_data); ;
            EhrDataService dataService = new(patientStay, patientMoveFac, symptomFac, NullLogger<EhrDataService>.Instance);
            SymptomService symptomService = new (symptomFac, dataService);

            Dictionary<string, Dictionary<string, int>> actual = symptomService.GetAllSymGroup(datum, min);
            Dictionary<string, Dictionary<string, int>> expected = GetSymptomList();

            Assert.Equal(expected.Count, actual.Count);
            Assert.Equal(expected.Keys, actual.Keys);
            Assert.Equal(expected.Values, actual.Values);          
        }

        private PatientStay CreatePatientStay(IRestDataAccess rest)
        {
            IStationaryFactory statFac = new StationaryFactory(rest);
            ICountFactory CountFac = new CountFactory(rest);

            return new PatientStay(statFac, CountFac);
        }

        private class SymptomTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                DateTime beginn = Convert.ToDateTime("2020-02-13");
                yield return new object[] { beginn, 3 };
            }
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private Dictionary<string, Dictionary<string, int>> GetSymptomList()
        {
            Dictionary<string, Dictionary<string, int>> symptomList =  new Dictionary<string,Dictionary<string, int>>()
            {
                { "Cough (finding)",     new Dictionary<string, int>()  {{ "Stationskennung X", 7 }} },                  
                { "Diarrhea (finding)",  new Dictionary<string, int>()  {{ "Stationskennung X", 3 }} },
                { "Fatigue (finding)",   new Dictionary<string, int>()  {{ "Stationskennung X", 3 }} },
                { "Fever (finding)",     new Dictionary<string, int>()  {{ "Stationskennung X", 4 }} },
                { "Vomiting (disorder)", new Dictionary<string, int>()  {{ "Stationskennung X", 4 }} },
            };
            return symptomList;
        }
    }
}
