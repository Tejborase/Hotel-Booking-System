using HotelBookingSystem.Services;
using HotelBookingSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingSystem.Controllers
{
    public class BookingsController : Controller
    {
        private readonly IRoomService _roomService;
        private readonly IBookingService _bookingService;

        public BookingsController(IRoomService roomService, IBookingService bookingService)
        {
            _roomService = roomService;
            _bookingService = bookingService;
        }

        // GET: /Bookings/Create?roomId=1&checkIn=...&checkOut=...&guests=...
        [HttpGet]
        public async Task<IActionResult> Create(int roomId, DateTime? checkIn, DateTime? checkOut, int? guests)
        {
            var room = await _roomService.GetRoomByIdAsync(roomId);
            if (room == null || !room.IsActive)
            {
                TempData["ErrorMessage"] = "The requested room is not available.";
                return RedirectToAction("Index", "Rooms");
            }

            var effectiveCheckIn = checkIn?.Date ?? DateTime.Today;
            if (effectiveCheckIn < DateTime.Today)
            {
                effectiveCheckIn = DateTime.Today;
            }

            var effectiveCheckOut = checkOut?.Date ?? effectiveCheckIn.AddDays(1);
            if (effectiveCheckOut <= effectiveCheckIn)
            {
                effectiveCheckOut = effectiveCheckIn.AddDays(1);
            }

            var nights = (effectiveCheckOut - effectiveCheckIn).Days;
            var guestCount = guests.HasValue && guests.Value > 0 ? guests.Value : Math.Min(2, room.Capacity);

            // Double check availability
            var isAvailable = await _bookingService.IsRoomAvailableAsync(roomId, effectiveCheckIn, effectiveCheckOut);
            if (!isAvailable)
            {
                TempData["ErrorMessage"] = $"Room {room.RoomNumber} ({room.RoomType}) is not available for {effectiveCheckIn:MMM dd, yyyy} to {effectiveCheckOut:MMM dd, yyyy}. Please choose different dates.";
                return RedirectToAction("Details", "Rooms", new { id = roomId, checkIn = effectiveCheckIn.ToString("yyyy-MM-dd"), checkOut = effectiveCheckOut.ToString("yyyy-MM-dd"), guests = guestCount });
            }

            var model = new BookingCreateViewModel
            {
                RoomId = room.RoomId,
                RoomNumber = room.RoomNumber,
                RoomType = room.RoomType,
                PricePerNight = room.PricePerNight,
                Capacity = room.Capacity,
                ImageUrl = room.ImageUrl,
                Amenities = room.Amenities,
                CheckInDate = effectiveCheckIn,
                CheckOutDate = effectiveCheckOut,
                GuestsCount = Math.Min(guestCount, room.Capacity),
                CalculatedNights = nights,
                CalculatedTotalAmount = room.PricePerNight * nights
            };

            return View(model);
        }

        // POST: /Bookings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookingCreateViewModel model)
        {
            var room = await _roomService.GetRoomByIdAsync(model.RoomId);
            if (room == null || !room.IsActive)
            {
                ModelState.AddModelError(string.Empty, "The selected room is unavailable.");
                return View(model);
            }

            // Repopulate display fields
            model.RoomNumber = room.RoomNumber;
            model.RoomType = room.RoomType;
            model.PricePerNight = room.PricePerNight;
            model.Capacity = room.Capacity;
            model.ImageUrl = room.ImageUrl;
            model.Amenities = room.Amenities;

            if (model.CheckInDate.Date < DateTime.Today)
            {
                ModelState.AddModelError("CheckInDate", "Check-in date cannot be in the past.");
            }

            if (model.CheckOutDate.Date <= model.CheckInDate.Date)
            {
                ModelState.AddModelError("CheckOutDate", "Check-out date must be later than check-in date.");
            }

            if (model.GuestsCount > room.Capacity)
            {
                ModelState.AddModelError("GuestsCount", $"This room accommodates a maximum of {room.Capacity} guests.");
            }

            if (!ModelState.IsValid)
            {
                var nights = Math.Max(1, (model.CheckOutDate.Date - model.CheckInDate.Date).Days);
                model.CalculatedNights = nights;
                model.CalculatedTotalAmount = room.PricePerNight * nights;
                return View(model);
            }

            var (success, message, createdBooking) = await _bookingService.CreateBookingAsync(model);

            if (!success || createdBooking == null)
            {
                ModelState.AddModelError(string.Empty, message);
                var nights = Math.Max(1, (model.CheckOutDate.Date - model.CheckInDate.Date).Days);
                model.CalculatedNights = nights;
                model.CalculatedTotalAmount = room.PricePerNight * nights;
                return View(model);
            }

            TempData["SuccessMessage"] = "Reservation successfully booked! Welcome to GrandVista.";
            return RedirectToAction("Confirmation", new { id = createdBooking.BookingId });
        }

        // GET: /Bookings/Confirmation?id=5
        [HttpGet]
        public async Task<IActionResult> Confirmation(int id)
        {
            if (id <= 0)
            {
                return RedirectToAction("Index", "Home");
            }

            var booking = await _bookingService.GetBookingByIdAsync(id);
            if (booking == null)
            {
                TempData["ErrorMessage"] = "Reservation not found.";
                return RedirectToAction("Index", "Home");
            }

            var nights = (booking.CheckOutDate.Date - booking.CheckInDate.Date).Days;

            var model = new BookingConfirmationViewModel
            {
                Booking = booking,
                Room = booking.Room!,
                Nights = nights
            };

            return View(model);
        }

        // GET: /Bookings/MyBooking
        [HttpGet]
        public IActionResult MyBooking()
        {
            var model = new MyBookingLookupViewModel();
            return View(model);
        }

        // POST: /Bookings/MyBooking
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MyBooking(MyBookingLookupViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var booking = await _bookingService.GetBookingByIdAndEmailAsync(model.BookingId!.Value, model.Email);
            model.HasSearched = true;

            if (booking == null)
            {
                model.SearchError = "No reservation matched that Booking ID and email address. Please double-check your confirmation details.";
                model.BookingResult = null;
            }
            else
            {
                model.BookingResult = booking;
            }

            return View(model);
        }
    }
}
