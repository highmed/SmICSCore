using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmICSDataGenerator.Tests;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmICSFactory.Tests
{
    public class ExpectedResultJsonReader
    {
        public static List<T> ReadPatientResults<T>(string path, int resultNo)
        {
            List<PatientIDs> patients = SmICSCoreLib.JSONFileStream.JSONReader<PatientIDs>.Read(@"../../../../SmICSDataGenerator.Test/Resources/GeneratedEHRIDs.json");

            using (StreamReader reader = new StreamReader(""))
            {
                string json = reader.ReadToEnd();
                JObject jObject = JsonConvert.DeserializeObject<JObject>(json);
                JArray arr = jObject.Property("" + resultNo).Value as JArray;
                foreach (JObject obj in arr)
                {
                    obj.Add(new JProperty("PatientID", patients[resultNo].EHR_ID));
                }
                return arr.ToObject<List<T>>();

            }
        }
    }
}
