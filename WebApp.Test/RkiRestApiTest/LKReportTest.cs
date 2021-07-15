using SmICSCoreLib.StatistikDataModels;
using SmICSCoreLib.StatistikServices;
using System;
using Xunit;

namespace WebApp.Test
{
    public class LKReportTest
    {
        RkiRestApi rkiRestApi = new();

        [Fact]
        public void LKReportSerializeTest()
        {
            string filePath = @"../../../../WebApp.Test/Resources";
            bool status = rkiRestApi.LKReportSerialize(filePath);

            Assert.True(status);
        }

        [Fact]
        public void LKReportDeserializeTest()
        {
            string filePath = @"../../../../WebApp.Test/Resources/LKReport.json";
            LKReportJson lKReportJson = rkiRestApi.LKReportDeserialize(filePath);

            Assert.Equal(DateTime.Now.ToString("dd.MM.yyyy"), lKReportJson.Datum);
        }

        [Fact]
        public void GetLKReportTest()
        {
            LKReportJson lKReportJson = rkiRestApi.GetLKReport("https://www.rki.de/DE/Content/InfAZ/N/Neuartiges_Coronavirus/Daten/Fallzahlen_Kum_Tab.xlsx?__blob=publicationFile",
                                                                   4, 4, 5, 3, 228);
            Assert.NotNull(lKReportJson);
        }

        [Fact]
        public void UpdateLklRkidataTest()
        {
            string dailyReportPath = @"../../../../WebApp.Test/Resources/" + DateTime.Now.ToString("yyyy-MM-dd") + ".json";
            string BLReportPath = @"../../../../WebApp.Test/Resources/LKReport.json";
            string targetPath = @"../../../../WebApp.Test/Resources";
            string blFilename = ("LKReport");
            bool status = rkiRestApi.UpdateLklRkidata(dailyReportPath, BLReportPath, targetPath, blFilename);

            Assert.True(status);
        }


    }
}
