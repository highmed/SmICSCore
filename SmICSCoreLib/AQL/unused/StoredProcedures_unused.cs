using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmICSCoreLib.REST;
using SmICSCoreLib.Util;

namespace SmICSCoreLib.AQL
{
    class StoredProcedures_unused
    {
        /*/* currently not in use, but needed for later development
        public JArray Contact_All_Contacts()
        {
            AQLQuery q = null; // AQLCatalog.Contact_PatientList_TpPs_I(timestamp, patientList);
            JArray result = this.client.AQLQuery(q.Query);
            if (result is null)
            {
                return null;
            }
            else
            {
                JArray retArr = new JArray();

                foreach (JObject obj in result)
                {
                    string ehrID = obj.Property("#0").Value.ToString();

                    AQLQuery q2 = null; // AQLCatalog.Contact_PatientList_TpPs_I(timestamp, patientList);
                    JArray result2 = this.client.AQLQuery(q2.Query);

                    foreach (JObject obj2 in result)
                    {
                        string station = obj2.Property("#0").Value.ToString();

                        string start = obj2.Property("#1").Value.ToString();
                        DateTime patStart = Convert.ToDateTime(obj.Property("#1").Value.ToString());

                        string end = obj2.Property("#2").Value.ToString();
                        DateTime patEnd = Convert.ToDateTime(obj.Property("#2").Value.ToString());

                        AQLQuery q3 = null;
                        JArray result3 = this.client.AQLQuery(q3.Query);

                        foreach (JObject obj3 in result3)
                        {
                            DateTime pat2Start = Convert.ToDateTime(obj.Property("#1").Value.ToString());
                            DateTime pat2End = Convert.ToDateTime(obj.Property("#2").Value.ToString());

                            JObject newObj = new JObject();
                            newObj.Add("paID", ehrID);
                            newObj.Add("pbID", obj.Property("#0").Value.ToString());
                            newObj.Add("Beginn", new DateTime(Math.Min(patStart.Ticks, pat2Start.Ticks)));
                            newObj.Add("Ende", new DateTime(Math.Min(patEnd.Ticks, pat2End.Ticks)));
                            newObj.Add("StationID", station);

                            retArr.Add(newObj);
                        }
                    }
                }
                return retArr;
            }
        }
        public JArray Contact_All_Contacts_TT(string starttime, string endtime)
        {
            AQLQuery q = null; //Query muss ehr, start und endtime + station geordnet nach ehr ausgeben
            JArray result = this.client.AQLQuery(q.Query);
            if (result is null)
            {
                return null;
            }
            else
            {
                JArray retArr = new JArray();
                foreach (JObject obj in result)
                {
                    string ehrID = obj.Property("#0").Value.ToString();
                    string start = obj.Property("#1").Value.ToString();
                    DateTime patStart = Convert.ToDateTime(obj.Property("#1").Value.ToString());
                    string end = obj.Property("#1").Value.ToString();
                    DateTime patEnd = Convert.ToDateTime(obj.Property("#2").Value.ToString());
                    string station = obj.Property("#1").Value.ToString();

                    AQLQuery q2 = null; //Query die nach Kontakten auf der Station in dem Zeitrahmen findet
                    JArray result2 = this.client.AQLQuery(q2.Query);

                    foreach (JObject obj2 in result)
                    {
                        DateTime pat2Start = Convert.ToDateTime(obj.Property("#1").Value.ToString());
                        DateTime pat2End = Convert.ToDateTime(obj.Property("#2").Value.ToString());

                        JObject newObj = new JObject();
                        newObj.Add("paID", ehrID);
                        newObj.Add("pbID", obj.Property("#0").Value.ToString());
                        newObj.Add("Beginn", new DateTime(Math.Min(patStart.Ticks, pat2Start.Ticks)));
                        newObj.Add("Ende", new DateTime(Math.Min(patEnd.Ticks, pat2End.Ticks)));
                        newObj.Add("StationID", station);

                        retArr.Add(newObj);
                    }
                }
                return retArr;
            }
        }
        public JArray Contact_PatientList_TpPs(string timestamp, string patientList)
        {
            AQLQuery q = AQLCatalog.Contact_PatientList_TpPs_I(timestamp, patientList);
            JArray result = this.client.AQLQuery(q.Query);
            if (result is null)
            {
                return null;
            }
            else
            {
                Dictionary<string, Dictionary<string, List<string[]>>> patientStation = new Dictionary<string, Dictionary<string, List<string[]>>>();
                JArray retArr = new JArray();

                foreach (JObject obj in result)
                {
                    string ehrid = obj.Property("#0").Value.ToString();
                    string station = obj.Property("#1").Value.ToString();
                    string[] dates = new string[] { obj.Property("#2").Value.ToString(), obj.Property("#3").Value.ToString() };
                    if (patientStation.ContainsKey(ehrid))
                    {
                        if (!patientStation[ehrid].ContainsKey(station))
                        {
                            patientStation[ehrid][station].Add(dates);
                        }
                        else
                        {
                            patientStation[ehrid].Add(station, new List<string[]> { dates });
                        }
                    }
                    else
                    {
                        patientStation.Add(ehrid, new Dictionary<string, List<string[]>> { { station, new List<string[]> { dates } } });
                    }
                }
                foreach (string ehr in patientStation.Keys)
                {
                    foreach (string stadion in patientStation[ehr].Keys)
                    {
                        foreach (string[] start_end_date in patientStation[ehr][stadion])
                        {
                            AQLQuery q2 = AQLCatalog.Contact_PatientList_TpPs_II(start_end_date[0], start_end_date[0], stadion);
                            JArray result2 = this.client.AQLQuery(q2.Query);
                            JObject newObj = new JObject();
                            foreach (JObject obj in result2)
                            {
                                string match = obj.Property("#0").Value.ToString();
                                newObj.Add(ehr, match);
                            }
                            retArr.Add(newObj);
                        }
                    }
                }
                return retArr;
            }
        }
        public JArray Contact_PatientList_TTPs(string starttime, string endtime, string patientList)
        {
            //TODO: Alr Rückgabe fehlt hier noch die Station
            AQLQuery q = AQLCatalog.Contact_PatientList_TTPs_I(starttime, endtime, patientList);
            JArray result = this.client.AQLQuery(q.Query);
            if (result is null)
            {
                return null;
            }
            else
            {
                Dictionary<string, Dictionary<string, List<string[]>>> patientStation = new Dictionary<string, Dictionary<string, List<string[]>>>();
                JArray retArr = new JArray();

                foreach (JObject obj in result)
                {
                    string ehrid = obj.Property("#0").Value.ToString();
                    string station = obj.Property("#1").Value.ToString();
                    string[] dates = new string[] { obj.Property("#2").Value.ToString(), obj.Property("#3").Value.ToString() };
                    if (patientStation.ContainsKey(ehrid))
                    {
                        if (!patientStation[ehrid].ContainsKey(station))
                        {
                            patientStation[ehrid][station].Add(dates);
                        }
                        else
                        {
                            patientStation[ehrid].Add(station, new List<string[]> { dates });
                        }
                    }
                    else
                    {
                        patientStation.Add(ehrid, new Dictionary<string, List<string[]>> { { station, new List<string[]> { dates } } });
                    }
                }
                foreach (string ehr in patientStation.Keys)
                {
                    foreach (string stadion in patientStation[ehr].Keys)
                    {
                        foreach (string[] start_end_date in patientStation[ehr][stadion])
                        {
                            AQLQuery q2 = AQLCatalog.Contact_PatientList_TTPs_II(start_end_date[0], start_end_date[0], stadion);
                            JArray result2 = this.client.AQLQuery(q2.Query);
                            JObject newObj = new JObject();
                            foreach (JObject obj in result2)
                            {
                                string match = obj.Property("#0").Value.ToString();
                                newObj.Add(ehr, match);
                            }
                            retArr.Add(newObj);
                        }
                    }
                }
                return retArr;
            }
        }*/

