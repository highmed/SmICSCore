using Newtonsoft.Json;
using SmICSCoreLib.JSONFileStream;
using SmICSCoreLib.StatistikDataModels;
using SmICSCoreLib.StatistikServices;
using System.IO;
using Xunit;

namespace WebApp.Test
{
    public class DistrictTest
    {
        RkiRestApi rkiRestApi = new();

        [Fact]
        public void CkeckRespone()
        {
            District districts = rkiRestApi.GetDistrictsByStateName("Hamburg");
            Assert.NotNull(districts);
        }


        [Fact]
        public void CkeckDistrict()
        {
            District district = rkiRestApi.GetDcistrictByName("Hamburg");
            Assert.Equal("Hamburg", district.Features[0].DistrictAttributes.GEN);
        }

        [Fact]
        public void DistrictsNumber()
        {
            District districts = rkiRestApi.GetDistrictsByStateName("Hamburg");
            Assert.Single(districts.Features);
        }

        [Fact]
        public void GetADistrict()
        {
            string pathFile = @"../../../../WebApp.Test/Resources";
            District district = rkiRestApi.GetDistrictsByStateName("Hamburg");
            JSONWriter.Write(district, pathFile, "District");

            //expected
            District districtFromJson;
            string path = @"../../../../WebApp.Test/Resources/District.json";
            using (StreamReader reader = new StreamReader(path))
            {
                string json = reader.ReadToEnd();
                districtFromJson = JsonConvert.DeserializeObject<District>(json);
            }

            //actual
            //State state = rkiRestApi.GetStateByName("Baden-Württemberg");

            Assert.Equal(district.Features[0].DistrictAttributes.GEN, district.Features[0].DistrictAttributes.GEN);
        }
    }
}
