using Microsoft.AspNet.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace IdentityUI.Controllers
{
    public abstract class BaseController : Controller
    {
        private readonly string secretKey = "MN05OPLoDVbTTa/QkqLNMI7cPLguaRyHzyg7n5qNBVjQmtBhz4SzYh4NBVCXi3KJHISXKP+oi2+bXr6CUYTR";

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!User.Identity.IsAuthenticated)
            {
                var token = Request.Cookies["AccessToken"]?.Value;
                if (!string.IsNullOrEmpty(token))
                {
                    var principal = ValidateToken(token);
                    if (principal != null)
                    {
                        var identity = new ClaimsIdentity(principal.Claims, DefaultAuthenticationTypes.ApplicationCookie);
                        var authenticationManager = HttpContext.GetOwinContext().Authentication;
                        authenticationManager.SignIn(new AuthenticationProperties { IsPersistent = true }, identity);
                    }
                }
            }
            base.OnActionExecuting(filterContext);
        }

        private ClaimsPrincipal ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(secretKey);

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
                var principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
                return principal;
            }
            catch
            {
                return null;
            }
        }
    }

}