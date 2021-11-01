using Newtonsoft.Json;
using System;

namespace SmICSCoreLib.OutbreakDetection
{
    public class OutbreakDetectionStoringModel
    {
        [JsonProperty("Zeitstempel")]
        public DateTime Date { get; set; }

        [JsonProperty("Ausbruchswahrscheinlichkeit")]
        public double? Probability { get; set; }

        [JsonProperty("pValue")]
        public double? pValue { get; set; }

        [JsonProperty("Erregeranzahl")]
        public int? PathogenCount { get; set; }

        [JsonProperty("Endemisches Niveau")]
        public double? EndemicNiveau { get; set; }

        [JsonProperty("Epidemisches Nivea")]
        public double? EpidemicNiveau { get; set; }

        [JsonProperty("Obergrenze")]
        public int? UpperBounds { get; set; }

        [JsonProperty("Faelle unter der Obergrenze")]
        public int? CasesBelowUpperBounds { get; set; }

        [JsonProperty("Faelle ueber der Obergrenze")]
        public int? CasesAboveUpperBounds { get; set; }

        [JsonProperty("Klassifikation der Alarmfaelle")]
        public string? AlarmClassification { get; set; }

        [JsonProperty("Algorithmusergebnis enthaelt keine null-Werte")]
        public bool HasNoNullValues { get; set; }

        [JsonProperty("Keine Warnungen zum Datenpunkt")]
        public bool HasWarnings { get; set; }
    }
}
