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
using SmICSCoreLib.Factories.InfectionSituation;

namespace WebApp.Test.DataServiceTest
{
    public class PatStayTest
    {
        [Fact]
        public void ProcessorTest()
        {
            RestDataAccess _data = TestConnection.Initialize();

            IPatientMovementFactory patientMoveFac = new PatientMovementFactory(_data, NullLogger<PatientMovementFactory>.Instance);
            ISymptomFactory symptomFac = new SymptomFactory(_data, NullLogger<SymptomFactory>.Instance);
            IPatientStay patientStay = CreatePatientStay(_data); ;
            EhrDataService dataService = new(patientStay, patientMoveFac, symptomFac, NullLogger<EhrDataService>.Instance);
            List<CountDataModel> positivTestList= dataService.GetAllPositivTest("260373001");
            List<CountDataModel> positivPatList= dataService.GetAllPatByTest(positivTestList);

            int actual = dataService.PatStay(positivPatList);
            //int expected = 3911;

            Assert.True(actual> 0);          
        }

        private PatientStay CreatePatientStay(IRestDataAccess rest)
        {
            IStationaryFactory statFac = new StationaryFactory(rest);
            ICountFactory CountFac = new CountFactory(rest);

            return new PatientStay(statFac, CountFac);
        }
    }
}
