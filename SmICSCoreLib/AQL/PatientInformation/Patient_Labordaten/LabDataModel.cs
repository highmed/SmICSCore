using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Newtonsoft.Json;
using SmICSCoreLib.AQL.Algorithm.NEC;
using SmICSCoreLib.AQL.PatientInformation.Patient_Labordaten.ReceiveModel;

namespace SmICSCoreLib.AQL.PatientInformation.Patient_Labordaten
{
    public class LabDataModel
    {
        [JsonProperty(PropertyName = "LabordatenID")] //Bericht ID - result-report context
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
        [JsonProperty(PropertyName = "Probenart")]
        public string MaterialID { get; set; } = "Abs";

        [JsonProperty(PropertyName = "Material_l")]
        [DefaultValue(" ")]
        public string Material_l { get; set; }
        [JsonProperty(PropertyName = "Befund")]
        public bool Befund { get; set; }
        [JsonProperty(PropertyName = "Befundkommentar")]
        public string Befundkommentar { get; set; }
        [JsonProperty(PropertyName = "KeimID")]
        public string KeimID { get; set; } = "COV";
        [JsonProperty(PropertyName = "Befunddatum")]
        public DateTime Befunddatum { get; set; } //context_time

        public LabDataModel() { }
        public LabDataModel(LabDataReceiveModel labDataReceive)
        {
            LabordatenID = labDataReceive.LabordatenID;
            PatientID = labDataReceive.PatientID;
            FallID = labDataReceive.FallID;
            ProbeID = labDataReceive.LabordatenID;
            ZeitpunktProbenentnahme = labDataReceive.ZeitpunktProbenentnahme;
            ZeitpunktProbeneingang = labDataReceive.ZeitpunktProbeneingang;
            MaterialID = labDataReceive.MaterialID == null ? "MissingID" : labDataReceive.MaterialID;
            Material_l = labDataReceive.Material_l == null ? " " : labDataReceive.Material_l;
            Befund = (labDataReceive.BefundCode == "260373001") ? true : false;
            KeimID = labDataReceive.KeimID;
            Befunddatum = labDataReceive.Befunddatum;
            Befundkommentar = labDataReceive.Befundkommentar;
        }
    }

   
}
