using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Newtonsoft.Json;

namespace SmICSCoreLib.AQL.Employees.ContactTracing
{
    public class ContactTracingModel
    {
        [JsonProperty(PropertyName = "bericht_id")]
        [Required]
        public string bericht_id { get; set; }
        [JsonProperty(PropertyName = "dokumentations_id")]
        [Required]
        public DateTime dokumentations_id { get; set; } = DateTime.Now;
        [JsonProperty(PropertyName = "event_kennung")]
        [Required]
        public string event_kennung { get; set; }
        [JsonProperty(PropertyName = "event_art")]
        [Required]
        public string event_art { get; set; }
        [JsonProperty(PropertyName = "art_der_person")]
        [Required]
        public string art_der_person { get; set; }
        [JsonProperty(PropertyName = "event_kategorie")]
        [Required]
        public string event_kategorie { get; set; }
        [JsonProperty(PropertyName = "kontakt_kommentar")]
        public string kontakt_kommentar { get; set; }
        [JsonProperty(PropertyName = "beschreibung")]
        public string beschreibung { get; set; }
        [JsonProperty(PropertyName = "beginn")]
        [Required]
        public string beginn { get; set; } = null;
        [JsonProperty(PropertyName = "ende")]
        [Required]
        public string ende { get; set; }
        [JsonProperty(PropertyName = "ort")]
        [Required]
        public string ort { get; set; }
        [JsonProperty(PropertyName = "gesamtdauer")]
        [Required]
        public string gesamtdauer { get; set; }
        [JsonProperty(PropertyName = "abstand")]
        [Required]
        public string abstand { get; set; }
        [JsonProperty(PropertyName = "schutzkleidung")]
        [Required]
        public string schutzkleidung { get; set; }
        [JsonProperty(PropertyName = "person")]
        [Required]
        public string person { get; set; }
        [JsonProperty(PropertyName = "kommentar")]
        public string kommentar { get; set; }
    }
}
