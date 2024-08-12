using System;
using System.ComponentModel.DataAnnotations;

namespace online_library_management_system.ViewModels
{
    public class SigninVM
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}

