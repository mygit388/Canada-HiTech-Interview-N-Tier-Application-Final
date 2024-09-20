using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using WebApi.Models;

namespace WebApi.App_Start
{
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
            
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["con"].ConnectionString;

            // Create an instance of CustomUserStore with the connection string
            var store = new ApiUserStore(connectionString);

            // Create and configure ApplicationUserManager
            var manager = new ApplicationUserManager(store);

            
            return manager;
        }
    }

    // Configure the application sign-in manager which is used in this application.
    
   
}