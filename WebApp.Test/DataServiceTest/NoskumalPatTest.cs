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

            return new PatientInformation(patMoveFac, patLabFac, symptomFac, mibiLabFac, vaccFac);
        }

        private List<Patient> GetPatientList()
        {
            List<Patient> patientList = new();
            patientList.Add(new Patient("52afa449-07a2-4d57-bb7a-86e7960124ac", DateTime.Parse("08.02.2020 11:13:00"), DateTime.Parse("08.02.2020 12:13:00"), DateTime.Parse("20.02.2020 12:13:00")));
            patientList.Add(new Patient("52afa449-07a2-4d57-bb7a-86e7960124ac", DateTime.Parse("08.02.2020 11:13:00"), DateTime.Parse("08.02.2020 12:13:00"), DateTime.Parse("20.02.2020 12:13:00")));
            patientList.Add(new Patient("959c26c9-de4a-40b8-bf76-a315574d3da7", DateTime.Parse("01.12.2020 09:13:09"), DateTime.Parse("27.11.2020 12:13:00"), DateTime.Parse("12.12.2020 12:13:00")));
            patientList.Add(new Patient("386ff697-7fe4-4fe0-942c-eb389b81704f", DateTime.Parse("02.12.2020 12:13:33"), DateTime.Parse("28.11.2020 12:13:00"), DateTime.Parse("12.12.2020 12:13:00")));
            patientList.Add(new Patient("f7f106b1-3aef-481e-aa76-af9092dc63dc", DateTime.Parse("02.12.2020 12:40:33"), DateTime.Parse("28.11.2020 12:13:00"), DateTime.Parse("12.12.2020 12:13:00")));
            patientList.Add(new Patient("c74f6215-4fc2-42a5-a3ad-f92536ca64dc", DateTime.Parse("01.01.2021 11:00:00"), DateTime.Parse("01.01.2021 09:00:00"), DateTime.Parse("05.01.2021 15:00:00")));
            patientList.Add(new Patient("96cdcae3-6c08-4eb7-8e41-45b012bf61d4", DateTime.Parse("02.01.2021 11:00:00"), DateTime.Parse("02.01.2021 09:00:00"), DateTime.Parse("07.01.2021 15:00:00")));
            patientList.Add(new Patient("059d9e68-c096-4ee7-8551-c088a5488813", DateTime.Parse("03.01.2021 11:00:00"), DateTime.Parse("02.01.2021 09:00:00"), DateTime.Parse("09.01.2021 15:00:00")));
            patientList.Add(new Patient("7dab2503-06f1-4c42-b4a4-76ddaae08794", DateTime.Parse("03.01.2021 11:00:00"), DateTime.Parse("03.01.2021 09:00:00"), DateTime.Parse("09.01.2021 15:00:00")));
            return patientList;
        }
    }
}
