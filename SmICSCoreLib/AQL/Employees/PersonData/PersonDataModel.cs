using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Newtonsoft.Json;

namespace SmICSCoreLib.AQL.Employees.PersonData
{
    public class PersonDataModel
    {
        [JsonProperty(PropertyName = "personID")]
        [Required]
        public string personID { get; set; }
        [JsonProperty(PropertyName = "art_der_person")]
        [Required]
        public string art_der_person { get; set; }
        [JsonProperty(PropertyName = "titel")]
        [Required]
        public string titel { get; set; }
        [JsonProperty(PropertyName = "vorname")]
        [Required]
        public string vorname { get; set; }
        [JsonProperty(PropertyName = "weiterer_vorname")]
        [Required]
        public string weiterer_vorname { get; set; }
        [JsonProperty(PropertyName = "nachname")]
        [Required]
        public string nachname { get; set; }
        [JsonProperty(PropertyName = "suffix")]
        [Required]
        public string suffix { get; set; }
        [JsonProperty(PropertyName = "geburtsdatum")]
        [Required]
        public Nullable<DateTime> geburtsdatum { get; set; } = null;
        [JsonProperty(PropertyName = "zeile")]
        [Required]
        public string zeile { get; set; }
        [JsonProperty(PropertyName = "stadt")]
        [Required]
        public string stadt { get; set; }
        [JsonProperty(PropertyName = "plz")]
        [Required]
        public string plz { get; set; }
        [JsonProperty(PropertyName = "kontakttyp")]
        [Required]
        public string kontakttyp { get; set; }
        [JsonProperty(PropertyName = "nummer")]
        [Required]
        public string nummer { get; set; }
        [JsonProperty(PropertyName = "fach_bez")]
        [Required]
        public string fach_bez { get; set; }
        [JsonProperty(PropertyName = "zeile_heil")]
        [Required]
        public string zeile_heil { get; set; }
        [JsonProperty(PropertyName = "stadt_heil")]
        [Required]
        public string stadt_heil { get; set; }
        [JsonProperty(PropertyName = "plz_heil")]
        [Required]
        public string plz_heil { get; set; }
    }
}
