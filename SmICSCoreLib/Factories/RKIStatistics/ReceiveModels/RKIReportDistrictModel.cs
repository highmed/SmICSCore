using Newtonsoft.Json;

namespace SmICSCoreLib.Factories.RKIStatistics.ReceiveModels
{
    public class RKIReportDistrictModel
    {
        [JsonProperty(PropertyName = "GEN")]
        public string GEN { get; set; }

        [JsonProperty(PropertyName = "BEZ")]
        public string BEZ { get; set; }

        [JsonProperty(PropertyName = "BL")]
        public string BL { get; set; }

        [JsonProperty(PropertyName = "BL_ID")]
        public string BL_ID { get; set; }

        [JsonProperty(PropertyName = "county")]
        public string County { get; set; }

        [JsonProperty(PropertyName = "EWZ")]
        public int EWZ { get; set; }

        [JsonProperty(PropertyName = "cases")]
        public int Cases { get; set; }

        [JsonProperty(PropertyName = "recovered")]
        public object Recovered { get; set; }

        [JsonProperty(PropertyName = "deaths")]
        public int Deaths { get; set; }

        [JsonProperty(PropertyName = "death_rate")]
        public float Death_rate { get; set; }

        [JsonProperty(PropertyName = "death7_lk")]
        public int Death7_lk { get; set; }

        [JsonProperty(PropertyName = "cases7_lk")]
        public int Cases7_lk { get; set; }

        [JsonProperty(PropertyName = "cases_per_100k")]
        public float Cases_per_100k { get; set; }

        [JsonProperty(PropertyName = "cases_per_population")]
        public float Cases_per_population { get; set; }

        [JsonProperty(PropertyName = "cases7_per_100k")]
        public float Cases7_per_100k { get; set; }

        [JsonProperty(PropertyName = "cases7_per_100k_txt")]
        public float Cases7_per_100k_txt { get; set; }

        [JsonProperty(PropertyName = "last_update")]
        public string Last_update { get; set; }

        [JsonProperty(PropertyName = "AdmUnitId")]
        public int AdmUnitId { get; set; }

    }
}