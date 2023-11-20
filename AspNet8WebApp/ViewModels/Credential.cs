using System.ComponentModel.DataAnnotations;

namespace AspNet8WebApp.ViewModels
{
    public class Credential
    {
        [Required]
        [Display(Description = "Username")]
        public string Username { get; set; } = string.Empty;
        [Required]
        [Display(Description = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Description = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}
