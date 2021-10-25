using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using SmICSCoreLib.REST;
using SmICSCoreLib.Factories.RKIConfig;
using SmICSCoreLib.Factories.PatientMovement;
using System.IO;
using System.Linq;
using SmICSCoreLib.Database;

namespace SmICSWebApp.Data
{
    public class RKIConfigService
    {
        private readonly IRestDataAccess _restData;
        private readonly IRKILabDataFactory _labdata;
        private readonly IPatientMovementFactory _patientInformation;
        //private readonly string path = @"./Resources/RKIConfig/RKIConfig.json";
        private readonly string path_time = @"./Resources/RKIConfig/RKIConfigTime.json";

        public RKIConfigService(IRestDataAccess restData, IPatientMovementFactory patientInfo, IRKILabDataFactory labdata)
        {
            _restData = restData;
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

        //DB
        public void StoreRules(List<RKIConfigTemplate> storedValues, LabDataTimeModel zeitpunkt, DBService dbService)
        {
            try
            {
                StoreTimeSet(zeitpunkt);
                storedValues.Where(w => w.Erreger != null).ToList().ForEach(s => s.ErregerID = GetErregerList(s.Erreger));

                //Testen ob er nur die neuen Configs speichert oder die ganze Liste nochmal
                dbService.Insert((RKIConfigTemplate)storedValues.AsEnumerable());
                //if (File.Exists(path) == false)
                //{
                //    string json = JsonConvert.SerializeObject(storedValues.ToArray(), Formatting.Indented);
                //    File.WriteAllText(path, json);
                //}
                //else
                //{
                //    string json = File.ReadAllText(path);

                //    List<RKIConfigTemplate> newList = JsonConvert.DeserializeObject<List<RKIConfigTemplate>>(json);
                //    newList.AddRange(storedValues);

                //    string storeJson = JsonConvert.SerializeObject(newList.ToArray(), Formatting.Indented);
                //    File.WriteAllText(path, storeJson);
                //}

            }
            catch(Exception)
            {
                throw new Exception($"Failed to store data");
            }
        }

        //DB
        public List<RKIConfigTemplate> ShowValues(DBService dbService)
        {
            //string json = File.ReadAllText(path);
            //List<RKIConfigTemplate> newList = JsonConvert.DeserializeObject<List<RKIConfigTemplate>>(json);
            List<RKIConfigTemplate> newList = dbService.FindAll<RKIConfigTemplate>().ToList();

            if (newList != null)
            {
                return newList;
            }
            else
            {
                return new List<RKIConfigTemplate>();
            }
        }

        //DB
        public void RestoreRules(List<RKIConfigTemplate> storedValues, DBService dbService)
        {
            try
            {
                storedValues.Where(w => w.Erreger != null).ToList().ForEach(s => s.ErregerID = GetErregerList(s.Erreger));

                dbService.Insert((RKIConfigTemplate)storedValues.AsEnumerable());
                //string json = JsonConvert.SerializeObject(storedValues.ToArray(), Formatting.Indented);
                //File.WriteAllText(path, json);

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

        public void StoreTimeSet(LabDataTimeModel time)
        {
            try
            {
                string json = JsonConvert.SerializeObject(time, Formatting.Indented);
                File.WriteAllText(path_time, json);
            }
            catch (Exception)
            {
                throw new Exception($"Failed to store timedata");
            }
        }

        public LabDataTimeModel GetTimeSet()
        {
            if(File.Exists(path_time) == true)
            {
                string json_time = File.ReadAllText(path_time);
                LabDataTimeModel timefromjson = JsonConvert.DeserializeObject<LabDataTimeModel>(json_time);
                if (timefromjson != null)
                {
                    return timefromjson;
                }
                else
                {
                    return new LabDataTimeModel(); ;
                }
            }
            else
            {
                return new LabDataTimeModel();
            }
            
        }
    }
}
