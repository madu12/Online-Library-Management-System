using online_library_management_system.Models;
using System.Collections.Generic;

namespace online_library_management_system.ViewModels
{
    public class ItemVM
    {
        public List<Item>? Items { get; set; }

        public Item? NewItem { get; set; }

        public IFormFile? ImageFile { get; set; }
    }
}
