using Newtonsoft.Json;



namespace SmICSCoreLib.Factories.RKIConfig
{
    public class LabDataKeimReceiveModel
    {
        [JsonProperty(PropertyName = "KeimID")]
        public string KeimID { get; set; }

    }
}
