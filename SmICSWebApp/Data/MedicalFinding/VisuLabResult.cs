using Newtonsoft.Json;
using SmICSCoreLib.Factories.General;
using System;

namespace SmICSWebApp.Data.MedicalFinding
{
    public class VisuLabResult : Case
    {
        [JsonProperty("LabordatenID")]
        public string LabID { get; internal set; }
        [JsonProperty("Eingangsdatum")]
        public DateTime SpecimenCollectionDateTime { get; internal set; }
        [JsonProperty("ZeitpunktProbeneingang")]
        public DateTime? SpecimenReceiptDateTime { get; internal set; } 
        [JsonProperty("Probenart")]
        public string MaterialID { get; internal set; }
        [JsonProperty("Material_l")]
        public string Material { get; internal set; }
        [JsonProperty("Befund")]
        public bool Result { get; internal set; }
        [JsonProperty("Befundtext")]
        public string ResultText { get; internal set; }
        [JsonProperty("Keim_l")]
        public string Pathogen { get; internal set; }
        [JsonProperty("KeimID")]
        public string PathogenID { get; internal set; }
        [JsonProperty("Befunddatum")]
        public DateTime ResultDate { get; internal set; }
        [JsonProperty("Befundkommentar")]
        public object Comment { get; internal set; }
        [JsonProperty("Quantity")]
        public string Quantity { get; internal set; }
        [JsonProperty("Einheit")]
        public string Unit { get; internal set; }
        [JsonProperty("Bereich")]
        public string Kind { get; internal set; }
    }
}