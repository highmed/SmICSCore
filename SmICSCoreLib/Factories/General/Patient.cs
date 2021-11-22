using Newtonsoft.Json;

namespace SmICSCoreLib.Factories.General
{
    public class Patient
    {
        [JsonProperty(PropertyName = "PatientID")]
        public string PatientID { get; set; }
    }
}
