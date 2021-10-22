using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SmICSCoreLib.Authentication
{
    public class TokenValidator
    {
        public static string GetValidatedTokenUsername(string jwtToken)
        {
            Console.WriteLine(Environment.GetEnvironmentVariable("AUTHORITY"));
            var configManager = new ConfigurationManager<OpenIdConnectConfiguration>(Environment.GetEnvironmentVariable("AUTHORITY"), new OpenIdConnectConfigurationRetriever());

            var openidconfig = configManager.GetConfigurationAsync().Result;

            //IdentityModelEventSource.ShowPII = true;

            SecurityToken validatedToken;
            TokenValidationParameters validationParameters = new TokenValidationParameters();

            validationParameters.ValidateLifetime = true;

            validationParameters.ValidAudience = "account";
            validationParameters.ValidIssuer = Environment.GetEnvironmentVariable("AUTHORITY");
            validationParameters.IssuerSigningKeys = openidconfig.SigningKeys;
            ClaimsPrincipal principal = new JwtSecurityTokenHandler().ValidateToken(jwtToken, validationParameters, out validatedToken);
            string username = principal.FindFirst("preferred_username")?.Value;
            Console.WriteLine("User: "+ username);
            return username;
        }
    }
}
