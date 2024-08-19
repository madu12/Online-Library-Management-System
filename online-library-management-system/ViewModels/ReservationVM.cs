using Microsoft.AspNetCore.Mvc.Rendering;
using online_library_management_system.Models;

namespace online_library_management_system.ViewModels
{
    public class ReservationVM
    {
        public int ReservationId { get; set; }

        public int ItemId { get; set; }

        public string? ItemTitle { get; set; }

        public string? ItemImageUrl { get; set; }

        public string? Author { get; set; }

        public string? ISBN { get; set; }

        public DateTime ReservedAt { get; set; }

        public ReservationStatus Status { get; set; }

        public string? AdminComment { get; set; }

        public string StatusText => Status.ToString();
    }
}
