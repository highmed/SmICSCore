using Newtonsoft.Json;

namespace SmICSCoreLib.Factories.General
{
    public class Case : Patient
    {
        [JsonProperty(PropertyName = "FallID")]
        public string CaseID { get; set; }
        public Case() : base() {}
        public Case(Case Case) : base (Case)
        {
            CaseID = Case.CaseID;
        }
    }
}
