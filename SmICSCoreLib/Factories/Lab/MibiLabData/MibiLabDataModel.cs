using Newtonsoft.Json;
using SmICSCoreLib.Factories.LabData.MibiLabdata.ReceiveModel;
using System;

namespace SmICSCoreLib.Factories.Lab.MibiLabData
{
    public class MibiLabDataModel
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
        [JsonProperty(PropertyName = "MREKlasse")]
        public string MREKlasse { get; set; }
        [JsonProperty(PropertyName = "Befunddatum")]
        public DateTime? Befunddatum { get; set; }

        public MibiLabDataModel() { }
        public MibiLabDataModel(MetaDataReceiveModel metaData, SampleReceiveModel sampleData, PathogenReceiveModel pathogenData)
        {
            ZeitpunktProbenentnahme = sampleData.ZeitpunktProbeentnahme;
            ZeitpunktProbeneingang = sampleData.ZeitpunktProbeneingang;
            MaterialID = sampleData.MaterialID;
            Material_l = sampleData.MaterialID;
            Befund = (pathogenData.Befund == "Nachweis") ? true : false;
            Befundkommentar = pathogenData.Befundkommentar;
            KeimID = pathogenData.KeimID;
            LabordatenID = sampleData.LabordatenID + pathogenData.IsolatNo;
            PatientID = metaData.PatientID;
            FallID = metaData.FallID;
            ProbeID = sampleData.LabordatenID;
            MREKlasse = pathogenData.MREKlasse;
        }
    }
}