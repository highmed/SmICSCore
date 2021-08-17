﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using SmICSCoreLib.REST;
using SmICSCoreLib.AQL.RKIConfig;
using SmICSCoreLib.AQL.PatientInformation.PatientMovement;
using SmICSCoreLib.AQL.PatientInformation;
using SmICSCoreLib.AQL.PatientInformation.Patient_Labordaten;
using SmICSCoreLib.AQL.PatientInformation.Patient_Mibi_Labordaten;
using System.IO;
using System.Linq;

namespace SmICSWebApp.Data
{
    public class RKIConfigService
    {
        private IRestDataAccess _restData;
        private readonly IPatientInformation _patientInformation;
        private readonly string path = @"./Resources/RKIConfig/RKIConfig.json";

        public RKIConfigService(IPatientInformation patientInformation, IRestDataAccess restData)
        {
            _patientInformation = patientInformation;
            _restData = restData;
        }

        public List<PatientMovementModel> GetAllStations()
        {
            try
            {
                List<PatientMovementModel> stationListe = _patientInformation.All_Stations();
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
                storedValues.Where(w => w.Erreger == "SARS-CoV-2").ToList().ForEach(s => s.ErregerID = GetErregerListViro(s.Erreger));
                //storedValues.Where(w => w.Erreger != "SARS-CoV-2").ToList().ForEach(s => s.ErregerID = GetErregerListMikro(s.Erreger));
                if (File.Exists(path) == false)
                {
                    string json = JsonConvert.SerializeObject(storedValues.ToArray(), Formatting.Indented);
                    File.WriteAllText(path, json);
                }
                else
                {
                    string json = File.ReadAllText(path);

                    List<RKIConfigTemplate> newList = JsonConvert.DeserializeObject<List<RKIConfigTemplate>>(json);
                    newList.AddRange(storedValues);

                    string storeJson = JsonConvert.SerializeObject(newList.ToArray(), Formatting.Indented);
                    File.WriteAllText(path, storeJson);
                }
                
            }
            catch(Exception)
            {
                throw new Exception($"Failed to store data");
            }
        }

        public List<RKIConfigTemplate> ShowValues()
        {
            string json = File.ReadAllText(path);
            List<RKIConfigTemplate> newList = JsonConvert.DeserializeObject<List<RKIConfigTemplate>>(json);

            if(newList != null)
            {
                return newList;
            }
            else
            {
                return new List<RKIConfigTemplate>();
            }
        }

        public void DeleteRuleInJson(int ID)
        {
            string json = File.ReadAllText(path);

            List<RKIConfigTemplate> newList = JsonConvert.DeserializeObject<List<RKIConfigTemplate>>(json);
            newList.RemoveAt(ID);

            string storeJson = JsonConvert.SerializeObject(newList.ToArray(), Formatting.Indented);
            File.WriteAllText(path, storeJson);
        }

        public List<LabDataModel> GetErregerListViro(string name)
        {
            try
            {
                List<LabDataModel> erregerListe = _patientInformation.ViroErreger(name);
                return erregerListe;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<MibiLabDataModel> GetErregerListMikro(string name)
        {
            try
            {
                List<MibiLabDataModel> erregerList = _patientInformation.MikroErreger(name);
                return erregerList;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}