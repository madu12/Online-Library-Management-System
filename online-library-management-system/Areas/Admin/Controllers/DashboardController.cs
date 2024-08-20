using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using online_library_management_system.Models;
using online_library_management_system.Services;
using online_library_management_system.ViewModels;

namespace online_library_management_system.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var issuedCount = await _context.Lendings.CountAsync(l => l.Status == LendingStatus.Issued);
            var returnedCount = await _context.Lendings.CountAsync(l => l.Status == LendingStatus.Returned);
            var overdueItems = await _context.Lendings
                .Include(l => l.Reservation!.Item)
                .Include(l => l.Reservation!.User)
                .Where(l => l.DueDate < DateTime.UtcNow && l.Status == LendingStatus.Issued)
                .Select(l => new OverdueItemVM
                {
                    LendingId = l.LendingId,
                    ItemTitle = l.Reservation!.Item!.Title,
                    UserName = $"{l.Reservation!.User!.FirstName} {l.Reservation!.User.LastName}",
                    UserEmail = l.Reservation.User.Email,
                    UserPhone = l.Reservation.User.PhoneNo,
                    DueDate = l.DueDate,
                    DaysOverdue = (DateTime.UtcNow - l.DueDate).Days
                })
                .ToListAsync();


            var dashboardViewModel = new DashboardViewModel
            {
                TotalUsers = await _context.Users.CountAsync(),
                TotalItems = await _context.Items.CountAsync(),
                PendingReservations = await _context.Reservations.CountAsync(r => r.Status == ReservationStatus.Pending),
                OverdueItems = await _context.Lendings.CountAsync(l => l.DueDate < DateTime.UtcNow && l.Status == LendingStatus.Issued),
                IssuedCount = issuedCount,
                ReturnedCount = returnedCount,
                OverdueItemsList = overdueItems
            };

            return View(dashboardViewModel);
        }

    }
}
