using Newtonsoft.Json.Linq;

namespace SmICSCoreLib.AQL.ConnectionTest
{
    public interface IConnectionTest
    {
        JArray Test();
    }
}