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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication;
using System.Linq;
using Microsoft.AspNetCore.Authentication.Certificate;
using SmICSCoreLib.Authentication;
using SmICSCWebApp.Authentication;

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
                options.Authority = Environment.GetEnvironmentVariable("AUTHORITY"); //"https://keycloak.mh-hannover.local:8443/auth/realms/Better";
                options.ClientId = Environment.GetEnvironmentVariable("CLIENT_ID"); //"medic-c-t";
                options.ClientSecret = Environment.GetEnvironmentVariable("CLIENT_SECRET"); //null;

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
                options.Authority = Environment.GetEnvironmentVariable("AUTHORITY");//"https://keycloak.mh-hannover.local:8443/auth/realms/Better";
                options.RequireHttpsMetadata = true;
                // name of the API resource
                options.Audience = "account";
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidIssuer = Environment.GetEnvironmentVariable("AUTHORITY"),//"https://keycloak.mh-hannover.local:8443/auth/realms/Better",
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
           
            services.AddScoped<RkiService>();            
            services.AddScoped<SymptomService>();
            services.AddScoped<EhrDataService>();

            //CronJob GetReport
            services.AddSingleton<IJobFactory, QuartzJobFactory>();
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
            services.AddSingleton<JobGetReport>();
            services.AddSingleton(new JobMetadata(Guid.NewGuid(), typeof(JobGetReport), "JobGetReport", "0 00 10 ? * *"));
            services.AddHostedService<QuartzHostedService>();

            services.AddScoped<ContactTracingService>();
            services.AddScoped<PersonInformationService>();
            services.AddScoped<PersInfoInfectCtrlService>();

            //CronJob UpdateRkidata
            services.AddSingleton<JobUpdateRkidata>();
            services.AddSingleton(new JobMetadata(Guid.NewGuid(), typeof(JobUpdateRkidata), "JobUpdateRkidata", "0 00 15 ? * *"));

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

            services.AddSmICSLibrary();
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

            OpenehrConfig.openehrEndpoint = Environment.GetEnvironmentVariable("OPENEHR_DB");

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "AQL API");
            });

            app.UseHttpsRedirection();
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
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapControllers();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
