using ExcelDataReader;
using System.Data;
using System.IO;
using System.Net;

namespace SmICSCoreLib.DownloadFile
{
    public class DownloadFile
    {
        public DataTableCollection Sheets;
        private static DownloadFile instance = new DownloadFile();

        public static DownloadFile GetInstance(string url)
        {
            _ = new DownloadFile(url);
            return instance;
        }
        private DownloadFile()
        {

        }

        public DownloadFile(string url) :this()
        { 
            var client = new WebClient();
            var fullPath = Path.GetTempFileName();
            client.DownloadFile(url, fullPath);

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            FileStream stream = File.Open(fullPath, FileMode.Open, FileAccess.Read);
            IExcelDataReader reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            DataSet result = reader.AsDataSet();

            instance.Sheets = result.Tables;

            reader.Close();
        }

        public static DataSet GetDataSetFromLink(string url, string useCase)
        {
            var client = new WebClient();
            var fullPath = Path.GetTempFileName();
            client.DownloadFile(url, fullPath);

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using var stream = File.Open(fullPath, FileMode.Open, FileAccess.Read);
            switch (useCase)
            {
                case "link": 
                    var reader = ExcelReaderFactory.CreateReader(stream); 
                    var result = reader.AsDataSet();
                    return result;
                case "csv":
                    var csvreader = ExcelReaderFactory.CreateCsvReader(stream);
                    var csvresult = csvreader.AsDataSet();
                    return csvresult;
            }
            return null;
        }
        
    }
}
