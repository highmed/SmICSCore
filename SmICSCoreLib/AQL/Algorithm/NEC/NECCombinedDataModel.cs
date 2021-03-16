using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmICSCoreLib.AQL.Algorithm.NEC
{
    public class NECCombinedDataModel
    {
        [JsonProperty(PropertyName = "LabourData")]
        public List<NECPatientLabDataModel> labdat { get; set; }
        [JsonProperty(PropertyName = "MovementData")]
        public List<NECPatientMovementDataModel> movementData { get; set; }
    }
}
