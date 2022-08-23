using Newtonsoft.Json;

namespace SmICSCoreLib.Factories.RKIStatistics.Models
{
    public class RKIReportLocationModel
    {
        [JsonProperty(PropertyName = "Description")]
        public string Description { get; set; }
        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; }

    }
}