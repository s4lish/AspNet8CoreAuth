using AspNet8Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLib;
using System.Net.Http.Headers;
using System.Text.Json;

namespace AspNet8Core.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            this._httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            JwtToken? jwtToken = await GetToken();
            var httpClient = _httpClientFactory.CreateClient("mainapi");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken?.access_toekn ?? string.Empty);
            var httpresponse = await httpClient.GetAsync("WeatherForecast");
            httpresponse.EnsureSuccessStatusCode();
            if (httpresponse.IsSuccessStatusCode)
            {
                var response = await httpresponse.Content.ReadFromJsonAsync<IEnumerable<WeatherForecast>>();
                return View(response);
            }

            return View();
        }
        [Authorize(Policy = "AdminOnly")]
        public IActionResult Manager(int id)
        {
            return View(id);
        }
        [Authorize(Policy = "MustBelongToHRDep")]
        public IActionResult HRDep()
        {
            return View();
        }

        [Authorize(Policy = "ManagerOnly")]
        public IActionResult HRManager()
        {

            return View();
        }

        [Authorize(Policy = "MustBelongToBIDep")]
        public IActionResult BIDep()
        {
            return View();
        }


        private async Task<JwtToken?> GetToken()
        {
            JwtToken? jwtToken = new();
            var httpClient = _httpClientFactory.CreateClient("mainapi");

            //get token from session
            var strTokenObj = HttpContext.Session.GetString("access_token");

            if (string.IsNullOrEmpty(strTokenObj))
            {
                var httptokenresponse = await httpClient.PostAsJsonAsync("Auth", new Credential { UserName = "admin", Password = "password" });
                if (httptokenresponse.IsSuccessStatusCode)
                {
                    jwtToken = await httptokenresponse.Content.ReadFromJsonAsync<JwtToken>();
                    string strJwt = await httptokenresponse.Content.ReadAsStringAsync();
                    //var token = JsonSerializer.Deserialize<JwtToken>(strJwt);
                    HttpContext.Session.SetString("access_token", strJwt);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                jwtToken = JsonSerializer.Deserialize<JwtToken>(strTokenObj);
            }


            if (jwtToken == null || string.IsNullOrEmpty(jwtToken.access_toekn) || jwtToken.expires_at <= DateTime.UtcNow)
            {
                var httptokenresponse = await httpClient.PostAsJsonAsync("Auth", new Credential { UserName = "admin", Password = "password" });
                if (httptokenresponse.IsSuccessStatusCode)
                {
                    jwtToken = await httptokenresponse.Content.ReadFromJsonAsync<JwtToken>();
                    string strJwt = await httptokenresponse.Content.ReadAsStringAsync();
                    //var token = JsonSerializer.Deserialize<JwtToken>(strJwt);
                    HttpContext.Session.SetString("access_token", strJwt);
                }
                else
                {
                    return null;
                }

            }

            return jwtToken;
        }

    }
}
