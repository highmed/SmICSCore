
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
        private readonly DashboardRestClientConnector _client;
        private readonly INUMNodeFactory _listFac;
        private readonly ILogger<NUMNodeFactory> _logger;
        private readonly string path = @"../SmICSWebApp/Resources/NUMNode/NUMNode_1_" + DateTime.Today.ToString("yyyy_mm_dd") + ".json";

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
            ConnectToDashboard();
            return Task.CompletedTask;
        }

        public void ConnectToDashboard()
        {
            try
            {
                GetDashboardVariables();

                PostSmICSResults(path);
            }
            catch (Exception e)
            {
                _logger.LogError("Data cannot been send to the dashboard." + e);
            }       
        }

        private static void GetDashboardVariables()
        {
            DashboardConfig.dashboardEndpoint = Environment.GetEnvironmentVariable("DASHBOARD_DB");
            DashboardConfig.dashboardUser = Environment.GetEnvironmentVariable("DASHBOARD_USER");
            DashboardConfig.dashboardPassword = Environment.GetEnvironmentVariable("DASHBOARD_PASSWD");
        }

        private void PostSmICSResults(string path)
        {
            Uri dashboard = new(DashboardConfig.dashboardEndpoint);
            if (File.Exists(path))
            {
                var json = JSONFileStream.JSONReader<NUMNodeSaveModel>.ReadObject(path);
                var finishedJson = JsonConvert.SerializeObject(json, Formatting.Indented);
                HttpContent content = new StringContent(finishedJson, Encoding.UTF8, "application/json");
                content.Headers.Add("Prefer", "return=representation");

                HttpResponseMessage response = _client.Client.PostAsync(dashboard.ToString(), content).Result;
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
