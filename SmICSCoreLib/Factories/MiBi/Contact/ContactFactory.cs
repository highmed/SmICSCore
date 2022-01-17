using SmICSCoreLib.Factories.General;
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
        public ContactFactory(IRestDataAccess restDataAccess, IHospitalizationFactory hospitalizationFac, IPatientStayFactory patientStayFac)
        {
            RestDataAccess = restDataAccess;
            _hospitalizationFac = hospitalizationFac;
            _patientStayFac = patientStayFac;
        }

        public Dictionary<Hospitalization, List<PatientMovementNew.PatientStays.PatientStay>> Process(Patient parameter)
        {
            Dictionary<Hospitalization, List<PatientMovementNew.PatientStays.PatientStay>> contacts = new Dictionary<Hospitalization, List<PatientMovementNew.PatientStays.PatientStay>>();

            List<Hospitalization> Hospitalizations = _hospitalizationFac.Process(parameter as Patient);
            Hospitalizations.ForEach(h => contacts.Add(h, null));

            Hospitalization hospitalization = Hospitalizations.Last();

            List<PatientMovementNew.PatientStays.PatientStay> patientStays = _patientStayFac.Process(hospitalization);
            List<PatientMovementNew.PatientStays.PatientStay> contactCases = DetermineContacts(Hospitalizations.Last());
             
            if (contacts[hospitalization] == null)
            {
                contacts[hospitalization] = contactCases;
            }
            
            return contacts;
        }

        public List<PatientMovementNew.PatientStays.PatientStay> Process(Hospitalization hospitalization)
        {
           return DetermineContacts(hospitalization);
        }

        private List<PatientMovementNew.PatientStays.PatientStay> DetermineContacts(Hospitalization hospitalization)
        {
            List<PatientMovementNew.PatientStays.PatientStay> patientStays = _patientStayFac.Process(hospitalization);
            List<PatientMovementNew.PatientStays.PatientStay> cases = new List<PatientMovementNew.PatientStays.PatientStay>();
            foreach (PatientMovementNew.PatientStays.PatientStay patientStay in patientStays)
            {
                WardParameter wardParameter = new WardParameter
                {
                    Ward = patientStay.Ward,
                    Start = patientStay.Admission,
                    End = patientStay.Discharge.Value
                };

                List<PatientMovementNew.PatientStays.PatientStay> casesOnWard = _patientStayFac.Process(wardParameter);
                cases.AddRange(casesOnWard);
            }

            return cases;
        }
    }
}
