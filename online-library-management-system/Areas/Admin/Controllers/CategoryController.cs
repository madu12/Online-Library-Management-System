using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using online_library_management_system.Services;
using online_library_management_system.Models;
using Microsoft.EntityFrameworkCore;
using online_library_management_system.ViewModels;

namespace online_library_management_system.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var viewModel = new CategoryVM
            {
                Categories = _context.Categories.ToList(),
                NewCategory = new Category()
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Index(CategoryVM categoryVM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Check if the category name already exists
                    bool categoryExists = await _context.Categories.AnyAsync(c => c.Name == categoryVM.NewCategory.Name);
                    if (categoryExists)
                    {
                        ModelState.AddModelError("NewCategory.Name", "Category name already exists.");
                        categoryVM.Categories = _context.Categories.ToList();
                        return View(categoryVM);
                    }

                    // If name does not exist, create the category
                    Category category = new()
                    {
                        Name = categoryVM.NewCategory.Name,
                        CreatedAt = DateTime.Now
                    };
                    _context.Categories.Add(category);
                    await _context.SaveChangesAsync();

                    // Set success message
                    TempData["SuccessMessage"] = "Category added successfully!";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Unable to save changes. Try again!";
            }
            categoryVM.Categories = _context.Categories.ToList();
            return View(categoryVM);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var category = await _context.Categories.FindAsync(id);
                if (category == null)
                {
                    TempData["ErrorMessage"] = "Category not found.";
                    return RedirectToAction(nameof(Index));
                }

                var viewModel = new CategoryVM
                {
                    NewCategory = category
                };

                return View(viewModel);
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Unable to load the category. Try again!";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, CategoryVM categoryVM)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(categoryVM);
                }

                var category = await _context.Categories.FindAsync(id);
                if (category == null)
                {
                    TempData["ErrorMessage"] = "Category not found.";
                    return RedirectToAction(nameof(Index));
                }

                category.Name = categoryVM.NewCategory.Name;

                _context.Categories.Update(category);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Category updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Unable to edit the category. Try again!";
                return View(categoryVM);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var category = await _context.Categories
                    .Include(c => c.Items)
                    .FirstOrDefaultAsync(c => c.Id == id);
                if (category == null)
                {
                    return Json(new { success = false, message = "Category not found." });
                }

                if (category.Items?.Count != 0)
                {
                    return Json(new { success = false, message = "Category cannot be deleted because it is in use." });
                }

                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Category deleted successfully!" });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Unable to delete the category. Try again!" });
            }
        }

    }

}
