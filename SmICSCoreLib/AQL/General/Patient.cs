using Newtonsoft.Json;

namespace SmICSCoreLib.AQL.General
{
    public class Patient
    {
        [JsonProperty(PropertyName = "PatientID")]
        public string PatientID { get; set; }
    }
}
