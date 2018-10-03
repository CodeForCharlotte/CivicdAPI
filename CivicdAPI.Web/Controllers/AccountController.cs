//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Security.Claims;
//using System.Security.Cryptography;
//using System.Text.Encodings.Web;
//using System.Threading.Tasks;
//using CivicdAPI.Web.Entities;
//using CivicdAPI.Web.Models;
//using CivicdAPI.Web.Services;
//using Microsoft.AspNetCore.Authentication;
//using Microsoft.AspNetCore.Authentication.Cookies;
//using Microsoft.AspNetCore.Authentication.OAuth;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Logging;

//namespace CivicdAPI.Web.Controllers
//{
//    [Authorize]
//    [Route("api/Account")]
//    public class AccountController : ControllerBase
//    {
//        private const string LocalLoginProvider = "Local";
//        private readonly UserManager<ApplicationUser> _userManager;
//        private readonly SignInManager<ApplicationUser> _signInManager;
//        private readonly ILogger<UserManager<ApplicationUser>> _logger;
//        private readonly EmailService _emailService;

//        public AccountController(UserManager<ApplicationUser> userManager,
//            SignInManager<ApplicationUser> signInManager,
//            ILogger<UserManager<ApplicationUser>> logger,
//            EmailService emailService,
//            ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
//        {
//            _userManager = userManager;
//            _signInManager = signInManager;
//            _logger = logger;
//            _emailService = emailService;
//            AccessTokenFormat = accessTokenFormat;
//        }

//        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }

//        // GET api/Account/UserInfo
//        [Route("UserInfo")]
//        public UserInfoViewModel GetUserInfo()
//        {
//            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

//            return new UserInfoViewModel
//            {
//                Email = User.Identity.Name,
//                HasRegistered = externalLogin == null,
//                LoginProvider = externalLogin != null ? externalLogin.LoginProvider : null
//            };
//        }

//        // POST api/Account/Logout
//        [Route("Logout")]
//        public async Task<IActionResult> OnPost(string returnUrl = null)
//        {
//            await _signInManager.SignOutAsync();
//            _logger.LogInformation("User logged out.");
//            if (returnUrl != null)
//            {
//                return LocalRedirect(returnUrl);
//            }
//            else
//            {
//                return new OkResult();
//            }
//        }

//        // GET api/Account/ManageInfo?returnUrl=%2F&generateState=true
//        [Route("ManageInfo")]
//        public async Task<ManageInfoViewModel> GetManageInfo(string returnUrl, bool generateState = false)
//        {
//            IdentityUser user = await _userManager.FindByEmailAsync(User.Identity.Name);

//            if (user == null)
//            {
//                return null;
//            }

//            List<UserLoginInfoViewModel> logins = new UserLoginInfoViewModel
//            {
//                LoginProvider = user.LoginProvider,
//                ProviderKey = user.ProviderKey
//            };

//            if (user.PasswordHash != null)
//            {
//                logins.Add(new UserLoginInfoViewModel
//                {
//                    LoginProvider = LocalLoginProvider,
//                    ProviderKey = user.UserName,
//                });
//            }

//            return new ManageInfoViewModel
//            {
//                LocalLoginProvider = LocalLoginProvider,
//                Email = user.UserName,
//                Logins = logins,
//                ExternalLoginProviders = GetExternalLogins(returnUrl, generateState)
//            };
//        }

//        // POST api/Account/ChangePassword
//        [Route("ChangePassword")]
//        public async Task<IActionResult> ChangePassword(ChangePasswordBindingModel model)
//        {
//            if (!ModelState.IsValid)
//            {
//                return BadRequest(ModelState);
//            }
//            var applicationUser = new ApplicationUser
//            {
//                Email = User.Identity.Name
//            };
//            IdentityResult result = await _userManager.ChangePasswordAsync(applicationUser, model.OldPassword,
//              model.NewPassword);

//            if (!result.Succeeded)
//            {
//                return GetErrorResult(result);
//            }

//            return Ok();
//        }

