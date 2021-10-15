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
using SmICSCoreLib.StatistikDataModels;
using System;
using SmICSCoreLib.Factories.InfectionSituation;

namespace WebApp.Test.DataServiceTest
{
    public class NoskumalPatTest
    {
        [Fact]
        public void ProcessorTest()
        {
            RestDataAccess _data = TestConnection.Initialize();

            IPatientMovementFactory patientMoveFac = new PatientMovementFactory(_data, NullLogger<PatientMovementFactory>.Instance);
            ISymptomFactory symptomFac = new SymptomFactory(_data, NullLogger<SymptomFactory>.Instance);
            IPatientStay patientStay = CreatePatientStay(_data); ;
            EhrDataService dataService = new(patientStay, patientMoveFac, symptomFac, NullLogger<EhrDataService>.Instance);
            List<CountDataModel> positivPatList= dataService.GetAllPositivTest("260373001");

            List<PatientModel> actual = dataService.GetAllNoskumalPat(positivPatList);
            List<PatientModel> expected = GetPatientList();

            Assert.Equal(expected.Count, actual.Count);
            for (int i = 0; i < expected.Count; i++)
            {
                Assert.Equal(expected[i].PatientID, actual[i].PatientID);
                Assert.Equal(expected[i].Probenentnahme, actual[i].Probenentnahme);
                Assert.Equal(expected[i].Aufnahme, actual[i].Aufnahme);
                Assert.Equal(expected[i].Entlastung, actual[i].Entlastung);
            }
            
        }

        private PatientStay CreatePatientStay(IRestDataAccess rest)
        {
            IStationaryFactory statFac = new StationaryFactory(rest);
            ICountFactory CountFac = new CountFactory(rest);

            return new PatientStay(statFac, CountFac);
        }

        private List<PatientModel> GetPatientList()
        {
            List<PatientModel> patientList = new();
            patientList.Add(new PatientModel("eae9b822-6015-430c-a694-123249e977eb", DateTime.Parse("16.02.2020 10:47:45"), DateTime.Parse("09.02.2020 12:13:00"), DateTime.Parse("04.03.2020 12:13:00")));
            patientList.Add(new PatientModel("52afa449-07a2-4d57-bb7a-86e7960124ac", DateTime.Parse("16.02.2020 13:03:00"), DateTime.Parse("08.02.2020 12:13:00"), DateTime.Parse("20.02.2020 12:13:00")));
            patientList.Add(new PatientModel("7a347dcf-209f-46cf-ad4e-5f242b73d5cd", DateTime.Parse("17.02.2020 21:47:45Z"), DateTime.Parse("05.02.2020 12:13:00"), DateTime.Parse("23.02.2020 12:13:00")));
            patientList.Add(new PatientModel("11fcaa10-d510-4eb7-a4a4-c96add98bae2", DateTime.Parse("28.02.2020 17:47:45Z"), DateTime.Parse("10.02.2020 12:13:00"), DateTime.Parse("03.03.2020 12:13:00")));
            patientList.Add(new PatientModel("ac8a6002-d728-4b4e-a629-bfa66ebeace7", DateTime.Parse("10.03.2020 13:13:00Z"), DateTime.Parse("05.03.2020 12:13:00"), DateTime.Parse("20.03.2020 12:13:00")));
            patientList.Add(new PatientModel("db7be72a-db86-4dc6-b55b-dcf510b0f23e", DateTime.Parse("14.03.2020 09:47:45Z"), DateTime.Parse("08.03.2020 12:13:00"), DateTime.Parse("27.03.2020 12:13:00")));
            patientList.Add(new PatientModel("db7be72a-db86-4dc6-b55b-dcf510b0f23e", DateTime.Parse("20.03.2020 17:47:45Z"), DateTime.Parse("08.03.2020 12:13:00"), DateTime.Parse("27.03.2020 12:13:00")));
            patientList.Add(new PatientModel("959c26c9-de4a-40b8-bf76-a315574d3da7", DateTime.Parse("01.12.2020 09:13:09Z"), DateTime.Parse("27.11.2020 12:13:00"), DateTime.Parse("12.12.2020 12:13:00")));
            patientList.Add(new PatientModel("386ff697-7fe4-4fe0-942c-eb389b81704f", DateTime.Parse("02.12.2020 12:13:33Z"), DateTime.Parse("28.11.2020 12:13:00"), DateTime.Parse("12.12.2020 12:13:00")));
            patientList.Add(new PatientModel("f7f106b1-3aef-481e-aa76-af9092dc63dc", DateTime.Parse("02.12.2020 12:40:33Z"), DateTime.Parse("26.11.2020 12:13:00"), DateTime.Parse("12.12.2020 12:13:00")));
            return patientList;
        }
    }
}
