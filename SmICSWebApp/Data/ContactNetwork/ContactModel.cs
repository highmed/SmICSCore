using Newtonsoft.Json;
using SmICSWebApp.Data.MedicalFinding;
using SmICSWebApp.Data.PatientMovement;
using System.Collections.Generic;

namespace SmICSWebApp.Data.ContactNetwork
{
    public class ContactModel
    {
        public ContactModel()
        {
        }
        [JsonProperty(PropertyName = "Patienten_Bewegungen")]
        public List<VisuPatientMovement> PatientMovements { get; set; }
        [JsonProperty(PropertyName = "Labordaten")]
        public List<VisuLabResult> LaborData { get; set; }
    }
}