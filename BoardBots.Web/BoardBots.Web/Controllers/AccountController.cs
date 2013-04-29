using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using BoardBots.Web.Infrastructure;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using BoardBots.Web.Models;

namespace BoardBots.Web.Controllers
{
    [Authorize]
    public class AccountController : RavenController
    {
        //
        // GET: /Account/Login

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(ViewModels.LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var userAccount = RavenSession.LoadUserAccount(model.UserName);
                if (userAccount.CheckPassword(model.Password))
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    return RedirectToLocal(returnUrl);
                }
            }

            ModelState.AddModelError("", "The user name or password provided is incorrect.");
            return View(model);
        }

        //
        // POST: /Account/LogOff

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
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
        public ActionResult Register(ViewModels.RegisterModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = RavenSession.LoadUserAccount(model.UserName);
            if (user != null)
            {
                ModelState.AddModelError(string.Empty, "This email address is already in use");
                return View(model);
            }

            user = new UserAccount
            {
                Username = model.UserName
            };
            user.SetPassword(model.Password);
            RavenSession.Store(user);

            FormsAuthentication.SetAuthCookie(model.UserName, false);
            return RedirectToAction("Index", "Home");
        }

        //
        // POST: /Account/Disassociate

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Disassociate(string provider, string providerUserId)
        {
            var oAuthAssociation = RavenSession.LoadOAuthAssociation(provider, providerUserId);
            var associatedUser = RavenSession.Load<UserAccount>(oAuthAssociation.UserAccountId);

            if (User.Identity.Name == associatedUser.Username && associatedUser.HasPasswordSet())
            {
                RavenSession.Delete(oAuthAssociation);
                TempData["Message"] = "The external login was removed.";
            }

            return RedirectToAction("Manage");
        }

        //
        // GET: /Account/Manage

        public ActionResult Manage()
        {
            ViewBag.StatusMessage = TempData["Message"];

            var userAccount = RavenSession.LoadUserAccount(User.Identity.Name);
            ViewBag.HasLocalPassword = userAccount.HasPasswordSet();
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(ViewModels.LocalPasswordModel model)
        {
            var userAccount = RavenSession.LoadUserAccount(User.Identity.Name);
            bool hasLocalAccount = userAccount.HasPasswordSet();
            ViewBag.HasLocalPassword = hasLocalAccount;
            ViewBag.ReturnUrl = Url.Action("Manage");

            if (!hasLocalAccount)
            {
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                    state.Errors.Clear();
            }

            if (!ModelState.IsValid)
                return View(model);

            if (hasLocalAccount && !userAccount.CheckPassword(model.OldPassword))
            {
                ModelState.AddModelError(string.Empty, "The current password is incorrect");
                return View(model);
            }

            userAccount.SetPassword(model.NewPassword);

            TempData["Message"] = hasLocalAccount ? "Your password has been set." : "Your password has been changed.";
            return RedirectToAction("Manage");
        }

        //
        // POST: /Account/ExternalLogin

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            return new ExternalLoginResult(provider, Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback

        [AllowAnonymous]
        public ActionResult ExternalLoginCallback(string returnUrl)
        {
            AuthenticationResult result = OAuthWebSecurity.VerifyAuthentication(Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
            if (!result.IsSuccessful)
            {
                return RedirectToAction("ExternalLoginFailure");
            }

            var oAuthAssociation = RavenSession.LoadOAuthAssociation(result.Provider, result.ProviderUserId);
            if (oAuthAssociation != null)
            {
                var userAccount = RavenSession.Load<UserAccount>(oAuthAssociation.UserAccountId);
                FormsAuthentication.SetAuthCookie(userAccount.Username, false);
                return RedirectToLocal(returnUrl);
            }

            if (User.Identity.IsAuthenticated)
            {
                var userAccount = RavenSession.LoadUserAccount(User.Identity.Name);
                oAuthAssociation = new OAuthAssociation
                {
                    ProviderName = result.Provider,
                    ProviderUserId = result.ProviderUserId,
                    UserAccountId = userAccount.Id
                };
                RavenSession.Store(oAuthAssociation);

                return RedirectToLocal(returnUrl);
            }
            else
            {
                string loginData = OAuthWebSecurity.SerializeProviderUserId(result.Provider, result.ProviderUserId);
                ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(result.Provider).DisplayName;
                ViewBag.ReturnUrl = returnUrl;
                return View("ExternalLoginConfirmation", new ViewModels.RegisterExternalLoginModel { UserName = result.UserName, ExternalLoginData = loginData });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLoginConfirmation(ViewModels.RegisterExternalLoginModel model, string returnUrl)
        {
            string provider;
            string providerUserId;

            if (User.Identity.IsAuthenticated || !OAuthWebSecurity.TryDeserializeProviderUserId(model.ExternalLoginData, out provider, out providerUserId))
            {
                return RedirectToAction("Manage");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(provider).DisplayName;
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }

            var userAccount = RavenSession.LoadUserAccount(model.UserName);
            if (userAccount != null)
            {
                ModelState.AddModelError(string.Empty, "Username already in use, please choose another.");
                return View(model);
            }

            userAccount = new UserAccount
            {
                Username = model.UserName,
            };
            RavenSession.Store(userAccount);

            var oAuthAssociation = new OAuthAssociation
            {
                ProviderName = provider,
                ProviderUserId = providerUserId,
                UserAccountId = userAccount.Id
            };
            RavenSession.Store(oAuthAssociation);

            FormsAuthentication.SetAuthCookie(userAccount.Username, false);
            return RedirectToLocal(returnUrl);
        }

        //
        // GET: /Account/ExternalLoginFailure

        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [AllowAnonymous]
        [ChildActionOnly]
        public ActionResult ExternalLoginsList(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return PartialView("_ExternalLoginsListPartial", OAuthWebSecurity.RegisteredClientData);
        }

        [ChildActionOnly]
        public ActionResult RemoveExternalLogins()
        {
            var userAccount = RavenSession.LoadUserAccount(User.Identity.Name);
            var externalLogins = RavenSession.Query<OAuthAssociation>()
                .Where(x => x.UserAccountId == userAccount.Id)
                .ToArray()
                .Select(x => new ViewModels.ExternalLogin
                {
                    Provider = x.ProviderName,
                    ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(x.ProviderName).DisplayName,
                    ProviderUserId = x.ProviderUserId
                })
                .ToList();


            ViewBag.ShowRemoveButton = externalLogins.Count > 1 || userAccount.HasPasswordSet();
            return PartialView("_RemoveExternalLoginsPartial", externalLogins);
        }

        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        internal class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl)
            {
                Provider = provider;
                ReturnUrl = returnUrl;
            }

            public string Provider { get; private set; }
            public string ReturnUrl { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
            }
        }

        #endregion
    }
}
