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

                    TempData["SuccessMessage"] = "The Book/CD is successfully added!";
                    return RedirectToAction(nameof(Index));
                }

            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Unable to save item. Try again!";
            }

            LoadCategoriesAndAuthors();
            return View(itemVM);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var item = await _context.Items.FindAsync(id);
                if (item == null)
                {
                    TempData["ErrorMessage"] = "Item not found.";
                    return RedirectToAction(nameof(Index));
                }

                LoadCategoriesAndAuthors();
                var viewModel = new ItemVM
                {
                    NewItem = item
                };

                return View(viewModel);
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Unable to load the item. Try again!";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, ItemVM itemVM)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    LoadCategoriesAndAuthors();
                    return View(itemVM);
                }

                var item = await _context.Items.FindAsync(id);
                if (item == null)
                {
                    TempData["ErrorMessage"] = "Item not found.";
                    return RedirectToAction(nameof(Index));
                }

                item.Title = itemVM.NewItem!.Title;
                item.Description = itemVM.NewItem.Description;
                item.ItemType = itemVM.NewItem.ItemType;
                item.CategoryId = itemVM.NewItem.CategoryId;
                item.PublishDate = itemVM.NewItem.PublishDate;
                item.AuthorAndArtistId = itemVM.NewItem.AuthorAndArtistId;
                item.ISBN = itemVM.NewItem.ISBN;

                if (itemVM.ImageFile != null)
                {
                    // Save the new image file
                    string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(itemVM.ImageFile!.FileName);
                    string imageFullPath = Path.Combine(_environment.WebRootPath, "images/books-media", newFileName);

                    using (var stream = System.IO.File.Create(imageFullPath))
                    {
                        await itemVM.ImageFile.CopyToAsync(stream);
                    }

                    // Delete the old image file if it exists
                    if (!string.IsNullOrEmpty(item.ImagePath))
                    {
                        var oldImagePath = Path.Combine(_environment.WebRootPath, "images/books-media", item.ImagePath);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    item.ImagePath = newFileName;
                }

                _context.Items.Update(item);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "The Book/CD is successfully updated!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Unable to edit the item. Try again!";
                LoadCategoriesAndAuthors();
                return View(itemVM);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null)
            {
                return Json(new { success = false, message = "Item not found." });
            }

            if (!string.IsNullOrEmpty(item.ImagePath))
            {
                var imagePath = Path.Combine(_environment.WebRootPath, "images/books-media", item.ImagePath);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            _context.Items.Remove(item);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "The Book/CD is successfully deleted!" });
        }

        [NonAction]
        private void LoadCategoriesAndAuthors()
        {
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name");
            ViewBag.AuthorsAndArtists = new SelectList(_context.AuthorsAndArtists, "Id", "Name");
        }
    }
}
