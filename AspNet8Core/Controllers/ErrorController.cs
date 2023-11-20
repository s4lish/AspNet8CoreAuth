using Microsoft.AspNetCore.Mvc;

namespace AspNet8Core.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult AccessDenied()
        {
            return View();
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
