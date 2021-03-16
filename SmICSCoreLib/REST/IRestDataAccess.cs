using System.Collections.Generic;
using System.Net.Http;

namespace SmICSCoreLib.REST
{
    public interface IRestDataAccess
    {
        List<T> AQLQuery<T>(string query) where T : new();

        //int AQLQueryInt<T>(string query) where T : new();

        void UpdateEHRStatus(string ehrID, bool queryable);
    }
}