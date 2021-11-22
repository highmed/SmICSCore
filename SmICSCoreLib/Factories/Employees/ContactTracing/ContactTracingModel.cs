
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace SmICSCoreLib.Factories.Employees.ContactTracing
{
    public class ContactTracingModel
    {
        [JsonProperty(PropertyName = "PatientID")]
        public string PatientID { get; set; }
        [JsonProperty(PropertyName = "BerichtID")]
        [Required]
        public string BerichtID { get; set; }
        [JsonProperty(PropertyName = "DokumentationsID")]
        public string DokumentationsID { get; set; }
        [JsonProperty(PropertyName = "EventKennung")]
        [Required]
        public string EventKennung { get; set; }
        [JsonProperty(PropertyName = "EventArt")]
        [Required]
        public string EventArt { get; set; }
        [JsonProperty(PropertyName = "EventKategorie")]
        [Required]
        public string EventKategorie { get; set; }
        [JsonProperty(PropertyName = "EventKommentar")]
        public string EventKommentar { get; set; }
        [JsonProperty(PropertyName = "Beschreibung")]
        public string Beschreibung { get; set; }
        [JsonProperty(PropertyName = "Beginn")]
        [Required]
        public string Beginn { get; set; } = null;
        [JsonProperty(PropertyName = "Ende")]
        [Required]
        public string Ende { get; set; }
        [JsonProperty(PropertyName = "Ort")]
        [Required]
        public string Ort { get; set; }
        [JsonProperty(PropertyName = "Gesamtdauer")]
        [Required]
        public string Gesamtdauer { get; set; }
        [JsonProperty(PropertyName = "Abstand")]
        [Required]
        public string Abstand { get; set; }
        [JsonProperty(PropertyName = "Kommentar")]
        public string Kommentar { get; set; }
    }
}
