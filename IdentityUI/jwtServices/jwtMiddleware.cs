using Microsoft.AspNet.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IdentityUI.jwtServices
{
    

    public class jwtMiddleware: OwinMiddleware
    {
        private readonly string secretKey = "MN05OPLoDVbTTa/QkqLNMI7cPLguaRyHzyg7n5qNBVjQmtBhz4SzYh4NBVCXi3KJHISXKP+oi2+bXr6CUYTR";

        public jwtMiddleware(OwinMiddleware next) : base(next) { }

        public async override Task Invoke(IOwinContext context)
        {
            var authCookie = context.Request.Cookies["AccessToken"];
            if (!string.IsNullOrEmpty(authCookie))
            {
                var principal = ValidateToken(authCookie);
                if (principal != null)
                {
                    //context.Request.User = principal;
                    
                    var identity = new ClaimsIdentity(principal.Claims, DefaultAuthenticationTypes.ApplicationCookie);
                                      
                    context.Authentication.SignIn(new AuthenticationProperties { IsPersistent = true }, identity);

                }
            }

            await Next.Invoke(context);
        }

        private ClaimsPrincipal ValidateToken(string authCookie)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                SecurityToken validatedToken;
                var principal = tokenHandler.ValidateToken(authCookie, validationParameters, out validatedToken);
                
                // Print or use the claims as needed
               
                return principal;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }
       
    }

  
    }