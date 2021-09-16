using Newtonsoft.Json;
using SmICSCoreLib.Factories.PatientStay.Stationary.ReceiveModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmICSCoreLib.Factories.PatientStay.Stationary
{
    public class StationaryDataModel
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

        public StationaryDataModel() { }
        public StationaryDataModel(StationaryDataReceiveModel stationaryDataReceiveModel)
        {

            FallID = stationaryDataReceiveModel.FallID;
            PatientID = stationaryDataReceiveModel.PatientID;
            Versorgungsfallgrund = stationaryDataReceiveModel.Versorgungsfallgrund;
            Aufnahmeanlass = stationaryDataReceiveModel.Aufnahmeanlass;
            Datum_Uhrzeit_der_Aufnahme = stationaryDataReceiveModel.Datum_Uhrzeit_der_Aufnahme;
            Art_der_Entlassung = stationaryDataReceiveModel.Art_der_Entlassung;
            Datum_Uhrzeit_der_Entlassung = stationaryDataReceiveModel.Datum_Uhrzeit_der_Entlassung;
            Station = stationaryDataReceiveModel.Station;
        }

    }
}
