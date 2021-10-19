using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace SmICSCoreLib.OutbreakDetection
{
    public class OutbreakDetectionResultModel
    {
        [JsonProperty("Zeitstempel")]
        public List<DateTime> Date { get; set; }

        [JsonProperty("Ausbruchswahrscheinlichkeit")]
        public List<double>? Probability { get; set; }

        [JsonProperty("p-Value")]
        public List<double>? pValue { get; set; }

        [JsonProperty("Erregeranzahl")]
        public List<int>? PathogenCount { get; set; }

        [JsonProperty("Endemisches Niveau")]
        public List<double>? EndemicNiveau { get; set; }

        [JsonProperty("Epidemisches Nivea")]
        public List<double>? EpidemicNiveau { get; set; }

        [JsonProperty("Obergrenze")]
        public List<int>? UpperBounds { get; set; }

        [JsonProperty("Faelle unter der Obergrenze")]
        public List<int>? CasesBelowUpperBounds { get; set; }

        [JsonProperty("Faelle ueber der Obergrenze")]
        public List<int>? CasesAboveUpperBounds { get; set; }

        [JsonProperty("Klassifikation der Alarmfaelle")]
        public List<string>? AlarmClassification { get; set; }

        [JsonProperty("Algorithmusergebnis enthaelt keine null-Werte")]
        public List<bool> HasNullValues { get; set; }

        [JsonProperty("Keine Warnungen zum Datenpunkt")]
        public List<bool> HasWarnings { get; set; }
    }
}
