using Newtonsoft.Json.Linq;
using SmICSCoreLib.OutbreakDetection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace SmICSCoreLib.JSONFileStream
{
    public class JSONReader<T> where T : new()
    {
        public static List<T> Read(string path)
        {
            using (StreamReader reader = new StreamReader(path))
            {
                string json = reader.ReadToEnd();
                return JsonSerializer.Deserialize<List<T>>(json);              
            }
        }

        public static T ReadObject(string path)
        {
            using (StreamReader reader = new StreamReader(path))
            {
                string json = reader.ReadToEnd();
                T obj = JsonSerializer.Deserialize<T>(json);
                return obj;
            }
        }

        public static T ReadSingle(string path)
        {
            using (StreamReader reader = new StreamReader(path))
            {
                string json = reader.ReadToEnd();
                return JsonSerializer.Deserialize<T>(json);

            }
        }

        public static List<OutbreakDetectionStoringModel> ReadOutbreakDetectionResult(string path)
        {
            List<OutbreakDetectionStoringModel> outbreakDetectionStorings = new List<OutbreakDetectionStoringModel>();
            using (StreamReader reader = new StreamReader(path))
            {
                string json = reader.ReadToEnd();
                JObject obj = Newtonsoft.Json.JsonConvert.DeserializeObject<JObject>(json);
                for (int i = 0; i < obj.Property("Zeitstempel").Count; i += 1)
                {
                    outbreakDetectionStorings.Add(
                    new OutbreakDetectionStoringModel
                    {
                        Date = DateTime.Parse(obj["Zeitstempel"][i].ToString()),
                        Probability = (double?)obj["Ausbruchswahrscheinlichkeit"][i],
                        pValue = (double?)obj["p-Value"][i],
                        PathogenCount = (int?)obj["Erregeranzahl"][i],
                        EndemicNiveau = (double?)obj["Endemisches Niveau"][i],
                        EpidemicNiveau = (double?)obj["Epidemisches Niveau"][i],
                        UpperBounds = (int?)obj["Obergrenze"][i],
                        CasesBelowUpperBounds = (int?)obj["Faelle unter der Obergrenze"][i],
                        CasesAboveUpperBounds = (int?)obj["Faelle ueber der Obergrenze"][i],
                        AlarmClassification = (string?)obj["Klassifikation der Alarmfaelle"][i],
                        HasNullValues = (bool)obj["Algorithmusergebnis enthaelt keine null-Werte"][0]
                    });
                }
            }
            return outbreakDetectionStorings;
        }
    }
}
