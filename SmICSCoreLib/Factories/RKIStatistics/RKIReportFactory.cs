using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SmICSCoreLib.REST;
using System;
using System.Threading.Tasks;
using SmICSCoreLib.Factories.RKIStatistics.ReceiveModels;
using System.Collections.Generic;
using System.Linq;

namespace SmICSCoreLib.Factories.RKIStatistics
{
    public class RKIReportFactory : IRKIReportFactory
    {
        
        public IRestDataAccess RestDataAccess { get; set; }
        private readonly ILogger<RKIReportFactory> _logger;
        private readonly RestClientWebConnector _client = new();
        private readonly string rkiRest= "https://services7.arcgis.com/mOBPykOjAyBO2ZKk/arcgis/rest/services/";
        public string[] states = new string[] { "Baden-Württemberg", "Bayern", "Berlin","Brandenburg", "Bremen", "Hamburg",
                        "Hessen", "Mecklenburg-Vorpommern", "Niedersachsen", "Nordrhein-Westfalen", "Rheinland-Pfalz", "Saarland",
                        "Sachsen", "Sachsen-Anhalt", "Schleswig-Holstein", "Thüringen"};

        public RKIReportFactory(IRestDataAccess restDataAccess, ILogger<RKIReportFactory> logger)
        {
            RestDataAccess = restDataAccess;
            _logger = logger;
        }

        public async Task<RKIReportFeatures<RKIReportStateCaseModel>> GetStateData(int ID)
        {
            try
            {
                _client.EndPoint = rkiRest + "rki_key_data_hubv/FeatureServer/0/query?where=AdmUnitId ='" + ID +
                "'&outFields=AnzFall,AnzTodesfall,AnzFallNeu,AnzTodesfallNeu,Inz7T&outSR=4326&f=json";
                string response = _client.GetResponse();
                var obj = JsonConvert.DeserializeObject<RKIReportFeatures<RKIReportStateCaseModel>>(response);
                await Task.CompletedTask;

                _logger.LogInformation("GetStateData");
                return obj;
            }
            catch (Exception e)
            {
                _logger.LogWarning("GetStateData " + e.Message);
                return null;
            }
        }

        public async Task GetAllStates()
        {
            try
            {
                _client.EndPoint = rkiRest + "Coronaf%C3%A4lle_in_den_Bundesl%C3%A4ndern/FeatureServer/0/query?" +
                "where=1=1&outFields=OBJECTID_1, LAN_ew_GEN, LAN_ew_BEZ, LAN_ew_EWZ, Fallzahl, faelle_100000_EW, Death, cases7_bl_per_100k, Aktualisierung&returnGeometry=false&outSR=4326&f=json";
                string response = _client.GetResponse();
                dynamic obj = JsonConvert.DeserializeObject(response);

                _logger.LogInformation("GetAllStates");
                await Task.CompletedTask;
            }
            catch (Exception e)
            {
                _logger.LogWarning("GetAllStates " + e.Message);
            }
        }

        public async Task<List<RKIReportFeatures<RKIReportStateModel>>> GetStateByName()
        {
            try
            {
                List<RKIReportFeatures<RKIReportStateModel>> list = new();
                foreach(string state in states)
                {
                    _client.EndPoint = rkiRest + "Coronaf%C3%A4lle_in_den_Bundesl%C3%A4ndern/FeatureServer/0/query?where=LAN_ew_GEN='" + state + 
                    "'&outFields=OBJECTID_1, LAN_ew_GEN, LAN_ew_BEZ, LAN_ew_EWZ, Fallzahl,cases7_bl, faelle_100000_EW, Death, death7_bl, cases7_bl_per_100k, Aktualisierung&returnGeometry=false&outSR=4326&f=json";
                    string response = _client.GetResponse();
                    var obj = JsonConvert.DeserializeObject<RKIReportFeatures<RKIReportStateModel>>(response);
                    list.Add(obj);
                }
                
                _logger.LogInformation("GetStateByName");
                await Task.CompletedTask;
                return list;
            }
            catch (Exception e)
            {
                _logger.LogWarning("GetStateByName " + e.Message);
                return null;
            }
        }

        public async Task<List<RKIReportFeatures<RKIReportDistrictModel>>> GetDistrictByName(List<string> gen)
        {
            try
            {
                List<RKIReportFeatures<RKIReportDistrictModel>> list = new();
                foreach (string g in gen)
                {
                    _client.EndPoint = rkiRest + "RKI_Landkreisdaten/FeatureServer/0/query?where=GEN='" +
                    g + "'&outFields=*&outSR=4326&f=json&returnGeometry=false";
                    string response = _client.GetResponse();
                    var obj = JsonConvert.DeserializeObject<RKIReportFeatures<RKIReportDistrictModel>>(response);
                    try
                    {
                        if (obj.features[0].GetType().GetProperties().Any())
                        {
                            list.Add(obj);
                        }
                    }catch (IndexOutOfRangeException ex)
                    {
                        _logger.LogWarning("District {i} does not have data", g);
                    }
                         
                }

                _logger.LogInformation("GetDcistrictByName");
                await Task.CompletedTask;
                return list;
            }
            catch (Exception e)
            {
                _logger.LogWarning("GetDcistrictByName " + e.Message);
                return null;
            }
        }

        public double GetRValue(int value)
        {
            string rValu;
            try
            {
                string url = "https://raw.githubusercontent.com/robert-koch-institut/SARS-CoV-2-Nowcasting_und_-R-Schaetzung/main/Nowcast_R_aktuell.csv";
                var result = DownloadFile.DownloadFile.GetDataSetFromLink(url, "csv");
                var dataRows = result.Tables[0].Rows;
                rValu = result.Tables[0].Rows[dataRows.Count - value][9].ToString();
                if (rValu.ElementAt(0) == '.')
                {
                    rValu = rValu.Insert(0, "0");
                }
                double rValue =  double.Parse(rValu, System.Globalization.CultureInfo.InvariantCulture);

                _logger.LogInformation("GetRValue");
                return rValue;
            }
            catch (Exception e)
            {
                _logger.LogWarning("GetRValue " + e.Message);
                return 0;
            }
        }
    }
}
