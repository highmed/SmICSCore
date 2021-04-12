using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmICSCoreLib.REST;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using Xunit;


namespace SmICSConnection.Tests
{
    public class RestClientTest
    {

        [Fact]
        public async void Endpoint_ConnectionTest()
        {
            OpenehrConfig.openehrEndpoint = Environment.GetEnvironmentVariable("OPENEHR_DB");
            OpenehrConfig.openehrUser = Environment.GetEnvironmentVariable("OPENEHR_USER");
            OpenehrConfig.openehrPassword = Environment.GetEnvironmentVariable("OPENEHR_PASSWD");
            //OpenehrConfig.openehrEndpoint = "http://192.168.2.132:8080/ehrbase/rest/openehr/v1";
            //OpenehrConfig.openehrEndpoint = "https://plri-highmed01.mh-hannover.local:8083/rest/openehr/v1";
            //OpenehrConfig.openehrUser = "etltestuser";
            //OpenehrConfig.openehrPassword = "etltestuser#01";

            var restClient = new RestClientConnector();

            JObject obj = new JObject();
            obj.Add("q", "SELECT e/ehr_id/value FROM EHR e");
            string json = JsonConvert.SerializeObject(obj, Formatting.Indented);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await restClient.Client.PostAsync(OpenehrConfig.openehrEndpoint + "/query/aql", content);
            
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
