using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Newtonsoft.Json;

namespace SmICSCoreLib.Factories.EpiCurve
{
    public class EpiCurveModel
    {
        [JsonProperty(PropertyName = "Datum")]
        public DateTime Datum { get; set; }
        [JsonProperty(PropertyName = "ErregerID")]
        public string ErregerID { get; set; }
        [JsonProperty(PropertyName = "ErregerBEZK")]
        public string ErregerBEZK { get; set; }
        [JsonProperty(PropertyName = "ErregerBEZL")]
        public string ErregerBEZL { get; set; } = " ";
        [JsonProperty(PropertyName = "Anzahl")]
        public int Anzahl { get; set; }
        [JsonProperty(PropertyName = "Anzahl_cs")]
        public int Anzahl_cs { get; set; }
        [JsonProperty(PropertyName = "MAVG7")]
        public int MAVG7 { get; set; }
        [JsonProperty(PropertyName = "MAVG28")]
        public int MAVG28 { get; set; }
        [JsonProperty(PropertyName = "MAVG7_cs")]
        [DefaultValue(0)]
        public int MAVG7_cs { get; set; }
        [JsonProperty(PropertyName = "MAVG28_cs")]
        [DefaultValue(0)]
        public int MAVG28_cs { get; set; }
        [JsonProperty(PropertyName = "StationID")]
        public string StationID { get; set; }
        [JsonProperty(PropertyName = "anzahl_gesamt")]
        public int anzahl_gesamt { get; set; }
        [JsonProperty(PropertyName = "anzahl_gesamt_av7")]
        public int anzahl_gesamt_av7 { get; set; }
        [JsonProperty(PropertyName = "anzahl_gesamt_av28")]
        public int anzahl_gesamt_av28 { get; set; }
    }
}