//        // POST api/Account/SetPassword
//        [Route("SetPassword")]
//        public async Task<IActionResult> SetPassword(SetPasswordBindingModel model)
//        {
//            if (!ModelState.IsValid)
//            {
//                return BadRequest(ModelState);
//            }
//            var applicationUser = new ApplicationUser
//            {
//                Email = User.Identity.Name
//            };
//            IdentityResult result = await _userManager.AddPasswordAsync(applicationUser, model.NewPassword);

//            if (!result.Succeeded)
//            {
//                return GetErrorResult(result);
//            }

//            return Ok();
//        }

//        // POST api/Account/AddExternalLogin
//        [Route("AddExternalLogin")]
//        public async Task<IActionResult> AddExternalLogin(AddExternalLoginBindingModel model)
//        {
//            if (!ModelState.IsValid)
//            {
//                return BadRequest(ModelState);
//            }

//            await _signInManager.SignOutAsync();

//            AuthenticationTicket ticket = AccessTokenFormat.Unprotect(model.ExternalAccessToken);

//            if (ticket == null || ticket.Principal.Identity == null || (ticket.Properties != null
//                                                              && ticket.Properties.ExpiresUtc.HasValue
//                                                              && ticket.Properties.ExpiresUtc.Value < DateTimeOffset.UtcNow))
//            {
//                return BadRequest("External login failure.");
//            }

//            ExternalLoginData externalData = ExternalLoginData.FromIdentity(ticket.Principal.Identity);

//            if (externalData == null)
//            {
//                return BadRequest("The external login is already associated with an account.");
//            }

//            IdentityResult result = await _userManager.AddLoginAsync(User.Identity.Name,
//              new UserLoginInfo(externalData.LoginProvider, externalData.ProviderKey, User.Identity.Name));

//            if (!result.Succeeded)
//            {
//                return GetErrorResult(result);
//            }

//            return Ok();
//        }

//        // POST api/Account/RemoveLogin
//        [Route("RemoveLogin")]
//        public async Task<IActionResult> RemoveLogin(RemoveLoginBindingModel model)
//        {
//            if (!ModelState.IsValid)
//            {
//                return BadRequest(ModelState);
//            }

//            IdentityResult result;

//            if (model.LoginProvider == LocalLoginProvider)
//            {
//                result = await _userManager.RemovePasswordAsync((ApplicationUser)User.Identity);
//            }
//            else
//            {
//                result = await _userManager.RemoveLoginAsync((ApplicationUser)User.Identity, model.LoginProvider, model.ProviderKey);

//                return new OkObjectResult(new UserLoginInfo(model.LoginProvider, model.ProviderKey, User.Identity.Name));
//            }

//            if (!result.Succeeded)
//            {
//                return GetErrorResult(result);
//            }

//            return Ok();
//        }

//        // GET api/Account/ExternalLogin
//        //[OverrideAuthentication]
//        //[HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
//        [AllowAnonymous]
//        [Route("ExternalLogin", Name = "ExternalLogin")]
//        public async Task<IActionResult> GetExternalLogin(string provider, string error = null)
//        {
//            if (error != null)
//            {
//                return Redirect(Url.Content("~/") + "#error=" + Uri.EscapeDataString(error));
//            }

//            if (!User.Identity.IsAuthenticated)
//            {
//                return new ChallengeResult(provider, this);
//            }

//            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

//            if (externalLogin == null)
//            {
//                return new StatusCodeResult(500);
//            }

//            if (externalLogin.LoginProvider != provider)
//            {
//                await _signInManager.SignOutAsync();
//                return new ChallengeResult(provider);
//            }

//            ApplicationUser user = await _userManager.FindByLoginAsync(externalLogin.LoginProvider, externalLogin.ProviderKey);

//            bool hasRegistered = user != null;

//            if (hasRegistered)
//            {
//                await _signInManager.SignOutAsync();

//                var oAuthIdentity = await user.GenerateUserIdentityAsync(_userManager, "Bearer");
//                var cookieIdentity = await user.GenerateUserIdentityAsync(_userManager,
//                  "Cookies");

