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
using SmICSCoreLib.Factories.Vaccination;
using System;
using SmICSCoreLib.Factories.InfectionSituation;

namespace WebApp.Test.Symptom
{
    public class GetSymGroupTest
    {
        [Theory]
        [ClassData(typeof(SymptomTestData))]
        public void ProcessorTest(string symptom, DateTime datum, int min)
        {
            RestDataAccess _data = TestConnection.Initialize();

            IPatientMovementFactory patientMoveFac = new PatientMovementFactory(_data, NullLogger<PatientMovementFactory>.Instance);
            ISymptomFactory symptomFac = new SymptomFactory(_data, NullLogger<SymptomFactory>.Instance);
            IPatientStay patientStay = CreatePatientStay(_data); ;
            EhrDataService dataService = new(patientStay, patientMoveFac, symptomFac, NullLogger<EhrDataService>.Instance);
            SymptomService symptomService = new (symptomFac, dataService);

            Dictionary<string, int> actual = symptomService.GetSymGroup(symptom, datum, min);
            Dictionary<string, int> expected = new Dictionary<string, int>(){ { "Stationskennung X", 7 } };

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
                string nameDesSymptoms = "Cough (finding)";
                DateTime beginn = Convert.ToDateTime("2020-02-13");
                yield return new object[] { nameDesSymptoms, beginn, 3 };
            }
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
