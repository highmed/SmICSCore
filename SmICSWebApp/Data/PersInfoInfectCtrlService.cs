using Newtonsoft.Json.Linq;
using System;
using SmICSCoreLib.REST;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using SmICSCoreLib.Factories.Employees.PersInfoInfecCtrl;
using System.Collections.Generic;
using SmICSCoreLib.Factories;

namespace SmICSWebApp.Data
{
    public class PersInfoInfectCtrlService
    {
        private IRestDataAccess _restData;
        private ILogger<PersInfoInfecCtrlFactory> _logger;

        public PersInfoInfectCtrlService(IRestDataAccess restData, ILogger<PersInfoInfecCtrlFactory> logger)
        {
            _restData = restData;
            _logger = logger;
        }
        public void PersInfoInfectCtrlServiceDataStorage(JObject createEntry, string ehr_id)
        {
            string composer = "SmICS";
            try
            {
                if (createEntry != null)
                {
                    var base_composition = "{\"_type\":\"COMPOSITION\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Personeninformation zur Infektionskontrolle\"}";
                    var base_uid = ",\"uid\":{\"_type\":\"OBJECT_VERSION_ID\",\"value\":\"c37d27fc-38c8-4ca3-9481-fba3f4050319::Infektionskontrolle::1\"}";
                    var base_archetype_details = ",\"archetype_details\":{\"archetype_id\":{\"value\":\"openEHR-EHR-COMPOSITION.report.v1\"},\"template_id\":{\"value\":\"Personeninformation zur Infektionskontrolle\"},\"rm_version\":\"1.0.4\"}";
                    var base_archetype_node_id = ",\"archetype_node_id\":\"openEHR-EHR-COMPOSITION.report.v1\"";
                    var base_language = ",\"language\":{\"terminology_id\":{\"value\":\"ISO_639-1\"},\"code_string\":\"de\"}";
                    var base_territory = ",\"territory\":{\"terminology_id\":{\"value\":\"ISO_3166-1\"},\"code_string\":\"DE\"}";
                    var base_category = ",\"category\":{\"_type\":\"DV_CODED_TEXT\",\"value\":\"event\",\"defining_code\":{\"terminology_id\":{\"value\":\"openehr\"},\"code_string\":\"433\"}}";
                    var base_composer = ",\"composer\":{\"_type\":\"PARTY_IDENTIFIED\",\"name\":\"" + composer + "\"}";
                    var base_context = ",\"context\":{\"_type\":\"EVENT_CONTEXT\",\"start_time\":{\"_type\":\"DV_DATE_TIME\",\"value\":\"" + DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss") + "\"}";
                    var base_setting = ",\"setting\":{\"_type\":\"DV_CODED_TEXT\",\"value\":\"other care\",\"defining_code\":{\"terminology_id\":{\"value\":\"openehr\"},\"code_string\":\"238\"}}";
                    var base_other_context = ",\"other_context\":{\"_type\":\"ITEM_TREE\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Tree\"},\"archetype_node_id\":\"at0001\",\"items\":[";
                    var base_bericht_id = "{\"_type\":\"ELEMENT\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Bericht ID\"},\"archetype_node_id\":\"at0002\",\"value\":{\"_type\":\"DV_TEXT\",\"value\":\"" + JObject.Parse(createEntry.ToString())["BerichtID"] + "\"}}]}}";
                    var base_content_beginn = ",\"content\":[";


                    var base_content = "";
                    var base_data_1 = "";
                    var base_data_2 = "";
                    var base_end_cluster = "";
                    var base_cluster_spez_symp = "";

                    var base_anzeichen = (string)JObject.Parse(createEntry.ToString())["SymptomVorhanden"];
                    var base_symp_auftreten = (string)JObject.Parse(createEntry.ToString())["AufgetretenSeit"];
                    var base_spez_symp = (string)JObject.Parse(createEntry.ToString())["Symptom"];
                    var base_all_komm = (string)JObject.Parse(createEntry.ToString())["SymptomKommentar"];


                    if (base_anzeichen != null | base_symp_auftreten != null | base_spez_symp != null | base_all_komm != null)
                    {
                        base_content = "{\"_type\":\"OBSERVATION\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Screening-Fragebogen zur Symptomen/Anzeichen\"},\"archetype_details\":{\"archetype_id\":{\"value\":\"openEHR-EHR-OBSERVATION.symptom_sign_screening.v0\"},\"rm_version\":\"1.0.4\"},\"archetype_node_id\":\"openEHR-EHR-OBSERVATION.symptom_sign_screening.v0\",\"language\":{\"terminology_id\":{\"value\":\"ISO_639-1\"},\"code_string\":\"de\"},\"encoding\":{\"terminology_id\":{\"value\":\"IANA_character-sets\"},\"code_string\":\"UTF-8\"},\"subject\":{\"_type\":\"PARTY_SELF\"}";
                        base_data_1 = ",\"data\":{\"_type\":\"HISTORY\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Event Series\"},\"archetype_node_id\":\"at0001\",\"origin\":{\"_type\":\"DV_DATE_TIME\",\"value\":\"" + DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss") + "\"},\"events\":[{\"_type\":\"POINT_EVENT\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Beliebiges Ereignis\"},\"archetype_node_id\":\"at0002\",\"time\":{\"_type\":\"DV_DATE_TIME\",\"value\":\"" + DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss") + "\"}";
                        base_data_2 = ",\"data\":{\"_type\":\"ITEM_TREE\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Tree\"},\"archetype_node_id\":\"at0003\",\"items\":[";

                        if (base_anzeichen == "Vorhanden")
                        {
                            base_anzeichen = "{\"_type\":\"ELEMENT\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Sind Symptome oder Anzeichen vorhanden?\"},\"archetype_node_id\":\"at0028\",\"value\":{\"_type\":\"DV_CODED_TEXT\",\"value\":\"" + JObject.Parse(createEntry.ToString())["SymptomVorhanden"] + "\",\"defining_code\":{\"terminology_id\":{\"value\":\"local\"},\"code_string\":\"at0031\"}}},";
                        }
                        else if (base_anzeichen == "Nicht vorhanden")
                        {
                            base_anzeichen = "{\"_type\":\"ELEMENT\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Sind Symptome oder Anzeichen vorhanden?\"},\"archetype_node_id\":\"at0028\",\"value\":{\"_type\":\"DV_CODED_TEXT\",\"value\":\"" + JObject.Parse(createEntry.ToString())["SymptomVorhanden"] + "\",\"defining_code\":{\"terminology_id\":{\"value\":\"local\"},\"code_string\":\"at0032\"}}},";
                        }
                        else if (base_anzeichen == "Unbekannt")
                        {
                            base_anzeichen = "{\"_type\":\"ELEMENT\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Sind Symptome oder Anzeichen vorhanden?\"},\"archetype_node_id\":\"at0028\",\"value\":{\"_type\":\"DV_CODED_TEXT\",\"value\":\"" + JObject.Parse(createEntry.ToString())["SymptomVorhanden"] + "\",\"defining_code\":{\"terminology_id\":{\"value\":\"local\"},\"code_string\":\"at0033\"}}},";
                        }

                        if (base_symp_auftreten != null)
                        {
                            base_symp_auftreten = "{\"_type\":\"ELEMENT\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Auftreten von Symptomen oder Anzeichen\"},\"archetype_node_id\":\"at0029\",\"value\":{\"_type\":\"DV_DATE_TIME\",\"value\":\"" + JObject.Parse(createEntry.ToString())["AufgetretenSeit"] + "\"}},";
                        }

                        base_cluster_spez_symp = "{\"_type\":\"CLUSTER\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Spezifisches Symptom/Anzeichen\"},\"archetype_details\":{\"archetype_id\":{\"value\":\"at0022\"},\"rm_version\":\"1.0.4\"},\"archetype_node_id\":\"at0022\",\"items\":[";
                        base_spez_symp = "{\"_type\":\"ELEMENT\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Bezeichnung des Symptoms oder Anzeichens.\"},\"archetype_node_id\":\"at0004\",\"value\":{\"_type\":\"DV_TEXT\",\"value\":\"" + JObject.Parse(createEntry.ToString())["Symptom"] + "\"}}]}";

                        if (base_all_komm != null)
                        {
                            base_all_komm = ",{\"_type\":\"ELEMENT\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Allgemeiner Kommentar\"},\"archetype_node_id\":\"at0025\",\"value\":{\"_type\":\"DV_TEXT\",\"value\":\"" + JObject.Parse(createEntry.ToString())["SymptomKommentar"] + "\"}}";
                        }

                        base_end_cluster = "]}}]}}";
                    }
                    var base_evulation_1 = "";
                    var base_erreg_nachweis = (string)JObject.Parse(createEntry.ToString())["Nachweis"];
                    var base_erregname = (string)JObject.Parse(createEntry.ToString())["Erregername"];
                    var base_zeitpunkt_kenn = (string)JObject.Parse(createEntry.ToString())["Zeitpunkt"];
                    var base_erreg_nachweis_klinik = (string)JObject.Parse(createEntry.ToString())["KlinischerNachweis"];
                    var base_zuletzt_akt = (string)JObject.Parse(createEntry.ToString())["LetzteAktualisierung"];
                    var base_komma_1 = "";
                    var base_komma_2 = "";
                    var base_komma_3 = "";
                    var base_komma_4 = "";
                    var base_komma_5 = "";
                    var base_evu_ende = "";


                    if (base_erreg_nachweis != "False" | base_erregname != null | base_zeitpunkt_kenn != null | base_erreg_nachweis_klinik != "False" | base_zuletzt_akt != null)
                    {
                        if (base_anzeichen != null | base_symp_auftreten != null | base_spez_symp != null | base_all_komm != null)
                        {
                            base_komma_1 = ",";
                        }
                        base_evulation_1 = "{\"_type\":\"EVALUATION\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Kennzeichnung Erregernachweis\"},\"archetype_details\":{\"archetype_id\":{\"value\":\"openEHR-EHR-EVALUATION.flag_pathogen.v0\"},\"rm_version\":\"1.0.4\"},\"archetype_node_id\":\"openEHR-EHR-EVALUATION.flag_pathogen.v0\",\"language\":{\"terminology_id\":{\"value\":\"ISO_639-1\"},\"code_string\":\"de\"},\"encoding\":{\"terminology_id\":{\"value\":\"IANA_character-sets\"},\"code_string\":\"UTF-8\"},\"subject\":{\"_type\":\"PARTY_SELF\"},\"data\":{\"_type\":\"ITEM_TREE\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Tree\"},\"archetype_node_id\":\"at0001\",\"items\":[";

                        if (base_erreg_nachweis != null)
                        {
                            if (base_erreg_nachweis == "True")
                            {
                                base_erreg_nachweis = "{\"_type\":\"ELEMENT\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Erregernachweis\"},\"archetype_node_id\":\"at0005\",\"value\":{\"_type\":\"DV_BOOLEAN\",\"value\":true}}";
                            }
                            else
                            {
                                base_erreg_nachweis = null;
                            }

                        }

                        if (base_erregname != null)
                        {
                            if (base_erreg_nachweis != null)
                            {
                                base_komma_2 = ",";
                            }
                            base_erregname = "{\"_type\":\"ELEMENT\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Erregername\"},\"archetype_node_id\":\"at0012\",\"value\":{\"_type\":\"DV_CODED_TEXT\",\"value\":\"" + JObject.Parse(createEntry.ToString())["Erregername"] + "\",\"defining_code\":{\"terminology_id\":{\"value\":\"local_terms\"},\"code_string\":\"COV\"}}}";
                        }

                        if (base_zeitpunkt_kenn != null)
                        {
                            if (base_erreg_nachweis != null | base_erregname != null)
                            {
                                base_komma_3 = ",";
                            }
                            base_zeitpunkt_kenn = "{\"_type\":\"ELEMENT\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Zeitpunkt der Kennzeichnung\"},\"archetype_node_id\":\"at0015\",\"value\":{\"_type\":\"DV_DATE_TIME\",\"value\":\"" + JObject.Parse(createEntry.ToString())["Zeitpunkt"] + "\"}}";
                        }

                        if (base_erreg_nachweis_klinik != "False")
                        {
                            if (base_erreg_nachweis != null | base_erregname != null | base_zeitpunkt_kenn != null)
                            {
                                base_komma_4 = ",";
                            }
                            base_erreg_nachweis_klinik = "{\"_type\":\"ELEMENT\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Erregernachweis in der Klinik\"},\"archetype_node_id\":\"at0011\",\"value\":{\"_type\":\"DV_BOOLEAN\",\"value\":true}}]}";
                        }
                        else
                        {
                            base_erreg_nachweis_klinik = null;
                        }


                        if (base_zuletzt_akt != null)
                        {
                            if (base_erreg_nachweis != null | base_erregname != null | base_zeitpunkt_kenn != null | base_erreg_nachweis_klinik != null)
                            {
                                base_komma_5 = ",";
                            }
                            base_zuletzt_akt = "\"protocol\":{\"_type\":\"ITEM_TREE\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Tree\"},\"archetype_node_id\":\"at0003\",\"items\":[{\"_type\":\"ELEMENT\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Zuletzt aktualisiert\"},\"archetype_node_id\":\"at0004\",\"value\":{\"_type\":\"DV_DATE_TIME\",\"value\":\"" + JObject.Parse(createEntry.ToString())["LetzteAktualisierung"] + "\"}}";
                        }

                        base_evu_ende = "]}}";
                    }
                    else
                    {
                        base_erreg_nachweis = null;
                        base_erreg_nachweis_klinik = null;
                    }

                    var base_evulation_2 = "";
                    var base_freigestellt = (string)JObject.Parse(createEntry.ToString())["Freistellung"];
                    var base_grund = (string)JObject.Parse(createEntry.ToString())["Grund"];
                    var base_beschreibung = (string)JObject.Parse(createEntry.ToString())["Beschreibung"];
                    var base_start_frei = (string)JObject.Parse(createEntry.ToString())["Startdatum"];
                    var base_wiederaufnahme = (string)JObject.Parse(createEntry.ToString())["Enddatum"];
                    var base_komm_frei = (string)JObject.Parse(createEntry.ToString())["AbwesendheitKommentar"];
                    var base_ende_evu_2 = "";
                    var base_komma_6 = "";
                    var base_komma_7 = "";
                    var base_komma_8 = "";
                    var base_komma_9 = "";
                    var base_komma_10 = "";
                    var base_komma_11 = "";

                    if (base_freigestellt != "False" | base_grund != null | base_beschreibung != null | base_start_frei != null | base_wiederaufnahme != null | base_komm_frei != null)
                    {
                        if (base_erreg_nachweis != null | base_erregname != null | base_zeitpunkt_kenn != null | base_erreg_nachweis_klinik != null | base_zuletzt_akt != null | base_anzeichen != null | base_symp_auftreten != null | base_spez_symp != null | base_all_komm != null)
                        {
                            base_komma_6 = ",";
                        }
                        base_evulation_2 = "{\"_type\":\"EVALUATION\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Freistellung von der Arbeit\"},\"archetype_details\":{\"archetype_id\":{\"value\":\"openEHR-EHR-EVALUATION.exemption_from_work.v0\"},\"rm_version\":\"1.0.4\"},\"archetype_node_id\":\"openEHR-EHR-EVALUATION.exemption_from_work.v0\",\"language\":{\"terminology_id\":{\"value\":\"ISO_639-1\"},\"code_string\":\"de\"},\"encoding\":{\"terminology_id\":{\"value\":\"IANA_character-sets\"},\"code_string\":\"UTF-8\"},\"subject\":{\"_type\":\"PARTY_SELF\"},\"data\":{\"_type\":\"ITEM_TREE\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Tree\"},\"archetype_node_id\":\"at0001\",\"items\":[";

                        if (base_freigestellt != null)
                        {
                            if (base_freigestellt == "True")
                            {
                                base_freigestellt = "{\"_type\":\"ELEMENT\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Freigestellt von der Arbeit?\"},\"archetype_node_id\":\"at0008\",\"value\":{\"_type\":\"DV_BOOLEAN\",\"value\":true}}";
                            }
                            else
                            {
                                base_freigestellt = null;
                            }

                        }

                        if (base_grund != null)
                        {
                            if (base_freigestellt != null)
                            {
                                base_komma_7 = ",";
                            }
                            base_grund = "{\"_type\":\"ELEMENT\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Grund für die Freistellung\"},\"archetype_node_id\":\"at0005\",\"value\":{\"_type\":\"DV_TEXT\",\"value\":\"" + JObject.Parse(createEntry.ToString())["Grund"] + "\"}}";
                        }

                        if (base_beschreibung != null)
                        {
                            if (base_freigestellt != null | base_grund != null)
                            {
                                base_komma_8 = ",";
                            }
                            base_beschreibung = "{\"_type\":\"ELEMENT\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Beschreibung\"},\"archetype_node_id\":\"at0002\",\"value\":{\"_type\":\"DV_TEXT\",\"value\":\"" + JObject.Parse(createEntry.ToString())["Beschreibung"] + "\"}}";
                        }

                        if (base_start_frei != null)
                        {
                            if (base_freigestellt != null | base_grund != null | base_beschreibung != null)
                            {
                                base_komma_9 = ",";
                            }
                            base_start_frei = "{\"_type\":\"ELEMENT\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Startdatum der Freistellung\"},\"archetype_node_id\":\"at0003\",\"value\":{\"_type\":\"DV_DATE\",\"value\":\"" + JObject.Parse(createEntry.ToString())["Startdatum"] + "\"}}";
                        }

                        if (base_wiederaufnahme != null)
                        {
                            if (base_freigestellt != null | base_grund != null | base_beschreibung != null | base_start_frei != null)
                            {
                                base_komma_10 = ",";
                            }
                            base_wiederaufnahme = "{\"_type\":\"ELEMENT\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Wiederaufnahme der Arbeit\"},\"archetype_node_id\":\"at0004\",\"value\":{\"_type\":\"DV_DATE\",\"value\":\"" + JObject.Parse(createEntry.ToString())["Enddatum"] + "\"}}";
                        }

                        if (base_komm_frei != null)
                        {
                            if (base_freigestellt != null | base_grund != null | base_beschreibung != null | base_start_frei != null | base_wiederaufnahme != null)
                            {
                                base_komma_11 = ",";
                            }
                            base_komm_frei = "{\"_type\":\"ELEMENT\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Kommentar\"},\"archetype_node_id\":\"at0007\",\"value\":{\"_type\":\"DV_TEXT\",\"value\":\"" + JObject.Parse(createEntry.ToString())["AbwesendheitKommentar"] + "\"}}";
                        }

                        base_ende_evu_2 = "]}}";
                    }
                    else
                    {
                        base_freigestellt = null;
                    }

                    var base_admin_entry = "";
                    var base_meldung = (string)JObject.Parse(createEntry.ToString())["Meldung"];
                    var base_ereignis = (string)JObject.Parse(createEntry.ToString())["Ereignis"];
                    var base_beschreibung_ereignis = (string)JObject.Parse(createEntry.ToString())["Ereignisbeschreibung"];
                    var base_datum_meldung = (string)JObject.Parse(createEntry.ToString())["Datum"];
                    var base_grund_meldung = (string)JObject.Parse(createEntry.ToString())["Ereignisgrund"];
                    var base_meldung_komm = (string)JObject.Parse(createEntry.ToString())["EreignisKommentar"];
                    var base_end_admin_entry = "";
                    var base_komma_12 = "";
                    var base_komma_13 = "";
                    var base_komma_14 = "";
                    var base_komma_15 = "";
                    var base_komma_16 = "";
                    var base_komma_17 = "";

                    if (base_meldung != "False" | base_ereignis != null | base_beschreibung_ereignis != null | base_datum_meldung != null | base_grund_meldung != null | base_meldung_komm != null)
                    {
                        if (base_erreg_nachweis != null | base_erregname != null | base_zeitpunkt_kenn != null | base_erreg_nachweis_klinik != null | base_zuletzt_akt != null | base_anzeichen != null | base_symp_auftreten != null | base_spez_symp != null | base_all_komm != null | base_freigestellt != null | base_grund != null | base_beschreibung != null | base_start_frei != null | base_wiederaufnahme != null | base_komm_frei != null)
                        {
                            base_komma_12 = ",";
                        }
                        base_admin_entry = "{\"_type\":\"ADMIN_ENTRY\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Meldung an das Gesundheitsamt\"},\"archetype_details\":{\"archetype_id\":{\"value\":\"openEHR-EHR-ADMIN_ENTRY.report_to_health_department.v0\"},\"rm_version\":\"1.0.4\"},\"archetype_node_id\":\"openEHR-EHR-ADMIN_ENTRY.report_to_health_department.v0\",\"language\":{\"terminology_id\":{\"value\":\"ISO_639-1\"},\"code_string\":\"de\"},\"encoding\":{\"terminology_id\":{\"value\":\"IANA_character-sets\"},\"code_string\":\"UTF-8\"},\"subject\":{\"_type\":\"PARTY_SELF\"},\"data\":{\"_type\":\"ITEM_TREE\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Tree\"},\"archetype_node_id\":\"at0001\",\"items\":[";

                        if (base_meldung != null)
                        {
                            if (base_meldung == "True")
                            {
                                base_meldung = "{\"_type\":\"ELEMENT\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Meldung an das Gesundheitsamt?\"},\"archetype_node_id\":\"at0009\",\"value\":{\"_type\":\"DV_BOOLEAN\",\"value\":true}}";
                            }
                            else
                            {
                                base_meldung = null;
                            }

                        }

                        if (base_ereignis != null)
                        {
                            if (base_meldung != null)
                            {
                                base_komma_13 = ",";
                            }
                            base_ereignis = "{\"_type\":\"ELEMENT\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Zu meldendes Ereignis\"},\"archetype_node_id\":\"at0003\",\"value\":{\"_type\":\"DV_TEXT\",\"value\":\"" + JObject.Parse(createEntry.ToString())["Ereignis"] + "\"}}";
                        }

                        if (base_beschreibung_ereignis != null)
                        {
                            if (base_meldung != null | base_ereignis != null)
                            {
                                base_komma_14 = ",";
                            }
                            base_beschreibung_ereignis = "{\"_type\":\"ELEMENT\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Beschreibung\"},\"archetype_node_id\":\"at0004\",\"value\":{\"_type\":\"DV_TEXT\",\"value\":\"" + JObject.Parse(createEntry.ToString())["Ereignisbeschreibung"] + "\"}}";
                        }

                        if (base_datum_meldung != null)
                        {
                            if (base_meldung != null | base_ereignis != null | base_beschreibung_ereignis != null)
                            {
                                base_komma_15 = ",";
                            }
                            base_datum_meldung = "{\"_type\":\"ELEMENT\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Datum der Meldung\"},\"archetype_node_id\":\"at0005\",\"value\":{\"_type\":\"DV_DATE_TIME\",\"value\":\"" + JObject.Parse(createEntry.ToString())["Datum"] + "\"}}";
                        }

                        if (base_grund_meldung != null)
                        {
                            if (base_meldung != null | base_ereignis != null | base_beschreibung_ereignis != null | base_datum_meldung != null)
                            {
                                base_komma_16 = ",";
                            }
                            base_grund_meldung = "{\"_type\":\"ELEMENT\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Grund\"},\"archetype_node_id\":\"at0006\",\"value\":{\"_type\":\"DV_TEXT\",\"value\":\"" + JObject.Parse(createEntry.ToString())["Ereignisgrund"] + "\"}}";
                        }

                        if (base_meldung_komm != null)
                        {
                            if (base_meldung != null | base_ereignis != null | base_beschreibung_ereignis != null | base_datum_meldung != null | base_grund_meldung != null)
                            {
                                base_komma_17 = ",";
                            }
                            base_meldung_komm = "{\"_type\":\"ELEMENT\",\"name\":{\"_type\":\"DV_TEXT\",\"value\":\"Kommentar\"},\"archetype_node_id\":\"at0007\",\"value\":{\"_type\":\"DV_TEXT\",\"value\":\"" + JObject.Parse(createEntry.ToString())["EreignisKommentar"] + "\"}}";
                        }
                        base_end_admin_entry = "]}}";
                    }
                    else
                    {
                        base_meldung = null;
                    }

                    var base_end_compositionn = "]}";


                    var json_all = base_composition + base_uid + base_archetype_details + base_archetype_node_id + base_language + base_territory + base_category + base_composer +
                        base_context + base_setting + base_other_context + base_bericht_id + base_content_beginn + base_content + base_data_1 + base_data_2 + base_anzeichen + base_symp_auftreten +
                        base_cluster_spez_symp + base_spez_symp + base_all_komm + base_end_cluster + base_komma_1 + base_evulation_1 + base_erreg_nachweis + base_komma_2 + base_erregname + base_komma_3 + base_zeitpunkt_kenn +
                        base_komma_4 + base_erreg_nachweis_klinik + base_komma_5 + base_zuletzt_akt + base_evu_ende + base_komma_6 + base_evulation_2 + base_freigestellt + base_komma_7 + base_grund + base_komma_8 + base_beschreibung + base_komma_9 + base_start_frei + base_komma_10 + base_wiederaufnahme +
                        base_komma_11 + base_komm_frei + base_ende_evu_2 + base_komma_12 + base_admin_entry + base_meldung + base_komma_13 + base_ereignis + base_komma_14 + base_beschreibung_ereignis + base_komma_15 + base_datum_meldung + base_komma_16 + base_grund_meldung +
                        base_komma_17 + base_meldung_komm + base_end_admin_entry + base_end_compositionn;

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
