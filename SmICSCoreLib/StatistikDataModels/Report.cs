using Newtonsoft.Json;

namespace SmICSCoreLib.StatistikDataModels
{
    public class Report
    {
        [JsonProperty(PropertyName = "Datum")]
        public string Datum { get; set; }

        [JsonProperty(PropertyName = "BLReport")]
        public BLReport[] BLReport { get; set; }
    }

    public class BLReport
    {
        [JsonProperty(PropertyName = "BlName")]
        public string BlName { get; set; }

        [JsonProperty(PropertyName = "BLReportAttribute")]
        public BLReportAttribute[] BLReportAttribute { get; set; }
    }

    public class BLReportAttribute
    {
        [JsonProperty(PropertyName = "Datum")]
        public string Datum { get; set; }
        
        [JsonProperty(PropertyName = "Fahlzahl")]
        public int Fahlzahl { get; set; }
    }

    public class LKReportJson
    {
        [JsonProperty(PropertyName = "Datum")]
        public string Datum { get; set; }

        [JsonProperty(PropertyName = "LKReport")]
        public LKReport[] LKReport { get; set; }
    }

    public class LKReport
    {
        [JsonProperty(PropertyName = "LKName")]
        public string LKName { get; set; }

        [JsonProperty(PropertyName = "AdmUnitId")]
        public int AdmUnitId { get; set; }

        [JsonProperty(PropertyName = "LKReportAttribute")]
        public LKReportAttribute[] LKReportAttribute { get; set; }
    }

    public class LKReportAttribute
    {
        [JsonProperty(PropertyName = "Datum")]
        public string Datum { get; set; }

        [JsonProperty(PropertyName = "Fahlzahl")]
        public int Fahlzahl { get; set; }
    }
}
