using Newtonsoft.Json;
using SmICSCoreLib.AQL.Algorithm.NEC.ReceiveModel;
using SmICSCoreLib.AQL.PatientInformation.PatientMovement.ReceiveModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmICSCoreLib.AQL.Algorithm.NEC
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

