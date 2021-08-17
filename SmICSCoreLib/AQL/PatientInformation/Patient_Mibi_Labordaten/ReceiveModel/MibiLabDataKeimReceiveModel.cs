using Newtonsoft.Json;

namespace SmICSCoreLib.AQL.PatientInformation.Patient_Mibi_Labordaten.ReceiveModel
{
    public class MibiLabDataKeimReceiveModel
    {
        [JsonProperty(PropertyName = "KeimID")]
        public string KeimID { get; set; }

    }
}
