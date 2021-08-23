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
using Microsoft.AspNetCore.Authorization;
using SmICSWebApp.Authentication;
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

            /*services.AddAuthentication(
               CertificateAuthenticationDefaults.AuthenticationScheme)
           .AddCertificate(options =>
           {
               options.AllowedCertificateTypes = CertificateTypes.All;
           });*/

            /*services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = "oidc";
            })
            .AddCookie("Cookies")
            .AddOpenIdConnect("oidc", options =>
            {
                options.Authority = Configuration["oidc:Authority"];
                options.ClientId = Configuration["oidc:ClientId"];
                options.ClientSecret = Configuration["oidc:ClientSecret"];

                options.ResponseType = "code";
                options.Scope.Clear();
                options.Scope.Add("openid");

                options.ClaimsIssuer = "User";
                options.RequireHttpsMetadata = false;

                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;

                options.Events = new OpenIdConnectEvents
                {

                    //OnRedirectToIdentityProviderForSignOut = (context) =>
                    //{

                    //};

                    OnAccessDenied = context =>
                    {
                        context.HandleResponse();
                        context.Response.Redirect("/");
                        return Task.CompletedTask;
                    }
                };
            });*/


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
                options.Authority = Configuration["oidc:Authority"];
                options.ClientId = Configuration["oidc:ClientId"];
                options.ClientSecret = Configuration["oidc:ClientSecret"];

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
                options.Authority = Configuration["oidc:Authority"];
                options.RequireHttpsMetadata = true;
                // name of the API resource
                options.Audience = "account";
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidIssuer = Configuration["oidc:Authority"],
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
            services.AddSmICSLibrary();
            services.AddSingleton<DataService>();
            services.AddSingleton<Symptom>();

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
            //OpenehrConfig.openehrEndpoint = "https://plri-highmed01.mh-hannover.local:8083/rest/openehr/v1";
            //OpenehrConfig.openehrUser = "smics";
            //OpenehrConfig.openehrPassword = "b+KzsSFD?cgdW2UA";

            OpenehrConfig.openehrEndpoint = "https://plri-highmed01.mh-hannover.local:8083/rest/openehr/v1";
            OpenehrConfig.openehrUser = "etltestuser";
            OpenehrConfig.openehrPassword = "etltestuser#01";

            /*OpenehrConfig.openehrEndpoint = "https://172.0.0.1:8080/ehrbase/rest/openehr/v1";
            OpenehrConfig.openehrUser = "test";
            OpenehrConfig.openehrPassword = "test";
            OpenehrConfig.openehrAdaptor = "STANDARD";*/

            //OpenehrConfig.openehrEndpoint = Environment.GetEnvironmentVariable("OPENEHR_DB");
            //OpenehrConfig.openehrUser = Environment.GetEnvironmentVariable("OPENEHR_USER");
            //OpenehrConfig.openehrPassword = Environment.GetEnvironmentVariable("OPENEHR_PASSWD");

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
