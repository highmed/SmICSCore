using SmICSCoreLib.AQL.PatientInformation.Symptome;
using SmICSDataGenerator.Tests;
using System.Collections.Generic;
using SmICSFactory.Tests;
using System.Collections;
using SmICSCoreLib.REST;
using SmICSCoreLib.StatistikServices;
using Xunit;
using SmICSCoreLib.AQL.PatientInformation;
using SmICSCoreLib.AQL.Patient_Stay;
using SmICSCoreLib.AQL.PatientInformation.Patient_Bewegung;
using SmICSCoreLib.AQL.PatientInformation.Patient_Labordaten;
using Microsoft.Extensions.Logging.Abstractions;
using SmICSCoreLib.AQL.PatientInformation.Patient_Mibi_Labordaten;
using SmICSCoreLib.AQL.Patient_Stay.Stationary;
using SmICSCoreLib.AQL.Patient_Stay.Count;
using SmICSCoreLib.AQL.PatientInformation.Vaccination;
using System;
using SmICSCoreLib.AQL.PatientInformation.Infection_situation;

namespace WebApp.Test.Symptom
{
    public class GetSymGroupTest
    {
        [Theory]
        [ClassData(typeof(SymptomTestData))]
        public void ProcessorTest(string symptom, DateTime datum, int min)
        {
            RestDataAccess _data = TestConnection.Initialize();

            IPatientInformation patientInformation = CreatePatientInformation(_data);
            IPatinet_Stay patinet_Stay = CreatePatinetStay(_data); ;
            EhrDataService dataService = new (patinet_Stay, patientInformation);
            SymptomService symptomService = new (patientInformation, dataService);

            Dictionary<string, int> actual = symptomService.GetSymGroup(symptom, datum, min);
            Dictionary<string, int> expected = new Dictionary<string, int>(){ { "Stationskennung X", 7 } };

            Assert.Equal(expected.Count, actual.Count);
            Assert.Equal(expected.Keys, actual.Keys);
            Assert.Equal(expected.Values, actual.Values);            
        }

        private Patinet_Stay CreatePatinetStay(IRestDataAccess rest)
        {
            IStationaryFactory statFac = new StationaryFactory(rest);
            ICountFactory CountFac = new CountFactory(rest);

            return new Patinet_Stay(statFac, CountFac);
        }

        private PatientInformation CreatePatientInformation(IRestDataAccess rest)
        {
            IPatientMovementFactory patMoveFac = new PatientMovementFactory(rest, NullLogger<PatientMovementFactory>.Instance);
            IPatientLabordataFactory patLabFac = new PatientLabordataFactory(rest, NullLogger<PatientLabordataFactory>.Instance);
            ISymptomFactory symptomFac = new SymptomFactory(rest, NullLogger<SymptomFactory>.Instance);
            IMibiPatientLaborDataFactory mibiLabFac = new MibiPatientLaborDataFactory(rest);
            IVaccinationFactory vaccFac = new VaccinationFactory(rest, NullLogger<VaccinationFactory>.Instance);
            ICountFactory countFactory = new CountFactory(rest);
            IStationaryFactory stationaryFactory = new StationaryFactory(rest); ;
            IInfectionSituationFactory infecFac = new InfectionSituationFactory(countFactory, stationaryFactory, symptomFac, patMoveFac, vaccFac, NullLogger<InfectionSituationFactory>.Instance);

            return new PatientInformation(patMoveFac, patLabFac, symptomFac, mibiLabFac, vaccFac, infecFac);
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
