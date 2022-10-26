using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
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
using SmICSWebApp.Data.OutbreakDetection;
using SmICSWebApp.Data.PatientView;
using SmICSWebApp.Data.WardView;
using SmICSWebApp.Data.Contact;
using SmICSWebApp.Data.MedicalFinding;
using SmICSWebApp.Data.PatientMovement;
using SmICSWebApp.Data.ContactNetwork;
using SmICSWebApp.Data.ContactComparison;
using SmICSWebApp.Data.Menu;
using SmICSCoreLib.CronJobs;

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
             services.AddBlazorise(options =>
              {
                  options.ChangeTextOnKeyPress = true; // optional
                })
            .AddBootstrapProviders()
            .AddFontAwesomeIcons();

            services.AddControllers();
            services.AddControllers().AddNewtonsoftJson();
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddBlazorContextMenu();
            services.AddLogging();
            services.AddSingleton<RkiService>();
            services.AddSingleton<SymptomService>();
            services.AddSingleton<EhrDataService>();
            services.AddScoped<WardOverviewService>();
            services.AddScoped<PatientViewService>();
            services.AddScoped<ContactService>();
            services.AddScoped<MedicalFindingService>();
            services.AddScoped<PatientMovementService>();
            services.AddScoped<ContactNetworkService>();
            services.AddScoped<ComparisonService>();
            services.AddScoped<MenuService>();

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

            services.AddScoped<OutbreakDetectionService>();

            services.AddSingleton<MenuItemsJob>();
            services.AddSingleton(new JobMetadata(Guid.NewGuid(), typeof(MenuItemsJob), "MenuItems", "0 0 1 ? * *"));
            services.AddSingleton<NUMNodeJob>();
            services.AddSingleton(new JobMetadata(Guid.NewGuid(), typeof(NUMNodeJob), "NumNode", "0 01 00 ? * 0"));
            //OpenehrConfig.OutbreakDetectionRuntime = Environment.GetEnvironmentVariable("OUTBREAK_DETECTION_TIME");
            //Console.WriteLine("Transformed: OUTBREAK_DETECTION_TIME " + Environment.GetEnvironmentVariable("OUTBREAK_DETECTION_TIME") + "to CONFIG: " + OpenehrConfig.OutbreakDetectionRuntime);
            //string[] runtimeArr = OpenehrConfig.OutbreakDetectionRuntime.Split(":");
            //OpenehrConfig.OutbreakDetectionRuntime = runtimeArr[2] + " " + runtimeArr[1] + " " + runtimeArr[0] + " * * ?";

            //services.AddSingleton<JobOutbreakDetection>();
            //services.AddSingleton(new JobMetadata(Guid.NewGuid(),
            //                      typeof(JobOutbreakDetection),
            //                      "JobOutbreakDetection",
            //                      OpenehrConfig.OutbreakDetectionRuntime));

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
                c.DefaultModelsExpandDepth(-1);
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
