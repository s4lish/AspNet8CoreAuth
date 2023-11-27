using AspNet8WebApp.Data;
using AspNet8WebApp.Data.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AspNet8WebApp.Middleware
{
    public class CheckTokenMiddleware
    {
        private readonly RequestDelegate next;
        //private readonly UserManager<User> userManager;
        //private readonly ApplicationDbContext applicationDbContext;

        public CheckTokenMiddleware(RequestDelegate next)
        {
            this.next = next;
            //this.userManager = userManager;
            //this.applicationDbContext = applicationDbContext;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext.User != null && httpContext.User.Identity != null && httpContext.User.Identity.IsAuthenticated)
            {
                var signInManager = httpContext.RequestServices.GetRequiredService<SignInManager<User>>();

                if (!httpContext.Request.Cookies.ContainsKey("secretkey"))
                {
                    await signInManager.SignOutAsync();
                    httpContext.Response.StatusCode = 404;
                    httpContext.Response.Redirect("/Login");
                    return;
                }

                var secretKey = Guid.Parse(httpContext.Request.Cookies["secretkey"]);

                var dbContext = httpContext.RequestServices.GetRequiredService<ApplicationDbContext>();
                var checkDB = await dbContext.SecretKey.AnyAsync(x => x.Key == secretKey);

                if (!checkDB)
                {
                    httpContext.Response.Cookies.Delete("secretkey");
                    await signInManager.SignOutAsync();
                    httpContext.Response.StatusCode = 404;
                    httpContext.Response.Redirect("/Login");
                    return;
                }

            }

            await next(httpContext); // calling next middleware
        }
    }


    public static class CheckTokenMiddlewareExtensions
    {
        public static IApplicationBuilder UseCheckTokenMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CheckTokenMiddleware>();
        }
    }
}
