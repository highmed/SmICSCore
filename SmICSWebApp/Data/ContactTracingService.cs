using Newtonsoft.Json.Linq;
using System;
using System.IO;
using SmICSCoreLib.REST;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using SmICSCoreLib.AQL.Employees.ContactTracing;
using System.Collections.Generic;
using SmICSCoreLib.AQL;

namespace SmICSWebApp.Data
{
    public class ContactTracingService
    {
        private IRestDataAccess _restData;
        private ILogger<ContactTracingFactory> _logger;
        public ContactTracingService(IRestDataAccess restData, ILogger<ContactTracingFactory> logger)
        {
            _restData = restData;
            _logger = logger;
        }
        public void ContactTracingDataStorage(JObject createEntry, string ehr_id)
        {
            try
            {
                if (createEntry != null)
                {

                    string filepath = @"../SmICSWebApp/Data/FormTemplates/Bericht_zur_Kontaktverfolgung.json";
                    string readResult = string.Empty;
                    string writeResult = string.Empty;
                    using (StreamReader r = new StreamReader(filepath))
                    {
                        var json = r.ReadToEnd();
                        var jobj = JObject.Parse(json);
                        readResult = jobj.ToString();

                        jobj["context"]["start_time"]["value"] = JObject.Parse(createEntry.ToString())["dokumentations_id"];
                        jobj["context"]["other_context"]["items"][0]["value"]["value"] = JObject.Parse(createEntry.ToString())["bericht_id"];
                        jobj["context"]["other_context"]["items"][0]["value"]["value"] = JObject.Parse(createEntry.ToString())["bericht_id"];
                        jobj["context"]["other_context"]["items"][1]["items"][0]["value"]["value"] = JObject.Parse(createEntry.ToString())["event_kennung"];
                        jobj["context"]["other_context"]["items"][1]["items"][1]["value"]["value"] = JObject.Parse(createEntry.ToString())["event_art"];
                        jobj["context"]["other_context"]["items"][1]["items"][2]["items"][0]["value"]["value"] = JObject.Parse(createEntry.ToString())["art_der_person_1"];
                        jobj["context"]["other_context"]["items"][1]["items"][2]["items"][1]["value"]["value"] = JObject.Parse(createEntry.ToString())["art_der_person_1_ID"];
                        jobj["context"]["other_context"]["items"][1]["items"][3]["items"][0]["value"]["value"] = JObject.Parse(createEntry.ToString())["art_der_person_2"];
                        jobj["context"]["other_context"]["items"][1]["items"][3]["items"][1]["value"]["value"] = JObject.Parse(createEntry.ToString())["art_der_person_2_ID"];
                        jobj["context"]["other_context"]["items"][1]["items"][4]["value"]["value"] = JObject.Parse(createEntry.ToString())["event_kategorie"];
                        jobj["context"]["other_context"]["items"][1]["items"][5]["value"]["value"] = JObject.Parse(createEntry.ToString())["kontakt_kommentar"];
                        jobj["content"][0]["description"]["items"][0]["value"]["value"] = JObject.Parse(createEntry.ToString())["beschreibung"];
                        jobj["content"][0]["description"]["items"][1]["value"]["value"] = JObject.Parse(createEntry.ToString())["beginn"];
                        jobj["content"][0]["description"]["items"][2]["value"]["value"] = JObject.Parse(createEntry.ToString())["ende"];
                        jobj["content"][0]["description"]["items"][3]["value"]["value"] = JObject.Parse(createEntry.ToString())["ort"];
                        jobj["content"][0]["description"]["items"][4]["value"]["value"] = JObject.Parse(createEntry.ToString())["gesamtdauer"];
                        jobj["content"][0]["description"]["items"][5]["value"]["value"] = JObject.Parse(createEntry.ToString())["abstand"];
                        jobj["content"][0]["description"]["items"][6]["items"][0]["value"]["value"] = JObject.Parse(createEntry.ToString())["schutzkleidung"];
                        jobj["content"][0]["description"]["items"][6]["items"][1]["value"]["value"] = JObject.Parse(createEntry.ToString())["person"];

                        string person = (string)JObject.Parse(createEntry.ToString())["person"];

                        if (person != "Indexperson")
                        {
                            jobj["content"][0]["description"]["items"][6]["items"][1]["value"]["defining_code"]["code_string"] = "at0004";
                        }
                        else
                        {
                            jobj["content"][0]["description"]["items"][6]["items"][1]["value"]["defining_code"]["code_string"] = "at0003";
                        }

                        jobj["content"][0]["description"]["items"][7]["value"]["value"] = JObject.Parse(createEntry.ToString())["kommentar"];

                        writeResult = jobj.ToString();

                    }

                    File.WriteAllText(filepath, writeResult);

                    SaveComposition(ehr_id, writeResult);
                }

            }
            catch (Exception)
            {
                throw new Exception($"Failed to POST data");
            }

        }

        public void SaveComposition(string subjectID, string writeResult)
        {
            string ehr_id = ExistsSubject(_restData, subjectID);

            if (ehr_id == null)
            {
                    HttpResponseMessage result = _restData.CreateEhrIDWithStatus("SmICSTest", subjectID).Result;
                    ehr_id = result.Headers.ETag.ToString();
                  
            }
            else
            {
                throw new Exception($"Failed to POST data");
            }

            HttpResponseMessage responseMessage = _restData.CreateComposition(ehr_id, writeResult).Result;
            System.Diagnostics.Debug.WriteLine(responseMessage);

            if (responseMessage.StatusCode != System.Net.HttpStatusCode.Created)
            {
                string returnValue = responseMessage.Content.ReadAsStringAsync().Result;
                throw new Exception($"Failed to POST data: ({responseMessage.StatusCode}): {returnValue}");
            }
            else
            {
                string returnValue = responseMessage.Content.ReadAsStringAsync().Result;
                _logger.LogInformation($"Succeded to POST data: ({responseMessage.StatusCode}): {returnValue}");
            }
                
        }

        private string ExistsSubject(IRestDataAccess _data, string subjectID)
        {
            List<Employee> subject = _data.AQLQuery<Employee>(AQLCatalog.GetEHRID(subjectID));
            return subject != null ? subject[0].ID : null;
        }

        private class Employee
        {
            public string ID { get; set; }
            public string Status { get; set; }
        }
    }
    
}