        /* currently not in use, but needed for later development
        public JArray Patient_Information_TT(string starttime, string endtime)
        {
            AQLQuery q = AQLCatalog.Patient_Information_TT(starttime, endtime);
            JArray result = this.client.AQLQuery(q.Query);
            if (result is null)
            {
                return null;
            }
            else
            {
                return result;
                //return JsonConvert.DeserializeObject<JArray>(result.ToString());
            }
        }
        public JArray Patient_Information_TTES(string starttime, string endtime, string stationsID, string erregername)
        {
            //Erstelle return JSON Array
            JArray retArray = new JArray();

            //Abfrage1 der Gesamtabfrage
            AQLQuery q1 = AQLCatalog.Patient_Information_TTES_I(starttime, endtime, erregername);
            JArray result1 = this.client.AQLQuery(q1.Query);

            Console.WriteLine(result1);

            if (!(result1 is null))
            {
                foreach (JObject obj in result1)
                {
                    //für jeden gefundenen Patienten wird ein neues JSON Object erstellt, was befüllt wird 
                    Dictionary<string, string> newObj = new Dictionary<string, string>();

                    string ehr_id = obj.Property("#0").Value.ToString();
                    newObj.Add(q1.ExpectedOutcome[0], ehr_id);

                    string aufnahmedatum = obj.Property("#1").Value.ToString();
                    string entlassdatum = obj.Property("#2").Value.ToString();
                    string probendatum = obj.Property("#3").Value.ToString();

                    //Tranformation der Daten (Aufnahme, ...) in DateTime zur der Gesamtzeit des Krankenhausaufenthalts, sowie für später ermittelten Infizierten Tage
                    DateTime start = Convert.ToDateTime(aufnahmedatum);
                    DateTime end = new DateTime();
                    if (!entlassdatum.Equals(""))
                    {
                        end = Convert.ToDateTime(entlassdatum);
                    } else
                    {
                        end = DateTime.UtcNow;
                    }

                    //Umrechnen der Krankenhauszeit in Stunden
                    TimeSpan hospitalTime = end.Subtract(start);
                    int hours = (int)(hospitalTime.TotalSeconds / (60 * 60 * 24));
                    newObj.Add(q1.ExpectedOutcome[1], hours.ToString());

                    DateTime probe = Convert.ToDateTime(probendatum);


                    //Einbindung der zweiten Abfrage, da hier erst die ehr_ids zur Verfügung stehen
                    AQLQuery q2 = AQLCatalog.Patient_Information_TTES_II(ehr_id, starttime, erregername);
                    JArray result2 = this.client.AQLQuery(q2.Query);

                    //@TODO: unsicher welchs Datum initialisiert wird. Prüfen! Muss möglichst weit in der Zukunft liegen! 
                    //Gibt es da eventuell eine legantere Lösung?! Oder gibt es elegantere Lösungen JSON Objekte nach einem Min Wert zu durchsuchen?
                    //
                    //Ansonsten Schleifendurchlauf um nach dem frühesten Nachweis für eine Infektion zu suchen. 
                    //Die einreichende Station gilt dann auch als Infektionsstation und die Probe als die Material bei dem es festgestellt wurde!
                    DateTime probeDatum = new DateTime();
                    string senderStationID = "";
                    string probenart = "";

                    foreach (JObject obj2 in result2) {
                        string probenentnahmeDatum = obj2.Property("#0").Value.ToString();
                        DateTime tmpProbeDatum = Convert.ToDateTime(probenentnahmeDatum);

                        if (tmpProbeDatum < probeDatum)
                        {
                            probeDatum = tmpProbeDatum;
                            senderStationID = obj2.Property("#2").Value.ToString(); ;
                            probenart = obj2.Property("#3").Value.ToString();
                        }
                    }

                    //Füllen des JSON Objects
                    newObj.Add(q1.ExpectedOutcome[3], stationsID.Equals(senderStationID).ToString()); //Hier Fragen ob nosokomial gemeint ist oder ob auf anderer Station infiziert!
                    newObj.Add(q1.ExpectedOutcome[4], probeDatum.ToString());
                    newObj.Add(q1.ExpectedOutcome[5], senderStationID);
                    newObj.Add(q1.ExpectedOutcome[6], probenart);

                    //Einbindung der zweiten Abfrage, da hier erst die ehr_ids zur Verfügung stehen
                    //Zum finden ob der Patient von der Infektion geheilt wurde
                    AQLQuery q3 = AQLCatalog.Patient_Information_TTES_III(ehr_id, starttime, erregername);
                    JArray result3 = this.client.AQLQuery(q3.Query);

                    TimeSpan sickTime = new TimeSpan();
                    int sickHours = 0;
                    //Falls keine Result zurück kommt, dann wurde der Patient nicht geheilt
                    //Annahme ist daher, dass der Patient bis zum Ende des Aufenthalts als iniziert gilt
                    if (result1 is null)
                    {
                        sickTime = end.Subtract(probeDatum);
                        sickHours = (int)(hospitalTime.TotalSeconds / (60 * 60 * 24));
                        newObj.Add(q1.ExpectedOutcome[2], hours.ToString());
                    }
                    else
                    {
                        //Falls es ein Result gibt heißt es der Patient wurde geheilt. Daher Schleifendurchlauf um nach dem kleinsten Datum zu suchen bei dem es 
                        //zu dem Erreger keinen Nachweis mehr gab. 
                        DateTime negativeProbeDatum = new DateTime();
                        foreach (JObject obj3 in result3)
                        {
                            string probenentnahmeDatum = obj3.Property("#0").Value.ToString();
                            DateTime tmpProbeDatum = Convert.ToDateTime(probenentnahmeDatum);
                            if (tmpProbeDatum < negativeProbeDatum)
                            {
                                negativeProbeDatum = tmpProbeDatum;
                            }
                            sickTime = negativeProbeDatum.Subtract(probeDatum);
                            sickHours = (int)(hospitalTime.TotalSeconds / (60 * 60 * 24));
                            newObj.Add(q1.ExpectedOutcome[2], sickHours.ToString());
                        }
                    }

                    //Hinzufügen zum Rückgabe JSON Array
                    retArray.Add(newObj);
                }
            }

            return retArray;
        }
        public JArray Patient_Information_TTE(string starttime, string endtime, string erregername)
        {
            AQLQuery q = AQLCatalog.Patient_Erkrankt_TTE(starttime, endtime, erregername);
            JArray result = this.client.AQLQuery(q.Query);
            if (result is null)
            {
                return null;
            }
            else
            {
                return result;
            }
        }
        public JArray Patient_Ersterkrankung_P(string patientenID)
        {
            AQLQuery q = AQLCatalog.Patient_Ersterkrankung_P(patientenID);
            JArray result = this.client.AQLQuery(q.Query);

            if (result is null)
            {
                return null;
            }
            else
            {
                JArray retArr = new JArray();
                List<string> ehrID = new List<string>();
                foreach (JObject obj in result)
                {
                    string ehr = obj.Property("#0").Value.ToString();
                    if (!ehrID.Contains(ehr)) {
                        JObject newObj = new JObject();
                        newObj.Add(q.ExpectedOutcome[0], ehr);
                        newObj.Add(q.ExpectedOutcome[1], obj.Property("#1").Value.ToString());
                        newObj.Add(q.ExpectedOutcome[2], obj.Property("#2").Value.ToString());
                        newObj.Add(q.ExpectedOutcome[3], obj.Property("#3").Value.ToString());

                        retArr.Add(newObj);
                    }
                }

                return retArr;
            }
        }
        public JArray Patient_Ersterkrankung_EP(string erregername, string patientenID)
        {
            AQLQuery q = AQLCatalog.Patient_Ersterkrankung_EP(erregername, patientenID);
            JArray result = this.client.AQLQuery(q.Query);

            if (result is null)
            {
                return null;
            }
            else
            {
                return ((JArray)result.First.ToString());
            }
        }
        public JArray Patient_Ersterkrankung_TTE(string starttime, string endtime, string erregername)
        {
            AQLQuery q = AQLCatalog.Patient_Ersterkrankung_TTE(starttime, endtime, erregername);
            JArray result = this.client.AQLQuery(q.Query);
            
            if (result is null)
            {
                return null;
            }
            else
            {
                JArray retArr = new JArray();
                List<string> ehrID = new List<string>();
                foreach (JObject obj in result)
                {
                    string ehr = obj.Property("#0").Value.ToString();
                    if (!ehrID.Contains(ehr))
                    {
                        JObject newObj = new JObject();
                        newObj.Add(q.ExpectedOutcome[0], ehr);
                        newObj.Add(q.ExpectedOutcome[1], obj.Property("#1").Value.ToString());

                        retArr.Add(newObj);
                    }
                }

                return retArr;
            }
        }*/

