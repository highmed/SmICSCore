using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace SmICSCoreLib.JSONFileStream
{
    public class JSONReader<T> where T : new()
    {
        public static List<T> Read(string path)
        {
            using (StreamReader reader = new StreamReader(path))
            {
                string json = reader.ReadToEnd();
                return JsonSerializer.Deserialize<List<T>>(json);              
            }
        }

        public static T ReadObject(string path)
        {
            using (StreamReader reader = new StreamReader(path))
            {
                string json = reader.ReadToEnd();
                return JsonSerializer.Deserialize<T>(json);
            }
        }

        public static T ReadSingle(string path)
        {
            using (StreamReader reader = new StreamReader(path))
            {
                string json = reader.ReadToEnd();
                return JsonSerializer.Deserialize<T>(json);

            }
        }
    }
}
