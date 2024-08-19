using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using online_library_management_system.Models;
using online_library_management_system.Services;
using online_library_management_system.ViewModels;

namespace online_library_management_system.Controllers
{
    public class ContactController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContactController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(SendMessageVM model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var message = new UserMessage
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email,
                        Subject = model.Subject,
                        Message = model.Message
                    };

                    _context.UserMessages.Add(message);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Your message was sent successfully! Our team will contact you soon.";
                }
                catch (Exception)
                {
                    TempData["Error"] = "Something went wrong, try refreshing and submitting the form again.";
                }

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

    }
}
