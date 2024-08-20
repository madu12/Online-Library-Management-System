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
                    Status = r.Status
                })
                .ToListAsync();

            var viewModel = new ReservationAdminVM
            {
                Reservations = reservations,
                Status = Status
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            var reservation = await _context.Reservations.Include(r => r.Item).FirstOrDefaultAsync(r => r.ReservationId == id);
            if (reservation == null)
            {
                TempData["ErrorMessage"] = "Reservation not found.";
                return RedirectToAction(nameof(Index));
            }

            if (reservation.Item!.Availability == "No")
            {
                TempData["ErrorMessage"] = "The item is no longer available.";
                return RedirectToAction(nameof(Index));
            }

            reservation.Status = ReservationStatus.Approved;
            reservation.ApprovedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Reservation approved successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Deny(int id)
        {
            var reservation = await _context.Reservations.Include(r => r.Item).FirstOrDefaultAsync(r => r.ReservationId == id);
            if (reservation == null)
            {
                TempData["ErrorMessage"] = "Reservation not found.";
                return RedirectToAction(nameof(Index));
            }

            reservation.Status = ReservationStatus.Rejected;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Reservation denied successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Expire(int id)
        {
            var reservation = await _context.Reservations.Include(r => r.Item).FirstOrDefaultAsync(r => r.ReservationId == id);
            if (reservation == null)
            {
                TempData["ErrorMessage"] = "Reservation not found.";
                return RedirectToAction(nameof(Index));
            }

            reservation.Status = ReservationStatus.Expired;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Reservation expired and item is available again.";
            return RedirectToAction(nameof(Index));
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