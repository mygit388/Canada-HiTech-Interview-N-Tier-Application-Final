using IdentityUI.Controllers;
using IdentityUI.Models;
using IdentityUI.ServiceStores;
using Interview.Repository.Implementations;
using Interview.Repository.Interfaces;
using Interview.Service.Implementations;
using Interview.Service.Interfaces;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Unity;
using Unity.AspNet.Mvc;
using Unity.Injection;

namespace IdentityUI
{
    public class DependencyConfigurations
    {
        public static void RegisterDependencies()
        {

            string connectionString = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
            var container = new UnityContainer();
            container.RegisterType<IProfileRepository, ProfileRepository>();
            container.RegisterType<ITaskRepository, TaskRepository>();
            container.RegisterType<ILoggerRepository, LoggerRepository>();
            container.RegisterType<IProfileService, ProfileService>();
            container.RegisterType<ITaskService, TaskService>();
            container.RegisterFactory<IAuthenticationManager>(c => HttpContext.Current.GetOwinContext().Authentication, new PerRequestLifetimeManager());

            // Register your controllers here (if not already done)
            container.RegisterType<AccountController>();
            container.RegisterType<ProfileController>();
           container.RegisterType<ILoggerService, LoggerService>();
            // Register CustomUserStore with connection string parameter
            container.RegisterType<IUserStore<ApplicationUser>, CustomUserStore>(new InjectionConstructor(connectionString));
            container.RegisterType<IUserLockoutStore<ApplicationUser,string>, CustomUserStore>(new InjectionConstructor(connectionString));
            container.RegisterType<UserManager<ApplicationUser>, ApplicationUserManager>();
            container.RegisterType<SignInManager<ApplicationUser,string>, ApplicationSignInManager>();
            // Register ApplicationUserManager
             container.RegisterType<ApplicationUserManager>();
            container.RegisterType<ApplicationSignInManager>();
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}