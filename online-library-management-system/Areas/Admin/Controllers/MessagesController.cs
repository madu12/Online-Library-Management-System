using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using online_library_management_system.Models;
using online_library_management_system.ViewModels;
using Microsoft.EntityFrameworkCore;
using online_library_management_system.Services;

namespace online_library_management_system.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class MessagesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const int PageSize = 10;

        public MessagesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var totalMessages = await _context.UserMessages.CountAsync();
            var messages = await _context.UserMessages
                .OrderByDescending(m => m.SentAt)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            var viewModel = new AdminUserMessageVM
            {
                Messages = messages,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalMessages / (double)PageSize)
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Preview(int id)
        {
            var message = await _context.UserMessages
                .FirstOrDefaultAsync(m => m.MessageId == id);

            if (message == null)
            {
                return NotFound();
            }

            return PartialView("_MessagePreview", message);
        }

    }
}
