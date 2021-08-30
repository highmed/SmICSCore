using Newtonsoft.Json.Linq;
using SmICSCoreLib.AQL.General;
using SmICSCoreLib.AQL.PatientInformation.Infection_situation;
using SmICSCoreLib.AQL.PatientInformation.Patient_Bewegung;
using SmICSCoreLib.AQL.PatientInformation.Patient_Labordaten;
using SmICSCoreLib.AQL.PatientInformation.Patient_Mibi_Labordaten;
using SmICSCoreLib.AQL.PatientInformation.PatientMovement;
using SmICSCoreLib.AQL.PatientInformation.Symptome;
using SmICSCoreLib.AQL.PatientInformation.Vaccination;
using SmICSCoreLib.StatistikDataModels;
using SmICSCoreLib.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SmICSCoreLib.AQL.PatientInformation
{
    public class PatientInformation : IPatientInformation
    {
        private IPatientMovementFactory _patMoveFac;
        private IPatientLabordataFactory _patLabFac;
        private IMibiPatientLaborDataFactory _mibiLabFac;
        private ISymptomFactory _symptomFac;
        private IVaccinationFactory _vaccFac;
        private IInfectionSituationFactory _infecFac;

        public PatientInformation(IPatientMovementFactory patMoveFac, IPatientLabordataFactory patLabFac, ISymptomFactory symptomFac, 
            IMibiPatientLaborDataFactory mibiLabFac, IVaccinationFactory vaccFac, IInfectionSituationFactory infecFac) 
        {
            _patMoveFac = patMoveFac;
            _patLabFac = patLabFac;
            _mibiLabFac = mibiLabFac;
            _symptomFac = symptomFac;
            _vaccFac = vaccFac;
            _infecFac = infecFac;
        }
       
        public List<PatientMovementModel> Patient_Bewegung_Ps(PatientListParameter parameter)
        {
            return _patMoveFac.Process(parameter);
        } 
        
        public List<PatientMovementModel> Patient_Bewegung_Station(PatientListParameter parameter, string station, DateTime starttime, DateTime endtime)
        {
            return _patMoveFac.ProcessFromStation(parameter, station, starttime, endtime);
        }
        
        public List<LabDataModel> Patient_Labordaten_Ps(PatientListParameter parameter)
        {
            return _patLabFac.Process(parameter);
        }
        
        public List<MibiLabDataModel> MibiLabData(PatientListParameter parameter)
        {
            return _mibiLabFac.Process(parameter);
        }
        
        public List<SymptomModel> Patient_Symptom(PatientListParameter parameter)
        {
            return _symptomFac.Process(parameter);
        }
        
        public List<SymptomModel> Patient_Symptom()
        {
            return _symptomFac.ProcessNoParam();
        }
        
        public List<SymptomModel> Patient_By_Symptom(string symptom)
        {
            return _symptomFac.PatientBySymptom(symptom);
        }

        public List<SymptomModel> Symptoms_By_PatientId(string patientId, DateTime datum)
        {
            return _symptomFac.SymptomByPatient(patientId, datum);
        }

        public List<VaccinationModel> Patient_Vaccination(PatientListParameter parameter)
        {
            return _vaccFac.Process(parameter);
        }

        public List<Patient> Infection_Situation(PatientListParameter parameter)
        {
            return _infecFac.Process(parameter);
        }
    }
}
