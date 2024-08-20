using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace online_library_management_system.Models
{
    public enum LendingStatus
    {
        Issued,
        Returned
    }

    public class Lending
	{
        [Key]
        public int LendingId { get; set; }

        [Required]
        [ForeignKey("Reservation")]
        public int? ReservationId { get; set; }
        public virtual Reservation? Reservation { get; set; }

        [Required]
        public DateTime IssuedAt { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        public DateTime? ReturnedAt { get; set; }

        [Required]
        public LendingStatus Status { get; set; } = LendingStatus.Issued;
    }
}

