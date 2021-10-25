
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace SmICSCoreLib.Factories.Employees.ContactTracing
{
    public class ContactTracingPostModel : ContactTracingModel
    {
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
        [JsonProperty(PropertyName = "Schutzkleidung1")]
        [Required]
        public string Schutzkleidung1 { get; set; }
        [JsonProperty(PropertyName = "Person1")]
        [Required]
        public string Person1 { get; set; }
        [JsonProperty(PropertyName = "Schutzkleidung2")]
        [Required]
        public string Schutzkleidung2 { get; set; }
        [JsonProperty(PropertyName = "Person2")]
        [Required]
        public string Person2 { get; set; }
    }
}
