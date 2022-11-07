using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace SmICSCoreLib.REST
{
    public class DashboardRestClientConnector
    {
        private HttpClientHandler handler = null;
        public HttpClient Client { get; }

        public DashboardRestClientConnector()
        {
            handler = new HttpClientHandler
            {
                Credentials = new NetworkCredential(DashboardConfig.dashboardUser, DashboardConfig.dashboardPassword)
            };
            Client = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromMilliseconds(400000)
            };
            Client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }       
    }
}
