using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using SmICSCoreLib.Factories.RKIConfig;
using System.IO;
using SmICSCoreLib.Factories.MiBi.Nosocomial;

namespace SmICSWebApp.Data
{
    public class RKIConfigService
    {
        private readonly string path = @"./Resources/OutbreakDetection/RKIConfig.json";

        public RKIConfigService()
        {

        }

        public void StoreRules(List<RKIConfigTemplate> storedValues)
        {
            try
            {
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

                string json = JsonConvert.SerializeObject(storedValues.ToArray(), Formatting.Indented);
                File.WriteAllText(path, json);

            }
            catch (Exception)
            {
                throw new Exception($"Failed to update data");
            }
        }

        public List<string> GetFilter(List<string> pathogenCodes)
        {
            List<string> filter = Rules.GetPossibleMREClasses(pathogenCodes);
            return filter;
        }
    }
}
