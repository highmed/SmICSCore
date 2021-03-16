using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmICSWebApp.Data;
using Microsoft.OpenApi.Expressions;
using Microsoft.IdentityModel.Tokens;
using System.IO;

namespace SmICSWebApp.Data
{
    public class PersonInformationService
    {
        public static void PersonInformationDataStorage(JObject createEntry)
        {
             try
             {
                 if(createEntry != null)
                 {
                    string json = File.ReadAllText("FormTemplates/PersonendatenComposition.json");
                    dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

                    jsonObj["context"]["other_context"]["items"][0]["value"]["value"]= JObject.Parse(createEntry.ToString())["personID"];
                    jsonObj["content"][0]["data"]["items"][0]["value"]["value"] = JObject.Parse(createEntry.ToString())["art_der_person"];
                    //jsonObj["content"][0]["data"]["items"][0]["value"]["value"] = JObject.Parse(createEntry.ToString())["titel"];
                    //jsonObj["content"][0]["data"]["items"][0]["value"]["value"] = JObject.Parse(createEntry.ToString())["vorname"];
                    //jsonObj["content"][0]["data"]["items"][0]["value"]["value"] = JObject.Parse(createEntry.ToString())["weiterer_vorname"];
                    //jsonObj["content"][0]["data"]["items"][0]["value"]["value"] = JObject.Parse(createEntry.ToString())["nachname"];
                    //jsonObj["content"][0]["data"]["items"][0]["value"]["value"] = JObject.Parse(createEntry.ToString())["suffix"];
                    //jsonObj["content"][0]["data"]["items"][0]["value"]["value"] = JObject.Parse(createEntry.ToString())["geburtsdatum"];
                    //jsonObj["content"][0]["data"]["items"][0]["value"]["value"] = JObject.Parse(createEntry.ToString())["zeile"];
                    //jsonObj["content"][0]["data"]["items"][0]["value"]["value"] = JObject.Parse(createEntry.ToString())["stadt"];
                    //jsonObj["content"][0]["data"]["items"][0]["value"]["value"] = JObject.Parse(createEntry.ToString())["plz"];
                    //jsonObj["content"][0]["data"]["items"][0]["value"]["value"] = JObject.Parse(createEntry.ToString())["kontakttyp"];
                    //jsonObj["content"][0]["data"]["items"][0]["value"]["value"] = JObject.Parse(createEntry.ToString())["nummer"];
                    //jsonObj["content"][0]["data"]["items"][0]["value"]["value"] = JObject.Parse(createEntry.ToString())["fach_bez"];
                    //jsonObj["content"][0]["data"]["items"][0]["value"]["value"] = JObject.Parse(createEntry.ToString())["zeile_heil"];
                    //jsonObj["content"][0]["data"]["items"][0]["value"]["value"] = JObject.Parse(createEntry.ToString())["stadt_heil"];
                    //jsonObj["content"][0]["data"]["items"][0]["value"]["value"] = JObject.Parse(createEntry.ToString())["plz_heil"];
                    string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
                    File.WriteAllText("FormTemplates/PersonendatenComposition.json", output);

                    
                }

             }
             catch (Exception ex)
             {

             }
           
        }
    }
}
