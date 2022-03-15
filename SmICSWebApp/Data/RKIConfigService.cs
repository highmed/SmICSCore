using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using SmICSCoreLib.Factories.RKIConfig;
using SmICSCoreLib.Factories.PatientMovement;
using System.IO;
using System.Linq;
using SmICSCoreLib.Factories.MiBi.Nosocomial;

namespace SmICSWebApp.Data
{
    public class RKIConfigService
    {
        private readonly IRKILabDataFactory _labdata;
        private readonly IPatientMovementFactory _patientInformation;
        private readonly string path = @"./Resources/OutbreakDetection/RKIConfig.json";

        public RKIConfigService(IPatientMovementFactory patientInfo, IRKILabDataFactory labdata)
        {
            _patientInformation = patientInfo;
            _labdata = labdata;
        }

        public List<PatientMovementModel> GetAllStations()
        {
            try
            {
                List<PatientMovementModel> stationListe = _patientInformation.ProcessGetStations();
                return stationListe;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void StoreRules(List<RKIConfigTemplate> storedValues)
        {
            try
            {
                storedValues.Where(w => w.Erreger != null).ToList().ForEach(s => s.ErregerID = GetErregerList(s.Erreger));
                if (File.Exists(path) == false)
                {
                    string json = JsonConvert.SerializeObject(storedValues.ToArray(), Formatting.Indented);
                    File.WriteAllText(path, json);
                }
                else
                {
                    string json = File.ReadAllText(path);
                    List<RKIConfigTemplate> newList = JsonConvert.DeserializeObject<List<RKIConfigTemplate>>(json);
                    if (newList != null)
                    {
                        newList.AddRange(storedValues);
                        string storeJson = JsonConvert.SerializeObject(newList.ToArray(), Formatting.Indented);
                        File.WriteAllText(path, storeJson);
                    }
                    else
                    {
                        string oldJson = JsonConvert.SerializeObject(storedValues.ToArray(), Formatting.Indented);
                        File.WriteAllText(path, oldJson);
                    }
                }
            }
            catch(Exception)
            {
                throw new Exception($"Failed to store data");
            }
        }

        public List<RKIConfigTemplate> ShowValues()
        {
            List<RKIConfigTemplate> newList = null;
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                if (json != "")
                {
                    newList = JsonConvert.DeserializeObject<List<RKIConfigTemplate>>(json);
                }
            }
            else
            {
                File.Create(path).Close(); ;
            }
            if (newList != null)
            {
                return newList;
            }
            else
            {
                return new List<RKIConfigTemplate>();
            }
        }

        public void RestoreRules(List<RKIConfigTemplate> storedValues)
        {
            try
            {
                storedValues.Where(w => w.Erreger != null).ToList().ForEach(s => s.ErregerID = GetErregerList(s.Erreger));

                string json = JsonConvert.SerializeObject(storedValues.ToArray(), Formatting.Indented);
                File.WriteAllText(path, json);

            }
            catch (Exception)
            {
                throw new Exception($"Failed to update data");
            }
        }

        public List<LabDataKeimReceiveModel> GetErregerList(string name)
        {
            try
            {
                List<LabDataKeimReceiveModel> erregerListe = _labdata.ProcessGetErreger(name);
                return erregerListe;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<string> GetFilter(string pathogen)
        {
            List<string> filter = Rules.GetPossibleMREClasses(pathogen);
            return filter;
        }
    }
}
