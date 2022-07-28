using Newtonsoft.Json;
using System;

namespace SmICSCoreLib.Factories.NUMNode
{
    public class LabPatientModel
    {
        [JsonProperty(PropertyName = "PatientID")]
        public string PatientID { get; set; }
        [JsonProperty(PropertyName = "CaseID")]
        public string CaseID { get; set; }
        [JsonProperty(PropertyName = "Starttime")]
        public DateTime Starttime { get; set; }
        [JsonProperty(PropertyName = "Endtime")]
        public DateTime? Endtime { get; set; }

    }
}