        /* currently not in use, but needed for later development
        public JArray Patient_InKrankenhaus_TT(string starttime, string endtime) 
        {
            AQLQuery q = AQLCatalog.Patient_InKrankenhaus_TT(starttime, endtime);
            JArray result = this.client.AQLQuery(q.Query);
            if (result is null)
            {
                return null;
            }
            else
            {
                return result;
            }
        }
        public JArray Patient_Nosokomial_EPsD(string erregername, string patientList, decimal nosokomialAnforderung) 
        {
            AQLQuery q = AQLCatalog.Patient_Nosokomial_EPsD(erregername, patientList);
            JArray result = this.client.AQLQuery(q.Query);

            if (result is null)
            {
                return null;
            }
            else
            {
                JArray retArr = new JArray();
                List<string> ehrID = new List<string>();
                foreach (JObject obj in result)
                {
                    string ehr = obj.Property("#0").Value.ToString();
                    if (!ehrID.Contains(ehr))
                    {
                        Dictionary<string, string> newObj = new Dictionary<string, string>();
                        newObj.Add(q.ExpectedOutcome[0], ehr);

                        DateTime entnahme = Convert.ToDateTime(obj.Property("#1").Value.ToString());
                        DateTime aufnahme = Convert.ToDateTime(obj.Property("#2").Value.ToString());

                        TimeSpan diff = entnahme.Subtract(aufnahme);
                        newObj.Add(q.ExpectedOutcome[1], ((decimal)diff.TotalSeconds/(60*60) <= nosokomialAnforderung).ToString());

                        retArr.Add(newObj);
                    }
                }

                return retArr;
            }
        }
        public JArray Patient_Nosokomial_EPsSD(string erregername, string patientList, string stationsID, decimal nosokomialAnforderung)
        {
            AQLQuery q = AQLCatalog.Patient_Nosokomial_EPsD(erregername, patientList);
            JArray result = this.client.AQLQuery(q.Query);

            if (result is null)
            {
                return null;
            }
            else
            {
                JArray retArr = new JArray();
                List<string> ehrID = new List<string>();
                foreach (JObject obj in result)
                {
                    string ehr = obj.Property("#0").Value.ToString();
                    if (!ehrID.Contains(ehr))
                    {
                        JObject newObj = new JObject();
                        newObj.Add(q.ExpectedOutcome[0], ehr);

                        DateTime entnahme = Convert.ToDateTime(obj.Property("#1").Value.ToString());
                        DateTime aufnahme = Convert.ToDateTime(obj.Property("#2").Value.ToString());

                        TimeSpan diff = entnahme.Subtract(aufnahme);
                        newObj.Add(q.ExpectedOutcome[1], ((decimal)diff.TotalSeconds / (60 * 60) <= nosokomialAnforderung).ToString());

                        retArr.Add(newObj);
                    }
                }

                return retArr;
            }
        }
        public JArray Patient_AnzahlNosokomial_EsD(string erregernameList, decimal nosokomialAnforderung)
        {
            AQLQuery q = AQLCatalog.Patient_AnzahlNosokomial_EsD(erregernameList);
            JArray result = this.client.AQLQuery(q.Query);

            if (result is null)
            {
                return null;
            }
            else
            {
                JArray retArr = new JArray();
                Dictionary<string, List<string>> ehrID = new Dictionary<string, List<string>>();
                Dictionary<string, int> erregerCount = new Dictionary<string, int>();
                
                foreach (JObject obj in result)
                {
                    string ehr = obj.Property("#0").Value.ToString();
                    string erreger = obj.Property("#3").Value.ToString();

                    if(ehrID.ContainsKey(ehr) && ehrID[ehr].Contains(erreger))
                    {
                        continue;
                    }
                    else
                    {
                        DateTime entnahme = Convert.ToDateTime(obj.Property("#1").Value.ToString());
                        DateTime aufnahme = Convert.ToDateTime(obj.Property("#2").Value.ToString());

                        TimeSpan diff = entnahme.Subtract(aufnahme);
                        bool isNosokomial = ((decimal)diff.TotalSeconds / (60 * 60) <= nosokomialAnforderung);
                        if (isNosokomial)
                        {
                            if (!ehrID.ContainsKey(ehr))
                            {
                                ehrID.Add(ehr, new List<string>() { erreger });
                            }
                            if (erregerCount.ContainsKey(erreger))
                            {
                                erregerCount[erreger]++;
                            }
                            else
                            {
                                erregerCount.Add(erreger, 1);
                            }
                        }
                    }
                }
                foreach(string key in erregerCount.Keys)
                {
                    JObject newObj = new JObject();
                    newObj.Add(key, erregerCount[key].ToString());
                    retArr.Add(newObj);
                }
                return retArr;
            }
        }
        public JArray Patient_AnzahlNosokomial_EsSsD(string erregernameList, string stationList, decimal nosokomialAnforderung)
        {
            AQLQuery q = AQLCatalog.Patient_AnzahlNosokomial_EsD(erregernameList);
            JArray result = this.client.AQLQuery(q.Query);

            if (result is null)
            {
                return null;
            }
            else
            {
                JArray retArr = new JArray();
                Dictionary<string, List<string>> ehrID = new Dictionary<string, List<string>>();
                Dictionary<string, int> erregerCount = new Dictionary<string, int>();

                foreach (JObject obj in result)
                {
                    string ehr = obj.Property("#0").Value.ToString();
                    string erreger = obj.Property("#3").Value.ToString();

                    if (ehrID.ContainsKey(ehr) && ehrID[ehr].Contains(erreger))
                    {
                        continue;
                    }
                    else
                    {
                        DateTime entnahme = Convert.ToDateTime(obj.Property("#1").Value.ToString());
                        DateTime aufnahme = Convert.ToDateTime(obj.Property("#2").Value.ToString());

                        TimeSpan diff = entnahme.Subtract(aufnahme);
                        bool isNosokomial = ((decimal)diff.TotalSeconds / (60 * 60) <= nosokomialAnforderung);
                        if (isNosokomial)
                        {
                            if (!ehrID.ContainsKey(ehr))
                            {
                                ehrID.Add(ehr, new List<string>() { erreger });
                            }
                            if (erregerCount.ContainsKey(erreger))
                            {
                                erregerCount[erreger]++;
                            }
                            else
                            {
                                erregerCount.Add(erreger, 1);
                            }
                        }
                    }
                }

                foreach (string key in erregerCount.Keys)
                {
                    Dictionary<string,string> newObj = new Dictionary<string, string>();
                    newObj.Add(key, erregerCount[key].ToString());
                    retArr.Add(newObj);
                }
                return retArr;
            }
        }
        */

