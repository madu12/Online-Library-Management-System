using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using online_library_management_system.Models;
using online_library_management_system.Services;
using online_library_management_system.ViewModels;

namespace online_library_management_system.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var viewModel = new ItemVM
        {
            Items = _context.Items.Include("Categories")
                                      .Include("AuthorsAndArtists")
                                      .OrderByDescending(i => i.ItemId)
                                      .Take(6)
                                      .ToList()
        };
        return View(viewModel);
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

