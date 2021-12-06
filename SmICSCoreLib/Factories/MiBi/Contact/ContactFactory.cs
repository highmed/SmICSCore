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
        public List<Hospitalization> Hospitalizations { get; private set; }
        List<SmICSCoreLib.Factories.PatientMovementNew.PatientStays.PatientStay> patientStays;
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

        public Dictionary<Hospitalization, List<Case>> Process(Patient parameter)
        {
            Dictionary<Hospitalization, List<Case>> contacts = new Dictionary<Hospitalization, List<Case>>();
            Hospitalizations = _hospitalizationFac.Process(parameter as Patient);

            Hospitalizations.ForEach(h => contacts.Add(h, null));

            patientStays = _patientStayFac.Process(Hospitalizations.Last());

            foreach (SmICSCoreLib.Factories.PatientMovementNew.PatientStays.PatientStay patientStay in patientStays)
            {
                WardOverviewParameters wardOverviewParameter = new WardOverviewParameters
                {
                    Ward = patientStay.Ward,
                    Start = patientStay.Admission,
                    End = patientStay.Discharge
                };

                List<Case> casesOnWard = _wardOverviewFac.Processm(wardOverviewParameter);

                Hospitalization hospitalization = Hospitalizations.Where(h => h.CaseID == patientStay.CaseID).FirstOrDefault();
                if (contacts[hospitalization] == null)
                {
                    contacts[hospitalization] = casesOnWard;
                }
                else
                {
                    contacts[hospitalization].AddRange(casesOnWard);
                }
            }
            return contacts;
        }

        public List<Case> Process(Hospitalization hospitalization)
        {

        }

    }
}
