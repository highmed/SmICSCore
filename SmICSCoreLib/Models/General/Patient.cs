using Newtonsoft.Json;
using System;

namespace SmICSCoreLib.Factories.General
{
    public class Patient : ICloneable
    {
        [JsonProperty(PropertyName = "PatientID")]
        public string PatientID { get; set; }
        public Patient()
        {

        }
        public Patient(Patient patient)
        {
            PatientID = patient.PatientID;
        }

        public bool Equals(Patient other)
        {
            if(other != null)
            {
                return PatientID == other.PatientID;
            }
            return false;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
