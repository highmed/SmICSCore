using SmICSCoreLib.StatistikDataModels;
using SmICSCoreLib.StatistikServices;
using System;
using Xunit;

namespace WebApp.Test
{
    public class RkiResourcesTest
    {
        RkiService rkiRestApi = new();
      
        [Fact]
        public void CkeckRespone()
        {
            string url = "https://www.rki.de/DE/Content/InfAZ/N/Neuartiges_Coronavirus/Daten/Fallzahlen_Kum_Tab.xlsx?__blob=publicationFile";
            Bericht bericht = rkiRestApi.GetBerichtFromUrl(url);

            Assert.NotNull(bericht);
        }

        [Fact]
        public void CkeckStatus()
        {
            string url = "https://www.rki.de/DE/Content/InfAZ/N/Neuartiges_Coronavirus/Daten/Fallzahlen_Kum_Tab.xlsx?__blob=publicationFile";
            Bericht bericht = rkiRestApi.GetBerichtFromUrl(url);

            Assert.True(bericht.StandAktuell);
        }

        [Fact]
        public void CkeckDate()
        {
            string url = "https://www.rki.de/DE/Content/InfAZ/N/Neuartiges_Coronavirus/Daten/Fallzahlen_Kum_Tab.xlsx?__blob=publicationFile";
            Bericht bericht = rkiRestApi.GetBerichtFromUrl(url);

            Assert.Equal(DateTime.Now.ToString("dd.MM.yyyy"),bericht.Stand);
        } 
        
        [Fact]
        public void CkeckIncorrectInput()
        {
            string url = "";
            Bericht bericht = rkiRestApi.GetBerichtFromUrl(url);

            Assert.Null(bericht);
        }
        
        [Fact]
        public void CkeckSerializtion()
        {
            bool status = rkiRestApi.SerializeRkiData();

            Assert.True(status);
        }

        [Fact]
        public void CkeckDeserializtion()
        {
            string dailyReportPath = @"../../../../WebApp.Test/Resources/" + DateTime.Now.ToString("yyyy-MM-dd") + ".json";
            DailyReport dailyReport = rkiRestApi.DeserializeRkiData(dailyReportPath);

            Assert.True(dailyReport.Bericht.StandAktuell);
        }

        [Fact]
        public void CkeckIncorrectDate()
        {
            string dailyReportPath = @"../../../../WebApp.Test/Resources/" + DateTime.Now.AddDays(10).ToString("yyyy-MM-dd") + ".json";
            DailyReport dailyReport = rkiRestApi.DeserializeRkiData(dailyReportPath);

            Assert.Null(dailyReport);
        }

    }
}
