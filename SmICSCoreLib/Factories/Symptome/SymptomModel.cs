using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace SmICSCoreLib.Factories.Symptome
{
    public class SymptomModel
    {
        [JsonProperty(PropertyName ="PatientenID")]
        public string PatientenID { get; set; }

        [JsonProperty(PropertyName = "BefundID")]
        public string BefundID { get; set; }
        [JsonProperty(PropertyName = "BefundDatum")]
        public DateTime BefundDatum { get; set; }
        [JsonProperty(PropertyName = "NameDesSymptoms")]
        public string NameDesSymptoms { get; set; }

        [JsonProperty(PropertyName = "Anzahl_Patienten")]
        public int Anzahl_Patienten { get; set; }

        [JsonProperty(PropertyName = "Lokalisation")]
        public string Lokalisation { get; set; }
        [JsonProperty(PropertyName = "Beginn")]
        public Nullable<DateTime> Beginn { get; set; } = null;
        [JsonProperty(PropertyName = "Schweregrad")]
        public string Schweregrad { get; set; }
        [JsonProperty(PropertyName = "Rueckgang")]
        public Nullable<DateTime> Rueckgang { get; set; } = null;
        [JsonProperty(PropertyName = "AusschlussAussage")]
        public string AusschlussAussage { get; set; }
        [JsonProperty(PropertyName = "Diagnose")]
        public string Diagnose { get; set; }
        [JsonProperty(PropertyName = "UnbekanntesSymptom")]
        public string UnbekanntesSymptom { get; set; }
        [JsonProperty(PropertyName = "AussageFehlendeInfo")]
        public string AussageFehlendeInfo { get; set; }

        public SymptomModel(){}

        public SymptomModel(string nameDesSymptoms, int anzahl_Patienten)
        {
            NameDesSymptoms = nameDesSymptoms;
            Anzahl_Patienten = anzahl_Patienten;
        }
    }
}
