using Newtonsoft.Json;



namespace SmICSCoreLib.AQL.Lab.RKIConfig
{
    public class LabDataTimeModel
    {
        [JsonProperty(PropertyName = "Zeitpunkt")]
        public string Zeitpunkt { get; set; }

    }
}
