using Newtonsoft.Json;
using System;

namespace SmICSCoreLib.Factories.RKIStatistics.Models
{
    public class RKIReportCaseModel
    {
        [JsonProperty(PropertyName = "Timestamp")]
        public DateTime Timestamp { get; set; }
        [JsonProperty(PropertyName = "CaseNumber")]
        public int CaseNumber { get; set; }

    }
}