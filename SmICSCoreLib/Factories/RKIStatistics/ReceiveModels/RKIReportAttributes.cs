using Newtonsoft.Json;

namespace SmICSCoreLib.Factories.RKIStatistics.ReceiveModels
{
    public class RKIReportAttributes<T>
    {
        [JsonProperty(PropertyName = "attributes")]
        public T attributes { get; set; }

    }
}