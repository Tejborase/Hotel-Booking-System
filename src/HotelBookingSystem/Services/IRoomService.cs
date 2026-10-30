using HotelBookingSystem.Models;
using HotelBookingSystem.ViewModels;

namespace HotelBookingSystem.Services
{
    public interface IRoomService
    {
        Task<IEnumerable<Room>> GetAllRoomsAsync(string? roomType = null, decimal? minPrice = null, decimal? maxPrice = null, int? guests = null, string? sortBy = null, bool onlyActive = true);
        Task<IEnumerable<Room>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut, int guests, string? roomType = null, decimal? minPrice = null, decimal? maxPrice = null, string? sortBy = null);
        Task<Room?> GetRoomByIdAsync(int roomId);
        Task<Room?> GetRoomByNumberAsync(string roomNumber);
        Task<IEnumerable<string>> GetDistinctRoomTypesAsync();
        Task<(bool Success, string Message, Room? Room)> CreateRoomAsync(RoomAdminViewModel model);
        Task<(bool Success, string Message)> UpdateRoomAsync(RoomAdminViewModel model);
        Task<(bool Success, string Message)> ToggleRoomActiveStatusAsync(int roomId);
        Task<(bool Success, string Message)> DeleteRoomAsync(int roomId);
        Task<IEnumerable<RoomAdminViewModel>> GetAllRoomsForAdminAsync();
    }
}
