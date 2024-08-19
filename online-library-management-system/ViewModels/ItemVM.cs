using Microsoft.AspNetCore.Mvc.Rendering;
using online_library_management_system.Models;
using System.Collections.Generic;

namespace online_library_management_system.ViewModels
{
    public class ItemVM
    {
        public Item? NewItem { get; set; }

        // For image upload
        public IFormFile? ImageFile { get; set; }

        // List of items to display
        public List<Item>? Items { get; set; } = new List<Item>();

        // Search query
        public string? Search { get; set; }

        // Selected category names
        public List<string>? SelectedCategories { get; set; }

        // Selected item type (Book/CD)
        public List<string>? SelectedItemTypes { get; set; }

        // Selected availability (Yes/No)
        public string? Availability { get; set; }

        // Current page number
        public int CurrentPage { get; set; }

        // Total number of pages
        public int TotalPages { get; set; }

        // List of categories
        public SelectList? Categories { get; set; } 
    }
}
