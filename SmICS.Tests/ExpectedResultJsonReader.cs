using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmICSDataGenerator.Tests;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SmICSFactory.Tests
{
    public class ExpectedResultJsonReader
    {
        public static List<T> ReadResults<T, U>(string testResultPath, string parameterPath, int resultNo, int ehrNo, ExpectedType type) where U : new()
        {
            List<U> patients = SmICSCoreLib.JSONFileStream.JSONReader<U>.Read(parameterPath);

            using (StreamReader reader = new StreamReader(testResultPath, System.Text.Encoding.GetEncoding("ISO-8859-1")))
            {
                string json = reader.ReadToEnd();
                JObject jObject = JsonConvert.DeserializeObject<JObject>(json);
                JArray arr = jObject.Property("" + resultNo).Value as JArray;

                switch (type)
                {
                    case ExpectedType.PATIENT_MOVEMENT:
                        if (patients[ehrNo].GetType() == typeof(PatientIDs))
                        {
                            ParsePatientMovement(arr, patients[ehrNo] as PatientIDs);
                        }
                        break;
                    case ExpectedType.LAB_DATA:
                        if (patients[ehrNo].GetType() == typeof(PatientIDs))
                        {
                            ParseLabData(arr, patients[ehrNo] as PatientIDs);
                        }
                        break;
                    case ExpectedType.STATIONARY:
                        if (patients[ehrNo].GetType() == typeof(PatientInfos))
                        {
                            ParseStationaryPatData(arr, patients[ehrNo] as PatientInfos);
                        }
                        break;
                    case ExpectedType.PATIENT_SYMPTOM:
                        if (patients[ehrNo].GetType() == typeof(PatientIDs))
                        {
                            ParsePatientSymptom(arr, patients[ehrNo] as PatientIDs);
                        }
                        break;
                    case ExpectedType.PATIENT_VACCINATION:
                        if (patients[ehrNo].GetType() == typeof(PatientIDs))
                        {
                            ParsePatientVaccination(arr, patients[ehrNo] as PatientIDs);
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

        private static void ParsePatientSymptom(JArray array, PatientIDs id)
        {
            foreach (JObject obj in array)
            {
                obj.Add(new JProperty("PatientenID", id.EHR_ID));
                obj.Property("BefundDatum").Value = DateTime.Parse(obj.Property("BefundDatum").Value.ToString());
                obj.Property("Beginn").Value = DateTime.Parse(obj.Property("Beginn").Value.ToString());

                if (obj.Property("Rueckgang").Value.Type == JTokenType.Null)
                {
                    obj.Property("Rueckgang").Value = DateTime.Now;
                }
                else
                {
                    obj.Property("Rueckgang").Value = DateTime.Parse(obj.Property("Rueckgang").Value.ToString());
                }
            }
        }

        private static void ParsePatientVaccination(JArray array, PatientIDs id)
        {
            foreach (JObject obj in array)
            {
                obj.Add(new JProperty("PatientenID", id.EHR_ID));
                obj.Property("DokumentationsID").Value = DateTime.Parse(obj.Property("DokumentationsID").Value.ToString());
            }
        }

        private static void ParseStationaryPatData(JArray array, PatientInfos info)
        {
            foreach (JObject obj in array)
            {
                obj.Add(new JProperty("PatientID", info.EHR_ID));
                obj.Add(new JProperty("FallID", info.FallID));
                obj.Property("Datum_Uhrzeit_der_Aufnahme").Value = DateTime.Parse(obj.Property("Datum_Uhrzeit_der_Aufnahme").Value.ToString());
                obj.Property("Datum_Uhrzeit_der_Entlassung").Value = DateTime.Parse(obj.Property("Datum_Uhrzeit_der_Entlassung").Value.ToString());
                obj.Property("Aufnahmeanlass").Value = obj.Property("Aufnahmeanlass").Value;
                obj.Property("Art_der_Entlassung").Value = obj.Property("Art_der_Entlassung").Value;
                obj.Property("Versorgungsfallgrund").Value = obj.Property("Versorgungsfallgrund").Value;
                obj.Property("Station").Value = obj.Property("Station").Value;

            }
        }
    }
}
