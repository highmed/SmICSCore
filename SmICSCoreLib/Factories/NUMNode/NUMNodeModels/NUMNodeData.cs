using Newtonsoft.Json;

namespace SmICSCoreLib.Factories.NUMNode
{
    public class NUMNodeData
    {
        [JsonProperty(PropertyName = "average")]
        public double average { get; set; }
        [JsonProperty(PropertyName = "median")]
        public double median { get; set; }
        [JsonProperty(PropertyName = "underquartil")]
        public double underquartil { get; set; }
        [JsonProperty(PropertyName = "upperquartil")]
        public double upperquartil { get; set; }
        [JsonProperty(PropertyName = "max")]
        public double max { get; set; }
        [JsonProperty(PropertyName = "min")]
        public double min { get; set; }

    }
}