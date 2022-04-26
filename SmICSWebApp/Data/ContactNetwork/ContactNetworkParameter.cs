
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace SmICSWebApp.Data.ContactNetwork
{
    public class ContactNetworkParameter : SmICSCoreLib.Factories.General.Patient
    {
        [JsonProperty("degree")]
        public int Degree { get; set; }
        [JsonProperty("starttime")]
        public DateTime starttime { get; set; }
        [JsonProperty("endtime")]
        public DateTime endtime { get; set; }
        [JsonProperty("pathogen")]
        public string pathogen { get; set; }

    }
}
