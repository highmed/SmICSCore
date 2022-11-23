using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace SmICSCoreLib.Factories.NUMNode
{
    public class NUMNodeModel
    {
        [JsonProperty(PropertyName = "provider")]
        public string provider { get; set; }
        [JsonProperty(PropertyName = "corona_dashboard_dataset_version")]
        public string corona_dashboard_dataset_version { get; set; }
        [JsonProperty(PropertyName = "exporttimestamp")]
        public DateTime exporttimestamp { get; set; }
        [JsonProperty(PropertyName = "author")]
        public string author { get; set; }
        [JsonProperty(PropertyName = "dataitems")]
        public List<NUMNodeDataItems> dataitems { get; set; }

    }
}