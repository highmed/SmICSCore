using Newtonsoft.Json;

namespace SmICSCoreLib.Factories.NUMNode
{
    public class NUMNodeDataItems
    {
        [JsonProperty(PropertyName = "itemname")]
        public string Itemname { get; set; }
        [JsonProperty(PropertyName = "itemtype")]
        public string Itemtype { get; set; }
        [JsonProperty(PropertyName = "data")]
        public NUMNodeData Data { get; set; }

    }
}