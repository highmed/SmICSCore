using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using SmICSCoreLib.AQL.Contact_Nth_Network.ReceiveModels;
using SmICSCoreLib.Util;
using Newtonsoft.Json;

namespace SmICSCoreLib.AQL.Contact_Nth_Network
{
    public class ContactModel
    {
        [JsonProperty(PropertyName = "paID")]
        public string paID { get; set; }
        [JsonProperty(PropertyName = "pbID")]
        public string pbID { get; set; }
        [JsonProperty(PropertyName = "Grad")]
        public int Grad { get; set; }
        [JsonProperty(PropertyName = "Beginn")]
        public DateTime Beginn { get; set; }
        [JsonProperty(PropertyName = "Ende")]
        public DateTime Ende { get; set; }
        [JsonProperty(PropertyName = "StationID")]
        public string StationID { get; set; }

        public ContactModel() { }
        public ContactModel(ContactPatientModel ContactPatient, PatientWardModel PatientWard, ContactParameter Parameter, int Degree)
        {
            paID = Parameter.PatientID;
            pbID = ContactPatient.PatientID;
            if (ContactPatient.Beginn > PatientWard.Beginn)
            {
                Beginn = ContactPatient.Beginn;
            } else
            {
                Beginn = PatientWard.Beginn;
            }

            if (ContactPatient.Ende < PatientWard.Ende && ContactPatient.Ende.HasValue)
            {
                Ende = ContactPatient.Ende?? DateTime.Now;
            }
            else
            {
                Ende = PatientWard.Ende;
            }
            Grad = Degree;
            StationID = PatientWard.StationID;

        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(ContactModel))
            {
                ContactModel contactModel = obj as ContactModel;
                return (paID.Equals(contactModel.paID)
                    && pbID.Equals(contactModel.pbID)
                    && Beginn.Equals(contactModel.Beginn)
                    && Ende.Equals(contactModel.Ende)
                    && StationID.Equals(contactModel.StationID)
                    && Grad.Equals(contactModel.Grad));
            }
            return false;
        }
        
    }
}
