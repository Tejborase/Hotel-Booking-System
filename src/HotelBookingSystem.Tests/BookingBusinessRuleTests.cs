using HotelBookingSystem.Data;
using HotelBookingSystem.Models;
using HotelBookingSystem.Services;
using HotelBookingSystem.ViewModels;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace HotelBookingSystem.Tests
{
    public class BookingBusinessRuleTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task IsRoomAvailable_WhenNoBookingsExist_ReturnsTrue()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var room = new Room
            {
                RoomNumber = "101",
                RoomType = "Executive King Suite",
                Description = "Luxury suite",
                PricePerNight = 300m,
                Capacity = 2,
                ImageUrl = "https://example.com/room.jpg",
                IsActive = true
            };
            await context.Rooms.AddAsync(room);
            await context.SaveChangesAsync();

            var bookingService = new BookingService(context);

            var checkIn = DateTime.Today.AddDays(5);
            var checkOut = DateTime.Today.AddDays(8);

            // Act
            var available = await bookingService.IsRoomAvailableAsync(room.RoomId, checkIn, checkOut);

            // Assert
            Assert.True(available);
        }

        [Fact]
        public async Task IsRoomAvailable_WhenActiveOverlappingBookingExists_ReturnsFalse()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var room = new Room
            {
                RoomNumber = "101",
                RoomType = "Executive King Suite",
                Description = "Luxury suite",
                PricePerNight = 300m,
                Capacity = 2,
                ImageUrl = "https://example.com/room.jpg",
                IsActive = true
            };
            await context.Rooms.AddAsync(room);
            await context.SaveChangesAsync();

            // Existing booking: Day 10 to Day 15
            var existingBooking = new Booking
            {
                BookingReference = "GV-EXIST-1",
                RoomId = room.RoomId,
                GuestName = "John Doe",
                Email = "john@example.com",
                Phone = "123456789",
                CheckInDate = DateTime.Today.AddDays(10),
                CheckOutDate = DateTime.Today.AddDays(15),
                GuestsCount = 2,
                PricePerNight = 300m,
                TotalAmount = 1500m,
                BookingStatus = "Confirmed"
            };
            await context.Bookings.AddAsync(existingBooking);
            await context.SaveChangesAsync();

            var bookingService = new BookingService(context);

            // Requested: Day 12 to Day 16 (Overlaps Day 12 to 15)
            var checkIn = DateTime.Today.AddDays(12);
            var checkOut = DateTime.Today.AddDays(16);

            // Act
            var available = await bookingService.IsRoomAvailableAsync(room.RoomId, checkIn, checkOut);

            // Assert
            Assert.False(available);
        }

        [Fact]
        public async Task IsRoomAvailable_WhenRequestedCheckInIsSameAsExistingCheckOut_ReturnsTrue()
        {
            // Arrange: Same-day turnover: Previous check-out is 11:00 AM, Next check-in is 3:00 PM.
            // ExistingCheckIn < RequestedCheckOut AND ExistingCheckOut > RequestedCheckIn
            // If ExistingCheckOut == RequestedCheckIn, then ExistingCheckOut > RequestedCheckIn is FALSE -> AVAILABLE!
            using var context = GetInMemoryDbContext();
            var room = new Room
            {
                RoomNumber = "201",
                RoomType = "Deluxe Oceanfront Suite",
                Description = "Ocean view",
                PricePerNight = 450m,
                Capacity = 2,
                ImageUrl = "https://example.com/room.jpg",
                IsActive = true
            };
            await context.Rooms.AddAsync(room);
            await context.SaveChangesAsync();

            var existingBooking = new Booking
            {
                BookingReference = "GV-EXIST-2",
                RoomId = room.RoomId,
                GuestName = "Alice Smith",
                Email = "alice@example.com",
                Phone = "555-0100",
                CheckInDate = DateTime.Today.AddDays(5),
                CheckOutDate = DateTime.Today.AddDays(10),
                GuestsCount = 2,
                PricePerNight = 450m,
                TotalAmount = 2250m,
                BookingStatus = "Confirmed"
            };
            await context.Bookings.AddAsync(existingBooking);
            await context.SaveChangesAsync();

            var bookingService = new BookingService(context);

            // Requested: Day 10 to Day 14 (starts on the day previous guest checks out)
            var checkIn = DateTime.Today.AddDays(10);
            var checkOut = DateTime.Today.AddDays(14);

            // Act
            var available = await bookingService.IsRoomAvailableAsync(room.RoomId, checkIn, checkOut);

            // Assert
            Assert.True(available);
        }

        [Fact]
        public async Task IsRoomAvailable_WhenExistingBookingIsCancelled_ReturnsTrue()
        {
            // Arrange: Cancelled bookings should NOT block availability
            using var context = GetInMemoryDbContext();
            var room = new Room
            {
                RoomNumber = "301",
                RoomType = "Grand Family Residence",
                Description = "Family suite",
                PricePerNight = 500m,
                Capacity = 4,
                ImageUrl = "https://example.com/room.jpg",
                IsActive = true
            };
            await context.Rooms.AddAsync(room);
            await context.SaveChangesAsync();

            var cancelledBooking = new Booking
            {
                BookingReference = "GV-CANCELLED-1",
                RoomId = room.RoomId,
                GuestName = "Bob Brown",
                Email = "bob@example.com",
                Phone = "555-0200",
                CheckInDate = DateTime.Today.AddDays(5),
                CheckOutDate = DateTime.Today.AddDays(10),
                GuestsCount = 3,
                PricePerNight = 500m,
                TotalAmount = 2500m,
                BookingStatus = "Cancelled"
            };
            await context.Bookings.AddAsync(cancelledBooking);
            await context.SaveChangesAsync();

            var bookingService = new BookingService(context);

            var checkIn = DateTime.Today.AddDays(6);
            var checkOut = DateTime.Today.AddDays(9);

            // Act
            var available = await bookingService.IsRoomAvailableAsync(room.RoomId, checkIn, checkOut);

            // Assert
            Assert.True(available);
        }

        [Fact]
        public async Task IsRoomAvailable_WhenRoomIsInactive_ReturnsFalse()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var room = new Room
            {
                RoomNumber = "999",
                RoomType = "Maintenance Suite",
                Description = "Out of order",
                PricePerNight = 100m,
                Capacity = 2,
                ImageUrl = "https://example.com/room.jpg",
                IsActive = false // INACTIVE
            };
            await context.Rooms.AddAsync(room);
            await context.SaveChangesAsync();

            var bookingService = new BookingService(context);

            // Act
            var available = await bookingService.IsRoomAvailableAsync(
                room.RoomId, 
                DateTime.Today.AddDays(2), 
                DateTime.Today.AddDays(4));

            // Assert
            Assert.False(available);
        }

        [Fact]
        public void CalculateBookingTotal_CalculatesAccuratelyBasedOnServerRate()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var bookingService = new BookingService(context);

            var pricePerNight = 350.50m;
            var checkIn = new DateTime(2026, 11, 1);
            var checkOut = new DateTime(2026, 11, 5); // 4 nights

            // Act
            var total = bookingService.CalculateBookingTotal(pricePerNight, checkIn, checkOut);

            // Assert: 350.50 * 4 = 1402.00
            Assert.Equal(1402.00m, total);
        }

        [Fact]
        public async Task UpdateBookingStatus_CancelledToConfirmed_FailsIfRoomNowOccupied()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var room = new Room
            {
                RoomNumber = "401",
                RoomType = "Signature Villa",
                Description = "Villa",
                PricePerNight = 800m,
                Capacity = 4,
                ImageUrl = "https://example.com/villa.jpg",
                IsActive = true
            };
            await context.Rooms.AddAsync(room);
            await context.SaveChangesAsync();

            var cancelledBooking = new Booking
            {
                BookingReference = "GV-OLD-CANCEL",
                RoomId = room.RoomId,
                GuestName = "Old Guest",
                Email = "old@example.com",
                Phone = "555-1234",
                CheckInDate = DateTime.Today.AddDays(5),
                CheckOutDate = DateTime.Today.AddDays(10),
                GuestsCount = 2,
                PricePerNight = 800m,
                TotalAmount = 4000m,
                BookingStatus = "Cancelled"
            };
            await context.Bookings.AddAsync(cancelledBooking);

            // In the meantime, another guest booked overlapping dates
            var newBooking = new Booking
            {
                BookingReference = "GV-NEW-CONFIRMED",
                RoomId = room.RoomId,
                GuestName = "New Guest",
                Email = "new@example.com",
                Phone = "555-5678",
                CheckInDate = DateTime.Today.AddDays(6),
                CheckOutDate = DateTime.Today.AddDays(9),
                GuestsCount = 2,
                PricePerNight = 800m,
                TotalAmount = 2400m,
                BookingStatus = "Confirmed"
            };
            await context.Bookings.AddAsync(newBooking);
            await context.SaveChangesAsync();

            var bookingService = new BookingService(context);

            // Act: Attempt to uncancel the old booking
            var (success, message) = await bookingService.UpdateBookingStatusAsync(cancelledBooking.BookingId, "Confirmed");

            // Assert: Must be rejected because dates are now taken!
            Assert.False(success);
            Assert.Contains("already been booked", message);
        }
    }
}
