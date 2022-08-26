using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SmICSCoreLib.Factories.General;
using SmICSCoreLib.REST;
using System;
using System.Threading.Tasks;
using SmICSCoreLib.Factories.RKIStatistics.Models;
using System.Collections.Generic;
using SmICSCoreLib.Factories.RKIStatistics.ReceiveModels;

namespace SmICSCoreLib.Factories.RKIStatistics
{
    public class RKIReportFactory : IRKIReportFactory
    {
        
        public IRestDataAccess RestDataAccess { get; set; }
        private readonly ILogger<RKIReportFactory> _logger;
        private readonly RestClientWebConnector _client = new();
        private readonly string path = @"../SmICSWebApp/Resources/RKIReportData";

        public RKIDailyReportModel dailyReport;
        public string[] states;
        public List<RKIDailyReporStateModel> stateReportModel;
        public RKIDailyReporStateModel dailyReportState;
        public RKIReportStateCaseModel caseModel;
        

        public RKIReportFactory(IRestDataAccess restDataAccess, ILogger<RKIReportFactory> logger)
        {
            RestDataAccess = restDataAccess;
            _logger = logger;
        }
        public void FirstDataEntry()
        {
            TimespanParameter timespan = new() { Starttime = DateTime.Now.AddYears(-10), Endtime = DateTime.Now };
            _ = Process(timespan);
        }

        public void RegularDataEntry()
        {
            TimespanParameter timespan = new() { Starttime = DateTime.Now, Endtime = DateTime.Now };
            _ = Process(timespan);
        }

        public async Task Process(TimespanParameter timespan)
        {
            try
            {
                InitializeGlobalVariables();
                var tasks = new Task[]
                {
                    GetStateData(states)
                };

                foreach (var task in tasks)
                {
                    await task;
                }
            }
            catch (Exception e)
            {
                _logger.LogWarning("Can not get aggregated Data " + e.Message);
            }
        }

        private void InitializeGlobalVariables()
        {
            dailyReport = new RKIDailyReportModel();
            states = new string[] { "Baden-Württemberg", "Bayern", "Berlin","Brandenburg", "Bremen", "Hamburg",
                        "Hessen", "Mecklenburg-Vorpommern", "Niedersachsen", "Nordrhein-Westfalen", "Rheinland-Pfalz", "Saarland",
                        "Sachsen", "Sachsen-Anhalt", "Schleswig-Holstein", "Thüringen"};
            dailyReportState = new RKIDailyReporStateModel();
            stateReportModel = new List<RKIDailyReporStateModel>();
            caseModel = new RKIReportStateCaseModel();
        }

        public async Task GetStateData(string[] states)
        {
            try
            {
                for(int i = 0; i < states.Length; i++)
                {
                    _client.EndPoint = "https://services7.arcgis.com/mOBPykOjAyBO2ZKk/arcgis/rest/services/rki_key_data_hubv/FeatureServer/0/query?where=AdmUnitId ='" + i +
                    "'&outFields=AnzFall,AnzTodesfall,AnzFallNeu,AnzTodesfallNeu,Inz7T&outSR=4326&f=json";
                    string response = _client.GetResponse();
                    dynamic obj = JsonConvert.DeserializeObject(response);
                }
                await Task.CompletedTask;
                _logger.LogInformation("GetStateData");
            }
            catch (Exception e)
            {
                _logger.LogWarning("GetStateData " + e.Message);
            }
        }
    }
}
