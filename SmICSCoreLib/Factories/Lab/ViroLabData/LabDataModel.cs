using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Newtonsoft.Json;
using SmICSCoreLib.Factories.NEC;
using SmICSCoreLib.Factories.Lab.ViroLabData.ReceiveModel;

namespace SmICSCoreLib.Factories.Lab.ViroLabData
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
       
        [JsonProperty(PropertyName = "Eingangsdatum")]
        public DateTime ZeitpunktProbenentnahme { get; set; }
        
        [JsonProperty(PropertyName = "ZeitpunktProbeneingang")]
        public DateTime? ZeitpunktProbeneingang { get; set; }

        [JsonProperty(PropertyName = "Viruslast")]
        public string Viruslast { get; set; }

        [JsonProperty(PropertyName = "Probenart")]
        public string MaterialID { get; set; }
        
        [JsonProperty(PropertyName = "Screening")]  //Nur für Visualisierung -> nicht essentiell
        public bool screening { get; set; } = false; //Nur für Visualisierung -> nicht essentiell
        
        [JsonProperty(PropertyName = "Material_l")]
        [DefaultValue(" ")]
        public string Material_l { get; set; }
        
        [JsonProperty(PropertyName = "Befund")]
        public bool Befund { get; set; }
        
        [JsonProperty(PropertyName = "Befundkommentar")]
        public string Befundkommentar { get; set; }
        
        [JsonProperty(PropertyName = "KeimID")]
        public string KeimID { get; set; }
       
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
            Viruslast = labDataReceive.Viruslast;
        }
    }

   
}
