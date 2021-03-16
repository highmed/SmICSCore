using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace SmICSCoreLib.AQL.PatientInformation.Patient_DiagnosticResult
{
    public class DiagnosticResultModel
    {
        [JsonProperty(PropertyName = "PatientID")]
        public string PatientID { get; set; }
        [JsonProperty(PropertyName = "ICD_Code")]
        public string ICD_Code { get; set; }
        [JsonProperty(PropertyName = "Diagnose")]
        public string Diagnose { get; set; }
        [JsonProperty(PropertyName = "Freitextbeschreibung")]
        public string Freitextbeschreibung { get; set; }
        [JsonProperty(PropertyName = "DokumentationsDatum")]
        public DateTime DokumentationsDatum { get; set; }
        

    }
}
