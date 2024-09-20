using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace WebApi.JwtServices
{
    public class jwtMiddlewareAttribute : AuthorizeAttribute
    {
        private static string secretKey = ConfigurationManager.AppSettings["JwtSecretKey"];
        private static readonly string Issuer = ConfigurationManager.AppSettings["JwtIssuer"];
        private static readonly string Audience = ConfigurationManager.AppSettings["JwtAudience"];

        private readonly string connectionString = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
       
        protected override bool IsAuthorized(HttpActionContext actionContext)
        //  protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var authHeader = actionContext.Request.Headers.Authorization;

            if (authHeader != null && authHeader.Scheme == "Bearer")
            {
                var token = authHeader.Parameter;
               
                    var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(secretKey);

                try
                {
                    JwtTokenHelper tk = new JwtTokenHelper();
                    var principal = tk.GetPrincipal(token);
                    actionContext.RequestContext.Principal = principal;
                    return true;
                }
                catch (SecurityTokenException Ex)
              
                {
                    Console.WriteLine(Ex);
                    // Token validation failed
                    return false;
                }
            }

            // No token found
            return false;

        }

        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
        }
    }
}