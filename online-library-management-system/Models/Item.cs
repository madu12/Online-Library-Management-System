using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace online_library_management_system.Models
{


    public class Item
    {
        [Key]
        public int ItemId { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; } = "";

        [StringLength(200)]
        public string Description { get; set; } = "";

        [Required(ErrorMessage = "Please select an item type.")]
        [StringLength(10)]
        public string ItemType { get; set; } = "";

        [Required(ErrorMessage = "Please select a category.")]
        [ForeignKey("Category")]
        public int? CategoryId { get; set; }
        public virtual Category? Categories { get; set; }

        public DateTime? PublishDate { get; set; }

        [Required(ErrorMessage = "Please select an author or artist.")]
        [ForeignKey("AuthorAndArtist")]
        public int? AuthorAndArtistId { get; set; }
        public virtual AuthorAndArtist? AuthorsAndArtists { get; set; }

        [StringLength(20)]
        public string? ISBN { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; } = "";

        [StringLength(10)]
        public string Availability { get; set; } = "";

        [StringLength(255)]
        public string ImagePath { get; set; } = "";
    }
}