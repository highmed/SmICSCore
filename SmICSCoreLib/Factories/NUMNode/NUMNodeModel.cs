using Newtonsoft.Json;
using System;

namespace SmICSCoreLib.Factories.NUMNode
{
    public class NUMNodeModel
    {
        [JsonProperty(PropertyName = "AverageNumberOfStays")]
        public double AverageNumberOfStays { get; set; }
        [JsonProperty(PropertyName = "AverageNumberOfNosCases")]
        public double AverageNumberOfNosCases { get; set; }
        [JsonProperty(PropertyName = "AverageNumberOfMaybeNosCases")]
        public double AverageNumberOfMaybeNosCases { get; set; }
        [JsonProperty(PropertyName = "AverageNumberOfContacts")]
        public double AverageNumberOfContacts { get; set; }
        [JsonProperty(PropertyName = "DateTime")]
        public DateTime DateTime { get; set; }

    }
}