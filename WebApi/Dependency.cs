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
using WebApi.App_Start;
using WebApi.Controllers;
using WebApi.JwtServices;
using WebApi.Models;

namespace WebApi
{
    public class Dependency
    {
        public static void RegisterDependencies()
        {

            string connectionString = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
            var container = new UnityContainer();
           
            container.RegisterFactory<IAuthenticationManager>(c => HttpContext.Current.GetOwinContext().Authentication, new PerRequestLifetimeManager());

            // Register your controllers here (if not already done)
           
            container.RegisterType<IUserStore<ApplicationUser>, ApiUserStore>(new InjectionConstructor(connectionString));
           container.RegisterType<UserManager<ApplicationUser>, ApplicationUserManager>();
            container.RegisterType<SignInManager<ApplicationUser, string>, ApplicationSignInManager>();
            //container.RegisterType<JwtTokenHelper>();
            container.RegisterType<jwtMiddlewareAttribute>();
            // Register ApplicationUserManager
            //container.RegisterType<ApplicationUserManager>();
            //container.RegisterType<ApplicationSignInManager>();
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}