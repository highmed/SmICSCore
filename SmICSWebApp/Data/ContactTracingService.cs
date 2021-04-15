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
        public ContactTracingService(IRestDataAccess restData)
        {
            _restData = restData;
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
                        jobj["context"]["other_context"]["items"][1]["items"][2]["items"][0]["value"]["value"] = JObject.Parse(createEntry.ToString())["art_der_person"];
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
                        //Console.WriteLine(writeResult);

                    }
                    //System.Diagnostics.Debug.WriteLine(readResult);
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
            System.Diagnostics.Debug.WriteLine(ehr_id);

            if (ehr_id == null)
            {
                string ehr_status = GetLastPatientStatus(_restData);
                System.Diagnostics.Debug.WriteLine(ehr_status);
                if (ehr_status != null)
                {
                    string ehr_statusNr = ehr_status.Remove(0, 6);
                    System.Diagnostics.Debug.WriteLine(ehr_statusNr);
                    HttpResponseMessage result = _restData.CreateEhrIDWithStatus("SmICSTest", "Patient" + ehr_statusNr + 1).Result;
                    ehr_id = result.Headers.ETag.ToString();
                    System.Diagnostics.Debug.WriteLine(ehr_id);
                }
                else
                {
                    throw new Exception($"Failed to POST data");
                }
                
            }

            HttpResponseMessage responseMessage = _restData.CreateComposition(ehr_id, writeResult).Result;
            System.Diagnostics.Debug.WriteLine(responseMessage);
            if (responseMessage.StatusCode != System.Net.HttpStatusCode.Created)
            {
                string returnValue = responseMessage.Content.ReadAsStringAsync().Result;
                throw new Exception($"Failed to POST data: ({responseMessage.StatusCode}): {returnValue}");
            }
            else
                throw new Exception($"Failed to POST data");
        }

        private string ExistsSubject(IRestDataAccess _data, string subjectID)
        {
            List<Employee> subject = _data.AQLQuery<Employee>(AQLCatalog.GetEHRID(subjectID));
            return subject != null ? subject[0].EmployeeID : null;
        }

        private string GetLastPatientStatus(IRestDataAccess _data)
        {
            List<Employee> patient = _data.AQLQuery<Employee>(AQLCatalog.GetLastEHRStatus());
            return patient != null ? patient[0].EmployeeStatus : null;
        }

        private class Employee
        {
            public string EmployeeID { get; set; }
            public string EmployeeStatus { get; set; }
        }
    }
    
}
