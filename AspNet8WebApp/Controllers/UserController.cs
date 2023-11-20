using AspNet8WebApp.Data.Account;
using AspNet8WebApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace AspNet8WebApp.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly UserManager<User> userManager;

        public UserController(UserManager<User> userManager)
        {
            this.userManager = userManager;
        }
        // GET: UserController
        public ActionResult Index()
        {
            return View();
        }

        // GET: UserController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: UserController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UserController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RegistrationViewModel registrationView)
        {



            return View();
        }

        // GET: UserController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: UserController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UserController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: UserController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }


        [HttpGet]
        public async Task<IActionResult> Profile()
        {

            var claims = await GetUserInfoAsync();

            if (claims.user != null)
            {
                UserProfile userProfile = new UserProfile()
                {
                    Department = claims.departmentClaim?.Value ?? "",
                    Position = claims.PositionClaim?.Value ?? "",
                    Email = claims.user?.Email ?? "",
                    TwoFactorAuthenticator = claims.user?.TwoFactorEnabled ?? false,
                };

                return View(userProfile);

            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Profile(UserProfile userProfile)
        {
            if (!ModelState.IsValid)
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return View();
            }

            var claims = await GetUserInfoAsync();

            try
            {
                if (claims.user == null)
                {
                    return RedirectToAction("Index", "Home");
                }


                claims.user.TwoFactorEnabled = userProfile.TwoFactorAuthenticator;

                if (claims.user != null && claims.departmentClaim != null)
                    await userManager.ReplaceClaimAsync(claims.user, claims.departmentClaim, new Claim(claims.departmentClaim.Type, userProfile.Department));
                if (claims.user != null && claims.PositionClaim != null)
                    await userManager.ReplaceClaimAsync(claims.user, claims.PositionClaim, new Claim(claims.PositionClaim.Type, userProfile.Position));


                await userManager.UpdateAsync(claims.user);

                ViewBag.messsage = "user profile updated successfully";
            }
            catch
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                ModelState.AddModelError("userProfile", "Error occured during update user profile");
            }



            return View(userProfile);
        }


        private async Task<(User? user, Claim? departmentClaim, Claim? PositionClaim)> GetUserInfoAsync()
        {
            var user = await userManager.FindByNameAsync(User?.Identity?.Name ?? "");

            if (user != null)
            {
                var claims = await userManager.GetClaimsAsync(user);
                var departmentClaim = claims.FirstOrDefault(x => x.Type == "Department");
                var PositionClaim = claims.FirstOrDefault(x => x.Type == "Position");

                return (user, departmentClaim, PositionClaim);
            }

            return (null, null, null);
        }
    }
}
