using Newtonsoft.Json;
using System.Collections.Generic;

namespace SmICSCoreLib.OutbreakDetection
{
    public class TransfersScriptModel
    {
        [JsonProperty("epoch")]
        public List<int> Epoch { get; set; }

        [JsonProperty("freq")]
        public int Frequency { get; set; }
        [JsonProperty("observed")]
        public List<int> Observed { get; set; }
    }
}
