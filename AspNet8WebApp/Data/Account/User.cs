using AspNet8WebApp.Enums;
using Microsoft.AspNetCore.Identity;

namespace AspNet8WebApp.Data.Account
{
    public class User : IdentityUser
    {
        public string Department { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public Twofactortypes? Twofactortypes { get; set; } = null;

    }
}
