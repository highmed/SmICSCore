
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace SmICSCoreLib.Factories.Employees.PersInfoInfecCtrl
{
    public class PersInfoInfecCtrlModel
    {
        [JsonProperty(PropertyName = "PatientID")]
        public string PatientID { get; set; }
        [JsonProperty(PropertyName = "BerichtID")]
        [Required]
        public string BerichtID { get; set; }
        [JsonProperty(PropertyName = "DokumentationsID")]
        public string DokumentationsID { get; set; }
        [JsonProperty(PropertyName = "SymptomVorhanden")]
        public string SymptomVorhanden { get; set; }
        [JsonProperty(PropertyName = "AufgetretenSeit")]
        public string AufgetretenSeit { get; set; }
        [JsonProperty(PropertyName = "Symptom")]
        [Required]
        public string Symptom { get; set; }
        [JsonProperty(PropertyName = "SymptomKommentar")]
        public string SymptomKommentar { get; set; }
        [JsonProperty(PropertyName = "Nachweis")]
        public bool Nachweis { get; set; } = false;
        [JsonProperty(PropertyName = "Erregername")]
        public string Erregername { get; set; }
        [JsonProperty(PropertyName = "Zeitpunkt")]
        public string Zeitpunkt { get; set; }
        [JsonProperty(PropertyName = "KlinischerNachweis")]
        public bool KlinischerNachweis { get; set; } = false;
        [JsonProperty(PropertyName = "LetzteAktualisierung")]
        public string LetzteAktualisierung { get; set; }
        [JsonProperty(PropertyName = "Freistellung")]
        public bool Freistellung { get; set; } = false;
        [JsonProperty(PropertyName = "Grund")]
        public string Grund { get; set; }
        [JsonProperty(PropertyName = "Beschreibung")]
        public string Beschreibung { get; set; }
        [JsonProperty(PropertyName = "Startdatum")]
        public string Startdatum { get; set; }
        [JsonProperty(PropertyName = "Enddatum")]
        public string Enddatum { get; set; }
        [JsonProperty(PropertyName = "AbwesendheitKommentar")]
        public string AbwesendheitKommentar { get; set; }
        [JsonProperty(PropertyName = "Meldung")]
        public bool Meldung { get; set; } = false;
        [JsonProperty(PropertyName = "Ereignis")]
        public string Ereignis { get; set; }
        [JsonProperty(PropertyName = "Ereignisbeschreibung")]
        public string Ereignisbeschreibung { get; set; }
        [JsonProperty(PropertyName = "Datum")]
        public string Datum { get; set; }
        [JsonProperty(PropertyName = "Ereignisgrund")]
        public string Ereignisgrund { get; set; }
        [JsonProperty(PropertyName = "EreignisKommentar")]
        public string EreignisKommentar { get; set; }
    }
}
