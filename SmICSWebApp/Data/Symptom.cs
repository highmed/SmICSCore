using System;
using System.Collections.Generic;
using System.Linq;
using SmICSCoreLib.AQL.PatientInformation;
using SmICSCoreLib.AQL.PatientInformation.PatientMovement;
using SmICSCoreLib.AQL.PatientInformation.Symptome;


namespace SmICSWebApp.Data
{
    public class Symptom
    {
        private readonly IPatientInformation _patientInformation;
        private readonly DataService _dataService;     

        public Symptom(IPatientInformation patientInformation, DataService dataService)
        {
            _patientInformation = patientInformation;
            _dataService = dataService;
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

        public List<SymptomModel> GetAllSymByPatID(string patientId, DateTime datum)
        {
            try
            {
                List<SymptomModel> symptomListe = _patientInformation.Symptoms_By_PatientId(patientId, datum);
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
                patientMovement = _dataService.GetPatMovement(item.PatientenID);
                foreach (var movment in patientMovement)
                {
                    if (movment.StationID == station)
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

        //Stationen in den ein Symptome min.3mal bei Stationär behandelte Patient aufgetreten ist.
        public Dictionary<string, int> GetSymGroup(string symptom, DateTime datum)
        {
            List<PatientMovementModel> patBewegungen = new List<PatientMovementModel>();
            List<SymptomModel> patinetList = GetAllPatBySym(symptom, datum);

            foreach (var patient in patinetList)
            {
                //Check wenn Patient Stationär behandelt worden ist
                List<PatientMovementModel> patBewegung = _dataService.GetPatMovement(patient.PatientenID);
                if (patBewegung.Count != 0)
                {
                    patBewegungen.Add(patBewegung.First());
                }
            }
            var result = patBewegungen.GroupBy(x => x.StationID).Select(x => new { StationID = x.Key, Count = x.Count() }).ToList();

            //Rueckgabe: List alle Symptome die min. 3 mal aufgetreten sind
            Dictionary<string, int> finalList = new Dictionary<string, int>();
            foreach (var item in result)
            {
                if (item.Count >= 3)
                {
                    finalList.Add(item.StationID, item.Count);
                }
            }
            return finalList;
        }

        //List der kombination: Symptom (Station, Haeufigkeit).  
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

    }
}
