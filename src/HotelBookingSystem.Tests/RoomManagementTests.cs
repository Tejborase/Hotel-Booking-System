using HotelBookingSystem.Data;
using HotelBookingSystem.Models;
using HotelBookingSystem.Services;
using HotelBookingSystem.ViewModels;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace HotelBookingSystem.Tests
{
    public class RoomManagementTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task CreateRoom_WhenRoomNumberAlreadyExists_ReturnsFailure()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var existingRoom = new Room
            {
                RoomNumber = "101",
                RoomType = "Executive Suite",
                Description = "Luxury suite",
                PricePerNight = 300m,
                Capacity = 2,
                ImageUrl = "https://example.com/room.jpg",
                IsActive = true
            };
            await context.Rooms.AddAsync(existingRoom);
            await context.SaveChangesAsync();

            var roomService = new RoomService(context);

            var newRoomModel = new RoomAdminViewModel
            {
                RoomNumber = "101", // DUPLICATE
                RoomType = "Deluxe Room",
                Description = "Another room",
                PricePerNight = 250m,
                Capacity = 2,
                ImageUrl = "https://example.com/room2.jpg",
                IsActive = true
            };

            // Act
            var (success, message, createdRoom) = await roomService.CreateRoomAsync(newRoomModel);

            // Assert
            Assert.False(success);
            Assert.Contains("already exists", message);
            Assert.Null(createdRoom);
        }

        [Fact]
        public async Task CreateRoom_WithNegativePriceOrZeroCapacity_ReturnsFailure()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var roomService = new RoomService(context);

            var invalidPriceModel = new RoomAdminViewModel
            {
                RoomNumber = "102",
                RoomType = "Standard Room",
                Description = "Desc",
                PricePerNight = -50m, // INVALID
                Capacity = 2,
                ImageUrl = "https://example.com/img.jpg"
            };

            var invalidCapacityModel = new RoomAdminViewModel
            {
                RoomNumber = "103",
                RoomType = "Standard Room",
                Description = "Desc",
                PricePerNight = 100m,
                Capacity = 0, // INVALID
                ImageUrl = "https://example.com/img.jpg"
            };

            // Act
            var result1 = await roomService.CreateRoomAsync(invalidPriceModel);
            var result2 = await roomService.CreateRoomAsync(invalidCapacityModel);

            // Assert
            Assert.False(result1.Success);
            Assert.False(result2.Success);
        }

        [Fact]
        public async Task DeleteRoom_WhenRoomHasActiveBookings_ReturnsFailureAndPreventsDelete()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var room = new Room
            {
                RoomNumber = "201",
                RoomType = "Ocean Suite",
                Description = "View",
                PricePerNight = 400m,
                Capacity = 2,
                ImageUrl = "https://example.com/ocean.jpg",
                IsActive = true
            };
            await context.Rooms.AddAsync(room);
            await context.SaveChangesAsync();

            // Add upcoming booking
            var activeBooking = new Booking
            {
                BookingReference = "GV-ACTIVE-1",
                RoomId = room.RoomId,
                GuestName = "Future Guest",
                Email = "guest@example.com",
                Phone = "555-9999",
                CheckInDate = DateTime.Today.AddDays(3),
                CheckOutDate = DateTime.Today.AddDays(7),
                GuestsCount = 2,
                PricePerNight = 400m,
                TotalAmount = 1600m,
                BookingStatus = "Confirmed"
            };
            await context.Bookings.AddAsync(activeBooking);
            await context.SaveChangesAsync();

            var roomService = new RoomService(context);

            // Act
            var (success, message) = await roomService.DeleteRoomAsync(room.RoomId);

            // Assert
            Assert.False(success);
            Assert.Contains("active or upcoming bookings", message);

            // Verify room still exists in DB
            var roomStillInDb = await context.Rooms.FindAsync(room.RoomId);
            Assert.NotNull(roomStillInDb);
        }

        [Fact]
        public async Task ToggleRoomActiveStatus_TogglesStateCorrectly()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var room = new Room
            {
                RoomNumber = "305",
                RoomType = "Garden Suite",
                Description = "Garden",
                PricePerNight = 280m,
                Capacity = 2,
                ImageUrl = "https://example.com/garden.jpg",
                IsActive = true
            };
            await context.Rooms.AddAsync(room);
            await context.SaveChangesAsync();

            var roomService = new RoomService(context);

            // Act: Deactivate
            var (success1, message1) = await roomService.ToggleRoomActiveStatusAsync(room.RoomId);
            Assert.True(success1);
            Assert.False(room.IsActive);

            // Act: Re-activate
            var (success2, message2) = await roomService.ToggleRoomActiveStatusAsync(room.RoomId);
            Assert.True(success2);
            Assert.True(room.IsActive);
        }

        [Fact]
        public async Task GetAllRooms_WithPriceAndCapacityFilters_ReturnsMatchingRooms()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var rooms = new List<Room>
            {
                new Room { RoomNumber = "101", RoomType = "Standard", Description = "A", PricePerNight = 150m, Capacity = 2, ImageUrl = "a", IsActive = true },
                new Room { RoomNumber = "201", RoomType = "Deluxe", Description = "B", PricePerNight = 350m, Capacity = 2, ImageUrl = "b", IsActive = true },
                new Room { RoomNumber = "301", RoomType = "Family", Description = "C", PricePerNight = 500m, Capacity = 5, ImageUrl = "c", IsActive = true }
            };
            await context.Rooms.AddRangeAsync(rooms);
            await context.SaveChangesAsync();

            var roomService = new RoomService(context);

            // Act: Filter min price 200, max price 400
            var filtered = await roomService.GetAllRoomsAsync(minPrice: 200m, maxPrice: 400m);

            // Assert: Only Deluxe (350) matches
            Assert.Single(filtered);
            Assert.Equal("201", filtered.First().RoomNumber);
        }
    }
}
