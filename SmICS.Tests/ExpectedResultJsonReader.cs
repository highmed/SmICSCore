using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmICSDataGenerator.Tests;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SmICSFactory.Tests
{
    public class ExpectedResultJsonReader
    {
        public static List<T> ReadResults<T, U>(string testResultPath, string parameterPath, int resultNo, ExpectedType type) where U : new()
        {
            List<U> patients = SmICSCoreLib.JSONFileStream.JSONReader<U>.Read(parameterPath);

            using (StreamReader reader = new (testResultPath, Encoding.UTF7))
            {
                string json = reader.ReadToEnd();
                JObject jObject = JsonConvert.DeserializeObject<JObject>(json);
                JArray arr = jObject.Property("" + resultNo).Value as JArray;

                switch (type)
                {
                    case ExpectedType.PATIENT_MOVEMENT:
                        if (patients[resultNo].GetType() == typeof(PatientIDs))
                        {
                            ParsePatientMovement(arr, patients[resultNo] as PatientIDs);
                        }
                        break;
                    case ExpectedType.LAB_DATA:
                        if (patients[resultNo].GetType() == typeof(PatientIDs))
                        {
                            ParseLabData(arr, patients[resultNo] as PatientIDs);
                        }
                        break;
                    case ExpectedType.STATIONARY:
                        if (patients[resultNo].GetType() == typeof(PatientInfos))
                        { 
                            ParseStationaryPat(arr, patients[resultNo] as PatientInfos);
                        }
                        break;
                    case ExpectedType.PATIENT_MOVEMENT_FROM_STATION:
                        if (patients[resultNo].GetType() == typeof(PatientInfos))
                        {
                            ParsePatMovementFromStation(arr, patients[resultNo] as PatientInfos);
                        }
                        break;
                    case ExpectedType.COUNT_DATA_MODEL:
                        if (patients[resultNo].GetType() == typeof(PatientInfos))
                        {
                            ParseGetCovidPat(arr);
                        }
                        break;
                    case ExpectedType.SYMPTOM_MODEL:
                        if (patients[resultNo].GetType() == typeof(PatientInfos))
                        {
                            ParseSymptomeData(arr, patients[resultNo] as PatientInfos);
                        }
                        break;
                }

                return arr.ToObject<List<T>>();
            }
        }

        private static void ParsePatientMovement(JArray array, PatientIDs id)
        {
            foreach (JObject obj in array)
            {
                obj.Add(new JProperty("PatientID", id.EHR_ID));
                obj.Property("Beginn").Value = DateTime.Parse(obj.Property("Beginn").Value.ToString());

                if (obj.Property("Ende").Value.Type == JTokenType.Null)
                {
                    obj.Property("Ende").Value = DateTime.Now;
                }
                else
                {
                    obj.Property("Ende").Value = DateTime.Parse(obj.Property("Ende").Value.ToString());
                }
            }
        }

        private static void ParseLabData(JArray array, PatientIDs id)
        {
            foreach (JObject obj in array)
            {
                obj.Add(new JProperty("PatientID", id.EHR_ID));
                obj.Property("Befunddatum").Value = DateTime.Parse(obj.Property("Befunddatum").Value.ToString());
                obj.Property("ZeitpunktProbeneingang").Value = DateTime.Parse(obj.Property("ZeitpunktProbeneingang").Value.ToString());
                obj.Property("Eingangsdatum").Value = DateTime.Parse(obj.Property("Eingangsdatum").Value.ToString());

            }
        }

        private static void ParseStationaryPat(JArray array, PatientInfos info)
        {
            foreach (JObject obj in array)
            {
                if (info.EHR_ID != null && info.FallID != null)
                {
                    obj.Add(new JProperty("PatientID", info.EHR_ID));
                    obj.Add(new JProperty("FallID", info.FallID));
                }
                
                obj.Property("Datum_Uhrzeit_der_Aufnahme").Value = DateTime.Parse(obj.Property("Datum_Uhrzeit_der_Aufnahme").Value.ToString());
                obj.Property("Datum_Uhrzeit_der_Entlassung").Value = DateTime.Parse(obj.Property("Datum_Uhrzeit_der_Entlassung").Value.ToString());
                obj.Property("Aufnahmeanlass").Value = obj.Property("Aufnahmeanlass").Value;
                obj.Property("Art_der_Entlassung").Value = obj.Property("Art_der_Entlassung").Value;
                obj.Property("Versorgungsfallgrund").Value = obj.Property("Versorgungsfallgrund").Value;
                //obj.Property("Station").Value = obj.Property("Station").Value;

            }
        }

        private static void ParsePatMovementFromStation(JArray array, PatientInfos info)
        {
            foreach (JObject obj in array)
            {
                obj.Add(new JProperty("PatientID", info.EHR_ID));
                obj.Add(new JProperty("StationID", info.StationID));
                obj.Add(new JProperty("Beginn", info.Beginn));
                obj.Add(new JProperty("Ende", info.Ende));
              
                obj.Property("FallID").Value = obj.Property("FallID").Value;
                obj.Property("Bewegungsart_l").Value = obj.Property("Bewegungsart_l").Value;
                obj.Property("Raum").Value = obj.Property("Raum").Value;
                obj.Property("Fachabteilung").Value = obj.Property("Fachabteilung").Value;
                obj.Property("FachabteilungsID").Value = obj.Property("FachabteilungsID").Value;

            }
        }

        private static void ParseGetCovidPat(JArray array)
        {
            foreach (JObject obj in array)
            {
                obj.Property("PatientID").Value = obj.Property("PatientID").Value;
                obj.Property("Fallkennung").Value = obj.Property("Fallkennung").Value;
                obj.Property("Zeitpunkt_des_Probeneingangs").Value = obj.Property("Zeitpunkt_des_Probeneingangs").Value;
           }
        }

        private static void ParseSymptomeData(JArray array, PatientInfos info)
        {
            foreach (JObject obj in array)
            {
                if (info.NameDesSymptoms == null)
                {
                    obj.Property("NameDesSymptoms").Value = obj.Property("NameDesSymptoms").Value;
                    obj.Property("Anzahl_Patienten").Value = obj.Property("Anzahl_Patienten").Value;
                }
                else if (info.EHR_ID != null)
                {
                    obj.Add(new JProperty("PatientID", info.EHR_ID));
                    obj.Add(new JProperty("Beginn", info.Beginn));
                    obj.Property("Rueckgang").Value = DateTime.Parse(obj.Property("Rueckgang").Value.ToString());
                    obj.Property("NameDesSymptoms").Value = obj.Property("NameDesSymptoms").Value;
                }
                else
                {
                    obj.Add(new JProperty("NameDesSymptoms", info.NameDesSymptoms));
                    obj.Property("Beginn").Value = DateTime.Parse(obj.Property("Beginn").Value.ToString());
                    obj.Property("Rueckgang").Value = DateTime.Parse(obj.Property("Rueckgang").Value.ToString());
                    obj.Property("PatientID").Value = obj.Property("PatientID").Value;
                }
               
            }
        }
    }
}
