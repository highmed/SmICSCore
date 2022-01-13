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

        public bool Equals(Case other)
        {
            if(base.Equals(other))
            {
                return CaseID == other.CaseID;
            }
            return false;
        }
    }
}
