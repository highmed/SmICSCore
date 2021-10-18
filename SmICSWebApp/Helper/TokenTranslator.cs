using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace SmICSWebApp.Helper
{
    public class TokenTranslator
    {
        public static ClaimsPrincipal getPrincipal(string token)
        {
            try
            {
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                JwtSecurityToken jwtToken = (JwtSecurityToken)tokenHandler.ReadToken(token);
                if (jwtToken == null)
                    return null;
                //byte[] key = Encoding.ASCII.GetBytes(config.jwtSecret);
                TokenValidationParameters parameters = new TokenValidationParameters()
                {
                    ValidIssuer = "https://keycloak.mh-hannover.local:8443/auth/realms/Better",
                    ValidAudience = "account",
                    ValidateAudience = true,
                    RequireSignedTokens = false,
                    ValidateIssuer = true,
                    ValidateIssuerSigningKey = false,
                };
                SecurityToken securityToken;
                ClaimsPrincipal principal = tokenHandler.ValidateToken(token,
                      parameters, out securityToken);
                return principal;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
    }
}
