using Newtonsoft.Json;
using System;


namespace SmICSCoreLib.AQL.General
{
    public class LabData : Case
    {
        [JsonProperty(PropertyName = "LabordatenID")]
        public string LabordatenID { get; set; }
        [JsonProperty(PropertyName = "ProbeID")]
        public string ProbeID { get; set; }
        [JsonProperty(PropertyName = "Eingangsdatum")]
        public DateTime ZeitpunktProbenentnahme { get; set; }
        [JsonProperty(PropertyName = "ZeitpunktProbeneingang")]
        public DateTime ZeitpunktProbeneingang { get; set; }
        [JsonProperty(PropertyName = "Probenart")]
        public string MaterialID { get; set; }
        [JsonProperty(PropertyName = "Material_l")]
        [System.ComponentModel.DefaultValue(" ")]
        public string Material_l { get; set; }
        [JsonProperty(PropertyName = "Befund")]
        public bool Befund { get; set; }
        [JsonProperty(PropertyName = "Befundkommentar")]
        public string Befundkommentar { get; set; }
        [JsonProperty(PropertyName = "KeimID")]
        public string KeimID { get; set; }
        [JsonProperty(PropertyName = "Screening")]  //Nur für Visualisierung -> nicht essentiell
        public bool screening { get; set; } = false;
        [JsonProperty(PropertyName = "Befunddatum")]
        public DateTime? Befunddatum { get; set; }
    }
}
