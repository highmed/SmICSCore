using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmICSCoreLib.AQL.General;
using SmICSCoreLib.AQL.Lab.EpiKurve;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;


namespace SmICSCoreLib.AQL.PatientInformation.Patient_Labordaten.ReceiveModel
{
    public class LabDataReceiveModel
    {
        [JsonProperty(PropertyName = "LabordatenID")]
        public string LabordatenID { get; set; }
        [JsonProperty(PropertyName = "PatientID")]
        public string PatientID { get; set; }
        [JsonProperty(PropertyName = "FallID")]
        public string FallID { get; set; }
        [JsonProperty(PropertyName = "ProbeID")]
        public string ProbeID { get; set; }
        [JsonProperty(PropertyName = "ZeitpunktProbenentnahme")]
        public DateTime ZeitpunktProbenentnahme { get; set; }
        [JsonProperty(PropertyName = "ZeitpunktProbeneingang")]
        public DateTime ZeitpunktProbeneingang { get; set; }
        [JsonProperty(PropertyName = "MaterialID")]
        public string MaterialID { get; set; } = "Abs";
        [JsonProperty(PropertyName = "Material_l")]
        [System.ComponentModel.DefaultValue(" ")]
        public string Material_l { get; set; }
        [JsonProperty(PropertyName = "Befund")]
        public string Befund { get; set; }
        [JsonProperty(PropertyName = "Befundkommentar")]
        public string Befundkommentar { get; set; }
        [JsonProperty(PropertyName = "KeimID")]
        public string KeimID { get; set; } = "COV";
        [JsonProperty(PropertyName = "Befunddatum")]
        public DateTime Befunddatum { get; set; }
    }
}
