using HotelBookingSystem.Services;
using HotelBookingSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingSystem.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    [Route("Admin/[controller]/[action]")]
    [Route("Admin/[controller]")]
    public class BookingsController : Controller
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        // GET: /Admin/Bookings
        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index(string? status, string? searchTerm)
        {
            var bookings = await _bookingService.GetAllBookingsAsync(status, searchTerm);
            var model = new BookingAdminFilterViewModel
            {
                Status = status ?? "All",
                SearchTerm = searchTerm,
                Bookings = bookings
            };

            return View("~/Views/Admin/Bookings/Index.cshtml", model);
        }

        // GET: /Admin/Bookings/Details/5
        [HttpGet("Details/{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            var booking = await _bookingService.GetBookingByIdAsync(id);
            if (booking == null)
            {
                TempData["ErrorMessage"] = "Booking not found.";
                return RedirectToAction(nameof(Index));
            }

            return View("~/Views/Admin/Bookings/Details.cshtml", booking);
        }

        // POST: /Admin/Bookings/Confirm/5
        [HttpPost("Confirm/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirm(int id)
        {
            var (success, message) = await _bookingService.UpdateBookingStatusAsync(id, "Confirmed");
            if (success)
            {
                TempData["SuccessMessage"] = message;
            }
            else
            {
                TempData["ErrorMessage"] = message;
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: /Admin/Bookings/Cancel/5
        [HttpPost("Cancel/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var (success, message) = await _bookingService.UpdateBookingStatusAsync(id, "Cancelled");
            if (success)
            {
                TempData["SuccessMessage"] = message;
            }
            else
            {
                TempData["ErrorMessage"] = message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
