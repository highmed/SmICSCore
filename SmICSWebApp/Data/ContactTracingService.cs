using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmICSWebApp.Data;
using Microsoft.OpenApi.Expressions;
using Microsoft.IdentityModel.Tokens;
using System.IO;
using SmICSCoreLib.REST;

namespace SmICSWebApp.Data
{
    public class ContactTracingService
    {
        public void ContactTracingDataStorage(JObject createEntry)
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

                    //string ehr_id = IRestDataAccess.CreateEhrIDWithStatus("SmICSTest", "Patient35");
                    //CreateComposition(ehr_id, writeResult);
                }

            }
            catch (Exception)
            {

            }

        }
    }
}
