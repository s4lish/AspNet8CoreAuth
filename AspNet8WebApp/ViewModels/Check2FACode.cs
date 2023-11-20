﻿using System.ComponentModel.DataAnnotations;

namespace AspNet8WebApp.ViewModels
{
    public class Check2FACode
    {
        [Required]
        [Display(Name = "Security Code")]
        public string Code { get; set; } = string.Empty;
        public bool RememberMe { get; set; } = false;
    }
}
