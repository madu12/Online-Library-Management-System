using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using online_library_management_system.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using online_library_management_system.Services;
using online_library_management_system.Models;

namespace online_library_management_system.Areas.Admin.Controllers
{
    public class ItemsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? search, string? selectedCategories, string? selectedItemTypes, string? availability, int page = 1, int pageSize = 9)
        {
            // Validate page and pageSize
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 100);

            // Parse selected categories and item types from the query string
            var selectedCategoryList = !string.IsNullOrWhiteSpace(selectedCategories)
                ? selectedCategories.Split(',').ToList()
                : new List<string>();

            var selectedItemTypeList = !string.IsNullOrWhiteSpace(selectedItemTypes)
                ? selectedItemTypes.Split(',').ToList()
                : new List<string>();


            // Validate selected item types
            var validItemTypes = new List<string> { "Book", "CD" };
            selectedItemTypeList = selectedItemTypeList
                .Where(validItemTypes.Contains)
                .ToList();

            // Validate availability
            if (!string.IsNullOrWhiteSpace(availability) && availability != "Yes" && availability != "No")
            {
                availability = null;
            }

            // Prepare the query
            var itemsQuery = _context.Items
                                     .Include(i => i.Categories)
                                     .Include(i => i.AuthorsAndArtists)
                                     .AsQueryable();

            // Search by title, author/artist name, or ISBN
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                itemsQuery = itemsQuery.Where(i => i.Title.Contains(search) || i.AuthorsAndArtists!.Name.Contains(search) || i.ISBN!.Contains(search));
            }

            // Filter by selected categories
            if (selectedCategoryList.Count != 0)
            {
                itemsQuery = itemsQuery.Where(i => selectedCategoryList.Contains(i.Categories!.Name));
            }

            // Filter by selected item types
            if (selectedItemTypeList.Count != 0)
            {
                itemsQuery = itemsQuery.Where(i => selectedItemTypeList.Contains(i.ItemType));
            }

            // Filter by availability
            if (!string.IsNullOrWhiteSpace(availability))
            {
                itemsQuery = itemsQuery.Where(i => i.Availability == availability);
            }

            // Pagination
            var totalItems = await itemsQuery.CountAsync();
            var items = await itemsQuery
                                    .OrderByDescending(i => i.PublishDate)
                                    .Skip((page - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToListAsync();

            var viewModel = new ItemVM
            {
                Items = items,
                Search = search,
                SelectedCategories = selectedCategoryList,
                SelectedItemTypes = selectedItemTypeList,
                Availability = availability,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
                Categories = new SelectList(await _context.Categories.ToListAsync(), "Name", "Name")
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Index(string? search, List<string>? selectedCategories, List<string>? selectedItemTypes, string? availability)
        {
            return RedirectToAction("Index", new
            {
                search,
                selectedCategories = selectedCategories != null ? string.Join(",", selectedCategories) : null,
                selectedItemTypes = selectedItemTypes != null ? string.Join(",", selectedItemTypes) : null,
                availability
            });
        }


        [Route("Items/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var item = await _context.Items
                .Include(i => i.AuthorsAndArtists)
                .Include(i => i.Categories)
                .FirstOrDefaultAsync(i => i.ItemId == id);

            if (item == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new ItemVM
            {
                Items = new List<Item> { item },
                Categories = new SelectList(await _context.Categories.ToListAsync(), "Name", "Name")
            };

            return View(viewModel);
        }

    }
}
