using AspNet8_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AspNet8_API.Controllers
{
    public class AuthController : APIBase
    {
        private readonly IConfiguration configuration;

        public AuthController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpPost]
        public IActionResult Post(Credential credential)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
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


                var expireAt = DateTime.UtcNow.AddMinutes(10);
                var token = CreateToken(claims, expireAt);
                //Console.WriteLine(token);
                return Ok(new
                {
                    access_toekn = token,
                    expires_at = expireAt,
                });

            }

            ModelState.AddModelError("Unauthorized", "You are not authorized to access the endpoint");

            return Unauthorized(ModelState);
        }


        private string CreateToken(IEnumerable<Claim> claims, DateTime expire_At)
        {
            var secretkey = Encoding.ASCII.GetBytes(configuration.GetValue<string>("secretkey") ?? "");
            //generate the jwt
            var jwt = new JwtSecurityToken(
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expire_At,
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(secretkey), SecurityAlgorithms.HmacSha256Signature)


                );

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
}
