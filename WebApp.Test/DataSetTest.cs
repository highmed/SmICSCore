using SmICSCoreLib.StatistikServices;
using System.Data;
using Xunit;

namespace WebApp.Test
{
    public class DataSetTest
    {
        RkiRestApi rkiRestApi = new();

        [Fact]
        public void CkeckRespone()
        {
            string url = "https://www.rki.de/DE/Content/InfAZ/N/Neuartiges_Coronavirus/Daten/Fallzahlen_Kum_Tab.xlsx?__blob=publicationFile";
            DataSet dataSet = rkiRestApi.GetDataSetFromLink(url);
            Assert.NotNull(dataSet);
        }

        [Fact]
        public void CkeckRespone2()
        {
            string url = "https://raw.githubusercontent.com/robert-koch-institut/SARS-CoV-2-Nowcasting_und_-R-Schaetzung/main/Nowcast_R_aktuell.csv";
            DataSet dataSet = rkiRestApi.GetCsvDataSet(url);
            Assert.NotNull(dataSet);
        }


        [Fact]
        public void GetRValueTest()
        {
            string rValue = rkiRestApi.GetRValue(2);
            Assert.NotEmpty(rValue);
        }

    }
}
