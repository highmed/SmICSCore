using Newtonsoft.Json;

namespace SmICSCoreLib.AQL.General
{
    public class Case : Patient
    {
        [JsonProperty(PropertyName = "FallID")]
        public string CaseID { get; set; }
    }
}
