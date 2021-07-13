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
            //'patients' can be removed because they have been replaced by 'patientInfos' 
            List<PatientIDs> patients = SmICSCoreLib.JSONFileStream.JSONReader<PatientIDs>.Read(@"../../../../WebApp.Test/Resources/EHRID_StayFromCase.json");
            List<PatientInfos> patientInfos = SmICSCoreLib.JSONFileStream.JSONReader<PatientInfos>.Read(@"../../../../WebApp.Test/Resources/EHRID_StayFromCase.json");

            using (StreamReader reader = new StreamReader(path, System.Text.Encoding.GetEncoding("ISO-8859-1")))
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
                    case ExpectedType.STATIONARY:
                        ParseStationaryPatData(arr, patients[resultNo].EHR_ID, patientInfos[resultNo].FallID);
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

        private static void ParseStationaryPatData(JArray array, string ehr, string fallID)
        {
            foreach (JObject obj in array)
            {
                obj.Add(new JProperty("PatientID", ehr));
                obj.Add(new JProperty("FallID", fallID));
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
