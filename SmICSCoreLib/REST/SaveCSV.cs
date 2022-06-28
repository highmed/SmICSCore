
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SmICSCoreLib.REST
{
    public static class SaveCSV
    {
        public static void SaveToCsv<T>(List<T> reportData, string path)
        {
            var lines = new List<string>();
            if (!File.Exists(path))
            { 
                IEnumerable<PropertyDescriptor> props = TypeDescriptor.GetProperties(typeof(T)).OfType<PropertyDescriptor>();
                var header = string.Join(";", props.ToList().Select(x => x.Name));
                lines.Add(header);
                var valueLines = reportData.Select(row => string.Join(";", header.Split(';').Select(a => row.GetType().GetProperty(a).GetValue(row, null))));
                lines.AddRange(valueLines);
                File.WriteAllLines(path, lines.ToArray());

            }
            else
            {
                string newContent = string.Join(";", reportData.ToArray());
                while (newContent != null)
                {
                    File.AppendAllText(path, newContent + Environment.NewLine);
                }
            }
            
        }
    }
}
