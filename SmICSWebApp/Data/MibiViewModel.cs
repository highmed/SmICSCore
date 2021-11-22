using SmICSCoreLib.Factories.MiBi.WardOverview;
using SmICSCoreLib.Factories.PatientInformation.PatientData;

namespace SmICSWebApp.Data
{
    public class MibiViewModel
    {
        public WardOverviewModel LabData { get; set; }
        public PatientData PatientData { get; set; }

        public MibiViewModel(PatientData patientData, WardOverviewModel labData)
        {
            PatientData = patientData;
            LabData = labData;
        }

        public MibiViewModel()
        {
            
        }
    }
}