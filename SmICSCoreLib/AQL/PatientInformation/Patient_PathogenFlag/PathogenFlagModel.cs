using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace SmICSCoreLib.AQL.PatientInformation.Patient_PathogenFlag
{
    public class PathogenFlagModel
    {
        [JsonProperty(PropertyName = "PatientID")]
        public string PatientID { get; set; }
        [JsonProperty(PropertyName = "DokumentationsDatum")]
        public DateTime DokumentationsDatum { get; set; }
        [JsonProperty(PropertyName = "Flag")]
        public bool Flag { get; set; }
        [JsonProperty(PropertyName = "KeimID")]
        public string KeimID { get; set; }
        [JsonProperty(PropertyName = "Keim_l")]
        public string Keim_l { get; set; }
    }
}
