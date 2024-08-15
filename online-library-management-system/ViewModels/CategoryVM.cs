using System;
using online_library_management_system.Models;

namespace online_library_management_system.ViewModels
{
    public class CategoryVM
    {
        public List<Category>? Categories { get; set; }

        public required Category NewCategory { get; set; }
    }
}

