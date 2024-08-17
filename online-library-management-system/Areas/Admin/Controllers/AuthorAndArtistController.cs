using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using online_library_management_system.Services;
using online_library_management_system.Models;
using Microsoft.EntityFrameworkCore;
using online_library_management_system.ViewModels;
using System.Threading.Tasks;
using System.Linq;

namespace online_library_management_system.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class AuthorAndArtistController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuthorAndArtistController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var viewModel = new AuthorAndArtistVM
            {
                AuthorsAndArtists = _context.AuthorsAndArtists.ToList(),
                NewAuthorAndArtist = new AuthorAndArtist()
            };
            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> Index(AuthorAndArtistVM authorAndArtistVM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Check if the author/artist name already exists
                    bool authorAndArtistExists = await _context.AuthorsAndArtists
                        .AnyAsync(c => c.Name == authorAndArtistVM.NewAuthorAndArtist.Name && c.Type == authorAndArtistVM.NewAuthorAndArtist.Type);
                    if (authorAndArtistExists)
                    {
                        ModelState.AddModelError("NewAuthorAndArtist.Name", "Author/Artist name already exists.");
                        authorAndArtistVM.AuthorsAndArtists = _context.AuthorsAndArtists.ToList();
                        return View(authorAndArtistVM);
                    }

                    // If name does not exist, create the author/artist
                    AuthorAndArtist authorAndArtist = new()
                    {
                        Name = authorAndArtistVM.NewAuthorAndArtist.Name,
                        Type = authorAndArtistVM.NewAuthorAndArtist.Type,
                    };
                    _context.AuthorsAndArtists.Add(authorAndArtist);
                    await _context.SaveChangesAsync();

                    // Set success message
                    TempData["SuccessMessage"] = "Author/Artist added successfully!";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Unable to save changes. Try again!";
            }
            authorAndArtistVM.AuthorsAndArtists = _context.AuthorsAndArtists.ToList();
            return View(authorAndArtistVM);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var authorAndArtist = await _context.AuthorsAndArtists.FindAsync(id);
                if (authorAndArtist == null)
                {
                    TempData["ErrorMessage"] = "Author/Artist not found.";
                    return RedirectToAction(nameof(Index));
                }

                var viewModel = new AuthorAndArtistVM
                {
                    NewAuthorAndArtist = authorAndArtist
                };

                return View(viewModel);
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Unable to load the Author/Artist. Try again!";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, AuthorAndArtistVM authorAndArtistVM)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(authorAndArtistVM);
                }

                var authorAndArtist = await _context.AuthorsAndArtists.FindAsync(id);
                if (authorAndArtist == null)
                {
                    TempData["ErrorMessage"] = "Author/Artist not found.";
                    return RedirectToAction(nameof(Index));
                }

                authorAndArtist.Name = authorAndArtistVM.NewAuthorAndArtist.Name;
                authorAndArtist.Type = authorAndArtistVM.NewAuthorAndArtist.Type;

                _context.AuthorsAndArtists.Update(authorAndArtist);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Author/Artist updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Unable to edit the Author/Artist. Try again!";
                return View(authorAndArtistVM);
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAuthorAndArtist(int id)
        {
            var authorAndArtist = await _context.AuthorsAndArtists
                .Include(a => a.Items)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (authorAndArtist == null)
            {
                return Json(new { success = false, message = "Author/Artist not found." });
            }

            if (authorAndArtist.Items?.Count != 0)
            {
                return Json(new { success = false, message = "Author/Artist cannot be deleted because they are associated with items." });
            }

            _context.AuthorsAndArtists.Remove(authorAndArtist);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Author/Artist deleted successfully!" });
        }

    }
}
