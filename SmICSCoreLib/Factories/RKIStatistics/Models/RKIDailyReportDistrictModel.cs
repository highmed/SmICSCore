using Newtonsoft.Json;

namespace SmICSCoreLib.Factories.RKIStatistics.Models
{
    public class RKIDailyReporDistrictModel
    {
        [JsonProperty(PropertyName = "City")]
        public string City { get; set; }

        [JsonProperty(PropertyName = "DistrictName")]
        public string DistrictName { get; set; }

        [JsonProperty(PropertyName = "CaseNumbers")]
        public int CaseNumbers { get; set; }

        [JsonProperty(PropertyName = "Cases7LK")]
        public double Cases7LK { get; set; }

        [JsonProperty(PropertyName = "CasesPer100000Citizens")]
        public double CasesPer100000Citizens { get; set; }

        [JsonProperty(PropertyName = "Inzidenz7Days")]
        public double Inzidenz7Days { get; set; }

        [JsonProperty(PropertyName = "Deathcases")]
        public int Deathcases { get; set; }

        [JsonProperty(PropertyName = "Deathcases7LK")]
        public double Deathcases7LK { get; set; }

        [JsonProperty(PropertyName = "AdmUnitID")]
        public int AdmUnitID { get; set; }

    }
}