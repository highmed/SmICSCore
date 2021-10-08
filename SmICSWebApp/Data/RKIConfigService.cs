using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using SmICSCoreLib.REST;
using SmICSCoreLib.AQL.Lab.RKIConfig;
using SmICSCoreLib.AQL.Lab;
using SmICSCoreLib.AQL.PatientInformation.PatientMovement;
using SmICSCoreLib.AQL.PatientInformation;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace SmICSWebApp.Data
{
    public class RKIConfigService
    {
        private IRestDataAccess _restData;
        private readonly IPatientInformation _patientInformation;
        private readonly ILabData _labData;
        private readonly string path = @"./Resources/RKIConfig/RKIConfig.json";
        //private readonly string path_time = @"./Resources/RKIConfig/RKIConfigTime.json";
        private readonly IConfiguration Configuration;


        public RKIConfigService(IPatientInformation patientInformation, IRestDataAccess restData, ILabData labData, IConfiguration configuration)
        {
            _patientInformation = patientInformation;
            _labData = labData;
            _restData = restData;
            Configuration = configuration;
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

        public void StoreRules(List<RKIConfigTemplate> storedValues, LabDataTimeModel zeitpunkt)
        {
            try
            {
                StoreTimeSet(zeitpunkt);
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
                List<LabDataKeimReceiveModel> erregerListe = _labData.ProcessGetErreger(name);
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
                //string json = JsonConvert.SerializeObject(time, Formatting.Indented);
                //File.WriteAllText(path_time, json);
                Configuration["Zeitpunkt"] = time.Zeitpunkt;
            }
            catch (Exception)
            {
                throw new Exception($"Failed to store timedata");
            }
        }

        public LabDataTimeModel GetTimeSet()
        {
            //if(File.Exists(path_time) == true)
            //{
            //    string json_time = File.ReadAllText(path_time);
            //    LabDataTimeModel timefromjson = JsonConvert.DeserializeObject<LabDataTimeModel>(json_time);
            //    if (timefromjson != null)
            //    {
            //        return timefromjson;
            //    }
            //    else
            //    {
            //        return new LabDataTimeModel(); ;
            //    }
            //}
            //else
            //{
            //    return new LabDataTimeModel();
            //}
            LabDataTimeModel labdata = new LabDataTimeModel(Configuration["Zeitpunkt"]);
            return labdata;
        }
    }
}
