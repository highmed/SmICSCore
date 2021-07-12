using System;
using Newtonsoft.Json;

namespace SmICSCoreLib.StatistikDataModels
{
    public class Patient
    {
        [JsonProperty(PropertyName = "PatientID")]
        public string PatientID { get; set; }

        [JsonProperty(PropertyName = "Zeitpunkt_der_Probenentnahme")]
        public DateTime Probenentnahme { get; set; }

        [JsonProperty(PropertyName = "Aufnahme_Datum")]
        public DateTime Aufnahme { get; set; }

        [JsonProperty(PropertyName = "Entlastung_cDatum")]
        public DateTime Entlastung { get; set; }

        public Patient() { }
        public Patient(string patientID, DateTime probenentnahme, DateTime aufnahme, DateTime entlastung)
        {
            PatientID = patientID;
            Probenentnahme = probenentnahme;
            Aufnahme = aufnahme;
            Entlastung = entlastung;
        }
    }
}
