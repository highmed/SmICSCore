using Newtonsoft.Json;
using SmICSCoreLib.Factories.PatientStay.Count.ReceiveModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmICSCoreLib.Factories.PatientStay.Count
{
    public class CountDataModel
    {

        [JsonProperty(PropertyName = "PatientID")]
        public string PatientID { get; set; }
        
        [JsonProperty(PropertyName = "Fallkennung")]
        public string Fallkennung { get; set; }

        [JsonProperty(PropertyName = "Zeitpunkt_des_Probeneingangs")]
        public DateTime Zeitpunkt_des_Probeneingangs { get; set; }

        public CountDataModel() { }
        public CountDataModel(CountDataReceiveModel countDataReceiveModel ) 
        {
            PatientID = countDataReceiveModel.PatientID;
            Fallkennung = countDataReceiveModel.Fallkennung;
            Zeitpunkt_des_Probeneingangs = countDataReceiveModel.Zeitpunkt_des_Probeneingangs;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            CountDataModel objAsPart = obj as CountDataModel;
            if (objAsPart == null) return false;
            else return (this.PatientID == objAsPart.PatientID);
        }
    }
}
