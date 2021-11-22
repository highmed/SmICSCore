using Newtonsoft.Json;
using SmICSCoreLib.Factories.ContactNetwork;
using SmICSCoreLib.Factories.Lab.MibiLabData;
using System.Collections.Generic;

namespace SmICSCoreLib.Factories.Contact_Nth_Network
{
    public class MibiContactModel : ContactModel
    {
        [JsonProperty(PropertyName = "Labordaten")]
        public List<MibiLabDataModel> LaborData { get; set; }
    }
}
