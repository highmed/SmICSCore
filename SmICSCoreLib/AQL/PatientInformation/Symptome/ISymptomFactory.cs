using SmICSCoreLib.AQL.General;
using System;
using System.Collections.Generic;

namespace SmICSCoreLib.AQL.PatientInformation.Symptome
{
    public interface ISymptomFactory
    {
        List<SymptomModel> Process(PatientListParameter parameter);
        List<SymptomModel> ProcessNoParam();
        List<SymptomModel> PatientBySymptom(string symptom);
        List<SymptomModel> SymptomByPatient(string patientId, DateTime datum);

    }
}