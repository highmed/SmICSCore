using Newtonsoft.Json;
using System.IO;

namespace SmICSCoreLib.JSONFileStream
{
    public class JSONWriter
    {
        public static void Write<T>(T data, string path, string filename)
        {
            string json = JsonConvert.SerializeObject(data);
            using (StreamWriter writer = File.CreateText(path + "/" + filename + ".json"))
            {
                writer.Write(json);
            }
        }

    }
}