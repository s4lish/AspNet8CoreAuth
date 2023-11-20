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
        public async Task<IActionResult> Index(Credential credential)
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
                    var securityCode = await userManager.GenerateTwoFactorTokenAsync(user, "Email");

                    var text = $"<h3>Two Factor Authentication code is</h3><h5>{securityCode}</h5>";
                    emailService.SendEmailAsync(user.Email, "Two Factor Authentication Code", text);

                    return RedirectToAction("Check2FactorCode", "Login", new { rmme = credential.RememberMe });
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
        public IActionResult Check2FactorCode(bool rmme)
        {
            Check2FACode check2FACode = new Check2FACode
            {
                RememberMe = rmme
            };

            return View(check2FACode);
        }


        [HttpPost]
        public async Task<IActionResult> Check2FactorCode(Check2FACode check2FACode)
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

    }
}
