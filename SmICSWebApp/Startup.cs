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
using SmICSCoreLib.CronJobs;
using Microsoft.IdentityModel.Logging;
using SmICSCoreLib.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using SmICSCWebApp.Authentication;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Certificate;

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
            IdentityModelEventSource.ShowPII = true;

            services.AddScoped<TokenProvider>();
            services.AddAuthentication(
                  CertificateAuthenticationDefaults.AuthenticationScheme)
              .AddCertificate(options =>
              {
                  options.AllowedCertificateTypes = CertificateTypes.All;

              });

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie("Cookies")
            .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {

                options.Authority = Environment.GetEnvironmentVariable("AUTHORITY");
                options.ClientId = Environment.GetEnvironmentVariable("CLIENT_ID");
                options.ClientSecret = Environment.GetEnvironmentVariable("CLIENT_SECRET");

                options.ResponseType = "code";
                options.Scope.Clear();
                options.Scope.Add("openid");

                options.ClaimsIssuer = "User";
                options.RequireHttpsMetadata = false;

                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;

                options.Events = new OpenIdConnectEvents
                {

                    /*OnRedirectToIdentityProviderForSignOut = (context) =>
                    {
                    };*/

                    OnAccessDenied = context =>
                    {
                        context.HandleResponse();
                        context.Response.Redirect("/");
                        return Task.CompletedTask;
                    }
                };
            });
            services.AddAuthentication()
            .AddJwtBearer(options =>
            {
                options.Authority = Environment.GetEnvironmentVariable("AUTHORITY");
                options.RequireHttpsMetadata = true;
                // name of the API resource
                options.Audience = "account";
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidIssuer = Environment.GetEnvironmentVariable("AUTHORITY"),
                    ValidAudience = "account"
                    //IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Tokens:Key"])) 
                };
            });
            services.AddAuthorization(options =>
            {
                var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    JwtBearerDefaults.AuthenticationScheme);
                defaultAuthorizationPolicyBuilder =
                    defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();
                options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
            });

            services.AddControllers();
            services.AddControllers().AddNewtonsoftJson();
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddLogging();
            services.AddSingleton<RkiService>();
            services.AddSingleton<SymptomService>();
            services.AddSingleton<EhrDataService>();

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

            services.AddSingleton<NUMNodeJob>();
            services.AddSingleton(new JobMetadata(Guid.NewGuid(), typeof(NUMNodeJob), "NumNode", "0 01 1,15 ? * 0"));
            DashboardConfig.dashboardEndpoint = Environment.GetEnvironmentVariable("DASHBOARD_DB");
            DashboardConfig.dashboardUser = Environment.GetEnvironmentVariable("DASHBOARD_USER");
            DashboardConfig.dashboardPassword = Environment.GetEnvironmentVariable("DASHBOARD_PASSWD");

            services.AddScoped<OutbreakDetectionService>();

            OpenehrConfig.OutbreakDetectionRuntime = Environment.GetEnvironmentVariable("OUTBREAK_DETECTION_TIME");
            Console.WriteLine("Transformed: OUTBREAK_DETECTION_TIME " + Environment.GetEnvironmentVariable("OUTBREAK_DETECTION_TIME") + "to CONFIG: " + OpenehrConfig.OutbreakDetectionRuntime);
            string[] runtimeArr = OpenehrConfig.OutbreakDetectionRuntime.Split(":");
            OpenehrConfig.OutbreakDetectionRuntime = runtimeArr[2] + " " + runtimeArr[1] + " " + runtimeArr[0] + " * * ?";

            services.AddSingleton<JobOutbreakDetection>();
            services.AddSingleton(new JobMetadata(Guid.NewGuid(),
                                  typeof(JobOutbreakDetection),
                                  "JobOutbreakDetection",
                                  OpenehrConfig.OutbreakDetectionRuntime));

            services.AddMvcCore(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                             .RequireAuthenticatedUser()
                             .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AQL API", Version = "v1" });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            services.AddSingleton<BlazorServerAuthStateCache>();
            services.AddScoped<AuthenticationStateProvider, BlazorServerAuthState>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            OpenehrConfig.openehrEndpoint = Environment.GetEnvironmentVariable("OPENEHR_DB");
            OpenehrConfig.smicsVisuPort = Environment.GetEnvironmentVariable("SMICS_VISU_PORT");

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

            app.UseAuthentication();
            app.Use(async (context, next) =>
            {
                await next();
                var bearerAuth = context.Request.Headers["Authorization"]
                    .FirstOrDefault()?.StartsWith("Bearer ") ?? false;
                if (context.Response.StatusCode == 401
                    && !context.User.Identity.IsAuthenticated
                    && !bearerAuth)
                {
                    await context.ChallengeAsync("oidc");
                }
            });

            app.UseSerilogRequestLogging();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}