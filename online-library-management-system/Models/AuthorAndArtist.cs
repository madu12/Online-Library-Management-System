using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace online_library_management_system.Models
{
    [Table("AuthorsAndArtists")]
    public class AuthorAndArtist
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = "";

        [Required]
        [StringLength(50)]
        public string Type { get; set; } = "";

        public ICollection<Item>? Items { get; set; }
    }

}

