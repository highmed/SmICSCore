using Newtonsoft.Json;
using System;

namespace SmICSCoreLib.Factories.NUMNode
{
    public class NUMNodeModel
    {
        [JsonProperty(PropertyName = "AverageNumberOfStays")]
        public int AverageNumberOfStays { get; set; }
        [JsonProperty(PropertyName = "AverageNumberOfNosCases")]
        public int AverageNumberOfNosCases { get; set; }
        [JsonProperty(PropertyName = "AverageNumberOfContacts")]
        public int AverageNumberOfContacts { get; set; }
        [JsonProperty(PropertyName = "DateTime")]
        public DateTime DateTime { get; set; }

    }
}