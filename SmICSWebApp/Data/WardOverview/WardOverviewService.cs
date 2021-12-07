using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.MiBi.Nosocomial;
using SmICSCoreLib.Factories.MiBi.PatientView;
using SmICSCoreLib.Factories.MiBi.WardOverview;
using SmICSCoreLib.Factories.PatientMovementNew;
using SmICSCoreLib.Factories.PatientMovementNew.PatientStays;
using System.Collections.Generic;
using System.Linq;

namespace SmICSWebApp.Data.WardOverview
{
    public class WardOverviewService
    {
        private readonly IWardOverviewFactory _wardOverFac;
        private readonly InfectionStatusFactory _infectionStatusFac;
        private readonly IHospitalizationFactory _hospitalizationFac;
        private readonly IPatientStayFactory _patientStayFac;
        private readonly IMibiResultFactory _labDataFac;
        public WardOverviewService(IWardOverviewFactory wardOverFac, InfectionStatusFactory infectionStatusFac, IHospitalizationFactory hospitalizationFac, IPatientStayFactory patientStayFac)
        {
            _wardOverFac = wardOverFac;
            _infectionStatusFac = infectionStatusFac;
            _hospitalizationFac = hospitalizationFac;
            _patientStayFac = patientStayFac;
        }

        public List<WardPatient> LoadData(WardOverviewParameter wardOverviewParameter)
        {
            List<WardPatient> patients = new List<WardPatient>();
            List<PatientLocation> patientLocations = _wardOverFac.Process(wardOverviewParameter);

            if (patientLocations != null)
            {
                foreach (PatientLocation location in patientLocations)
                {
                    WardPatient patient = new WardPatient(location)
                    {
                        Hospitalization = _hospitalizationFac.Process(location),
                        LabData = _labDataFac.Process(location),
                        CurrentStay = _patientStayFac.Process(location).Where(s => s.Admission <= wardOverviewParameter.Start && s.Discharge >= wardOverviewParameter.Start).FirstOrDefault()
                    };
                    patient.InfectionStatus = _infectionStatusFac.Process(location)[patient.Hospitalization][wardOverviewParameter.Pathogen];
                    patients.Add(patient);
                }
                return patients;
            }
            return null;
        }
    }
}
