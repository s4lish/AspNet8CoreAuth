using AspNet8Core.Authorization;
using AspNet8Core.Middlewares;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication().AddCookie("MyCookieAuth", opt =>
{
    opt.Cookie.Name = "MyCookieAuth";
    opt.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    opt.LoginPath = "/Login";
    opt.LogoutPath = "/Login";
    opt.AccessDeniedPath = "/Error/AccessDenied";
});

builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("AdminOnly", policy => policy.RequireClaim("Admin"));
    opt.AddPolicy("MustBelongToHRDep", policy => policy.RequireClaim("Department", "HR"));
    opt.AddPolicy("MustBelongToBIDep", policy => policy.RequireClaim("Department", "BI"));
    opt.AddPolicy("ManagerOnly", policy => policy.RequireClaim("Department", "HR")
    .RequireClaim("Manager")
    .Requirements.Add(new HRManagerProbationRequirment(3)));

});

builder.Services.AddHttpClient("mainapi", httpclient =>
{
    httpclient.BaseAddress = new Uri("http://localhost:5179/api/v1/");
});

builder.Services.AddSingleton<IAuthorizationHandler, HRManagerProbationRequirmentHandler>();

builder.Services.AddSession(opt =>
{
    opt.Cookie.HttpOnly = true;
    opt.IdleTimeout = TimeSpan.FromMinutes(20);
    opt.Cookie.IsEssential = true;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseAntiXssMiddleware();

app.UseStaticFiles();

app.UseAntiforgery();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
    );


app.Run();
