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
    public class LendingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LendingController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string search)
        {
            if (string.IsNullOrWhiteSpace(search))
            {
                return View();
            }

            var reservations = await (from reservation in _context.Reservations
                                      join lending in _context.Lendings
                                      on reservation.ReservationId equals lending.ReservationId into reservationLending
                                      from rl in reservationLending.DefaultIfEmpty()
                                      where (reservation.Status == ReservationStatus.Approved || reservation.Status == ReservationStatus.Issued) &&
                                            (reservation.ReservationId.ToString().Contains(search) ||
                                             reservation.User!.Email!.Contains(search) ||
                                             (reservation.User.FirstName + " " + reservation.User.LastName).Contains(search) ||
                                             reservation.Item!.ISBN!.Contains(search) ||
                                             reservation.Item.Title.Contains(search))
                                      select new ReservationSearchVM
                                      {
                                          ReservationId = reservation.ReservationId,
                                          UserFullName = $"{reservation.User!.FirstName} {reservation.User.LastName}",
                                          UserEmail = reservation.User.Email,
                                          ItemTitle = reservation.Item!.Title,
                                          ISBN = reservation.Item.ISBN,
                                          Status = reservation.Status,
                                          ReservedAt = reservation.ReservedAt,
                                          ItemType = reservation.Item.ItemType,
                                          IssuedAt = rl != null ? rl.IssuedAt : (DateTime?)null,
                                          ReturnedAt = rl != null ? rl.ReturnedAt : (DateTime?)null
                                      }).ToListAsync();
            if (reservations.Count == 0)
            {
                TempData["ErrorMessage"] = "No reservations found matching the search criteria.";
                return View(new List<ReservationSearchVM>());
            }
            return View(reservations);
        }

        [HttpPost]
        public async Task<IActionResult> Issue(int id)
        {
            try
            {
                // Retrieve the reservation
                var reservation = await _context.Reservations
                    .Include(r => r.Item)
                    .Include(r => r.User)
                    .FirstOrDefaultAsync(r => r.ReservationId == id);

                if (reservation == null || reservation.Status != ReservationStatus.Approved)
                {
                    TempData["ErrorMessage"] = "Invalid reservation or reservation is not approved.";
                    return RedirectToAction(nameof(Index));
                }

                // Check if the Book/CD has already been issued
                var existingLending = await _context.Lendings
                    .FirstOrDefaultAsync(l => l.ReservationId == id);

                if (existingLending != null)
                {
                    TempData["ErrorMessage"] = "This " + (reservation.Item!.ItemType == "Book" ? "Book" : "CD") + " has already been issued.";
                    return RedirectToAction(nameof(Index));
                }

                // Create the lending record
                var lending = new Lending
                {
                    ReservationId = id,
                    IssuedAt = DateTime.UtcNow,
                    DueDate = DateTime.UtcNow.AddDays(14),
                    Status = LendingStatus.Issued
                };

                _context.Lendings.Add(lending);

                // Update reservation status to Issued
                reservation.Status = ReservationStatus.Issued;

                // Update the item's availability to "No"
                reservation.Item!.Availability = "No";

                // Save changes
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = (reservation.Item.ItemType == "Book" ? "Book" : "CD") + " issued successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to issue the Book/CD: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Return(int id)
        {
            try
            {
                // Retrieve the lending record
                var lending = await _context.Lendings
                    .Include(l => l.Reservation)
                    .ThenInclude(r => r!.Item)
                    .FirstOrDefaultAsync(l => l.ReservationId == id && l.Status == LendingStatus.Issued);

                if (lending == null)
                {
                    TempData["ErrorMessage"] = "No active lending record found for this reservation.";
                    return RedirectToAction(nameof(Index));
                }

                // Set the returned date and update the status
                lending.ReturnedAt = DateTime.UtcNow;
                lending.Status = LendingStatus.Returned;

                // Update the reservation status to Completed
                lending.Reservation!.Status = ReservationStatus.Completed;

                // Update the item's availability to "Yes"
                lending.Reservation.Item!.Availability = "Yes";

                // Save changes
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = (lending.Reservation.Item.ItemType == "Book" ? "Book" : "CD") + " returned successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to return the Book/CD: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
