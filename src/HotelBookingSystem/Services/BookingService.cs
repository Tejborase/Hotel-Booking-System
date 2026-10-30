using HotelBookingSystem.Data;
using HotelBookingSystem.Models;
using HotelBookingSystem.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace HotelBookingSystem.Services
{
    public class BookingService : IBookingService
    {
        private readonly ApplicationDbContext _context;

        public BookingService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkIn, DateTime checkOut, int? excludeBookingId = null)
        {
            var room = await _context.Rooms.AsNoTracking().FirstOrDefaultAsync(r => r.RoomId == roomId);
            if (room == null || !room.IsActive)
            {
                return false;
            }

            var checkInDate = checkIn.Date;
            var checkOutDate = checkOut.Date;

            if (checkOutDate <= checkInDate)
            {
                return false;
            }

            // Overlap condition:
            // ExistingCheckIn < RequestedCheckOut AND ExistingCheckOut > RequestedCheckIn
            // Ignore Cancelled bookings
            var hasOverlap = await _context.Bookings.AnyAsync(b =>
                b.RoomId == roomId &&
                b.BookingStatus != "Cancelled" &&
                b.CheckInDate < checkOutDate &&
                b.CheckOutDate > checkInDate &&
                (!excludeBookingId.HasValue || b.BookingId != excludeBookingId.Value));

            return !hasOverlap;
        }

        public decimal CalculateBookingTotal(decimal pricePerNight, DateTime checkIn, DateTime checkOut)
        {
            var nights = (checkOut.Date - checkIn.Date).Days;
            if (nights <= 0)
            {
                return 0m;
            }
            return pricePerNight * nights;
        }

        public async Task<(bool Success, string Message, Booking? Booking)> CreateBookingAsync(BookingCreateViewModel model)
        {
            // 1. Basic Date Validation
            var checkInDate = model.CheckInDate.Date;
            var checkOutDate = model.CheckOutDate.Date;
            var today = DateTime.Today;

            if (checkInDate < today)
            {
                return (false, "Check-in date cannot be in the past.", null);
            }

            if (checkOutDate <= checkInDate)
            {
                return (false, "Check-out date must be later than check-in date.", null);
            }

            var nights = (checkOutDate - checkInDate).Days;
            if (nights < 1)
            {
                return (false, "Booking must be for at least one night.", null);
            }

            // 2. Fetch Room from DB
            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.RoomId == model.RoomId);
            if (room == null)
            {
                return (false, "The selected room does not exist.", null);
            }

            if (!room.IsActive)
            {
                return (false, "This room is currently inactive and cannot be booked.", null);
            }

            // 3. Guest Capacity Validation
            if (model.GuestsCount < 1)
            {
                return (false, "Guest count must be at least 1.", null);
            }

            if (model.GuestsCount > room.Capacity)
            {
                return (false, $"The selected room has a maximum capacity of {room.Capacity} guests.", null);
            }

            // 4. Concurrency & Availability verification immediately before saving
            // Using ExecutionStrategy to support execution under transaction
            var strategy = _context.Database.CreateExecutionStrategy();
            Booking? createdBooking = null;
            string? errorMessage = null;

            await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    bool isAvailable = await _context.Bookings.AnyAsync(b =>
                        b.RoomId == model.RoomId &&
                        b.BookingStatus != "Cancelled" &&
                        b.CheckInDate < checkOutDate &&
                        b.CheckOutDate > checkInDate);

                    if (isAvailable)
                    {
                        errorMessage = "We are sorry, this room was just reserved for the selected dates by another guest. Please choose different dates or another luxury suite.";
                        return;
                    }

                    // 5. Server-side Calculation of Total (NEVER trust browser)
                    var pricePerNightSnapshot = room.PricePerNight;
                    var serverCalculatedTotal = pricePerNightSnapshot * nights;

                    // 6. Generate Unique Reference Number
                    string reference = GenerateBookingReference();
                    while (await _context.Bookings.AnyAsync(b => b.BookingReference == reference))
                    {
                        reference = GenerateBookingReference();
                    }

                    createdBooking = new Booking
                    {
                        BookingReference = reference,
                        RoomId = room.RoomId,
                        GuestName = model.GuestName.Trim(),
                        Email = model.Email.Trim().ToLower(),
                        Phone = model.Phone.Trim(),
                        CheckInDate = checkInDate,
                        CheckOutDate = checkOutDate,
                        GuestsCount = model.GuestsCount,
                        PricePerNight = pricePerNightSnapshot,
                        TotalAmount = serverCalculatedTotal,
                        BookingStatus = "Confirmed",
                        SpecialRequests = model.SpecialRequests?.Trim(),
                        CreatedAt = DateTime.UtcNow
                    };

                    await _context.Bookings.AddAsync(createdBooking);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    errorMessage = $"An unexpected error occurred while confirming your reservation: {ex.Message}";
                }
            });

            if (errorMessage != null || createdBooking == null)
            {
                return (false, errorMessage ?? "Booking failed.", null);
            }

            // Attach room for confirmation display
            createdBooking.Room = room;

            return (true, "Booking confirmed successfully!", createdBooking);
        }

        public async Task<Booking?> GetBookingByReferenceAndEmailAsync(string bookingReference, string email)
        {
            if (string.IsNullOrWhiteSpace(bookingReference) || string.IsNullOrWhiteSpace(email))
            {
                return null;
            }

            var cleanRef = bookingReference.Trim().ToUpper();
            var cleanEmail = email.Trim().ToLower();

            return await _context.Bookings
                .Include(b => b.Room)
                .AsNoTracking()
                .FirstOrDefaultAsync(b => 
                    b.BookingReference.ToUpper() == cleanRef && 
                    b.Email.ToLower() == cleanEmail);
        }

        public async Task<Booking?> GetBookingByReferenceAsync(string bookingReference)
        {
            if (string.IsNullOrWhiteSpace(bookingReference))
            {
                return null;
            }

            var cleanRef = bookingReference.Trim().ToUpper();

            return await _context.Bookings
                .Include(b => b.Room)
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.BookingReference.ToUpper() == cleanRef);
        }

        public async Task<Booking?> GetBookingByIdAsync(int bookingId)
        {
            return await _context.Bookings
                .Include(b => b.Room)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);
        }

        public async Task<IEnumerable<Booking>> GetAllBookingsAsync(string? status = null, string? searchTerm = null)
        {
            var query = _context.Bookings
                .Include(b => b.Room)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(status) && status != "All")
            {
                query = query.Where(b => b.BookingStatus == status);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLower();
                query = query.Where(b => 
                    b.BookingReference.ToLower().Contains(term) ||
                    b.GuestName.ToLower().Contains(term) ||
                    b.Email.ToLower().Contains(term) ||
                    b.Phone.ToLower().Contains(term) ||
                    (b.Room != null && b.Room.RoomNumber.ToLower().Contains(term)));
            }

            return await query
                .OrderByDescending(b => b.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<(bool Success, string Message)> UpdateBookingStatusAsync(int bookingId, string newStatus)
        {
            var validStatuses = new[] { "Pending", "Confirmed", "Cancelled" };
            if (!validStatuses.Contains(newStatus))
            {
                return (false, "Invalid booking status.");
            }

            var booking = await _context.Bookings
                .Include(b => b.Room)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);

            if (booking == null)
            {
                return (false, "Booking not found.");
            }

            if (booking.BookingStatus == newStatus)
            {
                return (true, $"Booking is already {newStatus}.");
            }

            // Business rule: Prevent invalid status transitions.
            // Do not allow cancelled bookings to become confirmed without a deliberate new action and checking date overlap.
            if (booking.BookingStatus == "Cancelled" && newStatus == "Confirmed")
            {
                // Verify room availability before un-cancelling
                bool isRoomAvailable = await IsRoomAvailableAsync(
                    booking.RoomId, 
                    booking.CheckInDate, 
                    booking.CheckOutDate, 
                    booking.BookingId);

                if (!isRoomAvailable)
                {
                    return (false, "Cannot re-confirm this cancelled booking because the room has already been booked by another guest for those dates.");
                }
            }

            booking.BookingStatus = newStatus;
            await _context.SaveChangesAsync();

            return (true, $"Booking {booking.BookingReference} status updated to {newStatus}.");
        }

        public async Task<AdminDashboardViewModel> GetDashboardStatsAsync()
        {
            var today = DateTime.Today;

            var totalRooms = await _context.Rooms.CountAsync();
            var activeRooms = await _context.Rooms.CountAsync(r => r.IsActive);

            var totalBookings = await _context.Bookings.CountAsync();
            var confirmedBookings = await _context.Bookings.CountAsync(b => b.BookingStatus == "Confirmed");
            var pendingBookings = await _context.Bookings.CountAsync(b => b.BookingStatus == "Pending");
            var cancelledBookings = await _context.Bookings.CountAsync(b => b.BookingStatus == "Cancelled");

            var totalRevenue = await _context.Bookings
                .Where(b => b.BookingStatus == "Confirmed")
                .SumAsync(b => (decimal?)b.TotalAmount) ?? 0m;

            // Occupancy for today: Confirmed bookings where CheckIn <= Today and CheckOut > Today
            var currentBookingsToday = await _context.Bookings
                .Include(b => b.Room)
                .Where(b => b.BookingStatus == "Confirmed" &&
                            b.CheckInDate <= today &&
                            b.CheckOutDate > today)
                .ToListAsync();

            double occupancyRate = activeRooms > 0 
                ? Math.Round(((double)currentBookingsToday.Count / activeRooms) * 100, 1) 
                : 0.0;

            var recentBookings = await _context.Bookings
                .Include(b => b.Room)
                .OrderByDescending(b => b.CreatedAt)
                .Take(7)
                .AsNoTracking()
                .ToListAsync();

            var allRooms = await _context.Rooms
                .Include(r => r.Bookings)
                .Where(r => r.IsActive)
                .ToListAsync();

            var roomOccupancyList = allRooms.Select(r =>
            {
                var currentBooking = currentBookingsToday.FirstOrDefault(b => b.RoomId == r.RoomId);
                return new RoomOccupancySummary
                {
                    RoomId = r.RoomId,
                    RoomNumber = r.RoomNumber,
                    RoomType = r.RoomType,
                    PricePerNight = r.PricePerNight,
                    IsOccupiedToday = currentBooking != null,
                    CurrentGuestName = currentBooking?.GuestName,
                    TotalBookingsCount = r.Bookings.Count(b => b.BookingStatus != "Cancelled")
                };
            }).ToList();

            return new AdminDashboardViewModel
            {
                TotalRooms = totalRooms,
                ActiveRooms = activeRooms,
                TotalBookings = totalBookings,
                ConfirmedBookings = confirmedBookings,
                PendingBookings = pendingBookings,
                CancelledBookings = cancelledBookings,
                TotalRevenue = totalRevenue,
                OccupancyRate = occupancyRate,
                RecentBookings = recentBookings,
                RoomOccupancies = roomOccupancyList
            };
        }

        private static string GenerateBookingReference()
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            var bytes = new byte[5];
            RandomNumberGenerator.Fill(bytes);
            var code = new char[5];
            for (int i = 0; i < 5; i++)
            {
                code[i] = chars[bytes[i] % chars.Length];
            }
            return $"GV-{DateTime.UtcNow.Year}-{new string(code)}";
        }
    }
}
