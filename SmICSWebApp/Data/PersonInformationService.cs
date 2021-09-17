using Newtonsoft.Json.Linq;
using System;
using SmICSCoreLib.REST;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using SmICSCoreLib.Factories.Employees.PersonData;
using System.Collections.Generic;
using SmICSCoreLib.Factories;

namespace SmICSWebApp.Data
{
    public class PersonInformationService
    {
        private IRestDataAccess _restData;
        private ILogger<PersonDataFactory> _logger;

        public PersonInformationService(IRestDataAccess restData, ILogger<PersonDataFactory> logger)
        {
            _restData = restData;
            _logger = logger;
        }
        public void PersonInformationDataStorage(JObject createEntry)
        {
            string composer = "SmICS";
            string ehr_id = (string)JObject.Parse(createEntry.ToString())["PersonID"];
            try
            {
                if (createEntry != null)
                {
                    var base_composition = "{\"_type\":\"COMPOSITION\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Personendaten\"}";
                    var base_uid = ",\"uid\":{\"_type\":\"OBJECT_VERSION_ID\",\"value\":\"a066baca-fe84-4a54-b5d7-04b5cfbbc87e::Infektionskontrolle::1\"}";
                    var base_archetype_details = ",\"archetype_details\":{\"archetype_id\":{\"value\":\"openEHR-EHR-COMPOSITION.personendaten.v0\"},\"template_id\":{\"value\":\"Personendaten\"},\"rm_version\":\"1.0.4\"}";
                    var base_archetype_node_id = ",\"archetype_node_id\":\"openEHR-EHR-COMPOSITION.personendaten.v0\"";
                    var base_language = ",\"language\":{\"terminology_id\":{\"value\":\"ISO_639-1\"},\"code_string\":\"de\"}";
                    var base_territory = ",\"territory\":{\"terminology_id\":{\"value\":\"ISO_3166-1\"},\"code_string\":\"DE\"}";
                    var base_category = ",\"category\":{\"_type\":\"DV_CODED_TEXT\",\"value\":\"event\",\"defining_code\":{\"terminology_id\":{\"value\":\"openehr\"},\"code_string\":\"433\"}}";
                    var base_composer = ",\"composer\":{\"_type\":\"PARTY_IDENTIFIED\",\"name\":\"" + composer + "\"}";
                    var base_context = ",\"context\":{\"_type\":\"EVENT_CONTEXT\",\"start_time\":{\"_type\":\"DV_DATE_TIME\",\"value\":\"" + DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss") + "\"}";
                    var base_setting = ",\"setting\":{\"_type\":\"DV_CODED_TEXT\",\"value\":\"other care\",\"defining_code\":{\"terminology_id\":{\"value\":\"openehr\"},\"code_string\":\"238\"}}";
                    var base_other_context = ",\"other_context\":{\"_type\":\"ITEM_TREE\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Tree\"},\"archetype_node_id\":\"at0003\",\"items\":[";
                    var base_person_id = "{\"_type\":\"ELEMENT\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Personen ID\"},\"archetype_node_id\":\"at0004\",\"value\":{\"_type\":\"DV_TEXT\",\"value\":\"" + JObject.Parse(createEntry.ToString())["PersonID"] + "\"}}]}}";
                    var base_content = ",\"content\":[{\"_type\":\"ADMIN_ENTRY\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Personendaten\"},\"archetype_details\":{\"archetype_id\":{\"value\":\"openEHR-EHR-ADMIN_ENTRY.person_data.v0\"},\"rm_version\":\"1.0.4\"},\"archetype_node_id\":\"openEHR-EHR-ADMIN_ENTRY.person_data.v0\",\"language\":{\"terminology_id\":{\"value\":\"ISO_639-1\"},\"code_string\":\"de\"},\"encoding\":{\"terminology_id\":{\"value\":\"IANA_character-sets\"},\"code_string\":\"UTF-8\"},\"subject\":{\"_type\":\"PARTY_SELF\"},\"data\":{\"_type\":\"ITEM_TREE\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Baum\"},\"archetype_node_id\":\"at0001\",\"items\":[";
                    var base_art_d_person = (string)JObject.Parse(createEntry.ToString())["ArtDerPerson"];
                    if (base_art_d_person == "Mitarbeiter")
                    {
                        base_art_d_person = "{\"_type\":\"ELEMENT\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Art der Person\"},\"archetype_node_id\":\"at0008\",\"value\":{\"_type\":\"DV_CODED_TEXT\",\"value\":\"" + JObject.Parse(createEntry.ToString())["ArtDerPerson"] + "\",\"defining_code\":{\"terminology_id\":{\"value\":\"local\"},\"code_string\":\"at0009\"}}}";
                    }
                    else if (base_art_d_person == "Patient")
                    {
                        base_art_d_person = "{\"_type\":\"ELEMENT\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Art der Person\"},\"archetype_node_id\":\"at0008\",\"value\":{\"_type\":\"DV_CODED_TEXT\",\"value\":\"" + JObject.Parse(createEntry.ToString())["ArtDerPerson"] + "\",\"defining_code\":{\"terminology_id\":{\"value\":\"local\"},\"code_string\":\"at0010\"}}}";
                    }
                    else
                    {
                        base_art_d_person = "{\"_type\":\"ELEMENT\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Art der Person\"},\"archetype_node_id\":\"at0008\",\"value\":{\"_type\":\"DV_CODED_TEXT\",\"value\":\"" + JObject.Parse(createEntry.ToString())["ArtDerPerson"] + "\",\"defining_code\":{\"terminology_id\":{\"value\":\"local\"},\"code_string\":\"at0011\"}}}";
                    }
                    var base_personenname = ",{\"_type\":\"CLUSTER\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Personenname\"},\"archetype_details\":{\"archetype_id\":{\"value\":\"openEHR-EHR-CLUSTER.person_name.v0\"},\"rm_version\":\"1.0.4\"},\"archetype_node_id\":\"openEHR-EHR-CLUSTER.person_name.v0\",\"items\":[";
                    var base_name_struk = "{\"_type\":\"CLUSTER\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Name strukturiert\"},\"archetype_details\":{\"archetype_id\":{\"value\":\"at0002\"},\"rm_version\":\"1.0.4\"},\"archetype_node_id\":\"at0002\",\"items\":[";
                    var base_titel = (string)JObject.Parse(createEntry.ToString())["Titel"];
                    if (base_titel != null)
                    {
                        base_titel = "{\"_type\":\"ELEMENT\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Titel\"},\"archetype_node_id\":\"at0017\",\"value\":{\"_type\":\"DV_TEXT\",\"value\":\"" + JObject.Parse(createEntry.ToString())["Titel"] + "\"}},";
                    }
                    var base_vorname = "{\"_type\":\"ELEMENT\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Vorname\"},\"archetype_node_id\":\"at0003\",\"value\":{\"_type\":\"DV_TEXT\",\"value\":\"" + JObject.Parse(createEntry.ToString())["Vorname"] + "\"}}";
                    var base_weiterer_vorname = (string)JObject.Parse(createEntry.ToString())["WeitererVorname"];
                    if (base_weiterer_vorname != null)
                    {
                        base_weiterer_vorname = ",{\"_type\":\"ELEMENT\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Weiterer Vorname\"},\"archetype_node_id\":\"at0004\",\"value\":{\"_type\":\"DV_TEXT\",\"value\":\"" + JObject.Parse(createEntry.ToString())["WeitererVorname"] + "\"}}";
                    }
                    var base_nachname = ",{\"_type\":\"ELEMENT\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Nachname\"},\"archetype_node_id\":\"at0005\",\"value\":{\"_type\":\"DV_TEXT\",\"value\":\"" + JObject.Parse(createEntry.ToString())["Nachname"] + "\"}}";
                    var base_suffix = (string)JObject.Parse(createEntry.ToString())["Suffix"];
                    if (base_suffix != null)
                    {
                        base_suffix = ",{\"_type\":\"ELEMENT\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Suffix\"},\"archetype_node_id\":\"at0018\",\"value\":{\"_type\":\"DV_TEXT\",\"value\":\"" + JObject.Parse(createEntry.ToString())["Suffix"] + "\"}}";
                    }
                    var base_end_name_struk = "]}]}";
                    var base_daten_z_geburt = ",{\"_type\":\"CLUSTER\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Daten zur Geburt\"},\"archetype_details\":{\"archetype_id\":{\"value\":\"openEHR-DEMOGRAPHIC-CLUSTER.person_birth_data_iso.v0\"},\"rm_version\":\"1.0.4\"},\"archetype_node_id\":\"openEHR-DEMOGRAPHIC-CLUSTER.person_birth_data_iso.v0\",\"items\":[";
                    var base_geburtsdatum = "{\"_type\":\"ELEMENT\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Geburtsdatum\"},\"archetype_node_id\":\"at0001\",\"value\":{\"_type\":\"DV_DATE\",\"value\":\"" + JObject.Parse(createEntry.ToString())["Geburtsdatum"] + "\"}}]}";
                    var base_cluster_adresse = ",{\"_type\":\"CLUSTER\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Adresse\"},\"archetype_details\":{\"archetype_id\":{\"value\":\"openEHR-EHR-CLUSTER.address_cc.v0\"},\"rm_version\":\"1.0.4\"},\"archetype_node_id\":\"openEHR-EHR-CLUSTER.address_cc.v0\",\"items\":[";
                    var base_zeile_1 = "{\"_type\":\"ELEMENT\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Zeile\"},\"archetype_node_id\":\"at0011\",\"value\":{\"_type\":\"DV_TEXT\",\"value\":\"" + JObject.Parse(createEntry.ToString())["Zeile"] + "\"}}";
                    var base_stadt_1 = ",{\"_type\":\"ELEMENT\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Stadt\"},\"archetype_node_id\":\"at0012\",\"value\":{\"_type\":\"DV_TEXT\",\"value\":\"" + JObject.Parse(createEntry.ToString())["Stadt"] + "\"}}";
                    var base_plz_1 = ",{\"_type\":\"ELEMENT\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Postleitzahl\"},\"archetype_node_id\":\"at0014\",\"value\":{\"_type\":\"DV_TEXT\",\"value\":\"" + JObject.Parse(createEntry.ToString())["Plz"] + "\"}}]}";
                    var base_cluster_kommu = ",{\"_type\":\"CLUSTER\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Einzelheiten der Kommunikation\"},\"archetype_details\":{\"archetype_id\":{\"value\":\"openEHR-EHR-CLUSTER.telecom_details.v0\"},\"rm_version\":\"1.0.4\"},\"archetype_node_id\":\"openEHR-EHR-CLUSTER.telecom_details.v0\",\"items\":[";
                    var base_cluster_kontakt = "{\"_type\":\"CLUSTER\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Kontaktdaten\"},\"archetype_details\":{\"archetype_id\":{\"value\":\"at0001\"},\"rm_version\":\"1.0.4\"},\"archetype_node_id\":\"at0001\",\"items\":[";
                    var base_kontakttyp = (string)JObject.Parse(createEntry.ToString())["Kontakttyp"];
                    if (base_kontakttyp == "Telefon")
                    {
                        base_kontakttyp = "{\"_type\":\"ELEMENT\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Kontakttyp\"},\"archetype_node_id\":\"at0004\",\"value\":{\"_type\":\"DV_CODED_TEXT\",\"value\":\"" + JObject.Parse(createEntry.ToString())["Kontakttyp"] + "\",\"defining_code\":{\"terminology_id\":{\"value\":\"local\"},\"code_string\":\"at0013\"}}}";
                    }
                    var base_cluster_struk_adresse = ",{\"_type\":\"CLUSTER\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Strukturierte Kontaktadresse\"},\"archetype_details\":{\"archetype_id\":{\"value\":\"at0003\"},\"rm_version\":\"1.0.4\"},\"archetype_node_id\":\"at0003\",\"items\":[";
                    var base_nummer = "{\"_type\":\"ELEMENT\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Nummer\"},\"archetype_node_id\":\"at0007\",\"value\":{\"_type\":\"DV_TEXT\",\"value\":\"" + JObject.Parse(createEntry.ToString())["Nummer"] + "\"}}]}]}]}";
                    var base_cluster_heil = ",{\"_type\":\"CLUSTER\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Persönliche Daten Heilberufler\"},\"archetype_details\":{\"archetype_id\":{\"value\":\"openEHR-EHR-CLUSTER.individual_professional.v0\"},\"rm_version\":\"1.0.4\"},\"archetype_node_id\":\"openEHR-EHR-CLUSTER.individual_professional.v0\",\"items\":[";
                    var base_cluster_pers_daten = "{\"_type\":\"CLUSTER\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Persönliche Daten\"},\"archetype_details\":{\"archetype_id\":{\"value\":\"at0003\"},\"rm_version\":\"1.0.4\"},\"archetype_node_id\":\"at0003\",\"items\":[";
                    var base_fachbez = "{\"_type\":\"ELEMENT\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Fachbezeichnung\"},\"archetype_node_id\":\"at0006\",\"value\":{\"_type\":\"DV_TEXT\",\"value\":\"" + JObject.Parse(createEntry.ToString())["Fachbezeichnung"] + "\"}}";
                    var base_cluster_adresse_2 = ",{\"_type\":\"CLUSTER\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Adresse\"},\"archetype_details\":{\"archetype_id\":{\"value\":\"openEHR-EHR-CLUSTER.address_cc.v0\"},\"rm_version\":\"1.0.4\"},\"archetype_node_id\":\"openEHR-EHR-CLUSTER.address_cc.v0\",\"items\":[";
                    var base_zeile_2 = "{\"_type\":\"ELEMENT\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Zeile\"},\"archetype_node_id\":\"at0011\",\"value\":{\"_type\":\"DV_TEXT\",\"value\":\"" + JObject.Parse(createEntry.ToString())["HeilZeile"] + "\"}}";
                    var base_stadt_2 = ",{\"_type\":\"ELEMENT\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Stadt\"},\"archetype_node_id\":\"at0012\",\"value\":{\"_type\":\"DV_TEXT\",\"value\":\"" + JObject.Parse(createEntry.ToString())["HeilStadt"] + "\"}}";
                    var base_plz_2 = ",{\"_type\":\"ELEMENT\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Postleitzahl\"},\"archetype_node_id\":\"at0014\",\"value\":{\"_type\":\"DV_TEXT\",\"value\":\"" + JObject.Parse(createEntry.ToString())["HeilPLZ"] + "\"}}]";
                    var base_ende = "}]}]}]}}]}";

                    var json_all = base_composition + base_uid + base_archetype_details + base_archetype_node_id + base_language + base_territory +
                        base_category + base_composer + base_context + base_setting + base_other_context + base_person_id + base_content +
                        base_art_d_person + base_personenname + base_name_struk + base_titel + base_vorname + base_weiterer_vorname +
                        base_nachname + base_suffix + base_end_name_struk + base_daten_z_geburt + base_geburtsdatum + base_cluster_adresse +
                        base_zeile_1 + base_stadt_1 + base_plz_1 + base_cluster_kommu + base_cluster_kontakt + base_kontakttyp +
                        base_cluster_struk_adresse + base_nummer + base_cluster_heil + base_cluster_pers_daten + base_fachbez +
                        base_cluster_adresse_2 + base_zeile_2 + base_stadt_2 + base_plz_2 + base_ende;


                    var obj = JsonConvert.DeserializeObject(json_all);
                    var finishedJson = JsonConvert.SerializeObject(obj, Formatting.Indented);

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
