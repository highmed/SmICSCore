using Newtonsoft.Json;

namespace SmICSCoreLib.Factories.RKIStatistics.ReceiveModels
{
    public class RKIReportStateCaseModel
    {
        [JsonProperty(PropertyName = "BundeslandId")]
        public int BundeslandId { get; set; }

        [JsonProperty(PropertyName = "AnzFall")]
        public int AnzFall { get; set; }

        [JsonProperty(PropertyName = "AnzFallNeu")]
        public int AnzFallNeu { get; set; }

        [JsonProperty(PropertyName = "AnzTodesfall")]
        public int AnzTodesfall { get; set; }

        [JsonProperty(PropertyName = "AnzTodesfallNeu")]
        public int AnzTodesfallNeu { get; set; }

        [JsonProperty(PropertyName = "Inz7T")]
        public float Inz7T { get; set; }

    }
}