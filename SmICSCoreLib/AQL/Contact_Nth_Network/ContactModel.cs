using System;
using System.Collections;
using System.Collections.Generic;
using SmICSCoreLib.AQL.PatientInformation.PatientMovement;
using SmICSCoreLib.AQL.PatientInformation.Patient_Labordaten;
using Newtonsoft.Json;

namespace SmICSCoreLib.AQL.Contact_Nth_Network
{
    public class ContactModel
    {
        [JsonProperty(PropertyName = "Patienten_Bewegungen")]
        public List<PatientMovementModel> PatientMovements { get; set; }
        [JsonProperty(PropertyName = "Labordaten")]
        public List<LabDataModel> LaborData { get; set; }
        public ContactModel() { }   
    }
}
