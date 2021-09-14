using SmICSCoreLib.AQL.PatientInformation.Patient_Mibi_Labordaten;

namespace SmICSWebApp.Data
{
    public class PatientViewModel : MibiLabDataModel
    {
        public string Ward { get; set; }
        public string Room { get; set; }
        public string Departement { get; set; }
        
    }
}