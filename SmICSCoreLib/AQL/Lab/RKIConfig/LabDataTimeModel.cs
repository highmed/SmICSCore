using Newtonsoft.Json;



namespace SmICSCoreLib.AQL.Lab.RKIConfig
{
    public class LabDataTimeModel
    {
        public LabDataTimeModel() { }

        public LabDataTimeModel(string zeitpunkt)
        {
            Zeitpunkt = zeitpunkt;
        }

        [JsonProperty(PropertyName = "Zeitpunkt")]
        public string Zeitpunkt { get; set; }

    }
}
