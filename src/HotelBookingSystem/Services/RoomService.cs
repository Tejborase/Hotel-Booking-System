using HotelBookingSystem.Data;
using HotelBookingSystem.Models;
using HotelBookingSystem.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Services
{
    public class RoomService : IRoomService
    {
        private readonly ApplicationDbContext _context;

        public RoomService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Room>> GetAllRoomsAsync(
            string? roomType = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            int? guests = null,
            string? sortBy = null,
            bool onlyActive = true)
        {
            var query = _context.Rooms.AsQueryable();

            if (onlyActive)
            {
                query = query.Where(r => r.IsActive);
            }

            if (!string.IsNullOrWhiteSpace(roomType))
            {
                query = query.Where(r => r.RoomType == roomType);
            }

            if (minPrice.HasValue && minPrice.Value > 0)
            {
                query = query.Where(r => r.PricePerNight >= minPrice.Value);
            }

            if (maxPrice.HasValue && maxPrice.Value > 0)
            {
                query = query.Where(r => r.PricePerNight <= maxPrice.Value);
            }

            if (guests.HasValue && guests.Value > 0)
            {
                query = query.Where(r => r.Capacity >= guests.Value);
            }

            query = sortBy switch
            {
                "price_asc" => query.OrderBy(r => r.PricePerNight),
                "price_desc" => query.OrderByDescending(r => r.PricePerNight),
                "capacity_desc" => query.OrderByDescending(r => r.Capacity),
                _ => query.OrderBy(r => r.RoomNumber)
            };

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<Room>> GetAvailableRoomsAsync(
            DateTime checkIn,
            DateTime checkOut,
            int guests,
            string? roomType = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            string? sortBy = null)
        {
            // Business rule: Overlap condition:
            // ExistingCheckIn < RequestedCheckOut AND ExistingCheckOut > RequestedCheckIn
            // Ignore Cancelled bookings
            var bookedRoomIds = await _context.Bookings
                .Where(b => b.BookingStatus != "Cancelled" &&
                            b.CheckInDate < checkOut.Date &&
                            b.CheckOutDate > checkIn.Date)
                .Select(b => b.RoomId)
                .Distinct()
                .ToListAsync();

            var query = _context.Rooms
                .Where(r => r.IsActive && !bookedRoomIds.Contains(r.RoomId));

            if (guests > 0)
            {
                query = query.Where(r => r.Capacity >= guests);
            }

            if (!string.IsNullOrWhiteSpace(roomType))
            {
                query = query.Where(r => r.RoomType == roomType);
            }

            if (minPrice.HasValue && minPrice.Value > 0)
            {
                query = query.Where(r => r.PricePerNight >= minPrice.Value);
            }

            if (maxPrice.HasValue && maxPrice.Value > 0)
            {
                query = query.Where(r => r.PricePerNight <= maxPrice.Value);
            }

            query = sortBy switch
            {
                "price_asc" => query.OrderBy(r => r.PricePerNight),
                "price_desc" => query.OrderByDescending(r => r.PricePerNight),
                "capacity_desc" => query.OrderByDescending(r => r.Capacity),
                _ => query.OrderBy(r => r.PricePerNight)
            };

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<Room?> GetRoomByIdAsync(int roomId)
        {
            return await _context.Rooms
                .Include(r => r.Bookings)
                .FirstOrDefaultAsync(r => r.RoomId == roomId);
        }

        public async Task<Room?> GetRoomByNumberAsync(string roomNumber)
        {
            return await _context.Rooms
                .FirstOrDefaultAsync(r => r.RoomNumber.ToLower() == roomNumber.Trim().ToLower());
        }

        public async Task<IEnumerable<string>> GetDistinctRoomTypesAsync()
        {
            return await _context.Rooms
                .Where(r => r.IsActive)
                .Select(r => r.RoomType)
                .Distinct()
                .OrderBy(t => t)
                .ToListAsync();
        }

        public async Task<(bool Success, string Message, Room? Room)> CreateRoomAsync(RoomAdminViewModel model)
        {
            // Check for duplicate room number
            var exists = await _context.Rooms.AnyAsync(r => r.RoomNumber.ToLower() == model.RoomNumber.Trim().ToLower());
            if (exists)
            {
                return (false, $"A room with number '{model.RoomNumber}' already exists.", null);
            }

            if (model.PricePerNight <= 0)
            {
                return (false, "Price per night must be greater than zero.", null);
            }

            if (model.Capacity <= 0)
            {
                return (false, "Room capacity must be at least 1 guest.", null);
            }

            var room = new Room
            {
                RoomNumber = model.RoomNumber.Trim(),
                RoomType = model.RoomType.Trim(),
                Description = model.Description.Trim(),
                PricePerNight = model.PricePerNight,
                Capacity = model.Capacity,
                ImageUrl = string.IsNullOrWhiteSpace(model.ImageUrl) 
                    ? "https://images.unsplash.com/photo-1590490360182-c33d57733427?auto=format&fit=crop&w=1200&q=80" 
                    : model.ImageUrl.Trim(),
                Amenities = model.Amenities?.Trim() ?? string.Empty,
                IsActive = model.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Rooms.AddAsync(room);
            await _context.SaveChangesAsync();

            return (true, "Room created successfully.", room);
        }

        public async Task<(bool Success, string Message)> UpdateRoomAsync(RoomAdminViewModel model)
        {
            var room = await _context.Rooms.FindAsync(model.RoomId);
            if (room == null)
            {
                return (false, "Room not found.");
            }

            // Check if updated room number collides with another room
            var duplicate = await _context.Rooms.AnyAsync(r => 
                r.RoomId != model.RoomId && 
                r.RoomNumber.ToLower() == model.RoomNumber.Trim().ToLower());

            if (duplicate)
            {
                return (false, $"Room number '{model.RoomNumber}' is already in use by another room.");
            }

            if (model.PricePerNight <= 0)
            {
                return (false, "Price per night must be greater than zero.");
            }

            if (model.Capacity <= 0)
            {
                return (false, "Room capacity must be at least 1 guest.");
            }

            room.RoomNumber = model.RoomNumber.Trim();
            room.RoomType = model.RoomType.Trim();
            room.Description = model.Description.Trim();
            room.PricePerNight = model.PricePerNight;
            room.Capacity = model.Capacity;
            if (!string.IsNullOrWhiteSpace(model.ImageUrl))
            {
                room.ImageUrl = model.ImageUrl.Trim();
            }
            room.Amenities = model.Amenities?.Trim() ?? string.Empty;
            room.IsActive = model.IsActive;

            await _context.SaveChangesAsync();
            return (true, "Room updated successfully.");
        }

        public async Task<(bool Success, string Message)> ToggleRoomActiveStatusAsync(int roomId)
        {
            var room = await _context.Rooms.FindAsync(roomId);
            if (room == null)
            {
                return (false, "Room not found.");
            }

            room.IsActive = !room.IsActive;
            await _context.SaveChangesAsync();

            string status = room.IsActive ? "activated" : "deactivated";
            return (true, $"Room {room.RoomNumber} has been {status}.");
        }

        public async Task<(bool Success, string Message)> DeleteRoomAsync(int roomId)
        {
            var room = await _context.Rooms
                .Include(r => r.Bookings)
                .FirstOrDefaultAsync(r => r.RoomId == roomId);

            if (room == null)
            {
                return (false, "Room not found.");
            }

            // Check if there are active (future or current) bookings
            var today = DateTime.Today;
            var hasActiveBookings = room.Bookings.Any(b => 
                b.BookingStatus != "Cancelled" && 
                b.CheckOutDate >= today);

            if (hasActiveBookings)
            {
                return (false, $"Cannot delete Room {room.RoomNumber} because it has active or upcoming bookings. Please deactivate the room instead.");
            }

            // If it has historical bookings, prevent orphan FK constraint error and suggest deactivation
            if (room.Bookings.Any())
            {
                return (false, $"Cannot permanently delete Room {room.RoomNumber} because it has past booking records for financial auditing. Please deactivate the room to hide it from new guests.");
            }

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();
            return (true, $"Room {room.RoomNumber} was successfully deleted.");
        }

        public async Task<IEnumerable<RoomAdminViewModel>> GetAllRoomsForAdminAsync()
        {
            var today = DateTime.Today;
            var rooms = await _context.Rooms
                .Include(r => r.Bookings)
                .OrderBy(r => r.RoomNumber)
                .ToListAsync();

            return rooms.Select(r => new RoomAdminViewModel
            {
                RoomId = r.RoomId,
                RoomNumber = r.RoomNumber,
                RoomType = r.RoomType,
                Description = r.Description,
                PricePerNight = r.PricePerNight,
                Capacity = r.Capacity,
                ImageUrl = r.ImageUrl,
                Amenities = r.Amenities,
                IsActive = r.IsActive,
                TotalBookingsCount = r.Bookings.Count,
                ActiveBookingsCount = r.Bookings.Count(b => b.BookingStatus != "Cancelled" && b.CheckOutDate >= today)
            });
        }
    }
}
