using Newtonsoft.Json;

namespace SmICSCoreLib.Factories.General
{
    public class Patient
    {
        [JsonProperty(PropertyName = "PatientID")]
        public string PatientID { get; set; }
        public Patient()
        {

        }
        public Patient(Patient patient)
        {
            PatientID = patient.PatientID;
        }
    }
}
