using AspNet8WebApp.Data.Account;
using AspNet8WebApp.Services.EmailService;
using AspNet8WebApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AspNet8WebApp.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        private readonly SignInManager<User> signInManager;
        private readonly UserManager<User> userManager;
        private readonly IEmailService emailService;

        public LoginController(SignInManager<User> signInManager, UserManager<User> userManager, IEmailService emailService)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.emailService = emailService;
        }
        public IActionResult Index()
        {
            if (User != null && User.Identity != null && User.Identity.IsAuthenticated) return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(CredentialViewModel credential)
        {
            if (!ModelState.IsValid) return View();


            var result = await signInManager.PasswordSignInAsync(credential.Username, credential.Password, credential.RememberMe, true);


            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {

                if (result.RequiresTwoFactor)
                {
                    var user = await userManager.FindByNameAsync(credential.Username);
                    if (user.Twofactortypes == Enums.Twofactortypes.email)
                    {
                        return RedirectToAction("Check2FactorCode", "Login", new { rmme = credential.RememberMe, us = credential.Username });
                    }
                    else if (user.Twofactortypes == Enums.Twofactortypes.sms)
                    {
                        ModelState.AddModelError("Login", "SMS 2FA no support yet.");
                    }
                    else if (user.Twofactortypes == Enums.Twofactortypes.authenticator)
                    {
                        return RedirectToAction("Check2FactorAuthenticator", "Login", new { rmme = credential.RememberMe });
                    }
                    else
                    {
                        ModelState.AddModelError("Login", "unknow two factor type.");
                    }
                }

                if (result.IsLockedOut)
                {
                    ModelState.AddModelError("Login", "You are Locked out.");
                }
                else
                {
                    ModelState.AddModelError("Login", "Failed To Login.");
                }
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Check2FactorCode(bool rmme, string us)
        {

            var user = await userManager.FindByNameAsync(us);
            var securityCode = await userManager.GenerateTwoFactorTokenAsync(user, "Email");

            var text = $"<h2>Two Factor Authentication code is</h3><p style=\"font-size:20px;\">{securityCode}</p>";
            emailService.SendEmailAsync(user.Email, "Two Factor Authentication Code", text);


            EmailMFAViewModel check2FACode = new EmailMFAViewModel
            {
                RememberMe = rmme
            };

            return View(check2FACode);
        }


        [HttpPost]
        public async Task<IActionResult> Check2FactorCode(EmailMFAViewModel check2FACode)
        {
            if (!ModelState.IsValid) return View(check2FACode);


            var result = await signInManager.TwoFactorSignInAsync("Email", check2FACode.Code, check2FACode.RememberMe, false);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError("Check2FactorCode", "You are Locked out.");
                }
                else
                {
                    ModelState.AddModelError("Check2FactorCode", "Failed To Login.");
                }
            }

            return View(check2FACode);
        }

        [HttpGet]
        public IActionResult Check2FactorAuthenticator(bool rmme)
        {
            AuthenticatorMFAViewModel check2FACode = new AuthenticatorMFAViewModel
            {
                RememberMe = rmme
            };

            return View(check2FACode);
        }

        [HttpPost]
        public async Task<IActionResult> Check2FactorAuthenticator(AuthenticatorMFAViewModel authenticatorMFAViewModel)
        {
            if (!ModelState.IsValid) return View(authenticatorMFAViewModel);

            var result = await signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorMFAViewModel.Code, authenticatorMFAViewModel.RememberMe, false);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError("Check2FactorAuthenticator", "You are Locked out.");
                }
                else
                {
                    ModelState.AddModelError("Check2FactorAuthenticator", "Failed To Login.");
                }
            }

            return View(authenticatorMFAViewModel);
        }

    }
}
