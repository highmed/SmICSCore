using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmICSCoreLib.Factories.PatientStay.Count.ReceiveModel
{
    public class CountDataReceiveModel
    {
        [JsonProperty(PropertyName = "PatientID")]
        public string PatientID { get; set; }

        [JsonProperty(PropertyName = "Fallkennung")]
        public string Fallkennung { get; set; }

        [JsonProperty(PropertyName = "Zeitpunkt_des_Probeneingangs")]
        public DateTime Zeitpunkt_des_Probeneingangs { get; set; }
    }
}
