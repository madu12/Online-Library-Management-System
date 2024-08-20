using System;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace online_library_management_system.Models
{
	public class ApplicationUser:IdentityUser
	{
        [Required]
        public string? FirstName { get; set; } = "";

        public string? LastName { get; set; } = "";

        public string? PhoneNo { get; set; } = "";

        public string? Address { get; set; } = "";

    }
}

