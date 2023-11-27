using AspNet8WebApp.Data;
using AspNet8WebApp.Data.Account;
using AspNet8WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace AspNet8WebApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SignInManager<User> signInManager;
        private readonly UserManager<User> userManager;
        private readonly ApplicationDbContext applicationDbContext;

        public HomeController(ILogger<HomeController> logger, SignInManager<User> signInManager, UserManager<User> userManager, ApplicationDbContext applicationDbContext)
        {
            _logger = logger;
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.applicationDbContext = applicationDbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            Guid key = Guid.Parse(HttpContext.Request.Cookies["secretkey"]);
            var secretkey = await applicationDbContext.SecretKey.Where(x => x.Key == key && x.Username == User.Identity.Name).FirstOrDefaultAsync();
            if (secretkey != null)
            {
                applicationDbContext.SecretKey.Remove(secretkey);
                await applicationDbContext.SaveChangesAsync();
            }
            HttpContext.Response.Cookies.Delete("secretkey");
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Login");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
