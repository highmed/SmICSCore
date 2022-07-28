using Newtonsoft.Json;
using System;

namespace SmICSCoreLib.Factories.NUMNode
{
    public class NUMNodeData
    {
        [JsonProperty(PropertyName = "average_number_of_stays")]
        public double AverageNumberOfStays { get; set; }
        [JsonProperty(PropertyName = "average_number_of_nos_cases")]
        public double AverageNumberOfNosCases { get; set; }
        [JsonProperty(PropertyName = "average_number_of_maybe_nos_cases")]
        public double AverageNumberOfMaybeNosCases { get; set; }
        [JsonProperty(PropertyName = "average_number_of_contacts")]
        public double AverageNumberOfContacts { get; set; }

    }
}