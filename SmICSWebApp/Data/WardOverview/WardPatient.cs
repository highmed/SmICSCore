using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.MiBi.Nosocomial;
using SmICSCoreLib.Factories.MiBi.PatientView;
using SmICSCoreLib.Factories.PatientMovementNew;
using SmICSCoreLib.Factories.PatientMovementNew.PatientStays;
using System.Collections.Generic;

namespace SmICSWebApp.Data.WardOverview
{
    public class WardPatient : Case
    {
        public WardPatient(Case Case) : base(Case)
        {

        }
        public InfectionStatus InfectionStatus { get; set; }
        public PatientStay CurrentStay { get; set; }
        public Hospitalization Hospitalization { get; set; }
        public List<LabResult> LabData { get; set; }
    }
}