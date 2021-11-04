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
                Assert.Equal(expected[i].Probenentnahme.ToUniversalTime().ToString("s"), actual[i].Probenentnahme.ToUniversalTime().ToString("s"));
                Assert.Equal(expected[i].Aufnahme.ToUniversalTime().ToString("s"), actual[i].Aufnahme.ToUniversalTime().ToString("s"));
                Assert.Equal(expected[i].Entlastung.ToUniversalTime().ToString("s"), actual[i].Entlastung.ToUniversalTime().ToString("s"));
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
            patientList.Add(new PatientModel("Patient09", DateTime.Parse("15.02.2020 16:47:45Z"), DateTime.Parse("09.02.2020 12:13:00Z"), DateTime.Parse("04.03.2020 12:13:00Z")));
            patientList.Add(new PatientModel("Patient06", DateTime.Parse("16.02.2020 11:03:00Z"), DateTime.Parse("08.02.2020 12:13:00Z"), DateTime.Parse("20.02.2020 12:13:00Z")));
            patientList.Add(new PatientModel("Patient05", DateTime.Parse("17.02.2020 20:47:45Z"), DateTime.Parse("05.02.2020 12:13:00Z"), DateTime.Parse("23.02.2020 12:13:00Z")));
            patientList.Add(new PatientModel("Patient08", DateTime.Parse("28.02.2020 16:47:45Z"), DateTime.Parse("10.02.2020 12:13:00Z"), DateTime.Parse("03.03.2020 12:13:00Z")));
            patientList.Add(new PatientModel("Patient10", DateTime.Parse("10.03.2020 12:13:00Z"), DateTime.Parse("05.03.2020 12:13:00Z"), DateTime.Parse("20.03.2020 12:13:00Z")));
            patientList.Add(new PatientModel("Patient11", DateTime.Parse("13.03.2020 16:47:45Z"), DateTime.Parse("08.03.2020 12:13:00Z"), DateTime.Parse("27.03.2020 12:13:00Z")));
            patientList.Add(new PatientModel("Patient11", DateTime.Parse("20.03.2020 16:47:45Z"), DateTime.Parse("08.03.2020 12:13:00Z"), DateTime.Parse("27.03.2020 12:13:00Z")));
            patientList.Add(new PatientModel("Patient03", DateTime.Parse("01.12.2020 08:13:00Z"), DateTime.Parse("27.11.2020 12:13:00Z"), DateTime.Parse("12.12.2020 12:13:00Z")));
            patientList.Add(new PatientModel("Patient01", DateTime.Parse("02.12.2020 10:13:33Z"), DateTime.Parse("28.11.2020 12:13:00Z"), DateTime.Parse("12.12.2020 12:13:00Z")));
            patientList.Add(new PatientModel("Patient02", DateTime.Parse("02.12.2020 10:22:33Z"), DateTime.Parse("28.11.2020 12:13:00Z"), DateTime.Parse("12.12.2020 12:13:00Z")));
            return patientList;
        }
    }
}
