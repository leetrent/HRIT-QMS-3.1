using System;
using System.Text;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using AspNetCore.LegacyAuthCookieCompat;
using QmsCore.UIModel;
using QmsCore.Services;
using QMS.Extensions;
using QMS.Constants;
using QMS.Utils;
using QMS.ViewModels;
using QmsCore.QmsException;

namespace QMS.Controllers
{
    public class LoginController : Controller
    {
        private readonly IConfiguration _config;
        private readonly IStringLocalizer<LoginController> _localizer;
        private readonly IWebHostEnvironment _hostingEnv;
        private readonly IUserService _userService;
        private readonly IMenuBuilderService _menuBuilderService;

        public LoginController
        (
            IConfiguration configuration,
            IStringLocalizer<LoginController> localizer,
            IWebHostEnvironment hostEnv,
            IUserService usrSvc,
            IMenuBuilderService mbSvc
        )
        {
            _config = configuration;
            _localizer = localizer;
            _hostingEnv = hostEnv;
            _userService = usrSvc;
            _menuBuilderService = mbSvc;
         }

        public IActionResult UnauthorizedUser()
        {
            return View();
        }

        [Route("Login/LoginAsync")]
        public async Task<IActionResult> LoginAsync()
        {
            

            string logSnippet   = new StringBuilder("[")
                                .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                                .Append("][LoginController][LoginAsync] => ")
                                .ToString();


            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // BEGIN: TEMPORARY
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // string compatabilityMode = _config.GetValue<string>(MiscConstants.ENCRYPTION_COMPATABILITY_MODE);
            // Console.WriteLine(logSnippet + $"(compatabilityMode IsNullOrEmpty).....: {String.IsNullOrEmpty(compatabilityMode)}");
            // Console.WriteLine(logSnippet + $"(compatabilityMode IsNullOrWhiteSpace): {String.IsNullOrWhiteSpace(compatabilityMode)}");
            // Console.WriteLine(logSnippet + $"(compatabilityMode)...................: '{compatabilityMode}'");
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // END: TEMPORARY
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
          
            bool waringAgreedTo = HttpContext.Session.GetObject<bool>(UserConstants.WARNING_AGREED_TO);
            if ( waringAgreedTo == false)
            {
                Console.WriteLine(logSnippet + $"Warning HAS NOT been agreed to. Redirecting to the Warning page.");
                return RedirectToAction("Warning", "Home");
            }

            Console.WriteLine(logSnippet + $"Warning has been agreed to. Proceeding with user authentication.");
            Console.WriteLine(logSnippet + $"(_hostingEnv.EnvironmentName): '{_hostingEnv.EnvironmentName}'");
            Console.WriteLine(logSnippet + $"(HttpContext.User.Identity.IsAuthenticated): {HttpContext.User.Identity.IsAuthenticated}");

            string requestedUriTest = HttpContext.Session.GetObject<string>(MiscConstants.REQUESTED_URI);
            Console.WriteLine(logSnippet + $"(requestedUriTest) '{requestedUriTest}'");

            Console.WriteLine(logSnippet + $"(_hostingEnv.EnvironmentName): '{_hostingEnv.EnvironmentName}'");

            if (HttpContext.User.Identity.IsAuthenticated)
            {
                Console.WriteLine(logSnippet + $"'{HttpContext.User.Identity.Name}' has been authenticated, redirecting to [HomeController][Index]");
                return RedirectToAction("Index", "Home");
            }
            else if (_hostingEnv.IsDevelopment())
            {
                string localhostEmail = Environment.GetEnvironmentVariable("LOCALHOST_EMAIL");
                string localhostName  = Environment.GetEnvironmentVariable("LOCALHOST_NAME");
                Console.WriteLine(logSnippet + $"Bypassing SecureAuth and authenticating as [{localhostName}][{localhostEmail}]");

                ////////////////////////////////////////////////////////////////////////////
                // SecUser 
                ////////////////////////////////////////////////////////////////////////////
                User qmsUser = _userService.RetrieveByEmailAddress(localhostEmail);
                var claimsPrincipal = this.CreateClaimsPrincipal(qmsUser, localhostName, localhostEmail);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
                HttpContext.Session.SetObject(MiscConstants.USER_SESSION_KEY, qmsUser);
                HttpContext.Session.SetObject(MiscConstants.USER_SESSION_VM_KEY, UserUtil.MapToViewModel(qmsUser));

                ////////////////////////////////////////////////////////////////////////////
                // Place MenuItemViewModel List in Session
                ////////////////////////////////////////////////////////////////////////////
                //HashSet<MenuItemViewModel> menuBuilder = MenuUtil.GetMenuItemsforUser(qmsUser);
                //HttpContext.Session.SetObject(MiscConstants.MENU_SESSION_KEY, menuBuilder);

                ////////////////////////////////////////////////////////////////////////////
                // Place ModuleMenuItem in Session
                ////////////////////////////////////////////////////////////////////////////
                List<ModuleMenuItem> moduleMenuItems = _menuBuilderService.RetrieveMenuForUser(qmsUser.UserId);
                HttpContext.Session.SetObject(MiscConstants.MODULE_MENU_ITEMS_SESSION_KEY, moduleMenuItems);
                this.debug(moduleMenuItems, qmsUser.UserId);




                string requestedUri = HttpContext.Session.GetObject<string>(MiscConstants.REQUESTED_URI);

                Console.WriteLine(logSnippet + $"(Request.Protocol): '{Request.Protocol}'");
                Console.WriteLine(logSnippet + $"(Request.Host)....: '{Request.Host}'");
                Console.WriteLine(logSnippet + $"(Request.PathBase): '{Request.PathBase}'");
                Console.WriteLine(logSnippet + $"(requestedUri)....: '{requestedUri}'");

                //string redirectUrl  = $"https://dev-hrqms.gsa.gov/{requestedUri}";
                string redirectUrl  = $"https://{Request.Host}{Request.PathBase}/{requestedUri}";
                Console.WriteLine(logSnippet + $"(redirectUrl).....: '{redirectUrl}'");   

                if ( string.IsNullOrEmpty(requestedUri) == false
                        && string.IsNullOrWhiteSpace(requestedUri) == false )
                        // && Url.IsLocalUrl(requestedUri) )
                {
                    HttpContext.Session.Remove(MiscConstants.REQUESTED_URI);
                    //return LocalRedirect(requestedUri);
                    return Redirect(redirectUrl);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }                
            }
            else
            {
                Console.WriteLine(logSnippet + "User has not been authenticated, redirecting to SecureAuth based on parameters in the config file.");
                var samlEndpoint = _config["SecureAuth:RedirectURL"];
                Console.WriteLine(logSnippet + $"(_hostingEnv.EnvironmentName): '{_hostingEnv.EnvironmentName}'");
                Console.WriteLine(logSnippet + $"Redirecting to '{samlEndpoint}'");
                return Redirect(samlEndpoint);
            }
        }

