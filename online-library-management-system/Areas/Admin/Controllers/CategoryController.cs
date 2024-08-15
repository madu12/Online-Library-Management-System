using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using online_library_management_system.Services;
using online_library_management_system.Models;
using Microsoft.EntityFrameworkCore;
using online_library_management_system.ViewModels;
using Microsoft.AspNetCore.Identity;

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
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = "Unable to save changes. Try again!";
            }
            categoryVM.Categories = _context.Categories.ToList();
            return View(categoryVM);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var category = await _context.Categories.FindAsync(id);
                if (category != null)
                {
                    _context.Categories.Remove(category);
                    await _context.SaveChangesAsync();

                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "Category not found." });
                }
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Unable to delete the category. Try again!" });
            }
        }

    }

}
