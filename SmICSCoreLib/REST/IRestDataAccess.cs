using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SmICSCoreLib.REST
{
    public interface IRestDataAccess
    {
        List<T> AQLQuery<T>(string query) where T : new();

        //int AQLQueryInt<T>(string query) where T : new();

        void UpdateEHRStatus(string ehrID, bool queryable);

        List<string> GetTemplates();
        Task<HttpResponseMessage> SetTemplate(string value);
        Task<HttpResponseMessage> CreateComposition(string ehr_id, string json);
        Task<HttpResponseMessage> CreateEhrIDWithStatus(string Namespace, string ID);
    }
}