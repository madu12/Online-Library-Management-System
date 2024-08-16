using System.Collections.Generic;
using online_library_management_system.Models;

namespace online_library_management_system.ViewModels
{
    public class AuthorAndArtistVM
    {
        public List<AuthorAndArtist>? AuthorsAndArtists { get; set; }

        public required AuthorAndArtist NewAuthorAndArtist { get; set; }
    }
}
