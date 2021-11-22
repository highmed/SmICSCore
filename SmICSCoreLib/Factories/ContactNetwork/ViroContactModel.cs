using Newtonsoft.Json;
using SmICSCoreLib.Factories.ContactNetwork;
using SmICSCoreLib.Factories.Lab.ViroLabData;
using System.Collections.Generic;


namespace SmICSCoreLib.Factories.Contact_Nth_Network
{
    class ViroContactModel : ContactModel
    {
        [JsonProperty(PropertyName = "Labordaten")]
        public List<LabDataModel> LaborData { get; set; }
    }
}
