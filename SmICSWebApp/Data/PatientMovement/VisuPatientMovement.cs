using Newtonsoft.Json;
using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.PatientMovementNew;
using SmICSCoreLib.Factories.PatientMovementNew.PatientStays;
using System;

namespace SmICSWebApp.Data.PatientMovement
{
    public class VisuPatientMovement : Case
    {
        [JsonProperty("Beginn")]
        public DateTime Admission { get; internal set; }
        [JsonProperty("Ende")]
        public DateTime Discharge { get; internal set; }
        [JsonProperty("Raum")]
        public string Room { get; internal set; }
        [JsonProperty("Station")]
        public string Ward { get; internal set; }
        [JsonProperty("Fachabteilung")]
        public string Departement { get; internal set; }
        [JsonProperty("FachabteilungsID")]
        public string DepartementID { get; internal set; }
        [JsonProperty("BewegungstypID")]
        public int MovementTypeID { get; internal set; }
        [JsonProperty("Bewegungstyp")]
        public string MovementType { get; internal set; }

        public VisuPatientMovement(Admission admission, PatientStay patientStay)
        {
            InitializeBasic(patientStay);
            Admission = admission.Date;
            Discharge = admission.Date;
            MovementTypeID = (int)admission.MovementTypeID;
            MovementType = admission.MovementTypeName;
        }

        public VisuPatientMovement(PatientStay patientStay)
        {
            InitializeBasic(patientStay);
            Admission = patientStay.Admission;
            Discharge = patientStay.Discharge.HasValue ? patientStay.Discharge.Value : DateTime.Now;
            MovementTypeID = (int)patientStay.MovementType;
            MovementType = patientStay.MovementTypeName;
        }

        public VisuPatientMovement(Discharge discharge, PatientStay patientStay)
        {
            InitializeBasic(patientStay);
            Admission = discharge.Date.Value;
            Discharge = discharge.Date.Value;
            MovementTypeID = (int)discharge.MovementTypeID;
            MovementType = discharge.MovementTypeName;
        }

        private void InitializeBasic(PatientStay patientStay)
        {
            PatientID = patientStay.PatientID;
            CaseID = patientStay.CaseID;
            Room = patientStay.Room;
            Ward = string.IsNullOrEmpty(patientStay.Ward) ? patientStay.Departement : patientStay.Ward;
            Departement = patientStay.Departement;
            DepartementID = patientStay.DepartementID;
        }
    }
}
