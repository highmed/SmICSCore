using Newtonsoft.Json.Linq;
using SmICSCoreLib.AQL.General;
using SmICSCoreLib.AQL.PatientInformation.Patient_Labordaten;
using SmICSCoreLib.AQL.PatientInformation.Patient_Mibi_Labordaten;
using SmICSCoreLib.AQL.PatientInformation.PatientMovement;
using SmICSCoreLib.AQL.PatientInformation.Symptome;
using SmICSCoreLib.AQL.PatientInformation.Vaccination;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SmICSCoreLib.AQL.PatientInformation
{
    public interface IPatientInformation
    {
        List<LabDataModel> Patient_Labordaten_Ps(PatientListParameter parameter);
        List<PatientMovementModel> Patient_Bewegung_Ps(PatientListParameter parameter);
        List<PatientMovementModel> Patient_Bewegung_Station(PatientListParameter parameter, string station, DateTime starttime, DateTime endtime);
        List<MibiLabDataModel> MibiLabData(PatientListParameter parameter);
        List<SymptomModel> Patient_Symptom(PatientListParameter parameter);
        List<SymptomModel> Patient_Symptom();
        List<SymptomModel> Patient_By_Symptom(string symptom);
        List<SymptomModel> Symptoms_By_PatientId(string patientId, DateTime datum);
        List<VaccinationModel> Patient_Vaccination(PatientListParameter parameter);
        List<PatientMovementModel> All_Stations();
        List<LabDataModel> ViroErreger(string name);
        List<MibiLabDataModel> MikroErreger(string name);
    }
}
