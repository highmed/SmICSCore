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
using SmICSCoreLib.StatistikDataModels;
using System;
using SmICSCoreLib.AQL.PatientInformation.Infection_situation;

namespace WebApp.Test.DataServiceTest
{
    public class NoskumalPatTest
    {
        [Fact]
        public void ProcessorTest()
        {
            RestDataAccess _data = TestConnection.Initialize();

            IPatientInformation patientInformation = CreatePatientInformation(_data);
            IPatinet_Stay patinet_Stay = CreatePatinetStay(_data); ;
            EhrDataService dataService = new (patinet_Stay, patientInformation);
            List<CountDataModel> positivPatList= dataService.GetAllPositivTest();

            List<Patient> actual = dataService.GetAllNoskumalPat(positivPatList);
            List<Patient> expected = GetPatientList();

            Assert.Equal(expected.Count, actual.Count);
            for (int i = 0; i < expected.Count; i++)
            {
                Assert.Equal(expected[i].PatientID, actual[i].PatientID);
                Assert.Equal(expected[i].Probenentnahme, actual[i].Probenentnahme);
                Assert.Equal(expected[i].Aufnahme, actual[i].Aufnahme);
                Assert.Equal(expected[i].Entlastung, actual[i].Entlastung);
            }
            
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

        private List<Patient> GetPatientList()
        {
            List<Patient> patientList = new();
            patientList.Add(new Patient("eae9b822-6015-430c-a694-123249e977eb", DateTime.Parse("16.02.2020 10:47:45"), DateTime.Parse("09.02.2020 12:13:00"), DateTime.Parse("04.03.2020 12:13:00")));
            patientList.Add(new Patient("52afa449-07a2-4d57-bb7a-86e7960124ac", DateTime.Parse("16.02.2020 13:03:00"), DateTime.Parse("08.02.2020 12:13:00"), DateTime.Parse("20.02.2020 12:13:00")));
            patientList.Add(new Patient("7a347dcf-209f-46cf-ad4e-5f242b73d5cd", DateTime.Parse("17.02.2020 21:47:45Z"), DateTime.Parse("05.02.2020 12:13:00"), DateTime.Parse("23.02.2020 12:13:00")));
            patientList.Add(new Patient("11fcaa10-d510-4eb7-a4a4-c96add98bae2", DateTime.Parse("28.02.2020 17:47:45Z"), DateTime.Parse("10.02.2020 12:13:00"), DateTime.Parse("03.03.2020 12:13:00")));
            patientList.Add(new Patient("ac8a6002-d728-4b4e-a629-bfa66ebeace7", DateTime.Parse("10.03.2020 13:13:00Z"), DateTime.Parse("05.03.2020 12:13:00"), DateTime.Parse("20.03.2020 12:13:00")));
            patientList.Add(new Patient("db7be72a-db86-4dc6-b55b-dcf510b0f23e", DateTime.Parse("14.03.2020 09:47:45Z"), DateTime.Parse("08.03.2020 12:13:00"), DateTime.Parse("27.03.2020 12:13:00")));
            patientList.Add(new Patient("db7be72a-db86-4dc6-b55b-dcf510b0f23e", DateTime.Parse("20.03.2020 17:47:45Z"), DateTime.Parse("08.03.2020 12:13:00"), DateTime.Parse("27.03.2020 12:13:00")));
            patientList.Add(new Patient("959c26c9-de4a-40b8-bf76-a315574d3da7", DateTime.Parse("01.12.2020 09:13:09Z"), DateTime.Parse("27.11.2020 12:13:00"), DateTime.Parse("12.12.2020 12:13:00")));
            patientList.Add(new Patient("386ff697-7fe4-4fe0-942c-eb389b81704f", DateTime.Parse("02.12.2020 12:13:33Z"), DateTime.Parse("28.11.2020 12:13:00"), DateTime.Parse("12.12.2020 12:13:00")));
            patientList.Add(new Patient("f7f106b1-3aef-481e-aa76-af9092dc63dc", DateTime.Parse("02.12.2020 12:40:33Z"), DateTime.Parse("26.11.2020 12:13:00"), DateTime.Parse("12.12.2020 12:13:00")));
            return patientList;
        }
    }
}
