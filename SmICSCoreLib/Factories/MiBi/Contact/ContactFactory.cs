using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.MiBi.WardOverview;
using SmICSCoreLib.Factories.PatientMovementNew;
using SmICSCoreLib.Factories.PatientMovementNew.PatientStays;
using SmICSCoreLib.REST;
using System.Collections.Generic;
using System.Linq;

namespace SmICSCoreLib.Factories.MiBi.Contact
{
    public class ContactFactory : IContactFactory
    {      
        public IRestDataAccess RestDataAccess { get; set; }
        private readonly IHospitalizationFactory _hospitalizationFac;
        private readonly IPatientStayFactory _patientStayFac;
        readonly IWardOverviewFactory _wardOverviewFac;
        public ContactFactory(IRestDataAccess restDataAccess, IHospitalizationFactory hospitalizationFac, IPatientStayFactory patientStayFac, IWardOverviewFactory wardOverviewFac)
        {
            RestDataAccess = restDataAccess;
            _hospitalizationFac = hospitalizationFac;
            _patientStayFac = patientStayFac;
            _wardOverviewFac = wardOverviewFac;
        }

        public Dictionary<Hospitalization, List<PatientLocation>> Process(Patient parameter)
        {
            Dictionary<Hospitalization, List<PatientLocation>> contacts = new Dictionary<Hospitalization, List<PatientLocation>>();

            List<Hospitalization> Hospitalizations = _hospitalizationFac.Process(parameter as Patient);
            Hospitalizations.ForEach(h => contacts.Add(h, null));

            Hospitalization hospitalization = Hospitalizations.Last();

            List<SmICSCoreLib.Factories.PatientMovementNew.PatientStays.PatientStay> patientStays = _patientStayFac.Process(hospitalization);
            List<PatientLocation> contactCases = DetermineContacts(Hospitalizations.Last());
             
            if (contacts[hospitalization] == null)
            {
                contacts[hospitalization] = contactCases;
            }
            
            return contacts;
        }

        public List<PatientLocation> Process(Hospitalization hospitalization)
        {
           return DetermineContacts(hospitalization);
        }

        private List<PatientLocation> DetermineContacts(Hospitalization hospitalization)
        {
            List<SmICSCoreLib.Factories.PatientMovementNew.PatientStays.PatientStay> patientStays = _patientStayFac.Process(hospitalization);
            List<PatientLocation> cases = new List<PatientLocation>();
            foreach (SmICSCoreLib.Factories.PatientMovementNew.PatientStays.PatientStay patientStay in patientStays)
            {
                WardOverviewParameter wardOverviewParameter = new WardOverviewParameter
                {
                    Ward = patientStay.Ward,
                    Start = patientStay.Admission,
                    End = patientStay.Discharge
                };

                List<PatientLocation> casesOnWard = _wardOverviewFac.Processm(wardOverviewParameter);
                cases.AddRange(casesOnWard);
            }

            return cases;
        }
    }
}
