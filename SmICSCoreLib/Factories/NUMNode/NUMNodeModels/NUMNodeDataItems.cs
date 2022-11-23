using Newtonsoft.Json;

namespace SmICSCoreLib.Factories.NUMNode
{
    public class NUMNodeDataItems
    {
        [JsonProperty(PropertyName = "itemname")]
        public string itemname { get; set; }
        [JsonProperty(PropertyName = "itemtype")]
        public string itemtype { get; set; }
        [JsonProperty(PropertyName = "data")]
        public NUMNodeData data { get; set; }

    }
}