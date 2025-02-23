﻿using Microsoft.AspNetCore.Mvc.Rendering;
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

        public string? UserFullName { get; set; }

        public string? UserEmail { get; set; }


        public DateTime ReservedAt { get; set; }

        public DateTime? ApprovedAt { get; set; }

        public ReservationStatus Status { get; set; }

        public string StatusText => Status.ToString();

        public bool IsOverdue { get; set; }

    }
}
