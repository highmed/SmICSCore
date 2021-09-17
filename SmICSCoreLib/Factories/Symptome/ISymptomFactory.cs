using SmICSCoreLib.Factories.General;
using SmICSCoreLib.REST;
using System;
using System.Collections.Generic;

namespace SmICSCoreLib.Factories.Symptome
{
    public interface ISymptomFactory
    {
        IRestDataAccess RestDataAccess { get; }
        List<SymptomModel> Process(PatientListParameter parameter);
        List<SymptomModel> ProcessNoParam();
        List<SymptomModel> PatientBySymptom(string symptom);
        List<SymptomModel> SymptomByPatient(string patientId, DateTime datum);

    }
}