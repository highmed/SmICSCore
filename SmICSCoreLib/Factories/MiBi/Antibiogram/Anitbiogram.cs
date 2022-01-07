using Newtonsoft.Json;

namespace SmICSCoreLib.Factories.MiBi
{
    public class Antibiogram
    {
        public string Antibiotic { get; set; }
        public string AntibioticID { get; set; }
        public string Resistance { get; set; }
        public int MinInhibitorConcentration { get; set; }
        public string MICUnit { get; set; }
        [JsonIgnore]
        public bool Collapsed { get; set; } = false;
    }
}
