using Newtonsoft.Json;

namespace SmICSCoreLib.Factories.NUMNode
{
    public class NUMNodeSaveModel
    {
        [JsonProperty(PropertyName = "CountPatient")]
        public int CountPatient { get; set; }
        [JsonProperty(PropertyName = "NumberOfStays")]
        public int NumberOfStays { get; set; }
        [JsonProperty(PropertyName = "NumberOfNosCases")]
        public int NumberOfNosCases { get; set; }
        [JsonProperty(PropertyName = "NumberOfMaybeNosCases")]
        public int NumberOfMaybeNosCases { get; set; }
        [JsonProperty(PropertyName = "NumberOfContacts")]
        public int NumberOfContacts { get; set; }

    }
}