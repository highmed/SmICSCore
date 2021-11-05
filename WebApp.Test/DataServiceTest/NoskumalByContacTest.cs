using SmICSCoreLib.Factories.Symptome;
using SmICSDataGenerator.Tests;
using System.Collections.Generic;
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
using SmICSCoreLib.Factories.InfectionSituation;

namespace WebApp.Test.DataServiceTest
{
    public class NoskumalByContactTest
    {
        [Fact]
        public void ProcessorTest()
        {
            RestDataAccess _data = TestConnection.Initialize();
            IPatientMovementFactory patientMoveFac = new PatientMovementFactory(_data, NullLogger<PatientMovementFactory>.Instance);
            ISymptomFactory symptomFac = new SymptomFactory(_data, NullLogger<SymptomFactory>.Instance);
            IPatientStay patientStay = CreatePatientStay(_data); ;
            EhrDataService dataService = new (patientStay, patientMoveFac, symptomFac, NullLogger<EhrDataService>.Instance);
            List<CountDataModel> positivPatList= dataService.GetAllPositivTest("260373001");
            List<PatientModel> noskumalList = dataService.GetAllNoskumalPat(positivPatList);

            List<PatientModel> actual = dataService.GetNoskumalByContact(noskumalList, positivPatList);
            List<PatientModel> expected = GetPatientList();

            Assert.Equal(expected.Count, actual.Count);
            
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
            return patientList;
        }
    }
}
