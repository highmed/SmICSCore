using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace SmICSCoreLib.CSVFileStream
{
    public static class SaveCSV
    {
        public static void SaveToCsv<T>(List<T> reportData, string path)
        {
            var lines = new List<string>();
            IEnumerable<PropertyDescriptor> props = TypeDescriptor.GetProperties(typeof(T)).OfType<PropertyDescriptor>();
            var header = string.Join(";", props.ToList().Select(x => x.Name));
            if (!File.Exists(path))
            {
                lines.Add(header);
            }
            var valueLines = reportData.Select(row => string.Join(";", header.Split(';').Select(a => row.GetType().GetProperty(a).GetValue(row, null))));
            lines.AddRange(valueLines);
            if (!File.Exists(path))
            {
                File.WriteAllLines(path, lines.ToArray());
            }
            else
            {
                File.AppendAllText(path, string.Join(";", lines.ToArray()) + Environment.NewLine);
            }

        }
    }
}
