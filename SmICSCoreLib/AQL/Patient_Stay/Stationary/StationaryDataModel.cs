using Newtonsoft.Json;
using SmICSCoreLib.AQL.Patient_Stay.Stationary.ReceiveModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmICSCoreLib.AQL.Patient_Stay.Stationary
{
    public class StationaryDataModel
    {
        [JsonProperty(PropertyName = "FallID")]
        public string FallID { get; set; }

        [JsonProperty(PropertyName = "PatientID")]
        public string PatientID { get; set; }

        [JsonProperty(PropertyName = "Versorgungsfallgrund")]
        public string Versorgungsfallgrund { get; set; }

        [JsonProperty(PropertyName = "Art_der_Aufnahme")]
        public string Art_der_Aufnahme { get; set; }

        [JsonProperty(PropertyName = "Datum_Uhrzeit_der_Aufnahme")]
        public DateTime Datum_Uhrzeit_der_Aufnahme { get; set; }

        [JsonProperty(PropertyName = "Art_der_Entlassung")]
        public string Art_der_Entlassung { get; set; }

        [JsonProperty(PropertyName = "Datum_Uhrzeit_der_Entlassung")]
        public DateTime Datum_Uhrzeit_der_Entlassung { get; set; }

        public StationaryDataModel() { }
        public StationaryDataModel(StationaryDataReceiveModel stationaryDataReceiveModel)
        {

            FallID = stationaryDataReceiveModel.FallID;
            PatientID = stationaryDataReceiveModel.PatientID;
            Versorgungsfallgrund = stationaryDataReceiveModel.Versorgungsfallgrund;
            Art_der_Aufnahme = stationaryDataReceiveModel.Art_der_Aufnahme;
            Datum_Uhrzeit_der_Aufnahme = stationaryDataReceiveModel.Datum_Uhrzeit_der_Aufnahme;
            Art_der_Entlassung = stationaryDataReceiveModel.Art_der_Entlassung;
            Datum_Uhrzeit_der_Entlassung = stationaryDataReceiveModel.Datum_Uhrzeit_der_Entlassung;
        }

    }
}
