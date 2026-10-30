using HotelBookingSystem.Services;
using HotelBookingSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingSystem.Controllers
{
    public class RoomsController : Controller
    {
        private readonly IRoomService _roomService;
        private readonly IBookingService _bookingService;

        public RoomsController(IRoomService roomService, IBookingService bookingService)
        {
            _roomService = roomService;
            _bookingService = bookingService;
        }

        // GET: /Rooms
        public async Task<IActionResult> Index(RoomSearchViewModel search)
        {
            // If dates are provided, ensure check-out is after check-in
            if (search.CheckInDate.HasValue && search.CheckOutDate.HasValue)
            {
                if (search.CheckInDate.Value.Date < DateTime.Today)
                {
                    ModelState.AddModelError("CheckInDate", "Check-in date cannot be in the past.");
                }
                if (search.CheckOutDate.Value.Date <= search.CheckInDate.Value.Date)
                {
                    ModelState.AddModelError("CheckOutDate", "Check-out date must be later than check-in date.");
                }
            }

            IEnumerable<Models.Room> rooms;

            if (ModelState.IsValid && search.CheckInDate.HasValue && search.CheckOutDate.HasValue)
            {
                rooms = await _roomService.GetAvailableRoomsAsync(
                    search.CheckInDate.Value,
                    search.CheckOutDate.Value,
                    search.Guests ?? 1,
                    search.RoomType,
                    search.MinPrice,
                    search.MaxPrice,
                    search.SortBy);
            }
            else
            {
                rooms = await _roomService.GetAllRoomsAsync(
                    search.RoomType,
                    search.MinPrice,
                    search.MaxPrice,
                    search.Guests,
                    search.SortBy,
                    onlyActive: true);
            }

            var roomTypes = await _roomService.GetDistinctRoomTypesAsync();

            search.Rooms = rooms;
            search.AvailableRoomTypes = roomTypes;
            search.TotalResultsCount = rooms.Count();

            return View(search);
        }

        // GET: /Rooms/Details/5
        public async Task<IActionResult> Details(int id, DateTime? checkIn, DateTime? checkOut, int? guests)
        {
            var room = await _roomService.GetRoomByIdAsync(id);
            if (room == null || !room.IsActive)
            {
                return NotFound();
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
            var isAvailable = await _bookingService.IsRoomAvailableAsync(id, effectiveCheckIn, effectiveCheckOut);

            var allRooms = await _roomService.GetAllRoomsAsync(onlyActive: true);
            var similar = allRooms.Where(r => r.RoomId != id).Take(3).ToList();

            var viewModel = new RoomDetailsViewModel
            {
                Room = room,
                SimilarRooms = similar,
                CheckInDate = effectiveCheckIn,
                CheckOutDate = effectiveCheckOut,
                Guests = guests.HasValue && guests.Value > 0 ? guests.Value : Math.Min(2, room.Capacity),
                NumberOfNights = nights,
                EstimatedTotal = room.PricePerNight * nights,
                IsAvailableForSelectedDates = isAvailable
            };

            return View(viewModel);
        }
    }
}
