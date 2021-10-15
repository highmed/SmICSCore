using Newtonsoft.Json;
using SmICSCoreLib.Factories.Vaccination;
using System;
using System.Collections.Generic;

namespace SmICSCoreLib.StatistikDataModels
{
    public class PatientModel
    {
        [JsonProperty(PropertyName = "PatientID")]
        public string PatientID { get; set; }

        [JsonProperty(PropertyName = "Zeitpunkt_der_Probenentnahme")]
        public DateTime Probenentnahme { get; set; }

        [JsonProperty(PropertyName = "Aufnahme_Datum")]
        public DateTime Aufnahme { get; set; }

        [JsonProperty(PropertyName = "Entlastung_Datum")]
        public DateTime Entlastung { get; set; }

        [JsonProperty(PropertyName = "Infektion")]
        public string Infektion { get; set; }

        [JsonProperty(PropertyName = "VaccinationModel")]
        public List<VaccinationModel> VaccinationModel { get; set; }


        public PatientModel() { }
        public PatientModel(string patientID, DateTime probenentnahme, DateTime aufnahme, DateTime entlastung)
        {
            PatientID = patientID;
            Probenentnahme = probenentnahme;
            Aufnahme = aufnahme;
            Entlastung = entlastung;
        }

        public PatientModel(string patientID, string infektion)
        {
            PatientID = patientID;
            Infektion = infektion;
        }

        public PatientModel(string patientID, DateTime probenentnahme, DateTime aufnahme, DateTime entlastung, string infektion, List<VaccinationModel> vaccinationModel) 
            : this(patientID, probenentnahme, aufnahme, entlastung)
        {
            Infektion = infektion;
            VaccinationModel = vaccinationModel;
        }

        public override bool Equals(object obj)
        {
            return obj is PatientModel patient &&
                   PatientID == patient.PatientID;
        }
    }
}
