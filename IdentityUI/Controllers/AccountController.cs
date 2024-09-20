using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using IdentityUI.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using System.Configuration;
using System.Data.SqlClient;
using Dapper;
using System.Data;
using Interview.Repository.Interfaces;
using Newtonsoft.Json;
using System.Text;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace IdentityUI.Controllers
{
    
    public class AccountController : BaseController
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private readonly HttpClient _httpClient;
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
        ApiCallService api=new ApiCallService ();
      //  private readonly string apiBaseUrl = "https://localhost:44319/api/Auth/";
        public AccountController()
        {
            _httpClient = new HttpClient();
        }


        ILoggerRepository _iLog;
       
        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ILoggerRepository iLog)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            _iLog = iLog;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
         public ActionResult Login(string returnUrl)
         {
             ViewBag.ReturnUrl = returnUrl;
             return View();
         }
       /* [HttpGet]
        public async Task<ActionResult> Login(string returnUrl)
        {
            try
            {
                var authCookie = Request.Cookies["AccessToken"].Value;
                if (!string.IsNullOrEmpty(authCookie))
                {
                    var principal = ValidateToken(authCookie);
                    if (principal != null)
                    {
                        // Automatically sign in the user
                        var user = await UserManager.FindByIdAsync(principal.FindFirst(ClaimTypes.NameIdentifier).Value);
                        if (user != null)
                        {
                            await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                            return RedirectToLocal(returnUrl);
                        }
                    }

                }
                return View();
            }
            catch
            {
                return View();
            }
        }
        private ClaimsPrincipal ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("yourSecretKey");

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
        }*/

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
       // [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            try
            {


                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: true);

                switch (result)
                {
                    case SignInStatus.Success:
                        // Call the Web API to get the JWT token
                        using (var client = new HttpClient())
                        {

                            api = new ApiCallService();
                            client.BaseAddress = new Uri("https://localhost:44319/");
                            client.DefaultRequestHeaders.Accept.Clear();
                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                            var response = await client.PostAsJsonAsync("api/Auth/Authenticate", model);
                            if (response.IsSuccessStatusCode)
                            {
                                var tokenResponse = await response.Content.ReadAsAsync<TokenResponse>();

                                // Store access token in a cookie
                                var accessTokenCookie = new HttpCookie("AccessToken", tokenResponse.AccessToken)
                                {
                                    HttpOnly = true,
                                    Secure = true,
                                    SameSite = SameSiteMode.Strict,
                                    Expires = DateTime.UtcNow.AddDays(7)
                                };
                                Response.Cookies.Add(accessTokenCookie);

                                var refreshToken = tokenResponse.RefreshToken;
                                await api.StoreRefreshTokenAsync(model.Email, refreshToken, DateTime.UtcNow.AddMonths(1));
                                TempData["RefreshToken"] = refreshToken;
                                TempData["AccessTokenExpiry"] = DateTime.UtcNow.AddMinutes(2).ToString("o"); // ISO 8601 format

                                // Example expiry time in milliseconds

                                return RedirectToAction("Index", "Welcome");
                            }

                            //ModelState.AddModelError("", "Unable to get JWT token.");
                            return View(model);
                        }
                    case SignInStatus.LockedOut:
                        return View("Lockout");
                    case SignInStatus.RequiresVerification:
                        return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                    case SignInStatus.Failure:
                    default:
                        ModelState.AddModelError("", "Invalid login attempt.");
                        return View(model);
                }
            }
            catch (Exception ex)
            {
                _iLog.LogException(ex);
                throw new ApplicationException($"An error occurred : {ex.Message}", ex);

            }
        }
      [HttpPost]
        [AllowAnonymous]

        //[ValidateAntiForgeryToken]
      
         public async Task<ActionResult> RefreshToken(TokenResponse request)
         {
             var user = await api.GetUserByRefreshToken(request.RefreshToken);
             if (user != null)
             {
                 using (var client = new HttpClient())
                 {
                     client.BaseAddress = new Uri("https://localhost:44319/");
                     client.DefaultRequestHeaders.Accept.Clear();
                     client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                     var newresponse = await client.PostAsJsonAsync("api/Auth/Renewtoken", user);
                     if (newresponse.IsSuccessStatusCode)
                     {
                         var tokens = await newresponse.Content.ReadAsAsync<TokenResponse>();
                        var accessTokenCookie = new HttpCookie("AccessToken", tokens.AccessToken)
                         {
                             HttpOnly = true,
                             Secure = true,
                             SameSite = SameSiteMode.Strict,
                             Expires = DateTime.UtcNow.AddDays(7)
                        };
                         Response.Cookies.Add(accessTokenCookie);
                        // return Json(new { success = true, accessToken = tokens.AccessToken }, JsonRequestBehavior.AllowGet);
                        var response = new
                        {
                            Success = true,
                            AccessToken = tokens.AccessToken,
                            RefreshToken = tokens.RefreshToken,
                            expiresIn = 120
                        };

                        return Json(response, JsonRequestBehavior.AllowGet);

                    }                    
                 }

             }
             return Json(new { success = false }, JsonRequestBehavior.AllowGet);
         }

        // POST: /Account/GetValuesResource
        //  [HttpPost]
        [AllowAnonymous]
       // [ValidateAntiForgeryToken]
        public  async Task<ActionResult> GetValuesResource(LoginViewModel model)
       {
            //return  RedirectToAction ("Index","Profile");

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44319/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Retrieve access token from cookie
               var accessTokenCookie = Request.Cookies["AccessToken"];
                var accessToken = accessTokenCookie?.Value;
                string token = null;
                if (!string.IsNullOrEmpty(accessToken))
                {
                    token = accessToken;
                }
               /* else
                {
                    var refreshtoken = await api.GetRefreshTokenAsync(model.Email);
                    if (refreshtoken != null)
                    {
                        var newresponse = await client.PostAsJsonAsync("api/Auth/Renewtoken", refreshtoken);
                        if (newresponse.IsSuccessStatusCode)
                        {
                            token = await newresponse.Content.ReadAsAsync<string>();
                            accessTokenCookie = new HttpCookie("AccessToken", token)
                            {
                                HttpOnly = true,
                                Secure = true,
                                SameSite = SameSiteMode.Strict,
                                Expires = DateTime.UtcNow.AddMinutes(2)
                            };
                            Response.Cookies.Add(accessTokenCookie);
                        }
                    }
                }*/
                

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await client.GetAsync("api/values");//get
                if (response.IsSuccessStatusCode)
                {
                    var values = await response.Content.ReadAsAsync<IEnumerable<string>>();
                    // Handle the values
                    return View(values); // Pass data to the view or process it
                }
                else if(response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    // Token might be expired, handle token renewal
                    ModelState.AddModelError("", "Resource not Found / Not Authorized");
                }
            }
            return View();
       }
             //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            


                if (ModelState.IsValid)
                {
                    
                    var user = new ApplicationUser
                    {
                        UserName = model.Email,
                        Email = model.Email,
                        LockoutEnabled = true, // Allow lockout
                        LockoutEnd = null,     // No lockout at registration
                        AccessFailedCount = 0  // Reset failed attempts
                    };
                    var result = await UserManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        // Assign role to user
                        string roleName = "User"; // Replace with your desired role
                        var roleResult = await UserManager.AddToRoleAsync(user.Id, roleName);
                        if (roleResult.Succeeded)
                        {
                            await SignInManager.SignInAsync(user, isPersistent: true, rememberBrowser: true);
                            // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                            // Send an email with this link
                            // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                            // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                            // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
                            // Redirect to the home page or another page
                            return RedirectToAction("Index", "Welcome");
                        }
                        else
                        {
                            // Handle role assignment failure
                            AddErrors(roleResult);
                        }
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }

                // If we got this far, something failed, redisplay form
                return View(model);

            
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
       // [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Profile");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Profile");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}