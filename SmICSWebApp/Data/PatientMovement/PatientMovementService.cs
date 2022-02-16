using SmICSCoreLib.Factories.PatientMovementNew;
using SmICSCoreLib.Factories.PatientMovementNew.PatientStays;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmICSWebApp.Data.PatientMovement
{
    public class PatientMovementService
    {
        private IHospitalizationFactory _hospFac;
        private IPatientStayFactory _patStayFac;

        public PatientMovementService(IPatientStayFactory patStayFac, IHospitalizationFactory hospFac)
        {
            _patStayFac = patStayFac;
            _hospFac = hospFac;
        }

        public List<VisuPatientMovement> GetPatientMovements(SmICSCoreLib.Factories.General.Patient patient)
        {
            List<VisuPatientMovement> visuPatientMovements = new List<VisuPatientMovement>();
            List<Hospitalization> hospitalizations = _hospFac.Process(patient);
            foreach(Hospitalization hosp in hospitalizations)
            {
                List<PatientStay> patientStays = _patStayFac.Process(hosp);
                visuPatientMovements.Add(new VisuPatientMovement(hosp.Admission, patientStays.First()));
                foreach(PatientStay patientStay in patientStays)
                {
                    visuPatientMovements.Add(new VisuPatientMovement(patientStay));
                }
                if (hosp.Discharge.Date.HasValue)
                {
                    visuPatientMovements.Add(new VisuPatientMovement(hosp.Discharge, patientStays.Last()));
                }
            }

            return visuPatientMovements;
        }
    }
}
