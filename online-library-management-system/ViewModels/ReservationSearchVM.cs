using System;
using online_library_management_system.Models;

namespace online_library_management_system.ViewModels
{
	public class ReservationSearchVM
	{
        public int ReservationId { get; set; }
        public string? UserFullName { get; set; }
        public string? UserEmail { get; set; }
        public string? ItemTitle { get; set; }
        public string? ISBN { get; set; }
        public string? ItemType { get; set; }
        public ReservationStatus Status { get; set; }
        public DateTime ReservedAt { get; set; }
        public DateTime? IssuedAt { get; set; }
        public DateTime? ReturnedAt { get; set; }
    }
}