//                AuthenticationProperties properties = ApplicationOAuthProvider.CreateProperties(user.UserName);
//                _signInManager.SignInAsync(properties, oAuthIdentity, cookieIdentity);
//            }
//            else
//            {
//                IEnumerable<Claim> claims = externalLogin.GetClaims();
//                ClaimsIdentity identity = new ClaimsIdentity(claims, "Bearer");
//                Authentication.SignIn(identity);
//            }

//            return Ok();
//        }

//        // GET api/Account/ExternalLogins?returnUrl=%2F&generateState=true
//        [AllowAnonymous]
//        [Route("ExternalLogins")]
//        public IEnumerable<ExternalLoginViewModel> GetExternalLogins(string returnUrl, bool generateState = false)
//        {
//            IEnumerable<AuthenticationDescription> descriptions = Authentication.GetExternalAuthenticationTypes();
//            List<ExternalLoginViewModel> logins = new List<ExternalLoginViewModel>();

//            string state;

//            if (generateState)
//            {
//                const int strengthInBits = 256;
//                state = RandomOAuthStateGenerator.Generate(strengthInBits);
//            }
//            else
//            {
//                state = null;
//            }

//            foreach (AuthenticationDescription description in descriptions)
//            {
//                ExternalLoginViewModel login = new ExternalLoginViewModel
//                {
//                    Name = description.Caption,
//                    Url = Url.Route("ExternalLogin", new
//                    {
//                        provider = description.AuthenticationType,
//                        response_type = "token",
//                        client_id = Startup.PublicClientId,
//                        redirect_uri = new Uri(Request.RequestUri, returnUrl).AbsoluteUri,
//                        state = state
//                    }),
//                    State = state
//                };
//                logins.Add(login);
//            }

//            return logins;
//        }

//        // POST api/Account/Register
//        [AllowAnonymous]
//        [Route("Register")]
//        public async Task<ActionResult> Register(ApplicationUser model)
//        {
//            var returnUrl = Url.Content("~/");

//            if (!ModelState.IsValid)
//            {
//                return BadRequest(ModelState);
//            }

//            var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
//            var result = await _userManager.CreateAsync(user, model.PasswordHash);
//            if (result.Succeeded)
//            {
//                _logger.LogInformation("User created a new account with password.");

//                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
//                var callbackUrl = Url.Page(
//                    "/Account/ConfirmEmail",
//                    pageHandler: null,
//                    values: new { userId = user.Id, code = code },
//                    protocol: Request.Scheme);

//                await _emailService.SendAsync(new IdentityMessage()
//                {
//                    Subject = "Confirm your email",
//                    Body = $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.",
//                    Destination = model.Email
//                });
//            }
//            await _signInManager.SignInAsync(user, isPersistent: false);
//            return LocalRedirect(returnUrl);

//        }

//        // POST api/Account/RegisterExternal
//        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
//        [Route("RegisterExternal")]
//        public async Task<IActionResult> RegisterExternal(RegisterExternalBindingModel model)
//        {
//            if (!ModelState.IsValid)
//            {
//                return BadRequest(ModelState);
//            }

//            var info = await _signInManager.GetExternalLoginInfoAsync();
//            if (info == null)
//            {
//                return new StatusCodeResult(500);
//            }

//            var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };

//            IdentityResult result = await _userManager.CreateAsync(user);
//            if (!result.Succeeded)
//            {
//                return GetErrorResult(result);
//            }

//            result = await _userManager.AddLoginAsync(user, new UserLoginInfo(info.LoginProvider, info.ProviderKey, user.Email));
//            if (!result.Succeeded)
//            {
//                return GetErrorResult(result);
//            }
//            return Ok();
//        }


//        #region Helpers

//        private IActionResult GetErrorResult(IdentityResult result)
//        {
//            if (result == null)
//            {
//                return new StatusCodeResult(500);
//            }

//            if (!result.Succeeded)
//            {
//                if (result.Errors != null)
//                {
//                    foreach (var error in result.Errors)
//                    {
//                        ModelState.AddModelError(error.Code, error.Description);
//                    }
//                }

//                if (ModelState.IsValid)
//                {
//                    // No ModelState errors are available to send, so just return an empty BadRequest.
//                    return BadRequest();
//                }

//                return BadRequest(ModelState);
//            }

//            return null;
//        }

//        #endregion
//    }
//}