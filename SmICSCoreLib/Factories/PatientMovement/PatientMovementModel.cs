using SmICSCoreLib.Factories.PatientMovement.ReceiveModels;
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace SmICSCoreLib.Factories.PatientMovement
{
    public class PatientMovementModel
    {
        [JsonProperty(PropertyName = "PatientID")]
        public string PatientID { get; set; }
        [JsonProperty(PropertyName = "Beginn")]
        public DateTime Beginn { get; set; }
        [JsonProperty(PropertyName = "Ende")]
        public DateTime Ende { get; set; }
        [JsonProperty(PropertyName = "Bewegungstyp")]
        public string Bewegungstyp { get; set; }
        [JsonProperty(PropertyName = "BewegungstypID")]
        public int BewegungstypID { get; set; }
        [JsonProperty(PropertyName = "FallID")]
        public string FallID { get; set; }
        [JsonProperty(PropertyName = "Raum")]
        public string Raum { get; set; }
        [JsonProperty(PropertyName = "Bewegungsart_l")]
        public string Bewegungsart_l { get; set; }
        [JsonProperty(PropertyName = "StationID")]
        public string StationID { get; set; }

        [JsonProperty(PropertyName = "Fachabteilung")]
        public string Fachabteilung { get; set; } 
        
        [JsonProperty(PropertyName = "FachabteilungsID")]
        public string FachabteilungsID { get; set; }

        public PatientMovementModel() { }
        public PatientMovementModel(PatientStayModel patientStay)
        {
            PatientID = patientStay.PatientID;
            Beginn = patientStay.Beginn;
            Ende = patientStay.Ende;
            Raum = patientStay.Raum;
            FallID = patientStay.FallID;
            Bewegungsart_l = patientStay.Bewegungsart_l;
            StationID = patientStay.StationID == null ? patientStay.FachabteilungsID : patientStay.StationID;
            Fachabteilung = patientStay.Fachabteilung;
            FachabteilungsID = patientStay.FachabteilungsID;
        }

        public void AddMovementType(int typeID, string typeName)
        {
           BewegungstypID = typeID;
           Bewegungstyp = typeName;
        }

	// override object.Equals
        public override bool Equals(object obj)
        {
            //
            // See the full list of guidelines at
            //   http://go.microsoft.com/fwlink/?LinkID=85237
            // and also the guidance for operator== at
            //   http://go.microsoft.com/fwlink/?LinkId=85238
            //

            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            PatientMovementModel other = (PatientMovementModel) obj;
            return JsonConvert.SerializeObject(this).Equals(JsonConvert.SerializeObject(other));
        }

	public override int GetHashCode()
        {
            return JsonConvert.SerializeObject(this).GetHashCode();
        }
    }
}
