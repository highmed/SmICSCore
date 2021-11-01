
using Newtonsoft.Json;

namespace SmICSWebApp.Data.OutbreakDetection
{
    public class OutbreakDetectionConfig
    {
        [JsonProperty("Name")]
        public string Name { get; set; }
        [JsonProperty("StationID")]
        public string Ward { get; set; }
    }
}
