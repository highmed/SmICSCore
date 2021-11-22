using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmICSCoreLib.Factories.PatientStay.Stationary.ReceiveModel
{
    public class StationaryDataReceiveModel
    {
        [JsonProperty(PropertyName = "FallID")]
        public string FallID { get; set; }

        [JsonProperty(PropertyName = "PatientID")]
        public string PatientID { get; set; }

        [JsonProperty(PropertyName = "Versorgungsfallgrund")]
        public string Versorgungsfallgrund { get; set; }

        [JsonProperty(PropertyName = "Aufnahmeanlass")]
        public string Aufnahmeanlass { get; set; }

        [JsonProperty(PropertyName = "Datum_Uhrzeit_der_Aufnahme")]
        public DateTime Datum_Uhrzeit_der_Aufnahme { get; set; }

        [JsonProperty(PropertyName = "Art_der_Entlassung")]
        public string Art_der_Entlassung { get; set; }

        [JsonProperty(PropertyName = "Datum_Uhrzeit_der_Entlassung")]
        public DateTime Datum_Uhrzeit_der_Entlassung { get; set; }

        [JsonProperty(PropertyName = "Station")]
        public string Station { get; set; }
    }
}
