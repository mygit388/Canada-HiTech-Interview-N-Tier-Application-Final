using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using WebApi.App_Start;

[assembly: OwinStartup(typeof(WebApi.Startup))]
namespace WebApi
{
    public class Startup
    {
             
           
        public void Configuration(IAppBuilder app)
        {

          

            HttpConfiguration config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();
         
            ConfigureAuth(app);
            // Other configuration code...
         }

        private void ConfigureAuth(IAppBuilder app)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["con"].ConnectionString;

            // Store the connection string in the OWIN context
            app.Properties["ConnectionString"] = connectionString;

            // Set up OWIN context and ApplicationUserManager
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            // Configure Web API for self-host. 
           
          /*  var issuer = System.Configuration.ConfigurationManager.AppSettings["JwtIssuer"];
            var audience = System.Configuration.ConfigurationManager.AppSettings["JwtAudience"];
            var secretKey = System.Configuration.ConfigurationManager.AppSettings["JwtSecretKey"];
            var key = Encoding.UTF8.GetBytes(secretKey);

           
            app.UseJwtBearerAuthentication(new JwtBearerAuthenticationOptions
            {
                AuthenticationMode = AuthenticationMode.Active,
                TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                   
                },
             
            });*/
        }
    }
}