        /* currently not in use, but needed for later development
        public JArray Labor_Untersuchung_TTEP(string starttime, string endtime, string erregername, string patientList)
        {
            AQLQuery q = AQLCatalog.Labor_Untersuchung_TTEP(starttime, endtime, erregername, patientList);
            JArray result = this.client.AQLQuery(q.Query);

            if (result is null)
            {
                return null;
            }
            else
            {
                JArray retArr = new JArray();
                foreach(JObject obj in result)
                {
                    Dictionary<string, string> newObj = new Dictionary<string, string>();
                    newObj.Add(q.ExpectedOutcome[0], obj.Property("#0").Value.ToString());
                    newObj.Add(q.ExpectedOutcome[1], obj.Property("#1").Value.ToString());
                    newObj.Add(q.ExpectedOutcome[2], obj.Property("#2").Value.Equals("Nachweis").ToString());

                    retArr.Add(newObj);
                }

                return retArr;
            }
        }
        public JArray Labor_PositiveUntersuchung_TT(string starttime, string endtime)
        {
            AQLQuery q = AQLCatalog.Labor_PositiveUntersuchung_TT(starttime, endtime);
            JArray result = this.client.AQLQuery(q.Query);

            if (result is null)
            {
                return null;
            }
            else
            {
                JArray retArr = new JArray();
                Dictionary<string, int> erregerCount = new Dictionary<string, int>();
                foreach (JObject obj in result)
                {
                    string erreger = obj.Property("#0").Value.ToString();
                    if (erregerCount.ContainsKey(erreger))
                    {
                        erregerCount[erreger] += 1;
                    } 
                    else
                    {
                        erregerCount.Add(erreger, 1);
                    }
                }
                foreach(string key in erregerCount.Keys)
                {
                    JObject newObj = new JObject();
                    newObj.Add(key, erregerCount[key].ToString());
                    retArr.Add(newObj);
                }
                return retArr;
            }
        }
        public JArray Labor_PositiveUntersuchung_TTE(string starttime, string endtime, string erregername)
        {
            AQLQuery q = AQLCatalog.Labor_PositiveUntersuchung_TTE(starttime, endtime, erregername);
            JArray result = this.client.AQLQuery(q.Query);

            if (result is null)
            {
                return null;
            }
            else
            {
                JArray retArr = new JArray();
                JObject newObj = new JObject();
                newObj.Add(erregername, ((JObject)result.First).Property("#0").Value.ToString());
                retArr.Add(newObj);
                return retArr;
            }
        }
        public JArray Labor_AnzahlUntersuchungen_TTEs(string starttime, string endtime, string erregernamelist)
        {
            AQLQuery q = AQLCatalog.Labor_AnzahlUntersuchungen_TTEs(starttime, endtime, erregernamelist);
            JArray result = this.client.AQLQuery(q.Query);

            if (result is null)
            {
                return null;
            }
            else
            {
                JArray retArr = new JArray();
                Dictionary<string, int> erregerCount = new Dictionary<string, int>();
                foreach (JObject obj in result)
                {
                    string erreger = obj.Property("#0").Value.ToString();
                    if (erregerCount.ContainsKey(erreger))
                    {
                        erregerCount[erreger] += 1;
                    }
                    else
                    {
                        erregerCount.Add(erreger, 1);
                    }
                }
                foreach (string key in erregerCount.Keys)
                {
                    JObject newObj = new JObject();
                    newObj.Add(key, erregerCount[key].ToString());
                    retArr.Add(newObj);
                }
                return retArr;
            }
        }*/

