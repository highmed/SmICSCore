
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace SmICSCoreLib.Factories.Employees.PersonData
{
    public class PersonDataModel
    {
        [JsonProperty(PropertyName = "PatientID")]
        public string PatientID { get; set; }
        [JsonProperty(PropertyName = "PersonID")]
        [Required]
        public string PersonID { get; set; }
        [JsonProperty(PropertyName = "DokumentationsID")]
        public string DokumentationsID { get; set; }
        [JsonProperty(PropertyName = "ArtDerPerson")]
        [Required]
        public string ArtDerPerson { get; set; }
        [JsonProperty(PropertyName = "Titel")]
        public string Titel { get; set; }
        [JsonProperty(PropertyName = "Vorname")]
        [Required]
        public string Vorname { get; set; }
        [JsonProperty(PropertyName = "WeitererVorname")]
        public string WeitererVorname { get; set; }
        [JsonProperty(PropertyName = "Nachname")]
        [Required]
        public string Nachname { get; set; }
        [JsonProperty(PropertyName = "Suffix")]
        public string Suffix { get; set; }
        [JsonProperty(PropertyName = "Geburtsdatum")]
        [Required]
        public string Geburtsdatum { get; set; }
        [JsonProperty(PropertyName = "Zeile")]
        [Required]
        public string Zeile { get; set; }
        [JsonProperty(PropertyName = "Stadt")]
        [Required]
        public string Stadt { get; set; }
        [JsonProperty(PropertyName = "Plz")]
        [Required]
        public string Plz { get; set; }
        [JsonProperty(PropertyName = "Kontakttyp")]
        [Required]
        public string Kontakttyp { get; set; }
        [JsonProperty(PropertyName = "Nummer")]
        [Required]
        public string Nummer { get; set; }
        [JsonProperty(PropertyName = "Fachbezeichnung")]
        [Required]
        public string Fachbezeichnung { get; set; }
        [JsonProperty(PropertyName = "HeilZeile")]
        [Required]
        public string HeilZeile { get; set; }
        [JsonProperty(PropertyName = "HeilStadt")]
        [Required]
        public string HeilStadt { get; set; }
        [JsonProperty(PropertyName = "HeilPLZ")]
        [Required]
        public string HeilPLZ { get; set; }
    }
}
