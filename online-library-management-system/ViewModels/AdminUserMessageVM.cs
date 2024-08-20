using System;
using online_library_management_system.Models;

namespace online_library_management_system.ViewModels
{
	public class AdminUserMessageVM
	{
        public IEnumerable<UserMessage>? Messages { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }
}

