using System;
using System.Collections.Generic;
using System.Linq;
using SmICSCoreLib.AQL.PatientInformation;
using SmICSCoreLib.AQL.Patient_Stay;
using SmICSCoreLib.AQL.Patient_Stay.Stationary;
using SmICSCoreLib.AQL.PatientInformation.PatientMovement;
using SmICSCoreLib.AQL.PatientInformation.Symptome;
using SmICSCoreLib.AQL.General;


namespace SmICSWebApp.Data
{
    public class Symptom
    {
        private readonly IPatientInformation _patientInformation;
        private readonly IPatinet_Stay _paient_Stay;

        public Symptom(IPatientInformation patientInformation, IPatinet_Stay paient_Stay)
        {
            _patientInformation = patientInformation;
            _paient_Stay = paient_Stay;
        }

        public List<SymptomModel> GetAllSymptom()
        {
            try
            {
                List<SymptomModel> symptomListe = _patientInformation.Patient_Symptom();
                return symptomListe;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<SymptomModel> GetAllPatBySym(string symptom, DateTime datum)
        {
            List<SymptomModel> symptomListe = _patientInformation.Patient_By_Symptom(symptom);

            List<SymptomModel> symListe = new List<SymptomModel>();
            foreach (var item in symptomListe)
            {
                if (item.Beginn > datum)
                {
                    symListe.Add(item);
                }
            }
            return symListe;
        }

        public List<SymptomModel> GetAllPatBySta(string symptom, DateTime datum, string station)
        {
            List<SymptomModel> patientListe = GetAllPatBySym(symptom, datum);
            List<PatientMovementModel> patientMovement = new();
            List<SymptomModel> symListe = new();
            foreach (var item in patientListe)
            {
                patientMovement = GetPatMovement(item.PatientenID);
                foreach (var movment in patientMovement)
                {
                    if (movment.Fachabteilung == station)
                    {
                        if (!symListe.Contains(item))
                        {
                            symListe.Add(item);
                        }
                    }
                }
            }
            return symListe;
        }

        public List<PatientMovementModel> GetPatMovement(string patientId)
        {
            List<string> patientList = new List<string>();
            patientList.Add(patientId);
            PatientListParameter patListParameter = new();
            patListParameter.patientList = patientList;
            List<PatientMovementModel> patientMovement = _patientInformation.Patient_Bewegung_Ps(patListParameter);
            return patientMovement;
        }


        public Dictionary<string, int> GetSymGroup(string symptom, DateTime datum)
        {
            List<PatientMovementModel> patBewegungen = new List<PatientMovementModel>();
            List<SymptomModel> patinetList = GetAllPatBySym(symptom, datum);

            foreach (var patient in patinetList)
            {
                List<PatientMovementModel> patBewegung = GetPatMovement(patient.PatientenID);
                if (patBewegung.Count != 0)
                {
                    patBewegungen.Add(patBewegung.First());
                }
            }

            var result = patBewegungen.GroupBy(x => x.Fachabteilung).Select(x => new { Fachabteilung = x.Key, Count = x.Count() }).ToList();

            Dictionary<string, int> finalList = new Dictionary<string, int>();
            foreach (var item in result)
            {
                if (item.Count >= 3)
                {
                    finalList.Add(item.Fachabteilung, item.Count);
                }
            }
            return finalList;
        }

        public Dictionary<string, Dictionary<string, int>> GetAllSymGroup(DateTime datum)
        {
            Dictionary<string, Dictionary<string, int>> allSymGroup = new Dictionary<string, Dictionary<string, int>>();
            List<SymptomModel> symptomListe = GetAllSymptom();
            if (symptomListe != null)
            {
                foreach (var item in symptomListe)
                {
                    Dictionary<string, int> symGroup = GetSymGroup(item.NameDesSymptoms, datum);
                    if (symGroup.Count != 0)
                    {
                        allSymGroup.Add(item.NameDesSymptoms, symGroup);
                    }
                }
                return allSymGroup;
            }
            else
            {
                return null;
            }
        }

        public List<StationaryDataModel> GetPatStationary(string patientId, string fallId)
        {
            List<StationaryDataModel> pathStationary = _paient_Stay.StayFromCase(patientId, fallId);
            return pathStationary;
        }
    }
}
