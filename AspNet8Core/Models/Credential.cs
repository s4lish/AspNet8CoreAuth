using System.ComponentModel.DataAnnotations;

namespace AspNet8Core.Models
{
    public class Credential
    {
        [Required]
        [Display(Description = "User Name")]
        public string UserName { get; set; } = string.Empty;
        [Required]
        [Display(Description = "Password")]
        [DataType(DataType.Password)]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Display(Description = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}
