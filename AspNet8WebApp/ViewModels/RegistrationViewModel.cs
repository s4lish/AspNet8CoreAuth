using System.ComponentModel.DataAnnotations;

namespace AspNet8WebApp.ViewModels
{
    public class RegistrationViewModel
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; } = string.Empty;
        [DataType(DataType.Password)]
        [Required]
        public string Password { get; set; } = string.Empty;
        [Required]
        public string Username { get; set; } = string.Empty;
        [Required]
        public string PhoneNumber { get; set; } = string.Empty;
        [Required]
        public string Department { get; set; } = string.Empty;
        [Required]
        public string Position { get; set; } = string.Empty;

    }
}
