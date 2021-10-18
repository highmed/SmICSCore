using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SmICSCoreLib.REST;
using SmICS;
using System.IO;
using Microsoft.OpenApi.Models;
using System.Reflection;
using SmICSWebApp.Data;
using Serilog;
using Quartz.Spi;
using Quartz;
using Quartz.Impl;
using SmICSCoreLib.StatistikServices.CronJob;
using SmICSCoreLib.StatistikServices;
using Microsoft.Extensions.Logging;
using SmICSCoreLib.Factories.DetectionAlgorithmInterface;
using Microsoft.Extensions.Options;
using SmICSCoreLib.Factories.RKIConfig;

namespace SmICSWebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddControllers().AddNewtonsoftJson();
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddLogging();
            services.AddSingleton<RkiService>();            
            services.AddSingleton<SymptomService>();
            services.AddSingleton<EhrDataService>();
            
            //AUTH - START 

            //AUTH - ENDE
            
            services.AddSmICSLibrary();
            //CronJob GetReport
            services.AddSingleton<IJobFactory, QuartzJobFactory>();
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
            services.AddSingleton<JobGetReport>();
            services.AddSingleton(new JobMetadata(Guid.NewGuid(), typeof(JobGetReport), "JobGetReport", "0 00 10 ? * *"));
            services.AddHostedService<QuartzHostedService>();

            services.AddSingleton<RKIConfigService>();

            services.AddSingleton<ContactTracingService>();
            services.AddSingleton<PersonInformationService>();
            services.AddSingleton<PersInfoInfectCtrlService>();

            //CronJob UpdateRkidata
            services.AddSingleton<JobUpdateRkidata>();
            services.AddSingleton(new JobMetadata(Guid.NewGuid(), typeof(JobUpdateRkidata), "JobUpdateRkidata", "0 00 15 ? * *"));

            if (File.Exists(@"./Resources/RKIConfig/RKIConfigTime.json"))
            {
                LabDataTimeModel runtimeString = SmICSCoreLib.JSONFileStream.JSONReader<LabDataTimeModel>.ReadObject(@"./Resources/RKIConfig/RKIConfigTime.json");
                string[] runtimeArr = runtimeString.Zeitpunkt.Split(":");
                OpenehrConfig.OutbreakDetectionRuntime = runtimeArr[2] + " " + runtimeArr[1] + " " + runtimeArr[0] + " * * ?";
            }
            else
            {
                OpenehrConfig.OutbreakDetectionRuntime = null;
            }

            services.AddSingleton<JobOutbreakDetection>();
            services.AddSingleton(new JobMetadata(Guid.NewGuid(),
                                  typeof(JobOutbreakDetection),
                                  "JobOutbreakDetection",
                                  OpenehrConfig.OutbreakDetectionRuntime));
            #region PaulsCronJob
            ////CronJob DetectionAlgorithmJob
            //services.AddSingleton<myJob_00001_DetectionAlgorithmJob>();
            ////
            //DateTime startCronJob = new DateTime(2021, 9, 9, 17, 32, 0);
            //int myHour = startCronJob.Hour;
            //int myMinute = startCronJob.Minute;
            //string cronJobParams = "";
            //cronJobParams += "*";
            //cronJobParams += " ";
            //cronJobParams += myMinute.ToString();
            //cronJobParams += " ";
            //cronJobParams += myHour.ToString();
            //cronJobParams += " * * ?";
            ////
            //cronJobParams = "";
            //cronJobParams += "*"; // Sekunden
            //cronJobParams += " 0/4"; // Minuten
            //cronJobParams += " *"; // Stunden
            //cronJobParams += " *"; // Tag
            //cronJobParams += " *"; // Monat
            //cronJobParams += " ?"; // Wochentag
            ////
            //services.AddSingleton(new JobMetadata(Guid.NewGuid(),
            //                      typeof(myJob_00001_DetectionAlgorithmJob),
            //                      "myJob_00001_DetectionAlgorithmJob",
            //                      cronJobParams));
            ///*services.AddSingleton(new JobMetadata(Guid.NewGuid(),
            //                      typeof(myJob_00001_DetectionAlgorithmJob),
            //                      "myJob_00001_DetectionAlgorithmJob",
            //                      "* 0/4 * * * ?"));*/
            #endregion

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AQL API", Version = "v1" });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            OpenehrConfig.openehrEndpoint = "https://plri-highmed01.mh-hannover.local:8083/rest/openehr/v1";
            OpenehrConfig.openehrUser = "etltestuser";
            OpenehrConfig.openehrPassword = "etltestuser#01";
            OpenehrConfig.openehrAdaptor = "BETTER";

            /*OpenehrConfig.openehrEndpoint = "https://172.0.0.1:8080/ehrbase/rest/openehr/v1";
            OpenehrConfig.openehrUser = "test";
            OpenehrConfig.openehrPassword = "test";
            OpenehrConfig.openehrAdaptor = "STANDARD";*/

            //OpenehrConfig.openehrEndpoint = Environment.GetEnvironmentVariable("OPENEHR_DB");
            //OpenehrConfig.openehrUser = Environment.GetEnvironmentVariable("OPENEHR_USER");
            //OpenehrConfig.openehrPassword = Environment.GetEnvironmentVariable("OPENEHR_PASSWD");

            //Übernehmen in eine static Methode in anderer Klasse
            


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "AQL API");
            });

            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseSerilogRequestLogging();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
