using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

using System.Threading.Tasks;

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
            Client.Timeout = TimeSpan.FromMilliseconds(300000);
            Client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }       
    }
}
