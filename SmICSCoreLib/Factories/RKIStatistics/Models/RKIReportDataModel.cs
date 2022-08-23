using Newtonsoft.Json;
using System;

namespace SmICSCoreLib.Factories.RKIStatistics.Models
{
    public class RKIReportDataModel
    {
        [JsonProperty(PropertyName = "Location")]
        public RKIReportLocationModel Location { get; set; }
        [JsonProperty(PropertyName = "Timestamp")]
        public DateTime Timestamp { get; set; }
        [JsonProperty(PropertyName = "CaseNumber")]
        public int CaseNumber { get; set; }

    }
}