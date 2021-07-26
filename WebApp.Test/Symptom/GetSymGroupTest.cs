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
using SmICSCoreLib.AQL.Patient_Stay.Cases;
using System;

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
            Dictionary<string, int> expected = new Dictionary<string, int>(){ { "Stationskennung X", 8 } };

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
            ISymptomFactory symptomFac = new SymptomFactory(rest);
            IMibiPatientLaborDataFactory mibiLabFac = new MibiPatientLaborDataFactory(rest);

            return new PatientInformation(patMoveFac, patLabFac, symptomFac, mibiLabFac);
        }


        private class SymptomTestData : IEnumerable<object[]>
        {
            List<PatientInfos> patient = SmICSCoreLib.JSONFileStream.JSONReader<PatientInfos>.Read(@"../../../../WebApp.Test/Resources/Symptome.json");

            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { patient[0].NameDesSymptoms, patient[0].Beginn, 3 };
            }
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
