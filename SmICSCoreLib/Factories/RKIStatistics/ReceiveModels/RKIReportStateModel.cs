using Newtonsoft.Json;

namespace SmICSCoreLib.Factories.RKIStatistics.ReceiveModels
{
    public class RKIReportStateModel
    {
        [JsonProperty(PropertyName = "LAN_ew_GEN")]
        public string Bundesland { get; set; }

        [JsonProperty(PropertyName = "LAN_ew_BEZ")]
        public string Bezirk { get; set; }

        [JsonProperty(PropertyName = "LAN_ew_EWZ")]
        public string Einwohner { get; set; }

        [JsonProperty(PropertyName = "Fallzahl")]
        public int Fallzahl { get; set; }

        [JsonProperty(PropertyName = "faelle_100000_EW")]
        public float FaellePro100000Ew { get; set; }

        [JsonProperty(PropertyName = "cases7_bl")]
        public float Cases7_bl { get; set; }

        [JsonProperty(PropertyName = "Death")]
        public int Todesfaelle { get; set; }

        [JsonProperty(PropertyName = "death7_bl")]
        public int Death7_bl { get; set; }

        [JsonProperty(PropertyName = "cases7_bl_per_100k")]
        public float Faelle7BlPro100K { get; set; }

        [JsonProperty(PropertyName = "Aktualisierung")]
        public long Aktualisierung { get; set; }

    }
}