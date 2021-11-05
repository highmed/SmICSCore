using Newtonsoft.Json;
using System;

namespace SmICSCoreLib.Factories.PatientStay.Count.ReceiveModel
{
    public class CountDataReceiveModel
    {
        [JsonProperty(PropertyName = "PatientID")]
        public string PatientID { get; set; }

        [JsonProperty(PropertyName = "Fallkennung")]
        public string Fallkennung { get; set; }

        [JsonProperty(PropertyName = "Zeitpunkt_der_Probenentnahme")]
        public DateTime Zeitpunkt_der_Probenentnahme { get; set; }
    }
}
