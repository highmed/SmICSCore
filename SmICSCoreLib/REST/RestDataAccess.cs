using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmICSCoreLib.Factories;

namespace SmICSCoreLib.REST
{
    public class RestDataAccess : IRestDataAccess
    {
        private RestClientConnector _client;
        private ILogger<RestDataAccess> _logger;
        public RestDataAccess(ILogger<RestDataAccess> logger, RestClientConnector client)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<List<T>> AQLQueryAsync<T>(AQLQuery query) where T : new()
        {
            try{
                _logger.LogInformation("Posted Query: {Query}", query.Name);
                string restPath = "rest/openehr/v1/query/aql";
                Uri RestPath = GetRequestUri(restPath);
                HttpResponseMessage response = await _client.Client.PostAsync(RestPath, GetHttpContentQuery(query.ToString()));
                //System.Diagnostics.Debug.Print(response.RequestMessage.ToString());
                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == HttpStatusCode.NoContent)
                    {
                        return new List<T>();//return null;
                    }
                    _logger.LogInformation("Received AQL Result From {Query}", query.Name);
                    _logger.LogDebug("AQL Result: {Result}", response.Content.ReadAsStringAsync().Result);
                    return openEHRJSONSerializer<T>.ReceiveModelConstructor(response);
                }
                else
                {
                    _logger.LogInformation("NO AQL Result Received {Query} \n {QueryComplete}", query.Name, query.Query);
                    if(response.StatusCode == HttpStatusCode.RequestTimeout || response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        HttpError error = response.Content.ReadAsAsync<HttpError>().GetAwaiter().GetResult();
                        _logger.LogError("No Success Code: {status} \n {msg}", response.StatusCode, error.Message);
                        throw new HttpRequestException("No Success StatusCode: " + response.StatusCode + "\n" + error.Message);
                    }
                    else if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        _logger.LogError("No Success StatusCode: {status} \n Couldn't connect to {server}", response.StatusCode, RestPath.ToString());
                        throw new HttpRequestException("Couldn't connect to " + RestPath.ToString());
                    }
                    else if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        _logger.LogError("No Success StatusCode: {status} \n Couldn't connect to {server}: Unauthorized", response.StatusCode, RestPath.ToString());
                        throw new HttpRequestException("Couldn't connect to " + RestPath.ToString() + ": Unauthorized");
                    }
                    throw new HttpRequestException("Unknown Error " + response.StatusCode);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("RestDataAccess.AQLQuery:\n" + query.ToString() + "\n"+ e.Message);
                throw;
            }
        }
        public List<string> GetTemplates()
        {
            string restPath = "/definition/template/adl1.4";
            HttpResponseMessage response = _client.Client.GetAsync(GetRequestUri(restPath)).Result;
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
            return _client.Client.PostAsync(GetRequestUri(restPath), GetHttpContentADL(value));
        }
        public Task<HttpResponseMessage> CreateComposition(string ehr_id, string json)
        {
            string restPath = "/ehr/" + ehr_id + "/composition";
            return _client.Client.PostAsync(GetRequestUri(restPath), GetHttpContent(json));
        }
        public Task<HttpResponseMessage> CreateEhrIDWithStatus(string Namespace, string ID)
        {
            string restPath = "/ehr";
            return  _client.Client.PostAsync(GetRequestUri(restPath), GetEHRStatus(Namespace, ID));
        }

        public void SetAuthenticationHeader(string token)
        {
            if (token != "NoToken")
            {
                _client.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Split(" ")[1]);
            }
        }
        #region private Methods
        private HttpContent GetHttpContentADL(string xml)
        {
            HttpContent content = new StringContent(xml, Encoding.UTF8, "application/xml");
            return content;
        }

        private HttpContent GetHttpContent(string json)
        {
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
            //content.Headers.Add("Prefer", "return=representation");
            return content;
        }

        private HttpContent GetHttpContentQuery(string query)
        {
            string q = query;
            try
            {
                JObject obj = new JObject();
                
                if (!string.IsNullOrEmpty(OpenehrConfig.queryLimit))
                {
                    q = q + " LIMIT " + OpenehrConfig.queryLimit;
                }
                obj.Add("q", q);
                string json = JsonConvert.SerializeObject(obj, Formatting.Indented);
                //_client.Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
                HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

                return content;
            } 
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString() + "\n" + q);
                throw;
            }
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

        private Uri GetRequestUri(string relativePath)
        {
            return new Uri(string.Join("/", OpenehrConfig.openehrEndpoint.TrimEnd('/'), relativePath.TrimStart('/')));
        }
        #endregion
    }
}
