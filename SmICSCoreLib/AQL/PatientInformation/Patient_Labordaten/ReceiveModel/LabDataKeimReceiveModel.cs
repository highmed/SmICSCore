using Newtonsoft.Json;



namespace SmICSCoreLib.AQL.PatientInformation.Patient_Labordaten.ReceiveModel
{
    public class LabDataKeimReceiveModel
    {
        [JsonProperty(PropertyName = "KeimID")]
        public string KeimID { get; set; }

    }
}
