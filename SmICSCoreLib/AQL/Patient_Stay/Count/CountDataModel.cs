using Newtonsoft.Json;
using SmICSCoreLib.AQL.Patient_Stay.Count.ReceiveModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmICSCoreLib.AQL.Patient_Stay.Count
{
    public class CountDataModel
    {

        [JsonProperty(PropertyName = "PatientID")]
        public string PatientID { get; set; }
        
        //[JsonProperty(PropertyName = "Fallkennung")]
        //public string Fallkennung { get; set; }

        [JsonProperty(PropertyName = "Zeitpunkt_der_Probenentnahme")]
        public DateTime Zeitpunkt_der_Probenentnahme { get; set; }

        public CountDataModel() { }
        public CountDataModel(CountDataReceiveModel countDataReceiveModel ) 
        {
            PatientID = countDataReceiveModel.PatientID;
            //Fallkennung = countDataReceiveModel.Fallkennung;
            Zeitpunkt_der_Probenentnahme = countDataReceiveModel.Zeitpunkt_der_Probenentnahme;
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
