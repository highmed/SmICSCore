using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Newtonsoft.Json;

namespace SmICSCoreLib.AQL.Employees.ContactTracing
{
    public class ContactTracingModel
    {
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
        [JsonProperty(PropertyName = "ArtDerPerson1")]
        [Required]
        public string ArtDerPerson1 { get; set; }
        [JsonProperty(PropertyName = "PersonenID1")]
        [Required]
        public string PersonenID1 { get; set; }
        [JsonProperty(PropertyName = "ArtDerPerson2")]
        [Required]
        public string ArtDerPerson2 { get; set; }
        [JsonProperty(PropertyName = "PersonenID2")]
        [Required]
        public string PersonenID2 { get; set; }
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
        [JsonProperty(PropertyName = "Schutzkleidung")]
        [Required]
        public string Schutzkleidung { get; set; }
        [JsonProperty(PropertyName = "Person")]
        [Required]
        public string Person { get; set; }
        [JsonProperty(PropertyName = "Kommentar")]
        public string Kommentar { get; set; }
    }
}
