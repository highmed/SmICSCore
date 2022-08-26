using ExcelDataReader;
using System;
using System.Data;
using System.IO;
using System.Net;

namespace SmICSCoreLib.DownloadFile
{
    public static class DownloadFile
    {
        public static DataSet GetDataSetFromLink(String url)
        {
            var client = new WebClient();
            try
            {
                var fullPath = Path.GetTempFileName();
                client.DownloadFile(url, fullPath);

                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                using var stream = File.Open(fullPath, FileMode.Open, FileAccess.Read);
                using var reader = ExcelReaderFactory.CreateReader(stream);
                var result = reader.AsDataSet();

                return result;
            }
            catch
            {
                return null;
            }
        }

        public static DataSet GetCsvDataSet(String url)
        {
            var client = new WebClient();
            try
            {
                var fullPath = Path.GetTempFileName();
                client.DownloadFile(url, fullPath);

                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                using var stream = File.Open(fullPath, FileMode.Open, FileAccess.Read);
                using var reader = ExcelReaderFactory.CreateCsvReader(stream);
                var result = reader.AsDataSet();

                return result;
            }
            catch
            {
                return null;
            }
        }
    }
}
