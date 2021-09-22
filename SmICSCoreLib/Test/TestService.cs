using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;
using SmICSCoreLib.Database;
using SmICSCoreLib.JSONFileStream;

namespace SmICSCoreLib.Test
{
    public class TestService
    {
        private readonly string pathTest = @"./Resources/TestKlasse.json";
        private UnitOfWork test = new();

        public void ReadFile()
        {
            //string json = File.ReadAllText(pathTest);
            //List<TestKlasse> newList = JsonConvert.DeserializeObject<List<TestKlasse>>(json);
            TestKlasse newList;
            using (StreamReader reader = new StreamReader(pathTest))
            {
                string json = reader.ReadToEnd();
                newList = JsonSerializer.Deserialize<TestKlasse>(json);
            }

            test.testKlasse.Insert(newList);

            Console.WriteLine(newList);

            
        }
    }
}
