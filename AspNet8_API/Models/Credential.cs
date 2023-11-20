using System.ComponentModel.DataAnnotations;

namespace AspNet8_API.Models
{
    public class Credential
    {
        [Required]
        public string UserName { get; set; } = string.Empty;
        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;
    }
}
