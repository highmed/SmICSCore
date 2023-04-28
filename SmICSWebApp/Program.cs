using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Serilog;
using SmICSCoreLib.CronJobs;
using SmICSCoreLib.DB;
using SmICSCoreLib.DB.MenuItems;
using SmICSCoreLib.Factories.MenuList;
using SmICSCoreLib.REST;

namespace SmICSWebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
            try
            {
                Log.Information("SmICS Application Starting up");
                Log.Information("Setting Config Variables");
                GetEnvironmentVariables();
                Log.Information("Starting to configure Database");
                ConfigureDatabase();
                Log.Information("Finished to configure Database");

                CreateHostBuilder(args).Build().Run();
            }
            catch(Exception e)
            {
                Log.Fatal(e, "The application failed to start correctly.");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        private static void ConfigureDatabase()
        {
            try{
                if(!Directory.Exists("./Resources/db"))
                {
                    Directory.CreateDirectory("./Resources/db");
                }
                if(!File.Exists("./Resources/db/SmICS.db"))
                {
                    File.Create("./Resources/db/SmICS.db");
                }
                IMenuItemDataAccess dbStartup = new MenuItemDataAccess(new DataAccess(new DapperContext()));
                dbStartup.CreatePathogenTable();
                dbStartup.CreateWardTable();
                RestClientConnector conn = new RestClientConnector();
                IRestDataAccess rest = new RestDataAccess(new NullLogger<RestDataAccess>(), conn);
                IMenuListFactory menu = new MenuListFactory(rest);
                MenuItemsJob job = new MenuItemsJob(dbStartup, menu);
                //Task wardtask = new Task(delegate() { 
                //    _ = job.UpdateWards(); 
                //});
                //wardtask.RunSynchronously();

                //Task pathotask = new Task(delegate () {
                //    _ = job.UpdatePathogens();
                //});
                //pathotask.RunSynchronously();
                job.UpdateWards();
                job.UpdatePathogens();

            }
            catch(Exception e)
            {
                Console.WriteLine("Failed Database Configuration: \n" + e.Message);
                throw;
            }
        }

        private static void GetEnvironmentVariables()
        {
            OpenehrConfig.openehrEndpoint = "http://plri-highmed01.mh-hannover.local:8081/";//Environment.GetEnvironmentVariable("OPENEHR_DB");
            OpenehrConfig.openehrUser = "smics";//Environment.GetEnvironmentVariable("OPENEHR_USER");
            OpenehrConfig.openehrPassword = "b+KzsSFD?cgdW2UA";
            //OpenehrConfig.smicsVisuPort = Environment.GetEnvironmentVariable("SMICS_VISU_PORT");
            OpenehrConfig.queryLimit = "1000000";
            OpenehrConfig.queryTimeout = 400000; 
            //OpenehrConfig.smicsVisuPort = Environment.GetEnvironmentVariable("SMICS_VISU_PORT");

        }
    }
}
