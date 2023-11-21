using AspNet8WebApp.Data.Account;
using AspNet8WebApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QRCoder;
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

        [HttpGet]
        public async Task<IActionResult> Profile()
        {

            var claims = await GetUserInfoAsync();

            if (claims.user != null)
            {
                UserProfileViewModel userProfile = new UserProfileViewModel()
                {
                    Department = claims.departmentClaim?.Value ?? "",
                    Position = claims.PositionClaim?.Value ?? "",
                    Email = claims.user?.Email ?? "",
                    TwoFactorAuthenticator = claims.user?.TwoFactorEnabled ?? false,
                    Twofactortypes = claims.user?.Twofactortypes ?? Enums.Twofactortypes.email
                };

                return View(userProfile);

            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Profile(UserProfileViewModel userProfile)
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

                if (claims.user != null && claims.departmentClaim != null)
                    await userManager.ReplaceClaimAsync(claims.user, claims.departmentClaim, new Claim(claims.departmentClaim.Type, userProfile.Department));
                if (claims.user != null && claims.PositionClaim != null)
                    await userManager.ReplaceClaimAsync(claims.user, claims.PositionClaim, new Claim(claims.PositionClaim.Type, userProfile.Position));

                if (userProfile.TwoFactorAuthenticator)
                {
                    if (claims.user.Twofactortypes != userProfile.Twofactortypes && userProfile.Twofactortypes == Enums.Twofactortypes.authenticator)
                    {
                        return RedirectToAction("AuthenticatorWithMFASetup", "User");
                    }

                    claims.user.Twofactortypes = userProfile.Twofactortypes;
                }
                else
                {
                    claims.user.Twofactortypes = null;
                }

                claims.user.TwoFactorEnabled = userProfile.TwoFactorAuthenticator;


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

        [HttpGet]
        public async Task<IActionResult> AuthenticatorWithMFASetup()
        {
            var user = await userManager.GetUserAsync(User);
            string? key = null;
            if (user != null)
            {
                await userManager.ResetAuthenticatorKeyAsync(user);
                key = await userManager.GetAuthenticatorKeyAsync(user);
            }

            return View(new SetupMFAViewModel
            {
                key = key ?? string.Empty,
                QRCodeBytes = GenerateQRCodeBytes("AspNet8", key ?? "", user?.Email ?? "")
            });
        }

        [HttpPost]
        public async Task<IActionResult> AuthenticatorWithMFASetup(SetupMFAViewModel setupMFAViewModel)
        {
            if (!ModelState.IsValid)
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return View();
            }

            var user = await userManager.GetUserAsync(User);
            if (user != null)
            {
                var result = await userManager.VerifyTwoFactorTokenAsync(user, userManager.Options.Tokens.AuthenticatorTokenProvider, setupMFAViewModel.GeneratedCode);
                if (result)
                {
                    user.Twofactortypes = Enums.Twofactortypes.authenticator;
                    await userManager.UpdateAsync(user);
                    await userManager.SetTwoFactorEnabledAsync(user, true);
                    ViewBag.messsage = "authenticator setup successfully activated";
                }
                else
                {
                    ModelState.AddModelError("AuthenticatorWithMFASetup", "Something went wrong with the authenticator setup");
                }
            }
            else
            {
                ModelState.AddModelError("AuthenticatorWithMFASetup", "User not found");
            }


            return View(setupMFAViewModel);
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

        private Byte[] GenerateQRCodeBytes(string provider, string key, string userEmail)
        {
            var qrcodeGenerator = new QRCodeGenerator();
            var otpauth = $"otpauth://totp/{provider}:{userEmail}?secret={key}&issuer={provider}";
            var qrcodeData = qrcodeGenerator.CreateQrCode(otpauth, QRCodeGenerator.ECCLevel.Q);

            var qrCode = new PngByteQRCode(qrcodeData);

            return qrCode.GetGraphic(20);
        }
    }
}
