using Newtonsoft.Json;

namespace SmICSWebApp.Data
{
    public class District
    {
        [JsonProperty(PropertyName = "features")]
        public DistrictFeature[] features { get; set; }
    }

    public class DistrictFeature
    {
        [JsonProperty(PropertyName = "attributes")]
        public DistrictAttributes districtAttributes { get; set; }
    }

    public class DistrictAttributes
    {
        [JsonProperty(PropertyName = "OBJECTID")]
        public int OBJECTID { get; set; }

        [JsonProperty(PropertyName = "GEN")]
        public string GEN { get; set; }

        [JsonProperty(PropertyName = "BEZ")]
        public string BEZ { get; set; }

        [JsonProperty(PropertyName = "BL")]
        public string BL { get; set; }

        [JsonProperty(PropertyName = "BL_ID")]
        public string BL_ID { get; set; }

        [JsonProperty(PropertyName = "county")]
        public string county { get; set; }

        [JsonProperty(PropertyName = "EWZ")]
        public int EWZ { get; set; }

        [JsonProperty(PropertyName = "cases")]
        public int cases { get; set; }

        [JsonProperty(PropertyName = "recovered")]
        public object recovered { get; set; }

        [JsonProperty(PropertyName = "deaths")]
        public int deaths { get; set; }

        [JsonProperty(PropertyName = "death_rate")]
        public float death_rate { get; set; }
        
        [JsonProperty(PropertyName = "death7_lk")]
        public int death7_lk { get; set; }
        
        [JsonProperty(PropertyName = "cases7_lk")]
        public int cases7_lk { get; set; }

        [JsonProperty(PropertyName = "cases_per_100k")]
        public float cases_per_100k { get; set; }

        [JsonProperty(PropertyName = "cases_per_population")]
        public float cases_per_population { get; set; }

        [JsonProperty(PropertyName = "cases7_per_100k")]
        public float cases7_per_100k { get; set; }
        
        [JsonProperty(PropertyName = "cases7_per_100k_txt")]
        public float cases7_per_100k_txt { get; set; }

        [JsonProperty(PropertyName = "last_update")]
        public string last_update { get; set; }
    }
}
