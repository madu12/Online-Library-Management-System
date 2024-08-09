using System;
using Microsoft.AspNetCore.Identity;

namespace online_library_management_system.Models
{
	public class ApplicationUser:IdentityUser
	{
        public string FirstName { get; set; } = "";

        public string LastName { get; set; } = "";

        public string Address { get; set; } = "";

    }
}

