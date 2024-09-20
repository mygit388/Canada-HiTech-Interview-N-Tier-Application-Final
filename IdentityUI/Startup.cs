using IdentityUI.jwtServices;
using Microsoft.Owin;
using Owin;
using System.Configuration;

[assembly: OwinStartup(typeof(IdentityUI.Startup))]
namespace IdentityUI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["con"].ConnectionString;

            // Store the connection string in the OWIN context
            app.Properties["ConnectionString"] = connectionString;

            // Set up OWIN context and ApplicationUserManager
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);
            app.CreatePerOwinContext<ApplicationRoleManager>(ApplicationRoleManager.Create);
            app.Use<jwtMiddleware>();
            ConfigureAuth(app);
        }

    }
}
