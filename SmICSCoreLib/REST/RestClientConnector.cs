using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace SmICSCoreLib.REST
{
    public class RestClientConnector
    {
        private HttpClientHandler handler = null;
        public HttpClient Client { get; }

        public RestClientConnector()
        {
            handler = new HttpClientHandler();
            handler.Credentials = new NetworkCredential(OpenehrConfig.openehrUser, OpenehrConfig.openehrPassword);
            Client = new HttpClient(handler);
            Client.Timeout = TimeSpan.FromMilliseconds(OpenehrConfig.queryTimeout);
            Client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }       
    }
}
