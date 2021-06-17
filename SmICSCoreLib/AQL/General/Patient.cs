using Newtonsoft.Json;

namespace SmICSCoreLib.AQL.General
{
    public class Patient
    {
        [JsonProperty(PropertyName = "PatientID")]
        public string EHRID { get; set; }
        [JsonProperty(PropertyName = "Identifier")]
        public string PatientID { get; set; }
    }
}
