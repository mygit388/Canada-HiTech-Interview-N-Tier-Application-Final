using IdentityUI.Models;
using IdentityUI.ServiceStores;
using Interview.Service;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace IdentityUI
{
    public class MvcApplication : System.Web.HttpApplication
    {
        [Obsolete]
        protected void Application_Start()
        {
         
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            LogManager.LoadConfiguration(Server.MapPath("~/NLog.config"));
            //GlobalFilters.Filters.Add(new MyHandleErrorAttribute());
            DependencyConfigurations.RegisterDependencies();
            //GlobalFilters.Filters.Add(new JwtAuthoriseAttribute()); 
             AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;
            try
            {
                var rolesCreatedTask = CreateRoles();
                bool rolesCreatedSuccessfully = rolesCreatedTask.Wait(5000); // Wait for the task to complete (with timeout)

                if (!rolesCreatedSuccessfully)
                {
                    // Handle the case where role creation failed
                    Console.WriteLine("Role creation failed.");
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during role creation
                Console.WriteLine("An error occurred while creating roles: " + ex.Message);
            }
        }
        private async Task<bool> CreateRoles()
        {
            try
            {
                using (var roleManager = new ApplicationRoleManager(new CustomRoleStore(ConfigurationManager.ConnectionStrings["con"].ConnectionString)))
                {
                    bool rolesCreatedSuccessfully = true;

                    // Check if the "Admin" role exists
                    var adminRole = await roleManager.FindByNameAsync("Admin");
                    if (adminRole == null)
                    {
                        var role = new ApplicationRole { Name = "Admin" };
                        var result = await roleManager.CreateAsync(role);
                        if (!result.Succeeded)
                        {
                            // Handle creation failure
                            Console.WriteLine("Failed to create Admin role.");
                            rolesCreatedSuccessfully = false;
                        }
                    }

                    // Check if the "User" role exists
                    var userRole = await roleManager.FindByNameAsync("User");
                    if (userRole == null)
                    {
                        var role = new ApplicationRole { Name = "User" };
                        var result = await roleManager.CreateAsync(role);
                        if (!result.Succeeded)
                        {
                            // Handle creation failure
                            Console.WriteLine("Failed to create User role.");
                            rolesCreatedSuccessfully = false;
                        }
                    }

                    return rolesCreatedSuccessfully;
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during role creation
                Console.WriteLine("An error occurred while creating roles: " + ex.Message);
                return false;
            }
        }
        public void Application_Error()
        {
            // rowog unhandled exceptions
            var exception = Server.GetLastError();
            LogManager.GetCurrentClassLogger().Error(exception, "Unhandled exception");
            Server.ClearError();
           // Response.Redirect("Error");


        }

    }
    
}
