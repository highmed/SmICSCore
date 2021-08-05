
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace SmICSCoreLib.AQL.Employees.ContactTracing
{
    public class ContactTracingReceiveModel : ContactTracingModel
    {
        [JsonProperty(PropertyName = "ArtDerPerson")]
        [Required]
        public string ArtDerPerson { get; set; }
        [JsonProperty(PropertyName = "PersonenID")]
        [Required]
        public string PersonenID { get; set; }
    }
}
