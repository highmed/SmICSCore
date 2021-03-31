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
                    string filepath = @"../SmICSWebApp/Data/FormTemplates/Personendaten.json";
                    string readResult = string.Empty;
                    string writeResult = string.Empty;
                    using (StreamReader r = new StreamReader(filepath))
                    {
                        var json = r.ReadToEnd();
                        var jobj = JObject.Parse(json);
                        readResult = jobj.ToString();

                        jobj["context"]["other_context"]["items"][0]["value"]["value"]= JObject.Parse(createEntry.ToString())["personID"];
                        jobj["content"][0]["data"]["items"][0]["value"]["value"] = JObject.Parse(createEntry.ToString())["art_der_person"];
                        string person = (string)JObject.Parse(createEntry.ToString())["art_der_person"];
                        if (person == "Mitarbeiter")
                        {
                            jobj["content"][0]["data"]["items"][0]["value"]["defining_code"]["code_string"] = "at0009";
                        } 
                        else
                        {
                            if (person == "Patient")
                            {
                                jobj["content"][0]["data"]["items"][0]["value"]["defining_code"]["code_string"] = "at0010";
                            }
                            else
                            {
                                jobj["content"][0]["data"]["items"][0]["value"]["defining_code"]["code_string"] = "at0011";
                            }
                        }
                        jobj["content"][0]["data"]["items"][1]["items"][0]["items"][0]["value"]["value"] = JObject.Parse(createEntry.ToString())["titel"];
                        jobj["content"][0]["data"]["items"][1]["items"][0]["items"][1]["value"]["value"] = JObject.Parse(createEntry.ToString())["vorname"];
                        jobj["content"][0]["data"]["items"][1]["items"][0]["items"][2]["value"]["value"] = JObject.Parse(createEntry.ToString())["weiterer_vorname"];
                        jobj["content"][0]["data"]["items"][1]["items"][0]["items"][3]["value"]["value"] = JObject.Parse(createEntry.ToString())["nachname"];
                        jobj["content"][0]["data"]["items"][1]["items"][0]["items"][4]["value"]["value"] = JObject.Parse(createEntry.ToString())["suffix"];
                        jobj["content"][0]["data"]["items"][2]["items"][0]["value"]["value"] = JObject.Parse(createEntry.ToString())["geburtsdatum"];
                        jobj["content"][0]["data"]["items"][3]["items"][0]["value"]["value"] = JObject.Parse(createEntry.ToString())["zeile"];
                        jobj["content"][0]["data"]["items"][3]["items"][1]["value"]["value"] = JObject.Parse(createEntry.ToString())["stadt"];
                        jobj["content"][0]["data"]["items"][3]["items"][2]["value"]["value"] = JObject.Parse(createEntry.ToString())["plz"];
                        jobj["content"][0]["data"]["items"][4]["items"][0]["items"][0]["value"]["value"] = JObject.Parse(createEntry.ToString())["kontakttyp"];
                        jobj["content"][0]["data"]["items"][4]["items"][0]["items"][1]["items"][0]["value"]["value"] = JObject.Parse(createEntry.ToString())["nummer"];
                        jobj["content"][0]["data"]["items"][5]["items"][0]["items"][0]["value"]["value"] = JObject.Parse(createEntry.ToString())["fach_bez"];
                        jobj["content"][0]["data"]["items"][5]["items"][0]["items"][1]["items"][0]["value"]["value"] = JObject.Parse(createEntry.ToString())["zeile_heil"];
                        jobj["content"][0]["data"]["items"][5]["items"][0]["items"][1]["items"][1]["value"]["value"] = JObject.Parse(createEntry.ToString())["stadt_heil"];
                        jobj["content"][0]["data"]["items"][5]["items"][0]["items"][1]["items"][2]["value"]["value"] = JObject.Parse(createEntry.ToString())["plz_heil"];

                        writeResult = jobj.ToString();
                        //Console.WriteLine(writeResult);
                    }
                    //System.Diagnostics.Debug.WriteLine(readResult);
                    File.WriteAllText(filepath, writeResult);
                    
                }

             }
             catch (Exception)
             {

             }
           
        }
    }
}
