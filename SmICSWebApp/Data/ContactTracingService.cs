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
    public class ContactTracingService
    {
        public static void ContactTracingDataStorage(JObject createEntry)
        {
             try
             {
                 if(createEntry != null)
                 {
                    string json = File.ReadAllText("FormTemplates/ContactTracingReport.json");
                    dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

                    //jsonObj["context"]["other_context"]["items"][0]["value"]["value"]= JObject.Parse(createEntry.ToString())["personID"];
                    //jsonObj["content"][0]["data"]["items"][0]["value"]["value"] = JObject.Parse(createEntry.ToString())["art_der_person"];
                    

                    jsonObj["bericht_zur_kontaktverfolgung/context/bericht_id"] = JObject.Parse(createEntry.ToString())["bericht_id"];
                    jsonObj["bericht_zur_kontaktverfolgung/context/eventsummary/event-kennung"] = JObject.Parse(createEntry.ToString())["event_kennung"];
                    jsonObj["bericht_zur_kontaktverfolgung/context/eventsummary/event-art"] = JObject.Parse(createEntry.ToString())["event_art"];
                    jsonObj["bericht_zur_kontaktverfolgung/context/eventsummary/beteiligte_personen:0/art_der_person"] = JObject.Parse(createEntry.ToString())["art_der_person"];
                    jsonObj["bericht_zur_kontaktverfolgung/context/eventsummary/event-kategorie"] = JObject.Parse(createEntry.ToString())["event_kategorie"];
                    jsonObj["bericht_zur_kontaktverfolgung/context/eventsummary/kommentar"] = JObject.Parse(createEntry.ToString())["kontakt_kommentar"];
                    jsonObj["bericht_zur_kontaktverfolgung/kontakt:0/beschreibung"] = JObject.Parse(createEntry.ToString())["beschreibung"];
                    jsonObj["bericht_zur_kontaktverfolgung/kontakt:0/beginn"] = JObject.Parse(createEntry.ToString())["beginn"];
                    jsonObj["bericht_zur_kontaktverfolgung/kontakt:0/ende"] = JObject.Parse(createEntry.ToString())["ende"];
                    jsonObj["bericht_zur_kontaktverfolgung/kontakt:0/ort"] = JObject.Parse(createEntry.ToString())["ort"];
                    jsonObj["bericht_zur_kontaktverfolgung/kontakt:0/gesamtdauer"] = JObject.Parse(createEntry.ToString())["gesamtdauer"];
                    jsonObj["bericht_zur_kontaktverfolgung/kontakt:0/abstand"] = JObject.Parse(createEntry.ToString())["abstand"];
                    jsonObj["bericht_zur_kontaktverfolgung/kontakt:0/schutzkleidung:0/schutzkleidung:0"] = JObject.Parse(createEntry.ToString())["schutzkleidung"];
                    jsonObj["bericht_zur_kontaktverfolgung/kontakt:0/schutzkleidung:0/person|code"] = JObject.Parse(createEntry.ToString())["person"];
                    jsonObj["bericht_zur_kontaktverfolgung/kontakt:0/kommentar"] = JObject.Parse(createEntry.ToString())["kommentar"];

                    string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
                    File.WriteAllText("FormTemplates/ContactTracingReport.json", output); 
                }

             }
             catch (Exception ex)
             {

             }
           
        }
    }
}
