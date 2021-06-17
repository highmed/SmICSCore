using Newtonsoft.Json;
using SmICSCoreLib.AQL.PatientInformation.Patient_Mibi_Labordaten;
using System.Collections.Generic;

namespace SmICSCoreLib.AQL.Contact_Nth_Network
{
    public class MibiContactModel : ContactModel
    {
        [JsonProperty(PropertyName = "Labordaten")]
        public List<MibiLabDataModel> LaborData { get; set; }
    }
}
