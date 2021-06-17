using Newtonsoft.Json;
using SmICSCoreLib.AQL.PatientInformation.Patient_Labordaten;
using System.Collections.Generic;


namespace SmICSCoreLib.AQL.Contact_Nth_Network
{
    class ViroContactModel : ContactModel
    {
        [JsonProperty(PropertyName = "Labordaten")]
        public List<LabDataModel> LaborData { get; set; }
    }
}
