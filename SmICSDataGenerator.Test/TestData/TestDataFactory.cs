using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmICSCoreLib.Factories;
using SmICSCoreLib.REST;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Xunit;

namespace SmICSCoreLib.Tests.TestData
{
    public class TestDataFactory
    {
        [Fact]
        public async void CreateTemplates()
        {
            List<string> templateIDs = new List<string>() { "Virologischer Befund", "Stationärer Versorgungsfall", "Symptom", "Impfstatus", "Patientenaufenthalt" };

            RestDataAccess _data = CreateDataAccess();

            List<string> exisitingTemplateID = _data.GetTemplates();

            Assert.NotNull(exisitingTemplateID);

            if (exisitingTemplateID.Count > 0)
            {
                foreach (string id in exisitingTemplateID)
                {
                    if (templateIDs.Contains(id))
                    {
                        templateIDs.Remove(id);
                    }
                }
            }
            
            if(templateIDs.Count > 0)
            {
                foreach(string id in templateIDs)
                {
                    if (id == "Stationärer Versorgungsfall" || id == "Patientenaufenthalt" || id == "Virologischer Befund")
                    {
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.Load(@"../../../../TestData/templates/" + id.Replace(" ", "_") + ".opt");

                        StringWriter stringWriter = new StringWriter();
                        XmlTextWriter xmlWriter = new XmlTextWriter(stringWriter);
                        xmlDoc.WriteTo(xmlWriter);
                        string xmlString = stringWriter.ToString();
                        System.Diagnostics.Debug.WriteLine(xmlString);
                        HttpResponseMessage response = _data.SetTemplate(xmlString).Result;
                        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
                    }
                }
            } 
            else
            { 
                Assert.Equal(templateIDs.Count, 0);
            }
        }

        [Fact]
        public void CreateTestPatients()
        {
            JArray patientArray = new JArray();
            RestDataAccess _data = CreateDataAccess();
            for (int i = 1; i <= 38; i++)
            {
                JObject patientObj = new JObject();
                string patientNo = i.ToString();
                if (patientNo.Length == 1)
                {
                    patientNo = "0" + patientNo;
                }
                string ehr_id = ExistsPatient(_data, "Patient"+patientNo);
                if (ehr_id != null)
                {
                    patientObj.Add(new JProperty("EHR_ID", ehr_id));
                    patientObj.Add(new JProperty("Patient", "Patient"+patientNo));
                    patientArray.Add(patientObj);
                    continue;
                }

                HttpResponseMessage response = _data.CreateEhrIDWithStatus("SmICSTest", "Patient" + patientNo).Result;
                Assert.Equal(HttpStatusCode.Created, response.StatusCode);

                ehr_id = GetEHR_ID(response);

                string[] compositions = Directory.GetFiles(@"../../../../TestData/compositions/json/patient" + patientNo + "/");
                foreach (string comp in compositions)
                {
                    using (StreamReader reader = new StreamReader(comp))
                    {
                        string json = reader.ReadToEnd();
                        response = _data.CreateComposition(ehr_id, json).Result;
                        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                    }
                }
                patientObj.Add(new JProperty("EHR_ID", ehr_id));
                patientObj.Add(new JProperty("Patient", "Patient" + patientNo));
                patientArray.Add(patientObj); ;
            }
            JSONFileStream.JSONWriter.Write(patientArray, @"../../../../TestData/", "GeneratedEHRIDs");
        }

        private string ExistsPatient(RestDataAccess _data, string patientNo)
        {
            List<Patient> patient = _data.AQLQuery<Patient>(AQLCatalog.GetEHRID(patientNo));
            return patient != null ? patient[0].ID : null;
        }

        private RestDataAccess CreateDataAccess()
        {
            OpenehrConfig.openehrEndpoint = Environment.GetEnvironmentVariable("OPENEHR_DB");
            OpenehrConfig.openehrUser = Environment.GetEnvironmentVariable("OPENEHR_USER");
            OpenehrConfig.openehrPassword = Environment.GetEnvironmentVariable("OPENEHR_PASSWD");
            //OpenehrConfig.openehrEndpoint = "http://localhost:8080/ehrbase/rest/openehr/v1";
            //OpenehrConfig.openehrEndpoint = "https://plri-highmed01.mh-hannover.local:8083/rest/openehr/v1";
            //OpenehrConfig.openehrUser = "etltestuser";
            //OpenehrConfig.openehrPassword = "etltestuser#01";

            RestClientConnector restClient = new RestClientConnector();
            return new RestDataAccess(NullLogger<RestDataAccess>.Instance, restClient);

        }

        private string GetEHR_ID(HttpResponseMessage response)
        {
            string json = response.Content.ReadAsStringAsync().Result;
            JObject obj = JsonConvert.DeserializeObject<JObject>(json);
            obj = obj.Property("ehr_id").Value as JObject;
            string ehr_id = obj.Property("value").Value.ToString();

            return ehr_id;
        }

        private class Patient
        {
            public string ID { get; set; }
        }
    }
}
