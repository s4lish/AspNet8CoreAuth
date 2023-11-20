using AspNet8WebApp.Data.Account;
using AspNet8WebApp.Services.EmailService;
using AspNet8WebApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AspNet8WebApp.Controllers
{
    public class RegistrationController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly IEmailService emailService;

        public RegistrationController(UserManager<User> userManager, IEmailService emailService)
        {
            this.userManager = userManager;
            this.emailService = emailService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(RegistrationViewModel registrationView)
        {
            if (!ModelState.IsValid) return View();


            var user = new User()
            {
                Email = registrationView.Email,
                UserName = registrationView.Username,
                PhoneNumber = registrationView.PhoneNumber,
                //Position = registrationView.Position,
                //Department = registrationView.Department,
            };

            //Collect Data with Claims
            var claimDep = new Claim("Department", registrationView.Department);
            var claimPos = new Claim("Position", registrationView.Position);

            var result = await this.userManager.CreateAsync(user, registrationView.Password);

            if (result.Succeeded)
            {
                await this.userManager.AddClaimAsync(user, claimDep);
                await this.userManager.AddClaimAsync(user, claimPos);

                var confirmationToken = await this.userManager.GenerateEmailConfirmationTokenAsync(user);

                var link = Url.Action("ConfirmatioEmail", "Registration", new { userId = user.Id, token = confirmationToken }, "http", "localhost:5209");
                Console.WriteLine(link);

                var text = $"<h3>Hello {user.UserName}</h3><p>Please Click On the link To Confirm Your Email Address.</p><a style=\"font-size:20px;\" href=\"{link}\">Click Here To Confirm</a>";

                // no need to wait. Send In other task
                //emailService.SendEmailAsync(user.Email, "Confirm Your Email", text);

                return RedirectToAction("Index", "Login");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("Register", error.Description);
                }

            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmatioEmail(string userId, string token)
        {

            var user = await this.userManager.FindByIdAsync(userId);

            if (user == null)
            {
                ViewBag.message = "Failed To validate email";
                return View();
            }

            var result = await this.userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                ViewBag.success = true;
                ViewBag.message = "Email Address is Successfully confirm, you can now try to login";
                return View();
            }

            ViewBag.message = "Failed To validate email";
            return View();
        }
    }
}
