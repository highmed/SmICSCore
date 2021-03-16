using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace SmICSCoreLib.AQL.PatientInformation.Symptome
{
    public class SymptomModel
    {
        [JsonProperty(PropertyName ="PatientenID")]
        public string PatientenID { get; set; }
        [JsonProperty(PropertyName = "BefundID")]
        public string BefundID { get; set; }
        [JsonProperty(PropertyName = "NameDesSymptoms")]
        public string NameDesSymptoms { get; set; }
        [JsonProperty(PropertyName = "Lokalisation")]
        public string Lokalisation { get; set; }
        [JsonProperty(PropertyName = "Beginn")]
        public DateTime Beginn { get; set; }
        [JsonProperty(PropertyName = "Schweregrad")]
        public string Schweregrad { get; set; }
        [JsonProperty(PropertyName = "Rueckgang")]
        public DateTime Rueckgang { get; set; }
        [JsonProperty(PropertyName = "AusschlussAussage")]
        public string AusschlussAussage { get; set; }
        [JsonProperty(PropertyName = "Diagnose")]
        public string Diagnose { get; set; }
        [JsonProperty(PropertyName = "UnbekanntesSymptom")]
        public string UnbekanntesSymptom { get; set; }
        [JsonProperty(PropertyName = "AussageFehlendeInfo")]
        public string AussageFehlendeInfo { get; set; }
    }
}
