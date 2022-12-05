
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Quartz;
using SmICSCoreLib.Factories.NUMNode;
using SmICSCoreLib.REST;

namespace SmICSCoreLib.CronJobs
{
    [DisallowConcurrentExecution]
    public class NUMNodeJob : IJob
    {
        private DashboardRestClientConnector _client;
        private readonly INUMNodeFactory _listFac;
        private ILogger<NUMNodeFactory> _logger;
        private readonly string path = @"../SmICSWebApp/Resources/NUMNode/NUMNode_1_R" + DateTime.Today.ToString("yyyy_MM_dd") + ".json";
        private readonly string shortpath = @"../SmICSWebApp/Resources/NUMNode/";

        public NUMNodeJob(INUMNodeFactory listFac, ILogger<NUMNodeFactory> logger)
        {
            _listFac = listFac;
            _logger = logger;
        }
        public Task Execute(IJobExecutionContext context)
        {
            if (!Directory.Exists("../SmICSWebApp/Resources/NUMNode/"))
            {
                Directory.CreateDirectory("../SmICSWebApp/Resources/NUMNode/");
            }
            if (!File.Exists(path))
            {
                _listFac.FirstDataEntry();
            }else
            {
                _listFac.RegularDataEntry();
            }
            if(DashboardConfig.dashboardEndpoint is not null)
            {
                ConnectToDashboard();
            }           
            return Task.CompletedTask;
        }

        public void ConnectToDashboard()
        {
            try
            {
                PostSmICSResults(path);
            }
            catch (Exception e)
            {
                _logger.LogError("Data cannot been send to the dashboard." + e);
            }       
        }
        //need some adjustments and been tested
        private void PostSmICSResults(string path)
        {
            string restPath = "";
            Uri dashboard = new(DashboardConfig.dashboardEndpoint);
            Uri RestPath = new(dashboard, restPath);
            if (File.Exists(path))
            {
                for(int i = 0; i < 4; i++)
                {
                    string filepath = shortpath + "NUMNode_" + i + "_R" + DateTime.Today.ToString("yyyy_MM_dd") + ".json";
                    var json = JSONFileStream.JSONReader<NUMNodeSaveModel>.ReadObject(filepath);
                    var finishedJson = JsonConvert.SerializeObject(json, Formatting.Indented);
                    HttpContent content = new StringContent(finishedJson, Encoding.UTF8, "application/json");
                    content.Headers.Add("Prefer", "return=representation");

                    HttpResponseMessage response = _client.Client.PostAsync(RestPath.ToString(), content).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        _logger.LogInformation("Data has been send successfully");
                        _logger.LogDebug("Result: {Result}", response.Content.ReadAsStringAsync().Result);
                    }
                    else
                    {
                        _logger.LogInformation("Data could not been send.");
                        _logger.LogDebug("No Success Code: {statusCode} \n {responsePhrase}", response.StatusCode, response.ReasonPhrase);
                    }
                }
                
            }

        }
    }
}
