using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace SmICSCoreLib.Factories.RKIStatistics.Models
{
    public class RKIReportModel
    {
        [JsonProperty(PropertyName = "Timestamp")]
        public DateTime Timestamp { get; set; }
        [JsonProperty(PropertyName = "Data")]
        public List<RKIReportLocationModel> Data { get; set; }

    }
}