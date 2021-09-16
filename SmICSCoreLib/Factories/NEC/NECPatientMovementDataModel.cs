using Newtonsoft.Json;
using SmICSCoreLib.Factories.NEC.ReceiveModel;
using SmICSCoreLib.Factories.PatientMovement.ReceiveModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmICSCoreLib.Factories.NEC
{
    public class NECPatientMovementDataModel
    {
        [JsonProperty(PropertyName = "PatientID")]
        public string PatientID { get; set; }
        [JsonProperty(PropertyName = "Beginn")]
        public DateTime Beginn { get; set; }
        [JsonProperty(PropertyName = "Ende")]
        public DateTime Ende { get; set; }
        [JsonProperty(PropertyName = "StationID")]
        public string StationID { get; set; }
        [JsonProperty(PropertyName = "BewegungstypID")]
        public int BewegungstypID { get; set; }

    }
}

