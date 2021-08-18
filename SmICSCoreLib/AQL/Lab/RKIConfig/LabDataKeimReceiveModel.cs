using Newtonsoft.Json;



namespace SmICSCoreLib.AQL.Lab.RKIConfig
{
    public class LabDataKeimReceiveModel
    {
        [JsonProperty(PropertyName = "KeimID")]
        public string KeimID { get; set; }

    }
}
