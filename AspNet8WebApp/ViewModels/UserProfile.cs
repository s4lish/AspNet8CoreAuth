using System.ComponentModel.DataAnnotations;

namespace AspNet8WebApp.ViewModels
{
    public class UserProfile
    {
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Department { get; set; } = string.Empty;
        [Required]
        public string Position { get; set; } = string.Empty;

        public bool TwoFactorAuthenticator { get; set; } = false;

    }
}
