using SmICSCoreLib.Authentication;
using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace SmICSCoreLib.REST
{
    public class RestClientConnector
    {
        private HttpClientHandler handler = null;

        public HttpClient Client { get; }

        public RestClientConnector(TokenProvider token)
        {
            handler = new HttpClientHandler();
            //handler.Credentials = new NetworkCredential(OpenehrConfig.openehrUser, OpenehrConfig.openehrPassword);
            Client = new HttpClient(handler);
            Client.Timeout = TimeSpan.FromMilliseconds(300000);
            Client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
        }       
    }
}
