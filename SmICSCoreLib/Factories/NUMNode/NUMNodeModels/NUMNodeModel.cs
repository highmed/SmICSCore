using Newtonsoft.Json;
using System;

namespace SmICSCoreLib.Factories.NUMNode
{
    public class NUMNodeModel
    {
        [JsonProperty(PropertyName = "provider")]
        public string Provider { get; set; }
        [JsonProperty(PropertyName = "corona_dashboard_dataset_version")]
        public string CDDV { get; set; }
        [JsonProperty(PropertyName = "exporttimestamp")]
        public DateTime Timestamp { get; set; }
        [JsonProperty(PropertyName = "author")]
        public string Author { get; set; }
        [JsonProperty(PropertyName = "dataitems")]
        public NUMNodeDataItems Dataitems { get; set; }

    }
}