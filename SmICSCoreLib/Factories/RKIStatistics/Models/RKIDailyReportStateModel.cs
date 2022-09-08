using Newtonsoft.Json;
using System.Collections.Generic;

namespace SmICSCoreLib.Factories.RKIStatistics.Models
{
    public class RKIDailyReporStateModel
    {
        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "CaseNumbers")]
        public int CaseNumbers { get; set; }

        [JsonProperty(PropertyName = "Cases7BL")]
        public double Cases7BL { get; set; }

        [JsonProperty(PropertyName = "CasesPer100000Citizens")]
        public double CasesPer100000Citizens { get; set; }

        [JsonProperty(PropertyName = "Deathcases")]
        public int Deathcases { get; set; }

        [JsonProperty(PropertyName = "Deathcases7BL")]
        public double Deathcases7BL { get; set; }

        [JsonProperty(PropertyName = "Inzidenz7Days")]
        public double Inzidenz7Days { get; set; }

        [JsonProperty(PropertyName = "Color")]
        public string Color { get; set; }

        [JsonProperty(PropertyName = "District")]
        public List<RKIDailyReporDistrictModel> District { get; set; }

    }
}