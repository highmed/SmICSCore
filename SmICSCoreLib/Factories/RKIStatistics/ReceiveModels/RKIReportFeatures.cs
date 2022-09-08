using Newtonsoft.Json;

namespace SmICSCoreLib.Factories.RKIStatistics.ReceiveModels
{
    public class RKIReportFeatures<T>
    {
        [JsonProperty(PropertyName = "features")]
        public RKIReportAttributes<T>[] features { get; set; }

    }
}