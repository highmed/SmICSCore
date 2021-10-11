using System;
using Newtonsoft.Json;

namespace SmICSCoreLib.Factories.DetectionAlgorithmInterface
{
    public class ConfigSiteParametersReceiveModel
    {
        [JsonProperty(PropertyName = "Datum")]
        public DateTime Datum { get; set; }
        [JsonProperty(PropertyName = "PathogenCodestring")]
        public string PathogenCodestring { get; set; }
        [JsonProperty(PropertyName = "LookbackWeeks")]
        public int LookbackWeeks { get; set; }
        [JsonProperty(PropertyName = "EvaluationTimeHourMinute")]
        public string EvaluationTimeHourMinute { get; set; }
        /*string konsole_string_00 = "0115";
        Console.WriteLine(Convert.ToInt32(konsole_string_00.Substring(0, 2)));
        Console.WriteLine(Convert.ToInt32(konsole_string_00.Substring(2, 2)));*/
        [JsonProperty(PropertyName = "Ward")]
        public string Ward { get; set; }
        [JsonProperty(PropertyName = "Institution")]
        public string Institution { get; set; }
    }
}