        /*
  public JArray Labor_ErregerProTag_TTEsKSs(string starttime, string endtime, string pathogenList, string stationIDList)
  {
      try
      {
          //this.payloadControl.checkDateStringParam(starttime, PayloadControl.Pattern.DATETIME);
          //this.payloadControl.checkDateStringParam(endtime, PayloadControl.Pattern.DATETIME);
          this.payloadControl.checkStringParam(pathogenList, PayloadControl.Pattern.LIST);
          this.payloadControl.checkStringParam(stationIDList, PayloadControl.Pattern.LIST );
          //Sorted Dictionary with Date Entries for everey day in the given timespan. Containing another dictionary with the pathogens as key and their daily count as value.
          SortedDictionary<DateTime, Dictionary<string, int>> pathogenByDateClinic = new SortedDictionary<DateTime, Dictionary<string, int>>();

          //Prefills the Dictionary with all Dates in the given timespan
          this.Labor_ErregerProTag_DicToJArray_PreFillDateTime(pathogenByDateClinic, new DateTime(DateTime.Parse(starttime).Ticks, DateTimeKind.Local), new DateTime(DateTime.Parse(endtime).Ticks, DateTimeKind.Local), pathogenList);

          //Transforms the comma separated station list to an array of integers which represents the stations 
          int[] stations = this.stationTransformation.ConvertToStationIDArray(stationIDList);

          JArray retArr = new JArray();

          foreach (int station in stations)
          {
              SortedDictionary<DateTime, Dictionary<string, int>> pathogenByDate = new SortedDictionary<DateTime, Dictionary<string, int>>();
              this.Labor_ErregerProTag_DicToJArray_PreFillDateTime(pathogenByDate, new DateTime(DateTime.Parse(starttime).Ticks, DateTimeKind.Local), new DateTime(DateTime.Parse(endtime).Ticks, DateTimeKind.Local), pathogenList);

              Dictionary<string, string> stationnames = this.stationTransformation.getStationName(station);

              AQLQuery q = AQLCatalog.Labor_ErregerProTag_TTEsKSs_I(starttime, endtime, stationnames["Departement"], stationnames["Station"]);
              JArray result = this.client.AQLQuery(q.Query);
              foreach (JObject obj in result)
              {
                  string patID = obj.Property("PatientID").Value.ToString();

                  string pathoNameList = this.pathogenTransformation.ConvertToPathogenNameList(pathogenList);

                  List<string> uniquePathogens = new List<string>();

                  AQLQuery q2 = AQLCatalog.Labor_ErregerProTag_TTEsKSs_II(starttime, endtime, patID, pathoNameList);
                  JArray result2 = this.client.AQLQuery(q2.Query);

                  if (result2 is null)
                  {
                      continue;
                  }
                  foreach (JObject obj2 in result2)
                  {
                      DateTime dt = new DateTime(DateTime.Parse(obj2.Property("Datetime").Value.ToString()).Ticks, DateTimeKind.Local);
                      //string pathogen = obj2.Property("ErregerBEZK").Value.ToString();
                      string pathogen = "COV";
                      if(patID == "4100934317" || (dt >= Convert.ToDateTime("2020-03-06T00:00:00+01:00") && (dt <= Convert.ToDateTime("2020-03-06T23:59:59+01:00")))){
                          System.Diagnostics.Debug.Print(obj.ToString() + obj2.ToString());
                      }
                      if (!uniquePathogens.Contains(pathogen))
                      {
                          uniquePathogens.Add(pathogen);
                          this.Labor_ErregerProTag_FillDict(pathogenByDate, dt, pathogen);
                          this.Labor_ErregerProTag_FillDict(pathogenByDateClinic, dt, pathogen);
                      }
                  }
              }
              this.Labor_ErregerProTag_DicToJArray(retArr, pathogenByDateClinic, station);
          }
          this.Labor_ErregerProTag_DicToJArray(retArr, pathogenByDateClinic, -1);

          System.Diagnostics.Debug.Print(retArr.ToString());
          return retArr;
      }
      catch (Exception e)
      {
          System.Diagnostics.Debug.Print(e.ToString());
          throw this.parameterTransformation.CastException(e);
      }
  }
  #region Helpers: Labor_ErregerProTag_TTEsKSs
  private void Labor_ErregerProTag_DicToJArray(JArray jArray, SortedDictionary<DateTime, Dictionary<string, int>> dict, int station)
  {
      Dictionary<string, List<int>> last7days = new Dictionary<string, List<int>>();
      Dictionary<string, List<int>> last28days = new Dictionary<string, List<int>>();
      foreach (string patho in dict[dict.Keys.First()].Keys)
      {
          last7days.Add(patho, new List<int>());
          last28days.Add(patho, new List<int>());
      }

      foreach (DateTime dt in dict.Keys)
      {
          JProperty caseCount = Labor_DailyCaseCount(printDateTime(dt));
          foreach (KeyValuePair<string, int> kvp in dict[dt])
          {
              JObject newObj = new JObject();
              newObj.Add("Datum", dt);
              newObj.Add("ErregerBEZK", kvp.Key);
              newObj.Add("Anzahl", kvp.Value);
              newObj.Add("Anzahl_cs", 0);
              newObj.Add("MAVG7", MovingAverage(last7days, kvp, 7));
              newObj.Add("MAVG28", MovingAverage(last28days, kvp, 28));
              this.pathogenTransformation.ConvertToInt(newObj, "ErregerBEZK", "ErregerID");
              if (station == -1)
              {
                  newObj.Add("StationID", "klinik");
                  newObj.Add("anzahl_gesamt", Convert.ToInt32(caseCount.Value));
                  newObj.Add("anzahl_gesamt_av7", 0);
                  newObj.Add("anzahl_gesamt_av28", 0);
              }
              else
              {
                  newObj.Add("StationID", station);
                  newObj.Add("anzahl_gesamt", 0);
                  newObj.Add("anzahl_gesamt_av7", 0);
                  newObj.Add("anzahl_gesamt_av28", 0);
              }

              jArray.Add(newObj);
          }
      }
  }
  private void Labor_ErregerProTag_FillDict(SortedDictionary<DateTime, Dictionary<string, int>> dict, DateTime dt, string pathogen)
  {
      if (dict.ContainsKey(dt))
      {
          if (dict[dt].ContainsKey(pathogen))
          {
              dict[dt][pathogen] += 1;
          }
          else
          {
              dict[dt].Add(pathogen, 1);
          }
      }
      else
      {
          dict.Add(dt, new Dictionary<string, int> { { pathogen, 1 } });
      }
  }
  private void Labor_ErregerProTag_DicToJArray_PreFillDateTime(SortedDictionary<DateTime, Dictionary<string, int>> dict, DateTime start, DateTime end, string pathogenList)
  {
      int[] pathogens = this.parameterTransformation.ConvertToIntArray(pathogenList);
      for (DateTime date = start; date.Date <= end; date = date.AddDays(1))
      {
          dict.Add(date, new Dictionary<string, int>());
          foreach (int patho in pathogens)
          {
              dict[date].Add(this.pathogenTransformation.getPathogenName(patho), 0);
          }
      }
  }

  public JProperty Labor_DailyCaseCount(string date)
  {
      AQLQuery q = AQLCatalog.Dev_DailyCaseCount_T(date, "true");
      System.Diagnostics.Debug.Print(q.Query);
      JArray result = this.client.AQLQuery(q.Query);
      if (result is null)
      {
          return null;
      }
      AQLQuery q2 = AQLCatalog.Dev_DailyCaseCount_T(date, "false");
      System.Diagnostics.Debug.Print(q2.Query);
      JArray result2 = this.client.AQLQuery(q2.Query);
      if (result2 is null)
      {
          return null;
      }
      JObject obj = (JObject)result.First;
      obj.Property("TagesaktuelleFallzahl").Value = Convert.ToInt32(obj.Property("TagesaktuelleFallzahl").Value.ToString()) - Convert.ToInt32(((JObject)result2.First).Property("TagesaktuelleFallzahl").Value.ToString());

      return obj.Property("TagesaktuelleFallzahl");
  }

  private int MovingAverage(Dictionary<string, List<int>> dict, KeyValuePair<string, int> kvp, int mavgTime)
  {
      int mavg = 0;
      dict[kvp.Key].Add(kvp.Value);
      if (dict[kvp.Key].Count == mavgTime)
      {
          mavg = (int)Math.Ceiling((double)dict[kvp.Key].Sum() / mavgTime);
          dict[kvp.Key].RemoveAt(0);
      }
      return mavg;
  }*/
        /*#region dev
        public JArray Dev_AnzahlPatienten_TT(string starttime, string endtime)
        {
            AQLQuery q = AQLCatalog.Dev_AnzahlPatienten_TT(starttime, endtime);
            JArray result = this.client.AQLQuery(q.Query);

            if (result is null)
            {
                return null;
            }
            else
            {
                return JsonConvert.DeserializeObject<JArray>(result.ToString());
            }
        }
        public JArray Dev_IstPatientKrank_TEPs(string endtime, string erregername, string patientList)
        {
            AQLQuery q = AQLCatalog.Dev_IstPatientKrank_TEPs(endtime, erregername, patientList);
            JArray result = this.client.AQLQuery(q.Query);

            if (result is null)
            {
                return null;
            }
            else
            {
                JArray retArr = new JArray();
                List<string> patientInfected = new List<string>();
                foreach (JObject obj in result)
                {
                    string ehrId = obj.Property("#0").Value.ToString();
                    if (!patientInfected.Contains(ehrId) && obj.Property("#2").Value.ToString().Equals("Nachweis"))
                    {
                        patientInfected.Add(ehrId);
                        retArr.Add(new JObject(ehrId, obj.Property("#1").Value.ToString()));
                    }

                }
                return retArr;
            }
        }

        #endregion

        #region Base
        public JArray Base_Material()
        {
            AQLQuery q = AQLCatalog.Base_Material_Mibi();
            JArray result = this.client.AQLQuery(q.Query);

            if (result is null)
            {
                return null;
            }
            else
            {
                return result;
            }
        }
        public JArray Base_Erreger()
        {
            AQLQuery q = AQLCatalog.Base_Erreger_Mibi();
            JArray result = this.client.AQLQuery(q.Query);
            AQLQuery q2 = AQLCatalog.Base_Erreger_Mibi();
            JArray result2 = this.client.AQLQuery(q2.Query);

            if (result is null)
            {
                return null;
            }
            else
            {
                return result;
            }
        }
        #endregion*/

