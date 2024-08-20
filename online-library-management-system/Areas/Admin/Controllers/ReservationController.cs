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
    public class ReservationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _environment;

        public ReservationController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _environment = environment;
        }

        public async Task<IActionResult> Index(string Status)
        {
            LoadStatuses(Status);

            var query = _context.Reservations
                .Include(r => r.Item)
                .Include(r => r.User)
                .AsQueryable();

            // Apply status filter if provided
            if (!string.IsNullOrEmpty(Status) && Enum.TryParse(Status, out ReservationStatus status))
            {
                query = query.Where(r => r.Status == status);
            }

            var totalReservations = await query.CountAsync();

            var reservations = await query
                .OrderBy(r => r.ReservedAt)
                .Select(r => new ReservationVM
                {
                    ReservationId = r.ReservationId,
                    ItemTitle = r.Item!.Title,
                    UserFullName = $"{r.User!.FirstName} {r.User.LastName}",
                    UserEmail = r.User.Email,
                    ReservedAt = r.ReservedAt,
                    Status = r.Status,
                    AdminComment = r.AdminComment
                })
                .ToListAsync();

            var viewModel = new ReservationAdminVM
            {
                Reservations = reservations,
                Status = Status
            };

            return View(viewModel);
        }

        [NonAction]
        private void LoadStatuses(string? selectedStatus = null)
        {
            var statuses = Enum.GetValues(typeof(ReservationStatus))
                .Cast<ReservationStatus>()
                .Select(status => new SelectListItem
                {
                    Value = status.ToString(),
                    Text = status.ToString(),
                    Selected = status.ToString() == selectedStatus
                }).ToList();

            ViewBag.Statuses = statuses;
        }
    }
}