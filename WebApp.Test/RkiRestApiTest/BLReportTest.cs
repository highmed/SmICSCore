using SmICSCoreLib.JSONFileStream;
using SmICSCoreLib.StatistikDataModels;
using SmICSCoreLib.StatistikServices;
using System;
using Xunit;

namespace WebApp.Test
{
    public class BLReportTest
    {
        RkiService rkiRestApi = new();

        [Fact]
        public void BLReportSerializeTest()
        {
            string filePath = @"../../../../WebApp.Test/Resources";
            bool status = rkiRestApi.BLReportSerialize(filePath);

            Assert.True(status);
        }

        [Fact]
        public void BLReportDeserializeTest()
        {
            string targetPath = @"../../../../WebApp.Test/Resources";
            rkiRestApi.BLReportSerialize(targetPath);

            string filePath = @"../../../../WebApp.Test/Resources/BLReport.json";
            Report report = rkiRestApi.BLReportDeserialize(filePath);

            Assert.Equal(DateTime.Now.ToString("dd.MM.yyyy"), report.Datum);
        }

        [Fact]
        public void GetBLReportTest()
        {
            Report report = rkiRestApi.GetBLReport("https://www.rki.de/DE/Content/InfAZ/N/Neuartiges_Coronavirus/Daten/Fallzahlen_Kum_Tab.xlsx?__blob=publicationFile",
                                                    2, 2, 3, 1, 422);
            Assert.NotNull(report);
        } 
        
        [Fact]
        public void CheckBLReport()
        {
            string filePath = @"../../../../WebApp.Test/Resources/BLReport.json";
            Report firstRport = rkiRestApi.BLReportDeserialize(filePath);

            Report secondReport = rkiRestApi.GetBLReport("https://www.rki.de/DE/Content/InfAZ/N/Neuartiges_Coronavirus/Daten/Fallzahlen_Kum_Tab.xlsx?__blob=publicationFile",
                                                    2, 2, 3, 1, 422);
            Assert.Equal(firstRport.BLReport[0].BlName, secondReport.BLReport[0].BlName);
        }


        [Fact]
        public void UpdateBlRkidataTest()
        {
            string dailyReportPath = @"../../../../WebApp.Test/Resources/" + DateTime.Now.ToString("yyyy-MM-dd") + ".json";
            string BLReportPath = @"../../../../WebApp.Test/Resources/BLReport.json";
            string targetPath = @"../../../../WebApp.Test/Resources";
            string blFilename = ("BLReport");
            rkiRestApi.SerializeRkiData(targetPath);

            bool status = rkiRestApi.UpdateBlRkidata(dailyReportPath, BLReportPath, targetPath, blFilename);

            Assert.True(status);
        }

    }
}
