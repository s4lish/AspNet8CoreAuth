using System.ComponentModel.DataAnnotations;

namespace AspNet8WebApp.ViewModels
{
    public class SetupMFAViewModel
    {
        public string key { get; set; } = string.Empty;
        [Required]
        [Display(Name = "Generated Code")]
        public string GeneratedCode { get; set; } = string.Empty;

        public Byte[]? QRCodeBytes { get; set; }
    }
}
