using Newtonsoft.Json;
using System.Collections.Generic;

namespace SmICSCoreLib.Factories.RKIStatistics.Models
{
    public class RKIReportLocationModel
    {
        [JsonProperty(PropertyName = "Description")]
        public string Description { get; set; }
        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "ID")]
        public string ID { get; set; }
        [JsonProperty(PropertyName = "CaseNumbers")]
        public List<RKIReportCaseModel> CaseNumbers { get; set; }
    }
}