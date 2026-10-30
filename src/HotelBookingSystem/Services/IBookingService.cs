using HotelBookingSystem.Models;
using HotelBookingSystem.ViewModels;

namespace HotelBookingSystem.Services
{
    public interface IBookingService
    {
        Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkIn, DateTime checkOut, int? excludeBookingId = null);
        decimal CalculateBookingTotal(decimal pricePerNight, DateTime checkIn, DateTime checkOut);
        Task<(bool Success, string Message, Booking? Booking)> CreateBookingAsync(BookingCreateViewModel model);
        Task<Booking?> GetBookingByIdAndEmailAsync(int bookingId, string email);
        Task<Booking?> GetBookingByIdAsync(int bookingId);
        Task<IEnumerable<Booking>> GetAllBookingsAsync(string? status = null, string? searchTerm = null);
        Task<(bool Success, string Message)> UpdateBookingStatusAsync(int bookingId, string newStatus);
        Task<AdminDashboardViewModel> GetDashboardStatsAsync();
    }
}
