using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace online_library_management_system.Models
{
	public class Category
	{
		public int Id { get; set; }

		[Required, MaxLength(100)]
		public string Name { get; set; } = "";

		public DateTime CreatedAt { get; set; }

        public ICollection<Item>? Items { get; set; }
    }
}

