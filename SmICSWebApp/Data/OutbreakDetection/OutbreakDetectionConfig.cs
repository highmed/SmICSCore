
using Newtonsoft.Json;

namespace SmICSWebApp.Data.OutbreakDetection
{
    public class OutbreakDetectionConfig
    {
        public string Name { get; set; }
        [JsonProperty("StationID")]
        public string Ward { get; set; }
    }
}
