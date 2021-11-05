using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.EpiCurve;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;


namespace SmICSCoreLib.Factories.Lab.ViroLabData.ReceiveModel
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
        public DateTime? ZeitpunktProbeneingang { get; set; }
        [JsonProperty(PropertyName = "Befundzeit")]
        public DateTime Befundzeit { get; set; }
        [JsonProperty(PropertyName = "MaterialID")]
        public string MaterialID { get; set; } = "Abs";
        [JsonProperty(PropertyName = "Material_l")]
        [System.ComponentModel.DefaultValue(" ")]
        public string Material_l { get; set; }
        [JsonProperty(PropertyName = "Befund")]
        public string Befund { get; set; }
        [JsonProperty(PropertyName = "BefundCode")]
        public string BefundCode { get; set; }
        [JsonProperty(PropertyName = "Viruslast")]
        public string Viruslast { get; set; }
        [JsonProperty(PropertyName = "Befundkommentar")]
        public string Befundkommentar { get; set; }
        [JsonProperty(PropertyName = "KeimID")]
        public string KeimID { get; set; } = "COV";
        [JsonProperty(PropertyName = "Keim_l")]
        public string Keim_l { get; set; } = "SARS-CoV-2";
        [JsonProperty(PropertyName = "Befunddatum")]
        public DateTime Befunddatum { get; set; }
    }
}
