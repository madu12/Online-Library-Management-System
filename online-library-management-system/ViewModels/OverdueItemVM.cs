using System;
namespace online_library_management_system.ViewModels
{
	public class OverdueItemVM
	{
        public int LendingId { get; set; }
        public string? ItemTitle { get; set; }
        public string? UserName { get; set; }
        public string? UserEmail { get; set; }
        public string? UserPhone { get; set; }
        public DateTime DueDate { get; set; }
        public int DaysOverdue { get; set; }
    }
}

