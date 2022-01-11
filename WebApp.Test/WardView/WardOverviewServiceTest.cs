using Microsoft.Extensions.Logging.Abstractions;
using SmICSCoreLib.Factories.MiBi;
using SmICSCoreLib.Factories.MiBi.Nosocomial;
using SmICSCoreLib.Factories.MiBi.PatientView;
using SmICSCoreLib.Factories.PatientMovementNew;
using SmICSCoreLib.Factories.PatientMovementNew.PatientStays;
using SmICSCoreLib.REST;
using SmICSWebApp.Data.WardView;
using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace WebApp.Test.WardView
{
    public class WardOverviewServiceTest
    {
        [Theory]
        [ClassData(typeof(WardOverviewTestData))]
        public void GetDataTest(string ward, string pathogen, DateTime start, DateTime end)
        {
            WardParameter parameter = new WardParameter() { Ward = ward, Pathogen = pathogen, Start = start, End = end };

            RestDataAccess rest = Rest();
            IPatientStayFactory stayFac = new PatientStayFactory(rest);
            IAntibiogramFactory antiFac = new AntibiogramFactory(rest);
            IPathogenFactory pathoFac = new PathogenFactory(rest, antiFac);
            ISpecimenFactory specFac = new SpecimenFactory(rest, pathoFac);
            ILabResultFactory labFac = new LabResultFactory(rest, specFac);
            IHospitalizationFactory hosFac = new HospitalizationFactory(rest);
            InfectionStatusFactory infectFac = new InfectionStatusFactory(rest, labFac, hosFac);
            WardOverviewService service = new WardOverviewService(
                stayFac, 
                infectFac, 
                labFac);

            List<WardOverview> views = service.GetData(parameter);
            Assert.Equal(222, views.Count);
        }

        private RestDataAccess Rest() 
        {
            OpenehrConfig.openehrEndpoint = "https://plri-highmed01.mh-hannover.local:8083/rest/openehr/v1";
            OpenehrConfig.openehrUser = "etltestuser";
            OpenehrConfig.openehrPassword = "etltestuser#01";

            RestClientConnector restClient = new RestClientConnector();
            return new RestDataAccess(NullLogger<RestDataAccess>.Instance, restClient);
        }
        private class WardOverviewTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { "47", "Staphylococcus aureus", new DateTime(2021, 5, 26), new DateTime(2021, 6, 5) };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
