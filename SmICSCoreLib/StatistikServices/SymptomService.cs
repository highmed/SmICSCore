using System;
using System.Collections.Generic;
using System.Linq;
using SmICSCoreLib.Factories;
using SmICSCoreLib.Factories.PatientMovement;
using SmICSCoreLib.Factories.Symptome;

namespace SmICSCoreLib.StatistikServices
{
    public class SymptomService
    {
        private readonly ISymptomFactory _symptomFac;
        private readonly EhrDataService _dataService;

        public SymptomService(ISymptomFactory symptomFac, EhrDataService dataService)
        {
            _symptomFac = symptomFac;
            _dataService = dataService;
        }

        //Alle Symptome mit anzahl der Patienten, bei denen das Symptom min. 1mal aufgetreten ist.
        public List<SymptomModel> GetAllSymptom()
        {
            try
            {
                List<SymptomModel> symptomListe = _symptomFac.ProcessNoParam();
                return symptomListe;
            }
            catch (Exception)
            {
                return null;
            }
        }

        //Alle Symptome, die bei einem Patient in einem bestimmten Datum aufgetreten sind.
        public List<SymptomModel> GetAllSymByPatID(string patientId, DateTime datum)
        {
            try
            {
                List<SymptomModel> symptomListe = _symptomFac.SymptomByPatient(patientId, datum);
                return symptomListe;
            }
            catch (Exception)
            {
                return null;
            }
        }

        //Alle Symptome, die in einem bestimmten Datum aufgetreten sind.
        public List<SymptomModel> GetAllPatBySym(string symptom, DateTime datum)
        {
            List<SymptomModel> symptomListe = _symptomFac.PatientBySymptom(symptom);

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

        //Alle Symptome, die in einem bestimmten Datum in einer Station aufgetreten sind.
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
        //Rueckgabe:(Station, Haeufigkeit). 
        public Dictionary<string, int> GetSymGroup(string symptom, DateTime datum, int min)
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
                if (item.Count >= min)
                {
                    finalList.Add(item.StationID, item.Count);
                }
            }
            return finalList;
        }

        //List der kombination: Symptom (Station, Haeufigkeit).  
        public Dictionary<string, Dictionary<string, int>> GetAllSymGroup(DateTime datum, int min)
        {
            Dictionary<string, Dictionary<string, int>> allSymGroup = new Dictionary<string, Dictionary<string, int>>();
            List<SymptomModel> symptomListe = GetAllSymptom();
            if (symptomListe != null)
            {
                foreach (var item in symptomListe)
                {
                    Dictionary<string, int> symGroup = GetSymGroup(item.NameDesSymptoms, datum, min);
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
