
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SmICSCoreLib.Factories.DetectionAlgorithmInterface
{
    public class DetectionAlgorithmResultModel
    {
        [JsonProperty(PropertyName = "Datum")]
        public DateTime Datum { get; set; }
        [JsonProperty(PropertyName = "Ausbruchswahrscheinlichkeit")]
        public double? Ausbruchswahrscheinlichkeit { get; set; }
        [JsonProperty(PropertyName = "p-Value")]
        public double? PValue { get; set; }
        [JsonProperty(PropertyName = "Erregeranzahl")]
        public int? Erregeranzahl { get; set; }
        [JsonProperty(PropertyName = "Endemisches Niveau")]
        public double? EndemischesNiveau { get; set; }
        [JsonProperty(PropertyName = "Epidemisches Niveau")]
        public double? EpidemischesNiveau { get; set; }
        [JsonProperty(PropertyName = "Obergrenze")]
        public int? Obergrenze { get; set; }
        [JsonProperty(PropertyName = "Faelle unter der Obergrenze")]
        public int? FaelleUnterObergrenze { get; set; }
        [JsonProperty(PropertyName = "Faelle ueber der Obergrenze")]
        public int? FaelleUeberObergrenze { get; set; }
        [JsonProperty(PropertyName = "Klassifikation der Alarmfaelle")]
        public string? KlassifikationAlarmfaelle { get; set; }
        [JsonProperty(PropertyName = "Algorithmusergebnis enthaelt keine null-Werte")]
        public bool KeineNullWerte { get; set; }

        public List<DetectionAlgorithmResultModel> ListFromTable(JObject detectionAlgorithmResultTableJson, int lengthOfTable)
        {
            List<DetectionAlgorithmResultModel> outputListWithResultElements = new List<DetectionAlgorithmResultModel>();

            for (int o = 0; o < lengthOfTable; o++)
            {
                outputListWithResultElements.Add(
                new DetectionAlgorithmResultModel
                {
                    Datum                       = (DateTime) detectionAlgorithmResultTableJson["Zeitstempel"][o],
                    Ausbruchswahrscheinlichkeit = (double) detectionAlgorithmResultTableJson["Ausbruchswahrscheinlichkeit"][o],
                    PValue                      = (double) detectionAlgorithmResultTableJson["p-Value"][o],
                    Erregeranzahl               = (int) detectionAlgorithmResultTableJson["Erregeranzahl"][o],
                    EndemischesNiveau           = (double) detectionAlgorithmResultTableJson["Endemisches Niveau"][o],
                    EpidemischesNiveau          = (double) detectionAlgorithmResultTableJson["Epidemisches Niveau"][o],
                    Obergrenze                  = (int) detectionAlgorithmResultTableJson["Obergrenze"][o],
                    FaelleUnterObergrenze       = (int) detectionAlgorithmResultTableJson["Faelle unter der Obergrenze"][o],
                    FaelleUeberObergrenze       = (int) detectionAlgorithmResultTableJson["Faelle ueber der Obergrenze"][o],
                    KlassifikationAlarmfaelle   = (string) detectionAlgorithmResultTableJson["Klassifikation der Alarmfaelle"][o]
                });
            }

            return outputListWithResultElements;
        }
    }
}
