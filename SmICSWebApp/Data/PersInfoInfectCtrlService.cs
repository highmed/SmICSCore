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
    public class PersInfoInfectCtrlService
    {
        public static void PersInfoInfectCtrlServiceDataStorage(JObject createEntry)
        {
             try
             {
                 if(createEntry != null)
                 {
                    string filepath = @"../SmICSWebApp/Data/FormTemplates/Personeninformation_zur_Infektionskontrolle.json";
                    string readResult = string.Empty;
                    string writeResult = string.Empty;
                    using (StreamReader r = new StreamReader(filepath))
                    {
                        var json = r.ReadToEnd();
                        var jobj = JObject.Parse(json);
                        readResult = jobj.ToString();

                        jobj["context"]["other_context"]["items"][0]["value"]["value"] = JObject.Parse(createEntry.ToString())["berichtID"];
                        jobj["content"][0]["data"]["events"][0]["data"]["items"][0]["value"]["value"] = JObject.Parse(createEntry.ToString())["symp_vorhanden"];

                        string symp_vorhanden = (string)JObject.Parse(createEntry.ToString())["symp_vorhanden"];
                        if (symp_vorhanden == "Vorhanden")
                        {
                            jobj["content"][0]["data"]["events"][0]["data"]["items"][0]["value"]["defining_code"]["code_string"] = "at0031";
                        }
                        else
                        {
                            if (symp_vorhanden == "Nicht vorhanden")
                            {
                                jobj["content"][0]["data"]["events"][0]["data"]["items"][0]["value"]["defining_code"]["code_string"] = "at0032";
                            }
                            else
                            {
                                jobj["content"][0]["data"]["events"][0]["data"]["items"][0]["value"]["defining_code"]["code_string"] = "at0033";
                            }
                        }

                        jobj["content"][0]["data"]["events"][0]["data"]["items"][1]["value"]["value"] = JObject.Parse(createEntry.ToString())["symp_auftreten"];
                        jobj["content"][0]["data"]["events"][0]["data"]["items"][2]["items"][0]["value"]["value"] = JObject.Parse(createEntry.ToString())["bez_symp"];
                        jobj["content"][0]["data"]["events"][0]["data"]["items"][3]["value"]["value"] = JObject.Parse(createEntry.ToString())["allg_kommentar"];
                        jobj["content"][1]["data"]["items"][0]["value"]["value"] = JObject.Parse(createEntry.ToString())["erreg_nachweis"];
                        jobj["content"][1]["data"]["items"][1]["value"]["value"] = JObject.Parse(createEntry.ToString())["erreg_name"];
                        jobj["content"][1]["data"]["items"][2]["value"]["value"] = JObject.Parse(createEntry.ToString())["zeitpunkt_kennzeichnung"];
                        jobj["content"][1]["data"]["items"][3]["value"]["value"] = JObject.Parse(createEntry.ToString())["erreg_Nachweis_klinik"];
                        jobj["content"][1]["protocol"]["items"][0]["value"]["value"] = JObject.Parse(createEntry.ToString())["zuletzt_aktuell"];
                        jobj["content"][2]["data"]["items"][0]["value"]["value"] = JObject.Parse(createEntry.ToString())["freigetsellt"];
                        jobj["content"][2]["data"]["items"][1]["value"]["value"] = JObject.Parse(createEntry.ToString())["grund"];
                        jobj["content"][2]["data"]["items"][2]["value"]["value"] = JObject.Parse(createEntry.ToString())["beschreibung"];
                        jobj["content"][2]["data"]["items"][3]["value"]["value"] = JObject.Parse(createEntry.ToString())["start_freistellung"];
                        jobj["content"][2]["data"]["items"][4]["value"]["value"] = JObject.Parse(createEntry.ToString())["wiederaufnahme"];
                        jobj["content"][2]["data"]["items"][5]["value"]["value"] = JObject.Parse(createEntry.ToString())["kommentar_freistellung"];
                        jobj["content"][3]["data"]["items"][0]["value"]["value"] = JObject.Parse(createEntry.ToString())["meldung"];
                        jobj["content"][3]["data"]["items"][1]["value"]["value"] = JObject.Parse(createEntry.ToString())["ereignis"];
                        jobj["content"][3]["data"]["items"][2]["value"]["value"] = JObject.Parse(createEntry.ToString())["beschreibung_ereignis"];
                        jobj["content"][3]["data"]["items"][3]["value"]["value"] = JObject.Parse(createEntry.ToString())["datum_meldung"];
                        jobj["content"][3]["data"]["items"][4]["value"]["value"] = JObject.Parse(createEntry.ToString())["grund_meldung"];
                        jobj["content"][3]["data"]["items"][5]["value"]["value"] = JObject.Parse(createEntry.ToString())["meldung_kommentar"];

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