        /* currently not in use, but needed for later development
       public JArray Labor_Untersuchung_TTEP(string starttime, string endtime, string erregername, string patientList)
       {
           AQLQuery q = AQLCatalog.Labor_Untersuchung_TTEP(starttime, endtime, erregername, patientList);
           JArray result = this.client.AQLQuery(q.Query);

           if (result is null)
           {
               return null;
           }
           else
           {
               JArray retArr = new JArray();
               foreach(JObject obj in result)
               {
                   Dictionary<string, string> newObj = new Dictionary<string, string>();
                   newObj.Add(q.ExpectedOutcome[0], obj.Property("#0").Value.ToString());
                   newObj.Add(q.ExpectedOutcome[1], obj.Property("#1").Value.ToString());
                   newObj.Add(q.ExpectedOutcome[2], obj.Property("#2").Value.Equals("Nachweis").ToString());

                   retArr.Add(newObj);
               }

               return retArr;
           }
       }
       public JArray Labor_PositiveUntersuchung_TT(string starttime, string endtime)
       {
           AQLQuery q = AQLCatalog.Labor_PositiveUntersuchung_TT(starttime, endtime);
           JArray result = this.client.AQLQuery(q.Query);

           if (result is null)
           {
               return null;
           }
           else
           {
               JArray retArr = new JArray();
               Dictionary<string, int> erregerCount = new Dictionary<string, int>();
               foreach (JObject obj in result)
               {
                   string erreger = obj.Property("#0").Value.ToString();
                   if (erregerCount.ContainsKey(erreger))
                   {
                       erregerCount[erreger] += 1;
                   } 
                   else
                   {
                       erregerCount.Add(erreger, 1);
                   }
               }
               foreach(string key in erregerCount.Keys)
               {
                   JObject newObj = new JObject();
                   newObj.Add(key, erregerCount[key].ToString());
                   retArr.Add(newObj);
               }
               return retArr;
           }
       }
       public JArray Labor_PositiveUntersuchung_TTE(string starttime, string endtime, string erregername)
       {
           AQLQuery q = AQLCatalog.Labor_PositiveUntersuchung_TTE(starttime, endtime, erregername);
           JArray result = this.client.AQLQuery(q.Query);

           if (result is null)
           {
               return null;
           }
           else
           {
               JArray retArr = new JArray();
               JObject newObj = new JObject();
               newObj.Add(erregername, ((JObject)result.First).Property("#0").Value.ToString());
               retArr.Add(newObj);
               return retArr;
           }
       }
       public JArray Labor_AnzahlUntersuchungen_TTEs(string starttime, string endtime, string erregernamelist)
       {
           AQLQuery q = AQLCatalog.Labor_AnzahlUntersuchungen_TTEs(starttime, endtime, erregernamelist);
           JArray result = this.client.AQLQuery(q.Query);

           if (result is null)
           {
               return null;
           }
           else
           {
               JArray retArr = new JArray();
               Dictionary<string, int> erregerCount = new Dictionary<string, int>();
               foreach (JObject obj in result)
               {
                   string erreger = obj.Property("#0").Value.ToString();
                   if (erregerCount.ContainsKey(erreger))
                   {
                       erregerCount[erreger] += 1;
                   }
                   else
                   {
                       erregerCount.Add(erreger, 1);
                   }
               }
               foreach (string key in erregerCount.Keys)
               {
                   JObject newObj = new JObject();
                   newObj.Add(key, erregerCount[key].ToString());
                   retArr.Add(newObj);
               }
               return retArr;
           }
       }*/

