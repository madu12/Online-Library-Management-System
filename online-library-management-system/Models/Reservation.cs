using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace online_library_management_system.Models
{
    public enum ReservationStatus
    {
        Pending,
        Approved,
        Rejected,
        Cancelled,
        Expired,
        Completed
    }
    
	public class Reservation
	{
        [Key]
        public int ReservationId { get; set; }

        [Required]
        [ForeignKey("Item")]
        public int? ItemId { get; set; }
        public virtual Item? Item { get; set; }

        [Required]
        [ForeignKey("User")]
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }

        [Required]
        public DateTime ReservedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public ReservationStatus Status { get; set; } = ReservationStatus.Pending;

        public string? AdminComment { get; set; } 

        public DateTime? ApprovedAt { get; set; } 

        public DateTime? ExpirationDate { get; set; } 
    }
}

