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
        [JsonProperty(PropertyName = "CountStays")]
        public int CountStays { get; set; }
        [JsonProperty(PropertyName = "CountNosCases")]
        public int CountNosCases { get; set; }
        [JsonProperty(PropertyName = "CountMaybeNosCases")]
        public int CountMaybeNosCases { get; set; }
        [JsonProperty(PropertyName = "CountContacts")]
        public int CountContacts { get; set; }
    }
}