using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmICSCoreLib.AQL.Patient_Stay.Count.ReceiveModel
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
