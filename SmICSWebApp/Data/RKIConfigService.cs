using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using SmICSCoreLib.Factories.RKIConfig;
using SmICSCoreLib.Factories.PatientMovement;
using System.IO;
using System.Linq;
using SmICSCoreLib.Database;

namespace SmICSWebApp.Data
{
    public class RKIConfigService
    {
        private readonly IRKILabDataFactory _labdata;
        private readonly IPatientMovementFactory _patientInformation;
        private readonly string path = @"./Resources/OutbreakDetection/RKIConfig.json";
        private readonly string path_time = @"./Resources/RKIConfig/RKIConfigTime.json";
        public RKIConfigService(IPatientMovementFactory patientInfo, IRKILabDataFactory labdata)
        public RKIConfigService(IRestDataAccess restData, IPatientMovementFactory patientInfo, IRKILabDataFactory labdata)
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
        public void StoreRules(List<RKIConfigTemplate> storedValues, LabDataTimeModel zeitpunkt, DBService dbService)
        public void StoreRules(List<RKIConfigTemplate> storedValues, LabDataTimeModel zeitpunkt)
        {
            try
            {
                dbService.Insert((RKIConfigTemplate)storedValues.AsEnumerable());      
            }
            }
            catch(Exception)
            {
                Console.WriteLine("Es könnten keine Conig in DB gespeicher werden!");
            }
        }

        public List<RKIConfigTemplate> ShowValues(DBService dbService)
            List<RKIConfigTemplate> newList = dbService.FindAll<RKIConfigTemplate>().ToList();

            if (newList != null)
            if(newList != null)
            {
                return newList;
            }
            else
            {
                return new List<RKIConfigTemplate>();
            }
        }

        public void RestoreRules(string Id, DBService dbService)
        {
            dbService.DeleteByAttribute<RKIConfigTemplate>("Id", Id);
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
    }
}
