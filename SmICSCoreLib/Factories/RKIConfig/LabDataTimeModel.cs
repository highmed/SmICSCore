using Newtonsoft.Json;

namespace SmICSCoreLib.Factories.RKIConfig
{
    public class LabDataTimeModel
    {
        [JsonProperty(PropertyName = "Zeitpunkt")]
        public string Zeitpunkt { get; set; }

    }
}
