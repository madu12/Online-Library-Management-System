using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using online_library_management_system.Models;
using online_library_management_system.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using online_library_management_system.Services;

namespace online_library_management_system.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class ItemController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _environment;

        public ItemController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _environment = environment;
        }

        public IActionResult Index()
        {
            var viewModel = new ItemVM
            {
                Items = _context.Items.Include("Categories")
                                      .Include("AuthorsAndArtists")
                                      .ToList()
            };
            return View(viewModel);
        }

        public IActionResult Create()
        {
            LoadCategoriesAndAuthors();
            var viewModel = new ItemVM
            {
                NewItem = new Item()
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ItemVM itemVM)
        {
            try
            {
                if (itemVM.NewItem!.ItemType == "Book" && string.IsNullOrWhiteSpace(itemVM.NewItem.ISBN))
                {
                    ModelState.AddModelError("NewItem.ISBN", "The ISBN field is required for books.");
                }

                if (itemVM.ImageFile == null)
                {
                    ModelState.AddModelError("ImageFile", "An image is required.");
                }

                if (ModelState.IsValid)
                {
                    var loggedInUser = await _userManager.GetUserAsync(User);
                    if (loggedInUser == null)
                    {
                        TempData["ErrorMessage"] = "Unable to determine the current user. Please try again.";
                        return RedirectToAction(nameof(Index));
                    }

                    // Save the image file
                    string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(itemVM.ImageFile!.FileName);
                    string imageFullPath = Path.Combine(_environment.WebRootPath, "images/books-media", newFileName);

                    using (var stream = System.IO.File.Create(imageFullPath))
                    {
                        await itemVM.ImageFile.CopyToAsync(stream);
                    }


                    var item = new Item
                    {
                        Title = itemVM.NewItem.Title,
                        Description = itemVM.NewItem.Description,
                        ItemType = itemVM.NewItem.ItemType,
                        CategoryId = itemVM.NewItem.CategoryId,
                        PublishDate = itemVM.NewItem.PublishDate,
                        AuthorAndArtistId = itemVM.NewItem.AuthorAndArtistId,
                        ISBN = itemVM.NewItem.ISBN,
                        UserId = loggedInUser.Id,
                        Availability = "Yes",
                        ImagePath = newFileName
                    };

                    _context.Items.Add(item);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Item added successfully!";
                    return RedirectToAction(nameof(Index));
                }

            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = "Unable to save item. Try again!";
            }

            LoadCategoriesAndAuthors();
            return View(itemVM);
        }

        [NonAction]
        private void LoadCategoriesAndAuthors()
        {
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name");
            ViewBag.AuthorsAndArtists = new SelectList(_context.AuthorsAndArtists, "Id", "Name");
        }
    }
}
