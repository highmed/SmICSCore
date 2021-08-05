using SmICSCoreLib.AQL.PatientInformation.Symptome;
using SmICSDataGenerator.Tests;
using System.Collections.Generic;
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

namespace WebApp.Test.DataServiceTest
{
    public class NoskumalByContactTest
    {
        [Fact]
        public void ProcessorTest()
        {
            RestDataAccess _data = TestConnection.Initialize();

            IPatientInformation patientInformation = CreatePatientInformation(_data);
            IPatinet_Stay patinet_Stay = CreatePatinetStay(_data); ;
            EhrDataService dataService = new (patinet_Stay, patientInformation);
            List<CountDataModel> positivPatList= dataService.GetAllPositivTest();
            List<Patient> noskumalList = dataService.GetAllNoskumalPat(positivPatList);

            List<Patient> actual = dataService.GetNoskumalByContact(noskumalList, positivPatList);
            List<Patient> expected = GetPatientList();

            Assert.Equal(expected.Count, actual.Count);
            
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
            return patientList;
        }
    }
}
