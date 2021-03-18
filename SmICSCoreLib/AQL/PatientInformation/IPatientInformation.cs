using Newtonsoft.Json.Linq;
using SmICSCoreLib.AQL.General;
using SmICSCoreLib.AQL.PatientInformation.Patient_Labordaten;
using SmICSCoreLib.AQL.PatientInformation.Patient_Mibi_Labordaten;
using SmICSCoreLib.AQL.PatientInformation.PatientMovement;
using SmICSCoreLib.AQL.PatientInformation.Symptome;
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
        List<MibiLabDataModel> MibiLabData(PatientListParameter parameter);
        List<SymptomModel> Patient_Symptom_TTPs(PatientListParameter parameter);
    }
}
