using SmICSCoreLib.Factories;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SmICSCoreLib.REST
{
    public interface IRestDataAccess
    {
        List<T> AQLQuery<T>(AQLQuery query) where T : new();
        List<string> GetTemplates();
        Task<HttpResponseMessage> SetTemplate(string value);
        Task<HttpResponseMessage> CreateComposition(string ehr_id, string json);
        Task<HttpResponseMessage> CreateEhrIDWithStatus(string Namespace, string ID);
        void SetAuthenticationHeader(string token);
    }
}