        /*#region dev
        public JArray Dev_AnzahlPatienten_TT(string starttime, string endtime)
        {
            AQLQuery q = AQLCatalog.Dev_AnzahlPatienten_TT(starttime, endtime);
            JArray result = this.client.AQLQuery(q.Query);

            if (result is null)
            {
                return null;
            }
            else
            {
                return JsonConvert.DeserializeObject<JArray>(result.ToString());
            }
        }
        public JArray Dev_IstPatientKrank_TEPs(string endtime, string erregername, string patientList)
        {
            AQLQuery q = AQLCatalog.Dev_IstPatientKrank_TEPs(endtime, erregername, patientList);
            JArray result = this.client.AQLQuery(q.Query);

            if (result is null)
            {
                return null;
            }
            else
            {
                JArray retArr = new JArray();
                List<string> patientInfected = new List<string>();
                foreach (JObject obj in result)
                {
                    string ehrId = obj.Property("#0").Value.ToString();
                    if (!patientInfected.Contains(ehrId) && obj.Property("#2").Value.ToString().Equals("Nachweis"))
                    {
                        patientInfected.Add(ehrId);
                        retArr.Add(new JObject(ehrId, obj.Property("#1").Value.ToString()));
                    }

                }
                return retArr;
            }
        }

        #endregion*/

        /*#region Base
public JArray Base_Material() 
{
    AQLQuery q = AQLCatalog.Base_Material_Mibi();
    JArray result = this.client.AQLQuery(q.Query);

    if (result is null)
    {
        return null;
    }
    else
    {
        return result;
    }
}
public JArray Base_Erreger()
{
    AQLQuery q = AQLCatalog.Base_Erreger_Mibi();
    JArray result = this.client.AQLQuery(q.Query);
    AQLQuery q2 = AQLCatalog.Base_Erreger_Mibi();
    JArray result2 = this.client.AQLQuery(q2.Query);

    if (result is null)
    {
        return null;
    }
    else
    {
        return result;
    }
}
#endregion*/
    }
}
