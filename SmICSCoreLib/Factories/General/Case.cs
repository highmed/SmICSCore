using Newtonsoft.Json;

namespace SmICSCoreLib.Factories.General
{
    public class Case : Patient
    {
        [JsonProperty(PropertyName = "FallID")]
        public string CaseID { get; set; }
    }
}
