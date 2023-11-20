using AspNet8Core.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AspNet8Core.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            if (User != null && User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(Credential credential)
        {
            if (!ModelState.IsValid)
            {
                //var errors = ModelState.Values.SelectMany(v => v.Errors)
                //                               .Select(e => e.ErrorMessage)
                //                               .ToList();
                //ViewBag.Errors = errors;
                Response.StatusCode = 400;
                return View();
            }

            if (credential.UserName == "admin" && credential.Password == "password")
            {
                var claims = new List<Claim>() {

                    new Claim(ClaimTypes.Name,"admin"),
                    new Claim(ClaimTypes.Email,"admin@mynet.com"),
                    new Claim("Department","HR"),
                    new Claim("Admin","true"),
                    new Claim("Manager","true"),
                    new Claim("EmployementDate","2023-7-01")
                };

                var identity = new ClaimsIdentity(claims, "MyCookieAuth");

                ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);

                AuthenticationProperties authenticationProperties = new AuthenticationProperties
                {
                    IsPersistent = credential.RememberMe
                };

                await HttpContext.SignInAsync("MyCookieAuth", claimsPrincipal, authenticationProperties);

                return RedirectToAction("Index", "Home");
            }

            if (credential.UserName == "bi" && credential.Password == "password")
            {
                var claims = new List<Claim>() {

                    new Claim(ClaimTypes.Name,"BI Employee"),
                    new Claim(ClaimTypes.Email,"bi@mynet.com"),
                    new Claim("Department","BI"),
                };

                var identity = new ClaimsIdentity(claims, "MyCookieAuth");

                ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);

                AuthenticationProperties authenticationProperties = new AuthenticationProperties
                {
                    IsPersistent = credential.RememberMe
                };

                await HttpContext.SignInAsync("MyCookieAuth", claimsPrincipal, authenticationProperties);

                return RedirectToAction("Index", "Home");
            }

            return View();
        }


        public async Task<IActionResult> Logout()
        {

            await HttpContext.SignOutAsync("MyCookieAuth");

            return RedirectToAction("Index", "Login");
        }
    }
}
