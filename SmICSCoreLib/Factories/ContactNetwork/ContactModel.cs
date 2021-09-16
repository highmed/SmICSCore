using System;
using System.Collections;
using System.Collections.Generic;
using SmICSCoreLib.Factories.PatientMovement;
using SmICSCoreLib.Factories.Lab.ViroLabData;
using Newtonsoft.Json;

namespace SmICSCoreLib.Factories.Contact_Nth_Network
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
