using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmICSDataGenerator.Tests;
using System;
using System.Collections.Generic;
using System.IO;


namespace SmICSFactory.Tests
{
    public class ExpectedResultJsonReader
    {
        public static List<T> ReadResults<T>(string path, int resultNo, ExpectedType type)
        {
            List<PatientIDs> patients = SmICSCoreLib.JSONFileStream.JSONReader<PatientIDs>.Read(@"../../../../SmICSDataGenerator.Test/Resources/GeneratedEHRIDs.json");

            using (StreamReader reader = new StreamReader(path))
            {
                string json = reader.ReadToEnd();
                JObject jObject = JsonConvert.DeserializeObject<JObject>(json);
                JArray arr = jObject.Property("" + resultNo).Value as JArray;

                switch (type)
                {
                    case ExpectedType.PATIENT_MOVEMENT:
                        ParsePatientMovement(arr, patients[resultNo].EHR_ID);
                        break;
                    case ExpectedType.LAB_DATA:
                        ParseLabData(arr, patients[resultNo].EHR_ID);
                        break;
                    case ExpectedType.PATIENT_SYMPTOM:
                        ParsePatientSymptom(arr, patients[resultNo].EHR_ID);
                        break;
                    case ExpectedType.PATIENT_VACCINATION:
                        ParsePatientVaccination(arr, patients[resultNo].EHR_ID);
                        break;
                }
                
                return arr.ToObject<List<T>>();

            }
        }

        private static void ParsePatientMovement(JArray array, string ehr)
        {
            foreach (JObject obj in array)
            {
                obj.Add(new JProperty("PatientID", ehr));
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

        private static void ParseLabData(JArray array, string ehr)
        {
            foreach (JObject obj in array)
            {
                obj.Add(new JProperty("PatientID", ehr));
                obj.Property("Befunddatum").Value = DateTime.Parse(obj.Property("Befunddatum").Value.ToString());
                obj.Property("ZeitpunktProbeneingang").Value = DateTime.Parse(obj.Property("ZeitpunktProbeneingang").Value.ToString());
                obj.Property("Eingangsdatum").Value = DateTime.Parse(obj.Property("Eingangsdatum").Value.ToString());

            }
        }

        private static void ParsePatientSymptom(JArray array, string ehr)
        {
            foreach (JObject obj in array)
            {
                obj.Add(new JProperty("PatientID", ehr));
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

        private static void ParsePatientVaccination(JArray array, string ehr)
        {
            foreach (JObject obj in array)
            {
                obj.Add(new JProperty("PatientID", ehr));
                obj.Property("DokumentationsID").Value = DateTime.Parse(obj.Property("DokumentationsID").Value.ToString());
            }
        }
    }
}
