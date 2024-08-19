using System.ComponentModel.DataAnnotations;

namespace online_library_management_system.ViewModels
{
    public class SendMessageVM
    {
        [Required]
        [StringLength(50)]
        public required string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public required string LastName { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [StringLength(100)]
        public required string Subject { get; set; }

        [Required]
        [StringLength(1000)]
        public required string Message { get; set; }
    }
}
