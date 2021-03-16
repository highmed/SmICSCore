using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SmICSCoreLib.REST
{
    public class RestDataAccess : IRestDataAccess
    {
        private RestClientConnector _client;
        public RestDataAccess(RestClientConnector client)
        {
            _client = client;
        }

        public List<T> AQLQuery<T>(string query) where T : new()
        {
            string restPath = "/query/aql";
            HttpResponseMessage response = _client.Client.PostAsync(OpenehrConfig.openehrEndpoint + restPath, GetHttpContentQuery(query)).Result;
            //System.Diagnostics.Debug.Print(response.RequestMessage.ToString());
            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    return null;
                }
                return openEHRJSONSerializer<T>.ReceiveModelConstructor(response);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
                return null;
            }
        }
        public List<string> GetTemplates()
        {
            string restPath = "/definition/template/adl1.4";
            HttpResponseMessage response = _client.Client.GetAsync(OpenehrConfig.openehrEndpoint + restPath).Result;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                List<string> templateIDs = new List<string>();

                string json = response.Content.ReadAsStringAsync().Result;

                JArray arr = JsonConvert.DeserializeObject<JArray>(json);
                foreach (JObject obj in arr.Children<JObject>())
                {
                    string templateID = obj.Property("template_id").Value.ToString();
                    templateIDs.Add(templateID);
                }
                return templateIDs;
            }
            else
            {
                return null;
            }
        }
        public Task<HttpResponseMessage> SetTemplate(string value)
        {
            string restPath = "/definition/template/adl1.4";
            return _client.Client.PostAsync(OpenehrConfig.openehrEndpoint + restPath, GetHttpContentADL(value));
        }
        public Task<HttpResponseMessage> CreateComposition(string ehr_id, string json)
        {
            string restPath = "/ehr/" + ehr_id + "/composition";
            return _client.Client.PostAsync(OpenehrConfig.openehrEndpoint + restPath, GetHttpContent(json));
        }
        public Task<HttpResponseMessage> CreateEhrIDWithStatus(string Namespace, string ID)
        {
            string restPath = "/ehr";
            return  _client.Client.PostAsync(OpenehrConfig.openehrEndpoint + restPath, GetEHRStatus(Namespace, ID));
        }


        public void UpdateEHRStatus(string ehrID, bool queryable)
        {
            string restPath = "/ehr/"+ehrID+"/ehr_status";
            HttpResponseMessage response = _client.Client.GetAsync(OpenehrConfig.openehrEndpoint + restPath).Result;
            string content = response.Content.ReadAsStringAsync().Result;
            JObject ehrStatus = JsonConvert.DeserializeObject<JObject>(content, new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Local
            });
            bool isQueryable = (bool)ehrStatus.Property("is_queryable").Value;
            if (isQueryable == queryable)
            {
                return;
            }
            ehrStatus.Property("is_queryable").Value = queryable;
            _client.Client.PutAsync(OpenehrConfig.openehrEndpoint + restPath, ConvertJObjectToHTTPResponse(ehrStatus));
        }
        private HttpContent GetHttpContentADL(string xml)
        {
            HttpContent content = new StringContent(xml, Encoding.UTF8, "application/xml");
            return content;
        }

        private HttpContent GetHttpContent(string json)
        {
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
            content.Headers.Add("Prefer", "return=representation");
            return content;
        }

        private HttpContent GetHttpContentQuery(string query)
        {
            JObject obj = new JObject();
            //obj.Add("aql", query);
            obj.Add("q", query);
            string json = JsonConvert.SerializeObject(obj, Formatting.Indented);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

            return content;
        }

        private HttpContent ConvertJObjectToHTTPResponse(JObject obj)
        {
            string json = JsonConvert.SerializeObject(obj, Formatting.Indented);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
            return content;
        }

        private HttpContent GetEHRStatus(string Namespace, string ID)
        {
            string json = "{ \"_type\": \"EHR_STATUS\", \"archetype_node_id\": \"openEHR-EHR-ITEM_TREE.generic.v1\", \"name\":{ \"_type\": \"DV_TEXT\", \"value\": \"any EHR STATUS\"}, \"subject\": { \"external_ref\":{ \"_type\": \"PARTY_REF\", \"id\":{ \"_type\": \"GENERIC_ID\", \"value\": \"" + ID + "\", \"scheme\": \"id_scheme\" }, \"namespace\": \"" + Namespace + "\", \"type\": \"PERSON\" }}, \"is_queryable\": true, \"is_modifiable\": true }";
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
            content.Headers.Add("Prefer", "return=representation");
            return content;
        }
    }
}
