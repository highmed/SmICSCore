using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.MiBi.PatientView;
using SmICSCoreLib.Factories.MiBi.WardOverview;
using SmICSCoreLib.Factories.PatientMovementNew;
using SmICSCoreLib.Factories.PatientMovementNew.PatientStays;
using SmICSCoreLib.REST;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmICSCoreLib.Factories.MiBi.Contact
{
    public class ContactFactory
    {
        Dictionary<Case, Hospitalization> hospitalizations;
        List<SmICSCoreLib.Factories.PatientMovementNew.PatientStays.PatientStay> patientStays;
        public IRestDataAccess RestDataAccess { get; set; }
        private readonly IHospitalizationFactory _hospitalizationFac;
        private readonly IPatientStayFactory _patientStayFac;
        public ContactFactory(IRestDataAccess restDataAccess, IHospitalizationFactory hospitalizationFac, IPatientStayFactory patientStayFac)
        {
            RestDataAccess = restDataAccess;
            _hospitalizationFac = hospitalizationFac;
            _patientStayFac = patientStayFac;
        }

        public Dictionary<Hospitalization, Contact> Process(Case parameter)
        {
            List<Hospitalization> hospitalizations = _hospitalizationFac.Process(parameter as Patient);
            patientStays = _patientStayFac.Process(parameter);
            
            foreach(SmICSCoreLib.Factories.PatientMovementNew.PatientStays.PatientStay patientStay in patientStays)
            {
                List<Case> casesOnWard = _wardOverview.Process(WardOverviewParameters parameter);  
                
            }
            return null;
        }

       
    }
}
