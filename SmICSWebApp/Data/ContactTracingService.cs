using Newtonsoft.Json.Linq;
using System;
using SmICSCoreLib.REST;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using SmICSCoreLib.Factories.Employees.ContactTracing;
using System.Collections.Generic;
using SmICSCoreLib.Factories;

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
            string composer = "SmICS";

            try
            {
                if (createEntry != null)
                {
                    var base_composition = "{\"_type\":\"COMPOSITION\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Bericht zur Kontaktverfolgung\"}";
                    var base_uid = ",\"uid\":{\"_type\":\"OBJECT_VERSION_ID\",\"value\":\"fe129559-0aa8-4c7c-b348-a037dc0f88ef::Infektionskontrolle::1\"}";
                    var base_archetype_details = ",\"archetype_details\":{\"archetype_id\":{\"value\":\"openEHR-EHR-COMPOSITION.report.v1\"},\"template_id\":{\"value\":\"Bericht zur Kontaktverfolgung\"},\"rm_version\":\"1.0.4\"}";
                    var base_archetype_node_id = ",\"archetype_node_id\":\"openEHR-EHR-COMPOSITION.report.v1\"";
                    var base_language = ",\"language\":{\"terminology_id\":{\"value\":\"ISO_639-1\"},\"code_string\":\"de\"}";
                    var base_territory = ",\"territory\":{\"terminology_id\":{\"value\":\"ISO_3166-1\"},\"code_string\":\"DE\"}";
                    var base_category = ",\"category\":{\"_type\":\"DV_CODED_TEXT\",\"value\":\"event\",\"defining_code\":{\"terminology_id\":{\"value\":\"openehr\"},\"code_string\":\"433\"}}";
                    var base_composer = ",\"composer\":{\"_type\":\"PARTY_IDENTIFIED\",\"name\":\"" + composer + "\"}";
                    var base_context = ",\"context\":{\"_type\":\"EVENT_CONTEXT\",\"start_time\":{\"_type\":\"DV_DATE_TIME\",\"value\":\"" + DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss") + "\"}";
                    var base_setting = ",\"setting\":{\"_type\":\"DV_CODED_TEXT\",\"value\":\"other care\",\"defining_code\":{\"terminology_id\":{\"value\":\"openehr\"},\"code_string\":\"238\"}}";
                    var base_other_context = ",\"other_context\":{\"_type\":\"ITEM_TREE\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Tree\"},\"archetype_node_id\":\"at0001\",\"items\":[";
                    var base_bericht_id = "{\"_type\":\"ELEMENT\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Bericht ID\"},\"archetype_node_id\":\"at0002\",\"value\":{\"_type\":\"DV_TEXT\",\"value\":\"" + JObject.Parse(createEntry.ToString())["BerichtID"] + "\"}}";
                    var base_event_cluster = ",{\"_type\":\"CLUSTER\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Eventsummary\"},\"archetype_details\":{\"archetype_id\":{\"value\":\"openEHR-EHR-CLUSTER.eventsummary.v0\"},\"rm_version\":\"1.0.4\"},\"archetype_node_id\":\"openEHR-EHR-CLUSTER.eventsummary.v0\",\"items\":[";
                    var base_event_kennung = "{\"_type\":\"ELEMENT\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Event-Kennung\"},\"archetype_node_id\":\"at0001\",\"value\":{\"_type\":\"DV_TEXT\",\"value\":\"" + JObject.Parse(createEntry.ToString())["EventKennung"] + "\"}}";
                    var base_event_art = ",{\"_type\":\"ELEMENT\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Event-Art\"},\"archetype_node_id\":\"at0002\",\"value\":{\"_type\":\"DV_TEXT\",\"value\":\"" + JObject.Parse(createEntry.ToString())["EventArt"] + "\"}}";
                    var base_bet_person_cluster = ",{\"_type\":\"CLUSTER\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Beteiligte Personen\"},\"archetype_details\":{\"archetype_id\":{\"value\":\"at0007\"},\"rm_version\":\"1.0.4\"},\"archetype_node_id\":\"at0007\",\"items\":[";
                    var base_art_d_person_1 = "{\"_type\":\"ELEMENT\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Art der Person\"},\"archetype_node_id\":\"at0011\",\"value\":{\"_type\":\"DV_TEXT\",\"value\":\"" + JObject.Parse(createEntry.ToString())["ArtDerPerson1"] + "\"}}";
                    var base_art_d_person_1_ID = ",{\"_type\":\"ELEMENT\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"ID der Person\"},\"archetype_node_id\":\"at0010\",\"value\":{\"_type\":\"DV_IDENTIFIER\",\"id\":\"" + JObject.Parse(createEntry.ToString())["PersonenID1"] + "\"}}]}";
                    var base_art_d_person_2 = "{\"_type\":\"ELEMENT\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Art der Person\"},\"archetype_node_id\":\"at0011\",\"value\":{\"_type\":\"DV_TEXT\",\"value\":\"" + JObject.Parse(createEntry.ToString())["ArtDerPerson2"] + "\"}}";
                    var base_art_d_person_2_ID = ",{\"_type\":\"ELEMENT\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"ID der Person\"},\"archetype_node_id\":\"at0010\",\"value\":{\"_type\":\"DV_IDENTIFIER\",\"id\":\"" + JObject.Parse(createEntry.ToString())["PersonenID2"] + "\"}}]}";
                    var base_event_kategorie = ",{\"_type\":\"ELEMENT\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Event-Kategorie\"},\"archetype_node_id\":\"at0004\",\"value\":{\"_type\":\"DV_TEXT\",\"value\":\"" + JObject.Parse(createEntry.ToString())["EventKategorie"] + "\"}}";
                    var base_event_kommentar = (string)JObject.Parse(createEntry.ToString())["EventKommentar"];
                    if (base_event_kommentar != null)
                    {
                        base_event_kommentar = ",{\"_type\":\"ELEMENT\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Kommentar\"},\"archetype_node_id\":\"at0006\",\"value\":{\"_type\":\"DV_TEXT\",\"value\":\"" + JObject.Parse(createEntry.ToString())["EventKommentar"] + "\"}}";
                    }
                    var base_close_other_context = "]}]}}";
                    var base_content = ",\"content\":[{\"_type\":\"ACTION\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Kontakt\"},\"archetype_details\":{\"archetype_id\":{\"value\":\"openEHR-EHR-ACTION.contact.v0\"},\"rm_version\":\"1.0.4\"},\"archetype_node_id\":\"openEHR-EHR-ACTION.contact.v0\",\"language\":{\"terminology_id\":{\"value\":\"ISO_639-1\"},\"code_string\":\"de\"},\"encoding\":{\"terminology_id\":{\"value\":\"IANA_character-sets\"},\"code_string\":\"UTF-8\"},\"subject\":{\"_type\":\"PARTY_SELF\"},\"time\":{\"_type\":\"DV_DATE_TIME\",\"value\":\"" + DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss") + "\"},\"ism_transition\":{\"current_state\":{\"value\":\"planned\",\"defining_code\":{\"terminology_id\":{\"value\":\"openehr\"},\"code_string\":\"526\"}}},\"description\":{\"_type\":\"ITEM_TREE\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Tree\"},\"archetype_node_id\":\"at0001\",\"items\":[";
                    var base_beschreibung = (string)JObject.Parse(createEntry.ToString())["Beschreibung"];
                    if (base_beschreibung != null)
                    {
                        base_beschreibung = "{\"_type\":\"ELEMENT\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Beschreibung\"},\"archetype_node_id\":\"at0009\",\"value\":{\"_type\":\"DV_TEXT\",\"value\":\"" + JObject.Parse(createEntry.ToString())["Beschreibung"] + "\"}},";
                    }
                    var base_beginn = "{\"_type\":\"ELEMENT\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Beginn\"},\"archetype_node_id\":\"at0006\",\"value\":{\"_type\":\"DV_DATE_TIME\",\"value\":\"" + JObject.Parse(createEntry.ToString())["Beginn"] + "\"}}";
                    var base_ende = ",{\"_type\":\"ELEMENT\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Ende\"},\"archetype_node_id\":\"at0016\",\"value\":{\"_type\":\"DV_DATE_TIME\",\"value\":\"" + JObject.Parse(createEntry.ToString())["Ende"] + "\"}}";
                    var base_ort = ",{\"_type\":\"ELEMENT\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Ort\"},\"archetype_node_id\":\"at0017\",\"value\":{\"_type\":\"DV_TEXT\",\"value\":\"" + JObject.Parse(createEntry.ToString())["Ort"] + "\"}}";
                    var base_gesamtdauer = ",{\"_type\":\"ELEMENT\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Gesamtdauer\"},\"archetype_node_id\":\"at0003\",\"value\":{\"_type\":\"DV_TEXT\",\"value\":\"" + JObject.Parse(createEntry.ToString())["Gesamtdauer"] + "\"}}";
                    var base_abstand = ",{\"_type\":\"ELEMENT\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Abstand\"},\"archetype_node_id\":\"at0008\",\"value\":{\"_type\":\"DV_TEXT\",\"value\":\"" + JObject.Parse(createEntry.ToString())["Abstand"] + "\"}}";
                    var base_kleidung_cluster = ",{\"_type\":\"CLUSTER\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Schutzkleidung\"},\"archetype_details\":{\"archetype_id\":{\"value\":\"openEHR-EHR-CLUSTER.protective_clothing_.v0\"},\"rm_version\":\"1.0.4\"},\"archetype_node_id\":\"openEHR-EHR-CLUSTER.protective_clothing_.v0\",\"items\":[";
                    var base_schutzkleidung1 = "{\"_type\":\"ELEMENT\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Schutzkleidung\"},\"archetype_node_id\":\"at0001\",\"value\":{\"_type\":\"DV_TEXT\",\"value\":\"" + JObject.Parse(createEntry.ToString())["Schutzkleidung1"] + "\"}}";
                    var base_person1 = (string)JObject.Parse(createEntry.ToString())["Person1"];
                    if (base_person1 != "Indexperson")
                    {
                        base_person1 = ",{\"_type\":\"ELEMENT\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Person\"},\"archetype_node_id\":\"at0002\",\"value\":{\"_type\":\"DV_CODED_TEXT\",\"value\":\"" + JObject.Parse(createEntry.ToString())["Person1"] + "\",\"defining_code\":{\"terminology_id\":{\"value\":\"local\"},\"code_string\":\"at0004\"}}}]}";
                    }
                    else
                    {
                        base_person1 = ",{\"_type\":\"ELEMENT\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Person\"},\"archetype_node_id\":\"at0002\",\"value\":{\"_type\":\"DV_CODED_TEXT\",\"value\":\"" + JObject.Parse(createEntry.ToString())["Person1"] + "\",\"defining_code\":{\"terminology_id\":{\"value\":\"local\"},\"code_string\":\"at0003\"}}}]}";
                    }
                    var base_schutzkleidung2 = "{\"_type\":\"ELEMENT\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Schutzkleidung\"},\"archetype_node_id\":\"at0001\",\"value\":{\"_type\":\"DV_TEXT\",\"value\":\"" + JObject.Parse(createEntry.ToString())["Schutzkleidung2"] + "\"}}";
                    var base_person2 = (string)JObject.Parse(createEntry.ToString())["Person2"];
                    if (base_person2 != "Indexperson")
                    {
                        base_person2 = ",{\"_type\":\"ELEMENT\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Person\"},\"archetype_node_id\":\"at0002\",\"value\":{\"_type\":\"DV_CODED_TEXT\",\"value\":\"" + JObject.Parse(createEntry.ToString())["Person2"] + "\",\"defining_code\":{\"terminology_id\":{\"value\":\"local\"},\"code_string\":\"at0004\"}}}]}";
                    }
                    else
                    {
                        base_person2 = ",{\"_type\":\"ELEMENT\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Person\"},\"archetype_node_id\":\"at0002\",\"value\":{\"_type\":\"DV_CODED_TEXT\",\"value\":\"" + JObject.Parse(createEntry.ToString())["Person2"] + "\",\"defining_code\":{\"terminology_id\":{\"value\":\"local\"},\"code_string\":\"at0003\"}}}]}";
                    }
                    var base_kommentar = (string)JObject.Parse(createEntry.ToString())["Kommentar"];
                    if (base_kommentar != null)
                    {
                        base_kommentar = ",{\"_type\":\"ELEMENT\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Kommentar\"},\"archetype_node_id\":\"at0007\",\"value\":{\"_type\":\"DV_TEXT\",\"value\":\"" + JObject.Parse(createEntry.ToString())["Kommentar"] + "\"}}";
                    }
                    var base_composition_ende = "]}}]}";

                    var json_all = base_composition + base_uid + base_archetype_details + base_archetype_node_id + base_language + base_territory +
                        base_category + base_composer + base_context + base_setting + base_other_context + base_bericht_id + base_event_cluster + base_event_kennung +
                        base_event_art + base_bet_person_cluster + base_art_d_person_1 + base_art_d_person_1_ID + base_bet_person_cluster + base_art_d_person_2 +
                        base_art_d_person_2_ID + base_event_kategorie + base_event_kommentar + base_close_other_context + base_content + base_beschreibung +
                        base_beginn + base_ende + base_ort + base_gesamtdauer + base_abstand + base_kleidung_cluster + base_schutzkleidung1 + base_person1 +
                        base_kleidung_cluster + base_schutzkleidung2 + base_person2 + base_kommentar + base_composition_ende;

                    var obj = JsonConvert.DeserializeObject(json_all);
                    var finishedJson = JsonConvert.SerializeObject(obj, Formatting.Indented);

                    //File.WriteAllText(@".. / .. / .. / .. / TestData / contacttracing.txt", finishedJson);

                    SaveComposition(ehr_id, finishedJson);
                }

            }
            catch (Exception)
            {
                throw new Exception($"Failed to POST data");
            }

        }

        private void SaveComposition(string subjectID, string writeResult)
        {
            string ehr_id = ExistsSubject(_restData, subjectID);

            if (ehr_id == null)
            {
                HttpResponseMessage result = _restData.CreateEhrIDWithStatus("SmICSTest", subjectID).Result;
                ehr_id = (string)JsonConvert.DeserializeObject(result.Headers.ETag.Tag);

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
