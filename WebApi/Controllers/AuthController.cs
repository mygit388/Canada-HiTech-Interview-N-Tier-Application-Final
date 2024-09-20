using Dapper;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using WebApi.App_Start;
using WebApi.JwtServices;
using WebApi.Models;

namespace WebApi.Controllers
{
    public class AuthController : ApiController
    {       
        private static string secretKey = ConfigurationManager.AppSettings["JwtSecretKey"];
        private static readonly string Issuer = ConfigurationManager.AppSettings["JwtIssuer"];
        private static readonly string Audience =ConfigurationManager.AppSettings["JwtAudience"];
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["con"].ConnectionString;


        JwtTokenHelper _jwtTokenHelper = new JwtTokenHelper();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AuthController(ApplicationUserManager  userManager,  ApplicationSignInManager signInManager)
        {

            UserManager = userManager;
            SignInManager = signInManager;
            
        }
       
        public AuthController()
        {
            // Parameterless constructor
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }

        }
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.Current.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }

        }
        // GET: Login
        [HttpPost]
        [Route("api/Auth/Authenticate")]

        //[Route("api/[controller]")]

      
        public async Task<IHttpActionResult> Authenticate([FromBody] LoginModel model)
        {
            try
            {
                   
                var user = await UserManager.FindByEmailAsync(model.Email);
                if (user != null && await UserManager.CheckPasswordAsync(user , model.Password))
                {
                    var tokens = _jwtTokenHelper.GenerateTokens(user);
                    return Ok(new TokenResponse
                    {
                        AccessToken = tokens.AccessToken,
                        RefreshToken = tokens.RefreshToken
                    });
                }
               
                return Unauthorized();
            }
            catch(Exception ex)
            {
                throw new ApplicationException($"An error occurred : {ex.Message}", ex);
            }
        }
        [HttpPost]
        [Route("api/Auth/Renewtoken")]
        public  IHttpActionResult Renewtoken([FromBody] ApplicationUser user)
        {
            if (user == null)
            {
                return BadRequest("Invalid client request");
            }

            //var principal = _jwtTokenHelper.GetPrincipal(token);
            //var UserName = principal.Identity.Name;
           // var user = await _userManager.FindByNameAsync(UserName);
            // if (user == null || !await _jwtTokenHelper.ValidateRefreshToken(refreshtoken, user))
            //{
           //     return Unauthorized();
           // }

            var newaccessToken =  _jwtTokenHelper.GenerateTokens(user);

            return Ok(new TokenResponse
            {
                AccessToken = newaccessToken.AccessToken,
                RefreshToken = newaccessToken.RefreshToken
            });


        }
    }
   
}

