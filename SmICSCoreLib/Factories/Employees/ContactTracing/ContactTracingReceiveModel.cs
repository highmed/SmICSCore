
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace SmICSCoreLib.Factories.Employees.ContactTracing
{
    public class ContactTracingReceiveModel : ContactTracingModel
    {
        [JsonProperty(PropertyName = "ArtDerPerson")]
        [Required]
        public string ArtDerPerson { get; set; }
        [JsonProperty(PropertyName = "PersonenID")]
        [Required]
        public string PersonenID { get; set; }
        [JsonProperty(PropertyName = "Schutzkleidung")]
        [Required]
        public string Schutzkleidung { get; set; }
        [JsonProperty(PropertyName = "Person")]
        [Required]
        public string Person { get; set; }
    }
}