        [Route("Login/LogoutAsync")]
        public async Task<IActionResult> LogoutAsync()
        {
            string logSnippet   = new StringBuilder("[")
                                .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                                .Append("][LoginController][LogoutAsync] => ")
                                .ToString();

            UserViewModel qmsUserVM = HttpContext.Session.GetObject<UserViewModel>(MiscConstants.USER_SESSION_VM_KEY);

            ////////////////////////////////////////////////////////////
            // Remove SecUser from Session
            ////////////////////////////////////////////////////////////
            if ( qmsUserVM != null)
            {
                Console.WriteLine(logSnippet + $"Removing '{qmsUserVM.EmailAddress}' from HttpSession");
            }
            
            HttpContext.Session.Remove(MiscConstants.USER_SESSION_VM_KEY);

            ////////////////////////////////////////////////////////////
            // Remove UIMenuBuilder from Session
            ////////////////////////////////////////////////////////////
            if ( qmsUserVM != null)
            {
                Console.WriteLine(logSnippet + $"Removing UIMenuBuilder belonging to '{qmsUserVM.EmailAddress}' from HttpSession.");
            }

            //HttpContext.Session.Remove(MiscConstants.MENU_SESSION_KEY);
            HttpContext.Session.Remove(MiscConstants.MODULE_MENU_ITEMS_SESSION_KEY);

            ////////////////////////////////////////////////////////////
            // Remove 'Warning Agreed To' from Session
            ////////////////////////////////////////////////////////////
            if ( qmsUserVM != null)
            {
                Console.WriteLine(logSnippet + $"Removing 'Warning Agreed To' boolean for '{qmsUserVM.EmailAddress}' from HttpSession.");
            }

            HttpContext.Session.Remove(UserConstants.WARNING_AGREED_TO);

            if ( qmsUserVM != null)
            {
                Console.WriteLine(logSnippet + $"Signing out '{qmsUserVM.EmailAddress}' from HttpContext.");
            }

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            
            if ( qmsUserVM != null)
            {
                Console.WriteLine(logSnippet + $"Logout complete for '{qmsUserVM.EmailAddress}', redirecting to [HomeController][Index].");
            }

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> ExternalLoginCallback()  
        {
            User qmsUser = null;
            string logSnippet   = new StringBuilder("[")
                                .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                                .Append("][LoginController][ExternalLoginCallback] => ")
                                .ToString();

            /////////////////////////////////////////////////////////////////////////////
            // This is the SecureAuth callback, which is reached by a 302. The token that
            // SecureAuth sends is a cookie.
            /////////////////////////////////////////////////////////////////////////////
            var tokenName = _config.GetValue<string>("SecureAuth:TokenName");
            Console.WriteLine(logSnippet + $"(SecureAuth:TokenName IS NULL): {tokenName == null}");
            Console.WriteLine(logSnippet + $"(tokenName): '{tokenName}'");

            string token = Request.Cookies[tokenName];
            Console.WriteLine(logSnippet + $"(SecureAuth Cookie IS NULL): {token == null}");
            Console.WriteLine(logSnippet + $"(SecureAuth Cookie): '{token}'");

            if ( token != null)
            {                
                ////////////////////////////////////////////////////////////////////////////////
                // Decrypt the token with our SecureAuth keys (validation and decryption)
                ////////////////////////////////////////////////////////////////////////////////

                ////////////////////////////////////////////////////////////////////////////////
                // Validation Key
                ////////////////////////////////////////////////////////////////////////////////
                string validationKey = _config["SecureAuth:ValidationKey:Failover"];
                Console.WriteLine(logSnippet + $"(validationKey IS NULL): {validationKey == null}");
                Console.WriteLine(logSnippet + $"(validationKey): {validationKey}");

                ////////////////////////////////////////////////////////////////////////////////
                // Decryption Key
                ////////////////////////////////////////////////////////////////////////////////
                string decryptionKey = _config["SecureAuth:DecryptionKey:Failover"];
                Console.WriteLine(logSnippet + $"(decryptionKey IS NULL): {decryptionKey == null}");
                Console.WriteLine(logSnippet + $"(decryptionKey): {decryptionKey}");

                byte[] decryptionKeyBytes = HexUtils.HexToBinary(decryptionKey);
                byte[] validationKeyBytes = HexUtils.HexToBinary(validationKey);

                string compatabilityMode = _config.GetValue<string>(MiscConstants.ENCRYPTION_COMPATABILITY_MODE);
                Console.WriteLine(logSnippet + $"(compatabilityMode IsNullOrEmpty).....: {String.IsNullOrEmpty(compatabilityMode)}");
                Console.WriteLine(logSnippet + $"(compatabilityMode IsNullOrWhiteSpace): {String.IsNullOrWhiteSpace(compatabilityMode)}");
                Console.WriteLine(logSnippet + $"(compatabilityMode)...................: '{compatabilityMode}'");

                LegacyFormsAuthenticationTicketEncryptor legacyFormsAuthenticationTicketEncryptor = null;

                if ( compatabilityMode.Equals(MiscConstants.FRAMEWORK45))
                {
                    legacyFormsAuthenticationTicketEncryptor 
                        = new LegacyFormsAuthenticationTicketEncryptor(decryptionKeyBytes, validationKeyBytes, ShaVersion.Sha1, CompatibilityMode.Framework45);
                }
                else
                {
                    legacyFormsAuthenticationTicketEncryptor 
                        = new LegacyFormsAuthenticationTicketEncryptor(decryptionKeyBytes, validationKeyBytes, ShaVersion.Sha1, CompatibilityMode.Framework20SP2);
                }

                Console.WriteLine(logSnippet + $"Decrypting post-authentication cookie sent back from SecureAuth ...");
                FormsAuthenticationTicket decryptedTicket = legacyFormsAuthenticationTicketEncryptor.DecryptCookie(token);
                Console.WriteLine(logSnippet + $"(decryptedTicket IS NULL): {decryptedTicket == null}");

                if ( decryptedTicket != null)
                {
                    Console.WriteLine(logSnippet + $"(decryptedTicket.Name).....: '{decryptedTicket.Name}'");
                    Console.WriteLine(logSnippet + $"(decryptedTicket.UserData).: '{decryptedTicket.UserData}'");

                    HttpContext.Session.SetObject(MiscConstants.USER_EMAIL_ADDRESS, decryptedTicket.UserData);
                }

                ////////////////////////////////////////////////////////////////////////////////
                // If user is already authenticated but user names don't match, log user out.
                ////////////////////////////////////////////////////////////////////////////////
                Console.WriteLine(logSnippet + $"(User.Identity.IsAuthenticated): '{User.Identity.IsAuthenticated}'");

                ////////////////////////////////////////////////////////////////////////////////
                // User is Authenticated
                ////////////////////////////////////////////////////////////////////////////////
                if (User.Identity.IsAuthenticated == true)
                {
                    Console.WriteLine(logSnippet + $"(decryptedTicket.UserData): '{decryptedTicket.UserData}'");
                    Console.WriteLine(logSnippet + $"(decryptedTicket.Name)....: '{decryptedTicket.Name}'");
                    Console.WriteLine(logSnippet + $"(User.Identity.Name)......: '{User.Identity.Name}'");

                    if (decryptedTicket.Name != User.Identity.Name) 
                    {
                        Console.WriteLine(logSnippet + $"decryptedTicket.Name AND User.Identity.Name DON'T MATCH - logging user out");
                        return await LogoutAsync();
                    }
                }

                ////////////////////////////////////////////////////////////////////////////////
                // User is NOT Authenticated
                ////////////////////////////////////////////////////////////////////////////////
                if (User.Identity.IsAuthenticated == false)
                {
                    Console.WriteLine(logSnippet + $"Calling UserService.RetrieveByEmailAddress for: '{decryptedTicket.UserData}'");

                    try 
                    {
                        qmsUser = _userService.RetrieveByEmailAddress(decryptedTicket.UserData, true);
                    }
                    catch (UserNotFoundException)
                    {
                        /////////////////////////////////////////////////////////////////////////////////////////
                        // User record not found in the sec_user table. Redirecting to 'UnauthorizedUser page.
                        /////////////////////////////////////////////////////////////////////////////////////////
                        Console.WriteLine(logSnippet + $"qmsUser NOT FOUND for '{decryptedTicket.UserData}'. Redirecting to 'UnauthorizedUser page");
                        return RedirectToAction("UnauthorizedUser", "Login");
                    }
                        
                    if (qmsUser.UserRoles == null || qmsUser.UserRoles.Count < 1)
                    {
                        ///////////////////////////////////////////////////////////////////////////////////////
                        // User is Unauthorized to use QMS (no roles)(No rows found insec_user_org_role table)
                        ///////////////////////////////////////////////////////////////////////////////////////
                        Console.WriteLine(logSnippet + $"No QMS roles found for authenticated user '{decryptedTicket.UserData}'. Redirecting to 'UnauthorizedUser page.");
                        return RedirectToAction("UnauthorizedUser", "Login");                           
                    }

                    Console.WriteLine(logSnippet + $"'{decryptedTicket.UserData}' IS AUTHORIZED to use QMS. Creating ClaimsPrincipal.");

                    var claimsPrincipal = this.CreateClaimsPrincipal(qmsUser, decryptedTicket.Name, decryptedTicket.UserData);
                        
                    if (claimsPrincipal == null)
                    {
                        // ViewBag.LoginError = _localizer["User {0} does not have access to QMS.", decryptedTicket.Name];
                        ViewBag.LoginError = _localizer[$"User '{decryptedTicket.Name}' does not have access to QMS."];
                    }
                    else if ( this.UserHasMultipleSessions(claimsPrincipal) )
                    {
                        ViewBag.LoginError = _localizer["You have too many existing sessions. Please log out of one or them or wait 30 minutes and try again."];
                    }
                    else
                    {
                        Console.WriteLine(logSnippet + $"Calling HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal)");
                            
                        ////////////////////////////////////////////////////////////////////////////
                        // Place SecUser in Session
                        ////////////////////////////////////////////////////////////////////////////
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
                        HttpContext.Session.SetObject(MiscConstants.USER_SESSION_KEY, qmsUser);
                        HttpContext.Session.SetObject(MiscConstants.USER_SESSION_VM_KEY, UserUtil.MapToViewModel(qmsUser));

                        ////////////////////////////////////////////////////////////////////////////
                        // Place MenuItemViewModel List in Session
                        ////////////////////////////////////////////////////////////////////////////
                        //HashSet<MenuItemViewModel> menuBuilder = MenuUtil.GetMenuItemsforUser(qmsUser);
                        //HttpContext.Session.SetObject(MiscConstants.MENU_SESSION_KEY, menuBuilder);

                        ////////////////////////////////////////////////////////////////////////////
                        // Place ModuleMenuItem in Session
                        ////////////////////////////////////////////////////////////////////////////
                        List<ModuleMenuItem> moduleMenuItems = _menuBuilderService.RetrieveMenuForUser(qmsUser.UserId);
                        HttpContext.Session.SetObject(MiscConstants.MODULE_MENU_ITEMS_SESSION_KEY, moduleMenuItems);
                        this.debug(moduleMenuItems, qmsUser.UserId);
                    }
                }
             }

            string requestedUri = HttpContext.Session.GetObject<string>(MiscConstants.REQUESTED_URI);

            Console.WriteLine(logSnippet + $"(Request.Protocol): '{Request.Protocol}'");
            Console.WriteLine(logSnippet + $"(Request.Host)....: '{Request.Host}'");
            Console.WriteLine(logSnippet + $"(Request.PathBase): '{Request.PathBase}'");
            Console.WriteLine(logSnippet + $"(requestedUri)....: '{requestedUri}'");

            //string redirectUrl  = $"https://dev-hrqms.gsa.gov/{requestedUri}";
            string redirectUrl  = $"https://{Request.Host}{Request.PathBase}/{requestedUri}";
            Console.WriteLine(logSnippet + $"(redirectUrl).....: '{redirectUrl}'");           

            if ( string.IsNullOrEmpty(requestedUri) == false
                    && string.IsNullOrWhiteSpace(requestedUri) == false )
                    // && Url.IsLocalUrl(requestedUri) )
            {
                HttpContext.Session.Remove(MiscConstants.REQUESTED_URI);
                //return LocalRedirect(requestedUri);
                return Redirect(redirectUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            } 
        }   

        private void debug(List<ModuleMenuItem> moduleMenuItems, int userId)
        {
            string logSnippet   = new StringBuilder("[")
                                .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                                .Append("][LoginController][ExternalLoginCallback] => ")
                                .ToString();

            Console.WriteLine(logSnippet + $"BEGIN: ModuleMenuItems for User #{userId}:");
            foreach (ModuleMenuItem moduleMenuItem in moduleMenuItems)
            {
                Console.WriteLine($"(moduleMenuItem.Title): {moduleMenuItem.Title}");
                foreach (MenuItem menuItem in moduleMenuItem.MenuItems)
                {
                    Console.WriteLine(menuItem);

                }
            }
            Console.WriteLine(logSnippet + $"BEGIN: ModuleMenuItems for User #{userId}");
        }
        private ClaimsPrincipal CreateClaimsPrincipal(User qmsUser, string userName, string email)
        {
            string logSnippet   = new StringBuilder("[")
                                .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                                .Append("][LoginController][CreateClaimsPrincipal] => ")
                                .ToString();

            Console.WriteLine(logSnippet + $"(p_userName): '{userName}'");
            Console.WriteLine(logSnippet + $"(p_email)...: '{email}'");
            Console.WriteLine(logSnippet + $"(qmsUser == null): {qmsUser == null}");

            var claimsIdentity 
                = new ClaimsIdentity
                    (CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
            
            claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userName));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, userName));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Email, email));
            
            Console.WriteLine(logSnippet + $"(roleCount): {qmsUser.UserRoles.Count}");

            foreach ( var userRole in qmsUser.UserRoles )
            {
                Console.WriteLine(logSnippet + $"(role): '{userRole.Role.RoleCode}'");
                claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, userRole.Role.RoleCode));
            }

            return new ClaimsPrincipal(claimsIdentity);
        }   

        private bool UserHasMultipleSessions(ClaimsPrincipal claimsPrincipal)
        {
            return false;
        }
    }
}